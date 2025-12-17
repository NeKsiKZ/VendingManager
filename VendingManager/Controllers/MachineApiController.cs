using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendingManager.Data;
using VendingManager.Models;
using VendingManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Reflection.Metadata.Ecma335;
using VendingManager.Filters;
using Microsoft.AspNetCore.SignalR;
using VendingManager.Hubs;

namespace VendingManager.Controllers
{
	/// <summary>
	/// Główny kontroler API do komunikacji M2M (Machine-to-Machine) oraz obsługi klienta (React).
	/// </summary>
	[ServiceFilter(typeof(ApiKeyAuthFilter))]
    [ApiController]
    [Route("api/machine")]
    public class MachineApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<DashboardHub> _hubContext;

        public MachineApiController(ApplicationDbContext context, IHubContext<DashboardHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

		/// <summary>
		/// Odbiera sygnał "Heartbeat" (życia) od maszyny.
		/// </summary>
		/// <param name="id">ID Automatu.</param>
		/// <returns>Status operacji.</returns>
		[HttpPost("{id}/heartbeat")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> ReceiveHeartbeat(int id)
        {
            var machine = await _context.Machines.FindAsync(id);

            if (machine == null)
            {
                return NotFound(new { message = "Nie znaleziono maszyny o podanym ID." });
            }

            machine.Status = "Online";
            machine.LastContact = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Błąd zapisu do bazy.", error = ex.Message });
            }

            return Ok(new { message = $"Odebrano sygnał 'heartbeat' od maszyny: {machine.Name}" });
        }

		/// <summary>
		/// Rejestruje nową transakcję sprzedaży i aktualizuje stan magazynowy.
		/// </summary>
		/// <remarks>
		/// Metoda ta wykonuje kilka operacji:
		/// 1. Sprawdza, czy maszyna nie jest w trybie serwisowym.
		/// 2. Zmniejsza stan magazynowy (Quantity) o 1.
		/// 3. Tworzy nowy wpis w tabeli Transactions.
		/// 4. Wysyła powiadomienie Real-Time (SignalR) do panelu administratora.
		/// </remarks>
		/// <param name="id">ID Automatu.</param>
		/// <param name="saleRequest">Obiekt zawierający ID sprzedanego produktu.</param>
		/// <response code="201">Transakcja zakończona sukcesem.</response>
		/// <response code="503">Maszyna jest w trybie serwisowym.</response>
		/// <response code="400">Brak towaru w automacie.</response>
		/// <response code="404">Nie znaleziono maszyny lub produktu.</response>
		[HttpPost("{id}/sale")]
		[ProducesResponseType(typeof(Transaction), 201)]
		[ProducesResponseType(503)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> RecordSale(int id, [FromBody] SaleRequestDto saleRequest)
        {
            var slot = await _context.MachineSlots
				.Include(s => s.Machine)
				.FirstOrDefaultAsync(s => s.MachineId == id && s.ProductId == saleRequest.ProductId);

            if (slot == null)
            {
                return NotFound(new { message = "Błąd: Nie znaleziono slotu dla tego produktu w tej maszynie." });
            }

            if (slot.Machine.IsUnderMaintenance ||
                slot.Machine.Status == "Maintenance" ||
                slot.Machine.Status == "Offline")
            {
                return StatusCode(503, new { message = $"Maszyna jest niedostępna (Status: {slot.Machine.Status}). Zakup niemożliwy." });
            }

            if (slot.Quantity <= 0)
            {
                return BadRequest(new { message = "Błąd: Stan magazynowy dla tego produktu wynosi 0." });
            }

            var product = await _context.Products.FindAsync(saleRequest.ProductId);
            if (product == null)
            {
                return NotFound(new { message = "Błąd: Nie znaleziono produktu o podanym ID." });
            }

            slot.Quantity -= 1;

            var newTransaction = new Transaction
            {
                MachineId = id,
                ProductId = saleRequest.ProductId,
                TransactionDate = DateTime.Now,
                SalePrice = product.Price
            };

            _context.Transactions.Add(newTransaction);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Błąd zapisu transakcji do bazy.", error = ex.Message });
            }

            await _hubContext.Clients.All.SendAsync("ReceiveSaleNotification",
                slot.MachineId,
                product.Name);

            return CreatedAtAction(nameof(RecordSale), new { id = newTransaction.Id }, newTransaction);
        }

		// POST /api/machine/1/error
		/// <summary>
		/// Rejestruje błąd techniczny zgłoszony przez maszynę.
		/// </summary>
		/// <param name="id">ID Automatu.</param>
		/// <param name="errorRequest">Szczegóły błędu (kod i wiadomość).</param>
		/// <returns>Utworzony log błędu.</returns>
		[HttpPost("{id}/error")]
		[ProducesResponseType(typeof(MachineErrorLog), 201)]
		[ProducesResponseType(404)]
        public async Task<IActionResult> LogError(int id, [FromBody] ErrorRequestDto errorRequest)
        {
            var machineExists = await _context.Machines.AnyAsync(m => m.Id == id);
            if (!machineExists)
            {
                return NotFound(new { message = "Nie znaleziono maszyny o podanym ID." });
            }

            var errorLog = new MachineErrorLog
            {
                MachineId = id,
                Timestamp = DateTime.Now,
                ErrorCode = errorRequest.ErrorCode,
                Message = errorRequest.Message
            };

            _context.MachineErrorLogs.Add(errorLog);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveErrorNotification",
                id,
                errorLog.ErrorCode);

            return CreatedAtAction(nameof(LogError), new { id = errorLog.Id }, errorLog);
        }

