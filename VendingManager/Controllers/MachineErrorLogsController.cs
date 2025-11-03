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
    public class MachineErrorLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MachineErrorLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MachineErrorLogs
        public async Task<IActionResult> Index()
        {
            var logs = _context.MachineErrorLogs
                               .Include(m => m.Machine) // Dołącz nazwę maszyny
                               .OrderByDescending(m => m.Timestamp); // Najnowsze na górze

            return View(await logs.ToListAsync());
        }

        // GET: MachineErrorLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineErrorLog = await _context.MachineErrorLogs
                .Include(m => m.Machine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machineErrorLog == null)
            {
                return NotFound();
            }

            return View(machineErrorLog);
        }

        // GET: MachineErrorLogs/Create
        public IActionResult Create()
        {
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Name");
            return View();
        }

        // POST: MachineErrorLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MachineId,Timestamp,ErrorCode,Message")] MachineErrorLog machineErrorLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(machineErrorLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Name", machineErrorLog.MachineId);
            return View(machineErrorLog);
        }

        // GET: MachineErrorLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineErrorLog = await _context.MachineErrorLogs.FindAsync(id);
            if (machineErrorLog == null)
            {
                return NotFound();
            }
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Name", machineErrorLog.MachineId);
            return View(machineErrorLog);
        }

        // POST: MachineErrorLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MachineId,Timestamp,ErrorCode,Message")] MachineErrorLog machineErrorLog)
        {
            if (id != machineErrorLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(machineErrorLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineErrorLogExists(machineErrorLog.Id))
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
            ViewData["MachineId"] = new SelectList(_context.Machines, "Id", "Name", machineErrorLog.MachineId);
            return View(machineErrorLog);
        }

        // GET: MachineErrorLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineErrorLog = await _context.MachineErrorLogs
                .Include(m => m.Machine)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (machineErrorLog == null)
            {
                return NotFound();
            }

            return View(machineErrorLog);
        }

        // POST: MachineErrorLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machineErrorLog = await _context.MachineErrorLogs.FindAsync(id);
            if (machineErrorLog != null)
            {
                _context.MachineErrorLogs.Remove(machineErrorLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineErrorLogExists(int id)
        {
            return _context.MachineErrorLogs.Any(e => e.Id == id);
        }
    }
}
