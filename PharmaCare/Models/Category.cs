﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaCare.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Category Name")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
