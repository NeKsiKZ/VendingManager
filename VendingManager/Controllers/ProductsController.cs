using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VendingManager.Data;
using VendingManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace VendingManager.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
			_webHostEnvironment = webHostEnvironment;
		}

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price")] Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
				string? imageUrl = await SaveImage(imageFile);
				if (imageUrl != null)
				{
					product.ImageUrl = imageUrl;
				}

				_context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price")] Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					var productToUpdate = await _context.Products.FindAsync(id);
					if (productToUpdate == null)
					{
						return NotFound();
					}

					productToUpdate.Name = product.Name;
					productToUpdate.Description = product.Description;
					productToUpdate.Price = product.Price;

					if (imageFile != null)
					{

						DeleteImage(productToUpdate.ImageUrl);
						productToUpdate.ImageUrl = await SaveImage(imageFile);
					}

					await _context.SaveChangesAsync();
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
				DeleteImage(product.ImageUrl);
				_context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

		private async Task<string?> SaveImage(IFormFile? imageFile)
		{
			if (imageFile == null || imageFile.Length == 0)
			{
				return null;
			}

			string wwwRootPath = _webHostEnvironment.WebRootPath;
			string uploadPath = Path.Combine(wwwRootPath, "images", "products");

			if (!Directory.Exists(uploadPath))
			{
				Directory.CreateDirectory(uploadPath);
			}

			string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
			string filePath = Path.Combine(uploadPath, fileName);

			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await imageFile.CopyToAsync(fileStream);
			}

			return "/images/products/" + fileName;
		}

		private void DeleteImage(string? imageUrl)
		{
			if (string.IsNullOrEmpty(imageUrl)) return;

			string wwwRootPath = _webHostEnvironment.WebRootPath;
			string filePath = Path.Combine(wwwRootPath, imageUrl.TrimStart('/'));

			if (System.IO.File.Exists(filePath))
			{
				System.IO.File.Delete(filePath);
			}
		}
	}
}
