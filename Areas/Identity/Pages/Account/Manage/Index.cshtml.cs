using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AWEElectronics.Data;
using AWEElectronics.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AWEElectronics.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Full Name")]
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            var customer = await _context.Customers
        .FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Name = customer?.Name,
                Address = customer?.Address,
                City = customer?.City,
                State = customer?.State,
                ZipCode = customer?.ZipCode
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // 🔽 Insert or Update Customer info
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);

            if (customer == null)
            {
                customer = new Customer
                {
                    IdentityUserId = user.Id,
                    Phone = Input.PhoneNumber,
                    Email = user.Email,
                    Name = user.UserName,
                    Address = Input.Address,
                    City = Input.City,
                    State = Input.State,
                    ZipCode = Input.ZipCode
                };
                _context.Customers.Add(customer);
            }
            else
            {
                customer.Phone = Input.PhoneNumber;
                customer.Email = user.Email;
                customer.Name = user.UserName;
                customer.Address = Input.Address;
                customer.City = Input.City;
                customer.State = Input.State;
                customer.ZipCode = Input.ZipCode;
                _context.Customers.Update(customer);
            }

            await _context.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
