using System;
using System.ComponentModel.DataAnnotations;

namespace RichIZ.Models
{
    public class Investment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public InvestmentType Type { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal PurchasePrice { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        public decimal CurrentPrice { get; set; }

        public DateTime LastUpdated { get; set; }

        public string? Notes { get; set; }

        // 계산된 속성
        public decimal TotalInvested => Quantity * PurchasePrice;
        public decimal CurrentValue => Quantity * CurrentPrice;
        public decimal ProfitLoss => CurrentValue - TotalInvested;
        public decimal ReturnRate => TotalInvested > 0 ? (ProfitLoss / TotalInvested) * 100 : 0;
    }

    public enum InvestmentType
    {
        Stock,      // 주식
        Fund,       // 펀드
        Bond,       // 채권
        Crypto,     // 암호화폐
        RealEstate, // 부동산
        Other       // 기타
    }
}
