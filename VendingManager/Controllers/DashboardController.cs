using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VendingManager.Data;
using VendingManager.Models;
using VendingManager.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace VendingManager.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var today = DateTime.Today;

            DateTime filterStart = startDate ?? new DateTime(today.Year, 1, 1);

            DateTime filterEnd = endDate ?? today;

            DateTime filterEndInclusive = filterEnd.AddDays(1).AddTicks(-1);

            var transactions = await _context.Transactions
                .Include(t => t.Product)
                .Where(t => t.TransactionDate >= filterStart && t.TransactionDate <= filterEndInclusive)
                .ToListAsync();

            decimal totalRevenue = transactions.Sum(t => t.SalePrice);
            int totalTransactions = transactions.Count;

            var bestSellingProductQuery = transactions
                .Where(t => t.Product != null)
                .GroupBy(t => t.Product.Name)
                .Select(group => new
                {
                    ProductName = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            var monthlyRevenue = transactions
                .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
                .Select(g => new
                {
                    MonthYear = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Revenue = g.Sum(t => t.SalePrice)
                })
                .OrderBy(x => x.MonthYear)
                .ToList();

            var machines = await _context.Machines
                                         .Include(m => m.Slots)
                                         .ToListAsync();

            const int STOCK_LOW_THRESHOLD = 25;
            var machineStatusList = new List<MachineStatusViewModel>();

            foreach (var machine in machines)
            {
                int totalQuantity = machine.Slots.Sum(s => s.Quantity);
                int totalCapacity = machine.Slots.Sum(s => s.Capacity);
                int fillPercentage = (totalCapacity > 0) ? (int)Math.Round(((double)totalQuantity / totalCapacity) * 100) : 100;

                machineStatusList.Add(new MachineStatusViewModel
                {
					Id = machine.Id,
					Name = machine.Name,
					Location = machine.Location,
					Status = machine.Status,
					LastContact = machine.LastContact,
					FillPercentage = fillPercentage,
					IsStockLow = fillPercentage < STOCK_LOW_THRESHOLD,
					Latitude = machine.Latitude,
					Longitude = machine.Longitude
				});
            }

            var viewModel = new DashboardViewModel
            {
                StartDate = filterStart,
                EndDate = filterEnd,

                TotalRevenue = totalRevenue,
                TotalTransactions = totalTransactions,
                BestSellingProduct = bestSellingProductQuery?.ProductName ?? "Brak danych",
                BestSellingProductCount = bestSellingProductQuery?.Count ?? 0,
                ChartLabels = monthlyRevenue.Select(x => x.MonthYear.ToString("MMMM yyyy", new CultureInfo("pl-PL"))).ToList(),
                ChartData = monthlyRevenue.Select(x => x.Revenue).ToList(),

                MachineStatuses = machineStatusList
            };

            return View(viewModel);
        }
    }
}
