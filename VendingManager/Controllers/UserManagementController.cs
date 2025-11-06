// Plik: Controllers/UserManagementController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VendingManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using VendingManager.ViewModels;

namespace VendingManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /UserManagement
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // GET: /UserManagement/Details/xxxxx
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: /UserManagement/Delete/xxxxx
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /UserManagement/Delete/xxxxx
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /UserManagement/ManageRoles/xxxxx
        public async Task<IActionResult> ManageRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? "Brak Nazwy"
            };

            var allRoles = await _roleManager.Roles.ToListAsync();
            foreach (var role in allRoles)
            {
                var checkbox = new RoleCheckboxViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name ?? "Brak Nazwy Roli"
                };

                checkbox.IsSelected = await _userManager.IsInRoleAsync(user, role.Name ?? "");

                viewModel.Roles.Add(checkbox);
            }

            return View(viewModel);
        }

        // POST: /UserManagement/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel viewModel)
        {
            var user = await _userManager.FindByIdAsync(viewModel.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var newRoles = viewModel.Roles
                .Where(r => r.IsSelected)
                .Select(r => r.RoleName);

            var rolesToAdd = newRoles.Except(userRoles);
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            var rolesToRemove = userRoles.Except(newRoles);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            return RedirectToAction(nameof(Index));
        }
    }
}