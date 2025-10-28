using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VendingManager.Data;
using VendingManager.Models;
using VendingManager.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Index()
        {
            var machines = await _context.Machines
                                         .Include(m => m.Slots)
                                         .ToListAsync();

            const int STOCK_LOW_THRESHOLD = 25;

            var viewModelList = new List<MachineStatusViewModel>();

            foreach (var machine in machines)
            {
                int totalQuantity = machine.Slots.Sum(s => s.Quantity);
                int totalCapacity = machine.Slots.Sum(s => s.Capacity);

                int fillPercentage = 100;

                if (totalCapacity > 0)
                {
                    fillPercentage = (int)Math.Round(((double)totalQuantity / totalCapacity) * 100);
                }

                var machineStatus = new MachineStatusViewModel
                {
                    Id = machine.Id,
                    Name = machine.Name,
                    Location = machine.Location,
                    Status = machine.Status,
                    LastContact = machine.LastContact,
                    FillPercentage = fillPercentage,
                    IsStockLow = fillPercentage < STOCK_LOW_THRESHOLD
                };

                viewModelList.Add(machineStatus);
            }

            return View(viewModelList);
        }
    }
}
