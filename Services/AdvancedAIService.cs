using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichIZ.Services
{
    public class AdvancedAIService
    {
        /// <summary>
        /// 고급 재무 건강도 분석
        /// </summary>
        public string AnalyzeFinancialHealth()
        {
            using var context = new AppDbContext();

            var sb = new StringBuilder();
            sb.AppendLine("🏥 재무 건강도 종합 분석\n");

            var transactions = context.Transactions.ToList();
            var investments = context.Investments.ToList();
            var bankAccounts = context.BankAccounts.ToList();
            var goals = context.FinancialGoals.ToList();

            // 1. 유동성 분석
            var liquidAssets = bankAccounts.Where(b => b.AccountType == AccountType.Checking || b.AccountType == AccountType.Savings).Sum(b => b.Balance);
            var monthlyExpense = transactions.Where(t => t.Date.Month == DateTime.Now.Month && t.Type == TransactionType.Expense).Sum(t => t.Amount);
            var emergencyMonths = monthlyExpense > 0 ? liquidAssets / monthlyExpense : 0;

            sb.AppendLine("💧 유동성 분석:");
            sb.AppendLine($"  • 유동 자산: {liquidAssets:N0}원");
            sb.AppendLine($"  • 비상금 충족도: {emergencyMonths:N1}개월분");

            if (emergencyMonths < 3)
                sb.AppendLine("  ⚠️ 비상금이 부족합니다. 최소 3개월분 생활비 확보를 권장합니다.");
            else if (emergencyMonths >= 6)
                sb.AppendLine("  ✅ 충분한 비상금을 보유하고 있습니다!");
            else
                sb.AppendLine("  ⚡ 양호한 수준입니다. 6개월분까지 확보하면 더욱 안전합니다.");

            sb.AppendLine();

            // 2. 부채 대비 자산 비율
            var totalAssets = bankAccounts.Sum(b => b.Balance) + investments.Sum(i => i.CurrentValue);
            sb.AppendLine("💰 자산 분석:");
            sb.AppendLine($"  • 총 자산: {totalAssets:N0}원");
            sb.AppendLine($"  • 은행 자산: {bankAccounts.Sum(b => b.Balance):N0}원");
            sb.AppendLine($"  • 투자 자산: {investments.Sum(i => i.CurrentValue):N0}원");

            var assetAllocation = totalAssets > 0 ? (investments.Sum(i => i.CurrentValue) / totalAssets) * 100 : 0;
            sb.AppendLine($"  • 투자 비중: {assetAllocation:N1}%");

            if (assetAllocation < 10)
                sb.AppendLine("  📊 투자 비중이 낮습니다. 자산 증식을 위해 투자를 고려해보세요.");
            else if (assetAllocation > 70)
                sb.AppendLine("  ⚠️ 투자 비중이 높습니다. 안전 자산 확보를 권장합니다.");
            else
                sb.AppendLine("  ✅ 적절한 자산 배분입니다!");

            sb.AppendLine();

            // 3. 저축률 분석 (최근 6개월)
            var last6Months = transactions.Where(t => t.Date >= DateTime.Now.AddMonths(-6)).ToList();
            var avgIncome = last6Months.Where(t => t.Type == TransactionType.Income).GroupBy(t => new { t.Date.Year, t.Date.Month }).Average(g => g.Sum(t => t.Amount));
            var avgExpense = last6Months.Where(t => t.Type == TransactionType.Expense).GroupBy(t => new { t.Date.Year, t.Date.Month }).Average(g => g.Sum(t => t.Amount));
            var savingsRate = avgIncome > 0 ? ((avgIncome - avgExpense) / avgIncome) * 100 : 0;

            sb.AppendLine("💵 저축률 분석 (최근 6개월 평균):");
            sb.AppendLine($"  • 평균 수입: {avgIncome:N0}원");
            sb.AppendLine($"  • 평균 지출: {avgExpense:N0}원");
            sb.AppendLine($"  • 저축률: {savingsRate:N1}%");

            if (savingsRate < 10)
                sb.AppendLine("  ⚠️ 저축률이 매우 낮습니다. 지출 절감이 필요합니다!");
            else if (savingsRate < 20)
                sb.AppendLine("  ⚡ 저축률이 낮은 편입니다. 20% 이상 목표로 개선해보세요.");
            else if (savingsRate >= 30)
                sb.AppendLine("  ✅ 훌륭한 저축 습관입니다! 계속 유지하세요.");
            else
                sb.AppendLine("  ✅ 양호한 저축률입니다!");

            sb.AppendLine();

            // 4. 목표 달성 가능성 분석
            if (goals.Any(g => !g.IsCompleted))
            {
                sb.AppendLine("🎯 목표 달성 가능성:");
                foreach (var goal in goals.Where(g => !g.IsCompleted).Take(3))
                {
                    var remaining = goal.TargetAmount - goal.CurrentAmount;
                    var monthlyRequired = goal.DaysRemaining > 0 ? remaining / (goal.DaysRemaining / 30m) : 0;
                    var monthlySavings = avgIncome - avgExpense;

                    sb.AppendLine($"  • {goal.Title}:");
                    sb.AppendLine($"    - 필요 월 저축: {monthlyRequired:N0}원");
                    sb.AppendLine($"    - 현재 월 저축: {monthlySavings:N0}원");

                    if (monthlyRequired <= monthlySavings)
                        sb.AppendLine("    ✅ 달성 가능!");
                    else
                        sb.AppendLine($"    ⚠️ 월 {monthlyRequired - monthlySavings:N0}원 추가 저축 필요");
                }
            }

            sb.AppendLine();
            sb.AppendLine("📋 종합 건강도 점수:");

            var healthScore = 0;
            if (emergencyMonths >= 3) healthScore += 25;
            if (emergencyMonths >= 6) healthScore += 10;
            if (assetAllocation >= 20 && assetAllocation <= 60) healthScore += 20;
            if (savingsRate >= 20) healthScore += 25;
            if (savingsRate >= 30) healthScore += 10;
            if (totalAssets > avgExpense * 12) healthScore += 10;

            sb.AppendLine($"  점수: {healthScore}/100");

            if (healthScore >= 80)
                sb.AppendLine("  등급: 매우 우수 (A+) 🌟");
            else if (healthScore >= 60)
                sb.AppendLine("  등급: 우수 (A)");
            else if (healthScore >= 40)
                sb.AppendLine("  등급: 양호 (B)");
            else
                sb.AppendLine("  등급: 개선 필요 (C) - 재무 계획을 재검토하세요!");

            return sb.ToString();
        }

        /// <summary>
        /// 투자 리스크 분석
        /// </summary>
        public string AnalyzeInvestmentRisk(List<Investment> investments)
        {
            if (!investments.Any())
                return "투자 자산이 없습니다.";

            var sb = new StringBuilder();
            sb.AppendLine("⚠️ 투자 리스크 분석\n");

            // 자산 유형별 분산
            var typeDistribution = investments.GroupBy(i => i.Type)
                .Select(g => new { Type = g.Key, Percentage = (g.Sum(i => i.CurrentValue) / investments.Sum(i => i.CurrentValue)) * 100 })
                .OrderByDescending(x => x.Percentage);

            sb.AppendLine("📊 자산 유형별 분산:");
            foreach (var dist in typeDistribution)
            {
                sb.AppendLine($"  • {dist.Type}: {dist.Percentage:N1}%");
            }

            var maxConcentration = typeDistribution.First().Percentage;
            if (maxConcentration > 70)
                sb.AppendLine("  ⚠️ 높은 집중 리스크! 분산 투자를 권장합니다.");
            else if (maxConcentration < 40)
                sb.AppendLine("  ✅ 우수한 분산 투자!");

            sb.AppendLine();

            // 손실 자산 분석
            var losingAssets = investments.Where(i => i.ReturnRate < -10).ToList();
            if (losingAssets.Any())
            {
                sb.AppendLine($"⚠️ 주의 필요 자산 ({losingAssets.Count}개):");
                foreach (var asset in losingAssets.Take(5))
                {
                    sb.AppendLine($"  • {asset.Name}: {asset.ReturnRate:N2}% 손실");
                }
                sb.AppendLine("  → 손절 또는 평단가 낮추기 전략 고려");
            }

            sb.AppendLine();

            // 수익 자산 분석
            var profitableAssets = investments.Where(i => i.ReturnRate > 20).ToList();
            if (profitableAssets.Any())
            {
                sb.AppendLine($"🎉 우수 수익 자산 ({profitableAssets.Count}개):");
                foreach (var asset in profitableAssets.Take(5))
                {
                    sb.AppendLine($"  • {asset.Name}: {asset.ReturnRate:N2}% 수익");
                }
                sb.AppendLine("  → 일부 익절 고려");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 미래 자산 예측
        /// </summary>
        public string PredictFutureAssets(int months)
        {
            using var context = new AppDbContext();

            var sb = new StringBuilder();
            sb.AppendLine($"🔮 {months}개월 후 자산 예측\n");

            // 최근 6개월 평균 저축액
            var last6Months = context.Transactions.Where(t => t.Date >= DateTime.Now.AddMonths(-6)).ToList();
            var avgMonthlySavings = last6Months.GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Average(g => g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount) -
                             g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount));

            // 현재 자산
            var currentAssets = context.BankAccounts.Sum(b => b.Balance) + context.Investments.Sum(i => i.CurrentValue);

            // 투자 수익률 (연 5% 가정)
            var investmentAssets = context.Investments.Sum(i => i.CurrentValue);
            var monthlyGrowthRate = 0.05m / 12m;

            sb.AppendLine($"현재 총 자산: {currentAssets:N0}원");
            sb.AppendLine($"월평균 저축: {avgMonthlySavings:N0}원");
            sb.AppendLine($"투자 자산: {investmentAssets:N0}원");
            sb.AppendLine();

            sb.AppendLine("시나리오별 예측:");
            sb.AppendLine();

            // 보수적 시나리오 (투자 수익 0%)
            var conservativePrediction = currentAssets + (avgMonthlySavings * months);
            sb.AppendLine($"1️⃣ 보수적 (투자수익 0%):");
            sb.AppendLine($"   {months}개월 후: {conservativePrediction:N0}원");
            sb.AppendLine($"   증가액: {conservativePrediction - currentAssets:N0}원");
            sb.AppendLine();

            // 중립적 시나리오 (연 5% 수익)
            var neutralPrediction = currentAssets;
            for (int i = 0; i < months; i++)
            {
                neutralPrediction += avgMonthlySavings;
                neutralPrediction += investmentAssets * monthlyGrowthRate;
            }
            sb.AppendLine($"2️⃣ 중립적 (연 5% 수익):");
            sb.AppendLine($"   {months}개월 후: {neutralPrediction:N0}원");
            sb.AppendLine($"   증가액: {neutralPrediction - currentAssets:N0}원");
            sb.AppendLine();

            // 낙관적 시나리오 (연 10% 수익)
            var optimisticGrowthRate = 0.10m / 12m;
            var optimisticPrediction = currentAssets;
            for (int i = 0; i < months; i++)
            {
                optimisticPrediction += avgMonthlySavings;
                optimisticPrediction += investmentAssets * optimisticGrowthRate;
            }
            sb.AppendLine($"3️⃣ 낙관적 (연 10% 수익):");
            sb.AppendLine($"   {months}개월 후: {optimisticPrediction:N0}원");
            sb.AppendLine($"   증가액: {optimisticPrediction - currentAssets:N0}원");

            return sb.ToString();
        }
    }
}
