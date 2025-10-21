using System;
using System.ComponentModel.DataAnnotations;

namespace RichIZ.Models
{
    public class FinancialGoal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        [Required]
        public DateTime TargetDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public GoalCategory Category { get; set; }

        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        // 계산된 속성
        public decimal Progress => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;
        public int DaysRemaining => (TargetDate - DateTime.Now).Days;
    }

    public enum GoalCategory
    {
        Emergency,      // 비상금
        House,          // 주택구매
        Car,            // 자동차
        Education,      // 교육
        Retirement,     // 노후대비
        Vacation,       // 휴가
        Other           // 기타
    }
}
