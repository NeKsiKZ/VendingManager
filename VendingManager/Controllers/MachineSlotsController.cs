using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VendingManager.Data;
using VendingManager.Models;

namespace VendingManager.Controllers
{
    public class MachineSlotsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MachineSlotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MachineSlots
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MachineSlots.Include(m => m.Machine).Include(m => m.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MachineSlots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineSlot = await _context.MachineSlots
                .Include(m => m.Machine)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machineSlot == null)
            {
                return NotFound();
            }

            return View(machineSlot);
        }

        // GET: MachineSlots/Create
        public IActionResult Create()
        {
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: MachineSlots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MachineId,ProductId,Quantity,Capacity")] MachineSlot machineSlot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(machineSlot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Name", machineSlot.MachineId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", machineSlot.ProductId);
            return View(machineSlot);
        }

        // GET: MachineSlots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineSlot = await _context.MachineSlots.FindAsync(id);
            if (machineSlot == null)
            {
                return NotFound();
            }
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Name", machineSlot.MachineId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", machineSlot.ProductId);
            return View(machineSlot);
        }

        // POST: MachineSlots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MachineId,ProductId,Quantity,Capacity")] MachineSlot machineSlot)
        {
            if (id != machineSlot.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(machineSlot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineSlotExists(machineSlot.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Name", machineSlot.MachineId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", machineSlot.ProductId);
            return View(machineSlot);
        }

        // GET: MachineSlots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineSlot = await _context.MachineSlots
                .Include(m => m.Machine)
                .Include(m => m.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machineSlot == null)
            {
                return NotFound();
            }

            return View(machineSlot);
        }

        // POST: MachineSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machineSlot = await _context.MachineSlots.FindAsync(id);
            if (machineSlot != null)
            {
                _context.MachineSlots.Remove(machineSlot);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineSlotExists(int id)
        {
            return _context.MachineSlots.Any(e => e.Id == id);
        }
    }
}