		// GET: /api/machine
		/// <summary>
		/// Pobiera listę wszystkich maszyn z obsługą paginacji, sortowania i wyszukiwania.
		/// </summary>
		/// <param name="query">Obiekt zawierający parametry filtrowania (strona, rozmiar, fraza wyszukiwania).</param>
		/// <returns>Stronicowana lista maszyn.</returns>
		[HttpGet]
		[ProducesResponseType(typeof(PagedResult<Machine>), 200)]
		public async Task<ActionResult<PagedResult<Machine>>> GetAll([FromQuery] PaginationQuery query)
		{
			var baseQuery = _context.Machines.AsQueryable();

			if (!string.IsNullOrEmpty(query.SearchPhrase))
			{
				baseQuery = baseQuery.Where(r =>
					r.Name.Contains(query.SearchPhrase) ||
					r.Location.Contains(query.SearchPhrase));
			}

			if (string.IsNullOrEmpty(query.SortBy))
			{
				baseQuery = baseQuery.OrderBy(r => r.Name);
			}
			else
			{
				baseQuery = query.IsDescending
					? query.SortBy.ToLower() switch
					{
						"location" => baseQuery.OrderByDescending(r => r.Location),
						"status" => baseQuery.OrderByDescending(r => r.Status),
						_ => baseQuery.OrderByDescending(r => r.Name)
					}
					: query.SortBy.ToLower() switch
					{
						"location" => baseQuery.OrderBy(r => r.Location),
						"status" => baseQuery.OrderBy(r => r.Status),
						_ => baseQuery.OrderBy(r => r.Name)
					};
			}

			var totalItemsCount = await baseQuery.CountAsync();

			var machines = await baseQuery
				.Skip(query.PageSize * (query.PageNumber - 1))
				.Take(query.PageSize)
				.ToListAsync();

			var result = new PagedResult<Machine>(machines, totalItemsCount, query.PageSize, query.PageNumber);

			return Ok(result);
		}

		// GET: /api/machine/{id}/inventory
		/// <summary>
		/// Pobiera aktualny stan magazynowy (produkty) dla konkretnej maszyny.
		/// </summary>
		/// <param name="id">ID Automatu.</param>
		/// <returns>Lista produktów dostępnych w maszynie wraz z cenami i ilością.</returns>
		[HttpGet("{id}/inventory")]
		[ProducesResponseType(typeof(IEnumerable<MachineInventoryDto>), 200)]
		[ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<MachineInventoryDto>>> GetInventory(int id)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine == null) return NotFound();

            var inventory = await _context.MachineSlots
                .Where(s => s.MachineId == id)
                .Include(s => s.Product)
                .Select(s => new MachineInventoryDto
                {
                    ProductId = s.Product.Id,
                    ProductName = s.Product.Name,
                    Price = s.Product.Price,
                    Quantity = s.Quantity,
                    ImageUrl = s.Product.ImageUrl
                })
                .ToListAsync();

            return Ok(inventory);
        }

    }
}