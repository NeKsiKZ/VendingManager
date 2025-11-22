using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingManager.Data;
using VendingManager.Filters;

namespace VendingManager.Controllers
{
	[ServiceFilter(typeof(ApiKeyAuthFilter))]
	[Route("api/[controller]")]
	[ApiController]
	public class MaintenanceController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public MaintenanceController(ApplicationDbContext context)
		{
			_context = context;
		}

		// POST: api/maintenance/{machineId}/toggle-mode
		[HttpPost("{machineId}/toggle-mode")]
		public async Task<IActionResult> ToggleMaintenanceMode(int machineId)
		{
			var machine = await _context.Machines.FindAsync(machineId);
			if (machine == null) return NotFound("Maszyna nie istnieje.");

			machine.IsUnderMaintenance = !machine.IsUnderMaintenance;

			if (machine.IsUnderMaintenance)
			{
				machine.Status = "Maintenance";
			}
			else
			{
				machine.Status = "Online";
				machine.LastContact = DateTime.Now;
			}

			await _context.SaveChangesAsync();

			return Ok(new
			{
				machineId = machine.Id,
				isUnderMaintenance = machine.IsUnderMaintenance,
				status = machine.Status
			});
		}

		// POST: api/maintenance/{machineId}/restock-all
		[HttpPost("{machineId}/restock-all")]
		public async Task<IActionResult> RestockAll(int machineId)
		{
			var rowsAffected = await _context.MachineSlots
				.Where(s => s.MachineId == machineId)
				.ExecuteUpdateAsync(setters => setters
					.SetProperty(s => s.Quantity, s => s.Capacity));

			if (rowsAffected == 0)
			{
				return NotFound("Maszyna nie ma slotów lub nie istnieje.");
			}

			return Ok(new { message = $"Pomyślnie uzupełniono {rowsAffected} slotów do pełna." });
		}

		// DELETE: api/maintenance/{machineId}/errors
		[HttpDelete("{machineId}/errors")]
		public async Task<IActionResult> ClearErrors(int machineId)
		{
			var deletedCount = await _context.MachineErrorLogs
				.Where(e => e.MachineId == machineId)
				.ExecuteDeleteAsync();

			return Ok(new { message = $"Usunięto {deletedCount} wpisów z dziennika błędów." });
		}

		// POST: api/maintenance/{machineId}/reboot
		[HttpPost("{machineId}/reboot")]
		public async Task<IActionResult> RebootMachine(int machineId)
		{
			var machine = await _context.Machines.FindAsync(machineId);
			if (machine == null) return NotFound();

			machine.Status = "Rebooting...";
			await _context.SaveChangesAsync();

			machine.Status = "Online";
			machine.IsUnderMaintenance = false;
			machine.LastContact = DateTime.Now;

			await _context.SaveChangesAsync();

			return Ok(new { message = "Maszyna została zrestartowana pomyślnie." });
		}
	}
}