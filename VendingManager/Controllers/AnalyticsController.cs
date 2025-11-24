using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingManager.Data;
using VendingManager.ViewModels;
using VendingManager.Filters;

namespace VendingManager.Controllers
{
	/// <summary>
	/// Kontroler odpowiedzialny za agregację danych i statystyki biznesowe.
	/// </summary>
	[ServiceFilter(typeof(ApiKeyAuthFilter))]
	[Route("api/[controller]")]
	[ApiController]
	public class AnalyticsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public AnalyticsController(ApplicationDbContext context)
		{
			_context = context;
		}



		// GET: api/analytics/revenue/daily?days=30
		/// <summary>
		/// Pobiera zagregowane dane o przychodach dziennych z wybranego okresu.
		/// </summary>
		/// <remarks>
		/// Dane są grupowane po dacie transakcji. Idealne do generowania wykresów liniowych.
		/// </remarks>
		/// <param name="days">Liczba ostatnich dni do analizy (domyślnie 30).</param>
		/// <returns>Lista obiektów zawierających datę, sumę przychodu i liczbę transakcji.</returns>
		/// <response code="200">Zwraca listę danych analitycznych.</response>
		/// <response code="401">Brak ważnego klucza API.</response>
		[HttpGet("revenue/daily")]
		[ProducesResponseType(typeof(IEnumerable<DailyRevenueDto>), 200)]
		[ProducesResponseType(401)]
		public async Task<ActionResult<IEnumerable<DailyRevenueDto>>> GetDailyRevenue([FromQuery] int days = 30)
		{
			var startDate = DateTime.Now.Date.AddDays(-days);

			var rawData = await _context.Transactions
				.Where(t => t.TransactionDate >= startDate)
				.GroupBy(t => t.TransactionDate.Date)
				.Select(g => new
				{
					Date = g.Key,
					Revenue = g.Sum(t => t.SalePrice),
					TransactionCount = g.Count()
				})
				.OrderBy(x => x.Date)
				.ToListAsync();

			var result = rawData.Select(d => new DailyRevenueDto
			{
				Date = d.Date.ToString("yyyy-MM-dd"),
				Revenue = d.Revenue,
				TransactionCount = d.TransactionCount
			}).ToList();

			return Ok(result);
		}

		// GET: api/analytics/products/top?count=5
		/// <summary>
		/// Zwraca ranking najlepiej sprzedających się produktów (Bestsellery).
		/// </summary>
		/// <param name="count">Liczba produktów w rankingu (domyślnie 5).</param>
		/// <returns>Lista produktów posortowana malejąco według liczby sprzedanych sztuk.</returns>
		[HttpGet("products/top")]
		[ProducesResponseType(typeof(IEnumerable<ProductPopularityDto>), 200)]
		public async Task<ActionResult<IEnumerable<ProductPopularityDto>>> GetTopProducts([FromQuery] int count = 5)
		{
			var data = await _context.Transactions
				.Include(t => t.Product)
				.Where(t => t.Product != null)
				.GroupBy(t => t.Product.Name)
				.Select(g => new ProductPopularityDto
				{
					ProductName = g.Key,
					UnitsSold = g.Count(),
					TotalRevenue = g.Sum(t => t.SalePrice)
				})
				.OrderByDescending(x => x.UnitsSold)
				.Take(count)
				.ToListAsync();

			return Ok(data);
		}

		// GET: api/analytics/machines/performance
		/// <summary>
		/// Pobiera ranking wydajności finansowej poszczególnych automatów.
		/// </summary>
		/// <returns>Lista maszyn z ich całkowitym przychodem i średnią wartością koszyka.</returns>
		[HttpGet("machines/performance")]
		[ProducesResponseType(typeof(IEnumerable<MachinePerformanceDto>), 200)]
		public async Task<ActionResult<IEnumerable<MachinePerformanceDto>>> GetMachinePerformance()
		{
			var data = await _context.Transactions
				.Include(t => t.Machine)
				.Where(t => t.Machine != null)
				.GroupBy(t => t.Machine.Name)
				.Select(g => new MachinePerformanceDto
				{
					MachineName = g.Key,
					TotalTransactions = g.Count(),
					TotalRevenue = g.Sum(t => t.SalePrice),
					AverageTransactionValue = g.Count() > 0
						? (double)(g.Sum(t => t.SalePrice) / g.Count())
						: 0
				})
				.OrderByDescending(x => x.TotalRevenue)
				.ToListAsync();

			return Ok(data);
		}
	}
}