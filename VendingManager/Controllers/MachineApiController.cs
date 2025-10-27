using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendingManager.Data;
using VendingManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace VendingManager.Controllers
{
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
    }
}
