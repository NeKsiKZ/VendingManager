using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingManager.Data;
using VendingManager.Filters;

namespace VendingManager.Controllers
{
	/// <summary>
	/// Kontroler serwisowy do zdalnego zarządzania stanem maszyn i rozwiązywania problemów.
	/// </summary>
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
		/// <summary>
		/// Przełącza tryb serwisowy maszyny (Włącz/Wyłącz).
		/// </summary>
		/// <remarks>
		/// Gdy maszyna jest w trybie serwisowym (IsUnderMaintenance = true):
		/// 1. Status maszyny zmienia się na "Maintenance".
		/// 2. API blokuje możliwość dokonywania zakupów (zwraca kod 503).
		/// Użyj tego endpointu przed rozpoczęciem prac serwisowych.
		/// </remarks>
		/// <param name="machineId">ID Automatu.</param>
		/// <returns>Aktualny stan maszyny po przełączeniu.</returns>
		/// <response code="200">Stan maszyny został zmieniony.</response>
		/// <response code="404">Nie znaleziono maszyny o podanym ID.</response>
		[HttpPost("{machineId}/toggle-mode")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
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
		/// <summary>
		/// Uzupełnia wszystkie sloty w maszynie do pełna.
		/// </summary>
		/// <remarks>
		/// Ustawia ilość produktów (Quantity) na równą pojemności slotu (Capacity) dla wszystkich produktów w danej maszynie.
		/// Operacja jest wykonywana masowo w jednej transakcji bazy danych.
		/// </remarks>
		/// <param name="machineId">ID Automatu.</param>
		/// <returns>Informację o liczbie zaktualizowanych slotów.</returns>
		[HttpPost("{machineId}/restock-all")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
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
		/// <summary>
		/// Usuwa całą historię błędów dla danej maszyny.
		/// </summary>
		/// <remarks>
		/// Należy użyć tego endpointu po fizycznym usunięciu usterki przez serwisanta, aby wyczyścić logi.
		/// </remarks>
		/// <param name="machineId">ID Automatu.</param>
		/// <returns>Liczbę usuniętych wpisów.</returns>
		[HttpDelete("{machineId}/errors")]
		[ProducesResponseType(200)]
		public async Task<IActionResult> ClearErrors(int machineId)
		{
			var deletedCount = await _context.MachineErrorLogs
				.Where(e => e.MachineId == machineId)
				.ExecuteDeleteAsync();

			return Ok(new { message = $"Usunięto {deletedCount} wpisów z dziennika błędów." });
		}

		// POST: api/maintenance/{machineId}/reboot
		/// <summary>
		/// Symuluje zdalny restart (reboot) urządzenia.
		/// </summary>
		/// <remarks>
		/// Akcja ta:
		/// 1. Resetuje status maszyny na "Online".
		/// 2. Wyłącza tryb serwisowy (jeśli był włączony).
		/// 3. Aktualizuje datę ostatniego kontaktu (LastContact).
		/// </remarks>
		/// <param name="machineId">ID Automatu.</param>
		/// <returns>Potwierdzenie restartu.</returns>
		[HttpPost("{machineId}/reboot")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
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