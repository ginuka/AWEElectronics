﻿using Microsoft.AspNetCore.Identity;

namespace AWEElectronics.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string IdentityUserId { get; set; } // Foreign key
        public IdentityUser IdentityUser { get; set; } // Navigation property
    }
}
