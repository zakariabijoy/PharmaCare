using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaCare.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public IdentityUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 to 1000")]
        public int Count { get; set; }

        [NotMapped]
        public double Price { get; set; }
    }
}
