﻿using System.ComponentModel.DataAnnotations;

namespace ABC_MVC.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [DataType(DataType.Url)]
        public string ImageUrl { get; set; }
    }
}
