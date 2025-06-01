using AWEElectronics.Data;
using AWEElectronics.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AWEElectronics.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
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

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.CustomerId == customer.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }


        public async Task<IActionResult> Details(int id)
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
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customer.Id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: /Orders/Checkout
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            // Get current user/customer id from claims or session
            var customer = await GetCurrentCustomerAsync();

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

            // Load shopping cart with items
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customer.Id);

            if (cart == null || cart.Items.Count == 0)
            {
                ModelState.AddModelError("", "Your shopping cart is empty.");
                return RedirectToAction("Index", "ShoppingCart");
            }

            // Create order and copy cart items to order items
            var order = new Order
            {
                CustomerId = customer.Id,
                OrderDate = System.DateTime.Now,
                Status = "Pending",
            };

            foreach (var cartItem in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice
                });
            }

            // Add order to database
            _context.Orders.Add(order);

            // Clear shopping cart
            _context.ShoppingCartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();

            // Optionally: redirect to order confirmation page
            return RedirectToAction("Details", new { id = order.Id });
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


        public async Task<IActionResult> Invoice(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            var model = new OrderInvoiceViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                CustomerName = order.Customer.Name,
                CustomerEmail = order.Customer.Email,
                Items = order.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            return View(model);
        }
    }
}
