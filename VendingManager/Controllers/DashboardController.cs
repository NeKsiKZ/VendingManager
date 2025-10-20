using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VendingManager.Data;
using VendingManager.Models;

namespace VendingManager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Machine> allMachines = await _context.Machines.ToListAsync();
            
            return View(allMachines);
        }
    }
}
