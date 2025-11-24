using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using VendingManager.Data;
using VendingManager.Filters;

namespace VendingManager.Controllers
{
	/// <summary>
	/// Kontroler do generowania raportów i eksportu danych.
	/// </summary>
	[ServiceFilter(typeof(ApiKeyAuthFilter))]
	[Route("api/[controller]")]
	[ApiController]
	public class ReportsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public ReportsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: api/reports/sales/csv?month=11&year=2025
		/// <summary>
		/// Generuje raport sprzedaży za wybrany okres i zwraca go jako plik CSV.
		/// </summary>
		/// <remarks>
		/// Plik jest generowany dynamicznie w pamięci i zwracany jako strumień bajtów.
		/// Format CSV zawiera: ID Transakcji, Datę, Czas, Nazwę Automatu, Nazwę Produktu oraz Cenę.
		/// </remarks>
		/// <param name="month">Numer miesiąca (1-12).</param>
		/// <param name="year">Rok (np. 2025).</param>
		/// <returns>Plik CSV gotowy do pobrania.</returns>
		/// <response code="200">Zwraca plik CSV, jeśli znaleziono transakcje.</response>
		/// <response code="404">Brak transakcji w wybranym okresie.</response>
		[HttpGet("sales/csv")]
		[ProducesResponseType(typeof(FileContentResult), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> ExportSalesToCsv([FromQuery] int month, [FromQuery] int year)
		{
			var transactions = await _context.Transactions
				.Include(t => t.Machine)
				.Include(t => t.Product)
				.Where(t => t.TransactionDate.Month == month && t.TransactionDate.Year == year)
				.OrderByDescending(t => t.TransactionDate)
				.ToListAsync();

			if (!transactions.Any())
			{
				return NotFound(new { message = "Brak transakcji w wybranym okresie." });
			}

			var csvBuilder = new StringBuilder();

			csvBuilder.AppendLine("ID Transakcji,Data,Godzina,Automat,Produkt,Cena (PLN)");

			foreach (var t in transactions)
			{
				var line = string.Format("{0},{1},{2},{3},{4},{5}",
					t.Id,
					t.TransactionDate.ToString("yyyy-MM-dd"),
					t.TransactionDate.ToString("HH:mm:ss"),
					EscapeCsv(t.Machine?.Name ?? "Nieznany"),
					EscapeCsv(t.Product?.Name ?? "Nieznany"),
					t.SalePrice.ToString("F2")
				);

				csvBuilder.AppendLine(line);
			}

			var fileBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
			var fileName = $"raport_sprzedazy_{year}_{month:00}.csv";

			return File(fileBytes, "text/csv", fileName);
		}

		private string EscapeCsv(string field)
		{
			if (field.Contains(","))
			{
				return $"\"{field}\"";
			}
			return field;
		}
	}
}