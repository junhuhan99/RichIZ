using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RichIZ.Services
{
    public class AIAnalysisService
    {
        /// <summary>
        /// AI 기반 투자 포트폴리오 분석
        /// </summary>
        public string AnalyzeInvestmentPortfolio(List<Investment> investments)
        {
            if (!investments.Any())
                return "투자 자산이 없습니다. 투자를 시작해보세요!";

            var sb = new StringBuilder();
            sb.AppendLine("🤖 AI 투자 포트폴리오 분석 결과\n");

            // 총 자산 및 수익률
            var totalInvested = investments.Sum(i => i.TotalInvested);
            var totalCurrent = investments.Sum(i => i.CurrentValue);
            var totalReturn = totalCurrent - totalInvested;
            var returnRate = totalInvested > 0 ? (totalReturn / totalInvested) * 100 : 0;

            sb.AppendLine($"📊 포트폴리오 현황:");
            sb.AppendLine($"  • 총 투자금: {totalInvested:N0}원");
            sb.AppendLine($"  • 현재 가치: {totalCurrent:N0}원");
            sb.AppendLine($"  • 총 수익: {totalReturn:N0}원 ({returnRate:N2}%)");
            sb.AppendLine();

            // 자산 분산 분석
            var typeDistribution = investments.GroupBy(i => i.Type)
                .Select(g => new { Type = g.Key, Percentage = (g.Sum(i => i.CurrentValue) / totalCurrent) * 100 })
                .OrderByDescending(x => x.Percentage);

            sb.AppendLine("📈 자산 분산 현황:");
            foreach (var dist in typeDistribution)
            {
                sb.AppendLine($"  • {dist.Type}: {dist.Percentage:N1}%");
            }
            sb.AppendLine();

            // AI 추천
            sb.AppendLine("💡 AI 추천 사항:");

            // 분산 투자 권장
            var maxTypePercentage = typeDistribution.First().Percentage;
            if (maxTypePercentage > 60)
            {
                sb.AppendLine("  ⚠️ 특정 자산에 집중도가 높습니다. 분산 투자를 권장합니다.");
            }
            else if (maxTypePercentage < 30)
            {
                sb.AppendLine("  ✅ 우수한 분산 투자 포트폴리오입니다!");
            }

            // 수익률 분석
            if (returnRate > 10)
            {
                sb.AppendLine("  ✅ 훌륭한 수익률입니다! 현재 전략을 유지하세요.");
            }
            else if (returnRate < -10)
            {
                sb.AppendLine("  ⚠️ 손실이 큽니다. 포트폴리오 재조정을 고려하세요.");
            }

            // 개별 자산 분석
            var losingAssets = investments.Where(i => i.ReturnRate < -15).ToList();
            if (losingAssets.Any())
            {
                sb.AppendLine($"  ⚠️ {losingAssets.Count}개 자산이 15% 이상 손실 중입니다. 손절 고려 필요.");
            }

            return sb.ToString();
        }

        /// <summary>
        /// AI 기반 소비/지출 패턴 분석
        /// </summary>
        public string AnalyzeSpendingPattern(List<Transaction> transactions)
        {
            if (!transactions.Any())
                return "거래 내역이 없습니다. 수입과 지출을 기록해주세요!";

            var sb = new StringBuilder();
            sb.AppendLine("🤖 AI 소비 패턴 분석 결과\n");

            var expenses = transactions.Where(t => t.Type == TransactionType.Expense).ToList();
            var income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var totalExpense = expenses.Sum(t => t.Amount);

            sb.AppendLine($"💰 수입/지출 현황:");
            sb.AppendLine($"  • 총 수입: {income:N0}원");
            sb.AppendLine($"  • 총 지출: {totalExpense:N0}원");
            sb.AppendLine($"  • 저축액: {income - totalExpense:N0}원");
            sb.AppendLine($"  • 저축률: {(income > 0 ? ((income - totalExpense) / income) * 100 : 0):N1}%");
            sb.AppendLine();

            // 카테고리별 지출
            var categoryExpenses = expenses.GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount), Percentage = (g.Sum(t => t.Amount) / totalExpense) * 100 })
                .OrderByDescending(x => x.Amount)
                .Take(5);

            sb.AppendLine("📊 주요 지출 카테고리 (Top 5):");
            foreach (var cat in categoryExpenses)
            {
                sb.AppendLine($"  • {cat.Category}: {cat.Amount:N0}원 ({cat.Percentage:N1}%)");
            }
            sb.AppendLine();

            // AI 추천
            sb.AppendLine("💡 AI 분석 및 추천:");

            var savingsRate = income > 0 ? ((income - totalExpense) / income) * 100 : 0;
            if (savingsRate < 20)
            {
                sb.AppendLine("  ⚠️ 저축률이 낮습니다. 지출 절감을 권장합니다.");
                sb.AppendLine($"  → 권장 저축률: 최소 20% (월 {income * 0.2m:N0}원)");
            }
            else if (savingsRate >= 30)
            {
                sb.AppendLine("  ✅ 우수한 저축 습관입니다! 계속 유지하세요.");
            }

            // 가장 큰 지출 카테고리 분석
            var topCategory = categoryExpenses.First();
            if (topCategory.Percentage > 40)
            {
                sb.AppendLine($"  ⚠️ '{topCategory.Category}' 지출이 과도합니다 ({topCategory.Percentage:N1}%)");
                sb.AppendLine($"  → 해당 카테고리 지출을 30% 이하로 줄여보세요.");
            }

            return sb.ToString();
        }

        /// <summary>
        /// AI 기반 예산 최적화 추천
        /// </summary>
        public string RecommendBudgetOptimization(List<Budget> budgets, List<Transaction> transactions)
        {
            var sb = new StringBuilder();
            sb.AppendLine("🤖 AI 예산 최적화 추천\n");

            if (!budgets.Any())
            {
                sb.AppendLine("예산이 설정되지 않았습니다.");
                sb.AppendLine("💡 월별 예산을 설정하여 지출을 관리해보세요!");
                return sb.ToString();
            }

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var monthlyBudgets = budgets.Where(b => b.Month == currentMonth && b.Year == currentYear).ToList();
            var monthlyExpenses = transactions
                .Where(t => t.Date.Month == currentMonth && t.Date.Year == currentYear && t.Type == TransactionType.Expense)
                .ToList();

            sb.AppendLine("📊 이번 달 예산 현황:");

            foreach (var budget in monthlyBudgets)
            {
                var spent = monthlyExpenses.Where(e => e.Category == budget.Category).Sum(e => e.Amount);
                var percentage = budget.Amount > 0 ? (spent / budget.Amount) * 100 : 0;
                var status = percentage > 90 ? "⚠️" : percentage > 70 ? "⚡" : "✅";

                sb.AppendLine($"  {status} {budget.Category}: {spent:N0}원 / {budget.Amount:N0}원 ({percentage:N1}%)");
            }
            sb.AppendLine();

            sb.AppendLine("💡 최적화 제안:");
            sb.AppendLine("  • 예산 초과 카테고리는 다음 달 지출을 줄이세요");
            sb.AppendLine("  • 예산 여유 카테고리는 저축으로 전환하세요");
            sb.AppendLine("  • 50/30/20 규칙 적용: 필수 50%, 선택 30%, 저축 20%");

            return sb.ToString();
        }

        /// <summary>
        /// AI 종합 재무 추천
        /// </summary>
        public string GenerateFinancialRecommendation(
            List<Investment> investments,
            List<Transaction> transactions,
            List<BankAccount> bankAccounts,
            List<FinancialGoal> goals)
        {
            var sb = new StringBuilder();
            sb.AppendLine("🤖 AI 종합 재무 분석 및 추천\n");

            // 총 자산 계산
            var totalAssets = investments.Sum(i => i.CurrentValue) +
                            bankAccounts.Sum(b => b.Balance);
            var monthlyIncome = transactions
                .Where(t => t.Date.Month == DateTime.Now.Month && t.Type == TransactionType.Income)
                .Sum(t => t.Amount);
            var monthlyExpense = transactions
                .Where(t => t.Date.Month == DateTime.Now.Month && t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            sb.AppendLine($"💼 재무 현황 요약:");
            sb.AppendLine($"  • 총 자산: {totalAssets:N0}원");
            sb.AppendLine($"  • 월 수입: {monthlyIncome:N0}원");
            sb.AppendLine($"  • 월 지출: {monthlyExpense:N0}원");
            sb.AppendLine($"  • 월 저축: {monthlyIncome - monthlyExpense:N0}원");
            sb.AppendLine();

            sb.AppendLine("🎯 맞춤형 추천 사항:");

            // 비상금 체크
            var liquidAssets = bankAccounts.Where(b => b.AccountType == AccountType.Checking ||
                                                      b.AccountType == AccountType.Savings)
                                          .Sum(b => b.Balance);
            var emergencyFund = monthlyExpense * 6;

            if (liquidAssets < emergencyFund)
            {
                sb.AppendLine($"  1️⃣ 비상금 확보 필요: 현재 {liquidAssets:N0}원 → 목표 {emergencyFund:N0}원");
            }
            else
            {
                sb.AppendLine($"  1️⃣ ✅ 충분한 비상금 확보됨 ({liquidAssets:N0}원)");
            }

            // 투자 다각화
            if (investments.Any())
            {
                var investmentRatio = (investments.Sum(i => i.CurrentValue) / totalAssets) * 100;
                if (investmentRatio < 20)
                {
                    sb.AppendLine($"  2️⃣ 투자 비중 확대 권장: 현재 {investmentRatio:N1}% → 목표 20-40%");
                }
                else if (investmentRatio > 70)
                {
                    sb.AppendLine($"  2️⃣ 투자 비중이 높습니다: {investmentRatio:N1}% → 안전 자산 확보 권장");
                }
            }
            else
            {
                sb.AppendLine("  2️⃣ 투자를 시작하여 자산을 증식하세요!");
            }

            // 목표 달성
            if (goals.Any())
            {
                var nearestGoal = goals.Where(g => !g.IsCompleted)
                                      .OrderBy(g => g.TargetDate)
                                      .FirstOrDefault();
                if (nearestGoal != null)
                {
                    var monthlyRequired = nearestGoal.DaysRemaining > 0 ?
                        (nearestGoal.TargetAmount - nearestGoal.CurrentAmount) / (nearestGoal.DaysRemaining / 30m) : 0;
                    sb.AppendLine($"  3️⃣ '{nearestGoal.Title}' 달성을 위해 월 {monthlyRequired:N0}원 저축 필요");
                }
            }

            sb.AppendLine();
            sb.AppendLine("🌟 장기 재무 전략:");
            sb.AppendLine("  • 자산 배분: 안전 자산 40%, 성장 자산 40%, 투기 자산 20%");
            sb.AppendLine("  • 노후 대비: 월 소득의 10-15%를 연금 저축");
            sb.AppendLine("  • 정기적인 포트폴리오 리밸런싱 (분기별)");

            return sb.ToString();
        }
    }
}
