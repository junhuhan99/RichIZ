using System;
using System.ComponentModel.DataAnnotations;

namespace RichIZ.Models
{
    public class Budget
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        public string? Notes { get; set; }
    }
}
