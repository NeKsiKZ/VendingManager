using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VendingManager.Data;
using VendingManager.ViewModels;

namespace VendingManager.Controllers
{
    [Authorize]
    public class RestockingController : Controller
    {
        private readonly ApplicationDbContext _context;

        private const double RESTOCK_THRESHOLD_PERCENT = 0.25;

        public RestockingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var slotsToRestock = await _context.MachineSlots
                .Include(s => s.Machine)
                .Include(s => s.Product)
                .Where(s => s.Capacity > 0 && (double)s.Quantity / s.Capacity < RESTOCK_THRESHOLD_PERCENT)
                .ToListAsync();

            var calculatedSlots = slotsToRestock.Select(s => new
            {
                s.MachineId,
                MachineName = s.Machine.Name,
                MachineLocation = s.Machine.Location,
                ProductId = s.Product.Id,
                ProductName = s.Product.Name,
                CurrentQuantity = s.Quantity,
                s.Capacity,
                AmountToRefill = s.Capacity - s.Quantity
            }).ToList();

            var shoppingList = calculatedSlots
                .GroupBy(s => s.ProductName)
                .Select(group => new ShoppingListItem
                {
                    ProductName = group.Key,
                    TotalNeeded = group.Sum(item => item.AmountToRefill)
                })
                .OrderByDescending(item => item.TotalNeeded)
                .ToList();

            var routeList = calculatedSlots
                .GroupBy(s => s.MachineId)
                .Select(group => new RestockRouteItem
                {
                    MachineId = group.Key,
                    MachineName = group.First().MachineName,
                    MachineLocation = group.First().MachineLocation,
                    SlotsToRestock = group.Select(item => new SlotToRestock
                    {
                        ProductName = item.ProductName,
                        CurrentQuantity = item.CurrentQuantity,
                        Capacity = item.Capacity,
                        AmountToRefill = item.AmountToRefill
                    }).ToList()
                })
                .ToList();

            var viewModel = new RestockingViewModel
            {
                ShoppingList = shoppingList,
                RouteList = routeList
            };

            return View(viewModel);
        }
    }
}
