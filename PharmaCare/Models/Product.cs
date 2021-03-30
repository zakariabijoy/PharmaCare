using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaCare.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(1, 10000)]
        public double ListPrice { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price50 { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price100 { get; set; }
        public string ImageUrl { get; set; }

        // navigation property or relationship making properties

        [Required]
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }


    }
}
