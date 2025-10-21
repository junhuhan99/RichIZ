using System;
using System.ComponentModel.DataAnnotations;

namespace RichIZ.Models
{
    public class AIAnalysis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public AnalysisType Type { get; set; }

        [Required]
        public string Analysis { get; set; } = string.Empty;

        public string? Recommendation { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string? AdditionalData { get; set; }
    }

    public enum AnalysisType
    {
        InvestmentPortfolio,  // 투자 포트폴리오 분석
        SpendingPattern,      // 소비 패턴 분석
        BudgetOptimization,   // 예산 최적화
        GeneralRecommendation // 일반 추천
    }
}
