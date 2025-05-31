using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AWEElectronics.Data;
using AWEElectronics.Models;
using AWEElectronics.ViewModel;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AWEElectronics.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ProductsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var productViewModel = new List<ProductViewModel>();
            var result = await _context.Products.ToListAsync();

            foreach(var item in result)
            {
                var ViewModel = new ProductViewModel();
                ViewModel.Id = item.Id;
                ViewModel.Name = item.Name;
                ViewModel.Price = item.Price;
                ViewModel.Availability = item.Availability;
                ViewModel.ImageBytes = item.Image;

                productViewModel.Add(ViewModel);
            }

            return View(productViewModel);
        }

        // GET: Products/Details/5
        [AllowAnonymous]
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

            var addproduct = await _context.ProductGroups.FindAsync(product.ProductGroupId);

            var productGroups = _context.ProductGroups
                .Select(pg => new SelectListItem
                {
                    Value = pg.Id.ToString(),
                    Text = pg.Name
                }).ToList();

            var productViewModel = new ProductViewModel();
            productViewModel.Id = product.Id;
            productViewModel.Name = product.Name;
            productViewModel.Price = product.Price;
            productViewModel.Availability = product.Availability;
            productViewModel.ImageBytes = product.Image;

            productViewModel.ProductGroupId = addproduct == null ? 0 : addproduct.Id;
            productViewModel.ProductGroups = productGroups;
            productViewModel.ProductGroupName = addproduct?.Name;

            return View(productViewModel);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var groups = _context.ProductGroups
                .Select(pg => new SelectListItem
                {
                    Value = pg.Id.ToString(),
                    Text = pg.Name
                })
                .ToList();

            var viewModel = new ProductViewModel
            {
                ProductGroups = groups
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                byte[] imageData = null;
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.ImageFile.CopyToAsync(ms);
                        imageData = ms.ToArray();
                    }
                }

                var product = new Product
                {
                    Id = model.Id,
                    Name = model.Name,
                    Price = model.Price,
                    Availability = model.Availability,
                    Image = imageData,
                    ProductGroupId = model.ProductGroupId
                };

                _context.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
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

            var addproduct = await _context.ProductGroups.FindAsync(product.ProductGroupId);
            //if (addproduct == null) return NotFound();

            var productGroups = _context.ProductGroups
                .Select(pg => new SelectListItem
                {
                    Value = pg.Id.ToString(),
                    Text = pg.Name
                }).ToList();

            var productViewModel = new ProductViewModel();
            productViewModel.Id = product.Id;
            productViewModel.Name = product.Name;
            productViewModel.Price = product.Price;
            productViewModel.Availability = product.Availability;
            productViewModel.ImageBytes = product.Image;

            productViewModel.ProductGroupId = addproduct == null ? 0: addproduct.Id;
            productViewModel.ProductGroups = productGroups;
            productViewModel.ProductGroupName = addproduct?.Name;

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return NotFound();

                product.Name = model.Name;
                product.Price = model.Price;
                product.Availability = model.Availability;
                product.ProductGroupId = model.ProductGroupId;


                // If new image uploaded, replace it
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.ImageFile.CopyToAsync(ms);
                        product.Image = ms.ToArray();
                    }
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }



        [HttpGet]
        public IActionResult Search(string term)
        {
            var products = _context.Products
                .Where(p => string.IsNullOrEmpty(term) || p.Name.Contains(term))
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Availability = p.Availability,
                    ImageBytes = p.Image // byte[] from DB
                })
                .ToList();

            return Ok(products);
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

            var addproduct = await _context.ProductGroups.FindAsync(product.ProductGroupId);

            var productGroups = _context.ProductGroups
                .Select(pg => new SelectListItem
                {
                    Value = pg.Id.ToString(),
                    Text = pg.Name
                }).ToList();

            var productViewModel = new ProductViewModel();
            productViewModel.Id = product.Id;
            productViewModel.Name = product.Name;
            productViewModel.Price = product.Price;
            productViewModel.Availability = product.Availability;
            productViewModel.ImageBytes = product.Image;

            productViewModel.ProductGroupId = addproduct == null ? 0 : addproduct.Id;
            productViewModel.ProductGroups = productGroups;
            productViewModel.ProductGroupName = addproduct?.Name;

            return View(productViewModel);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        


        
    }


}
