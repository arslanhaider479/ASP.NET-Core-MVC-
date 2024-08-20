using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CustomIdentity.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string Brand { get; set; } = "";

        [MaxLength(100)]
        public string Category { get; set; } = "";

        [Required]
        [Precision(16,2)]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = "";
        public string ImageFileName { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}

