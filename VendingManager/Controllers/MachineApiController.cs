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

        [HttpPost("{id}/heartbeat")]
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

        [HttpPost("{id}/sale")]
        public async Task<IActionResult> RecordSale(int id, [FromBody] SaleRequestDto saleRequest)
        {
            var slot = await _context.MachineSlots
				.Include(s => s.Machine)
				.FirstOrDefaultAsync(s => s.MachineId == id && s.ProductId == saleRequest.ProductId);

            if (slot == null)
            {
                return NotFound(new { message = "Błąd: Nie znaleziono slotu dla tego produktu w tej maszynie." });
            }

			if (slot.Machine.IsUnderMaintenance)
			{
				return StatusCode(503, new { message = "Maszyna jest w trybie serwisowym. Zakup niemożliwy." });
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
        [HttpPost("{id}/error")]
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
		[HttpGet]
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
		[HttpGet("{id}/inventory")]
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