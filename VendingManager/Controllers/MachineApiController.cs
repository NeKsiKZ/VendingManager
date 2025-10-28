using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendingManager.Data;
using VendingManager.Models;
using VendingManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Reflection.Metadata.Ecma335;
using VendingManager.Filters;

namespace VendingManager.Controllers
{
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [ApiController]
    [Route("api/machine")]
    public class MachineApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MachineApiController(ApplicationDbContext context)
        {
            _context = context;
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
                .FirstOrDefaultAsync(s => s.MachineId == id && s.ProductId == saleRequest.ProductId);

            if (slot == null)
            {
                return NotFound(new { message = "Błąd: Nie znaleziono slotu dla tego produktu w tej maszynie." });
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
            return CreatedAtAction(nameof(RecordSale), new { id = newTransaction.Id }, newTransaction);
        }
    }
}
