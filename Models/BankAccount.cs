using System;
using System.ComponentModel.DataAnnotations;

namespace RichIZ.Models
{
    public class BankAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BankName { get; set; } = string.Empty;

        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        public AccountType AccountType { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public decimal InterestRate { get; set; }

        public string? AccountNickname { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public string? Notes { get; set; }
    }

    public enum AccountType
    {
        Checking,    // 입출금
        Savings,     // 저축
        FixedDeposit,// 정기예금
        InstallmentSavings // 적금
    }
}
