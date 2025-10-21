using System;
using System.ComponentModel.DataAnnotations;

namespace RichIZ.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public enum TransactionType
    {
        Income,  // 수입
        Expense  // 지출
    }
}
