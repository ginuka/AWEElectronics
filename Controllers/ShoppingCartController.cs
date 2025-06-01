using AWEElectronics.Data;
using AWEElectronics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AWEElectronics.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /ShoppingCart
        public async Task<IActionResult> Index()
        {
            var customer = new Customer();
            try
            {
                customer = await GetCurrentCustomerAsync();
            }
            catch
            {
                TempData["Error"] = "Please complete your shipping details.";
                return LocalRedirect("/Identity/Account/Manage");
            }

            // Check if shipping/customer details are complete
            if (string.IsNullOrWhiteSpace(customer.Name) ||
                string.IsNullOrWhiteSpace(customer.Address) ||
                string.IsNullOrWhiteSpace(customer.City) ||
                string.IsNullOrWhiteSpace(customer.State) ||
                string.IsNullOrWhiteSpace(customer.ZipCode))
            {
                TempData["Error"] = "Please complete your shipping details before proceeding to checkout.";
                return LocalRedirect("/Identity/Account/Manage");
            }

            // Load shopping cart with items and product info
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customer.Id);

            if (cart == null)
            {
                // Create a new cart for the user if not exists
                cart = new ShoppingCart
                {
                    CustomerId = customer.Id
                };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            var customer = new Customer();
            try
            {
                customer = await GetCurrentCustomerAsync();
            }
            catch
            {
                TempData["Error"] = "Please complete your shipping details.";
                return LocalRedirect("/Identity/Account/Manage");
            }

            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customer.Id);

            if (cart == null)
            {
                cart = new ShoppingCart { CustomerId = customer.Id };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == id);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return NotFound();

                var item = new ShoppingCartItem
                {
                    ProductId = id,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    ShoppingCartId = cart.Id
                };
                _context.ShoppingCartItems.Add(item);
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Product added to cart!";
            return RedirectToAction("Details", "Products", new { id });
        }

        // POST: /ShoppingCart/Remove/5  (5 = cart item id)
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var cartItem = await _context.ShoppingCartItems.FindAsync(id);
            if (cartItem == null)
                return NotFound();

            _context.ShoppingCartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /ShoppingCart/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var customer = new Customer();
            try
            {
                customer = await GetCurrentCustomerAsync();
            }
            catch
            {
                TempData["Error"] = "Please complete your shipping details.";
                return LocalRedirect("/Identity/Account/Manage");
            }

            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customer.Id);

            if (cart != null)
            {
                _context.ShoppingCartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper to get current user/customer id
        private int GetCurrentUserId()
        {
            // Assuming your CustomerId is mapped to the logged in user’s Id (e.g., from Claims)
            // Modify this as per your authentication and customer-user mapping
            // For example, if User.Identity.Name stores username, fetch CustomerId from DB accordingly.

            // Here's a placeholder, replace with your actual logic:
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "CustomerId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var customerId))
                return customerId;

            // Alternatively, if your app user id is string, map it to CustomerId
            // throw or return 0 if unauthenticated or unknown
            throw new UnauthorizedAccessException("User is not logged in or CustomerId claim missing.");
        }

        private async Task<Customer> GetCurrentCustomerAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) throw new Exception("User not logged in.");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);

            if (customer == null) throw new Exception("No customer profile found for this user.");

            return customer;
        }
    }
}
