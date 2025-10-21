using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichIZ.Data;
using RichIZ.Services;
using System.Linq;

namespace RichIZ.ViewModels
{
    public partial class AIViewModel : ObservableObject
    {
        private readonly AIAnalysisService _aiService = new();

        [ObservableProperty]
        private string portfolioAnalysis = string.Empty;

        [ObservableProperty]
        private string spendingAnalysis = string.Empty;

        [ObservableProperty]
        private string budgetRecommendation = string.Empty;

        [ObservableProperty]
        private string financialRecommendation = string.Empty;

        [ObservableProperty]
        private bool isAnalyzing;

        public AIViewModel()
        {
            LoadAnalyses();
        }

        [RelayCommand]
        private async Task AnalyzeAll()
        {
            IsAnalyzing = true;

            await Task.Run(() =>
            {
                // JSON DataStore 사용

                var investments = JsonDataStore.LoadInvestments().ToList();
                var transactions = JsonDataStore.LoadTransactions().ToList();
                var budgets = JsonDataStore.LoadBudgets().ToList();
                var bankAccounts = JsonDataStore.LoadBankAccounts().ToList();
                var goals = JsonDataStore.LoadFinancialGoals().ToList();

                PortfolioAnalysis = _aiService.AnalyzeInvestmentPortfolio(investments);
                SpendingAnalysis = _aiService.AnalyzeSpendingPattern(transactions);
                BudgetRecommendation = _aiService.RecommendBudgetOptimization(budgets, transactions);
                FinancialRecommendation = _aiService.GenerateFinancialRecommendation(
                    investments, transactions, bankAccounts, goals);
            });

            IsAnalyzing = false;
        }

        [RelayCommand]
        private void LoadAnalyses()
        {
            // JSON DataStore 사용

            var investments = JsonDataStore.LoadInvestments().ToList();
            var transactions = JsonDataStore.LoadTransactions().ToList();
            var budgets = JsonDataStore.LoadBudgets().ToList();
            var bankAccounts = JsonDataStore.LoadBankAccounts().ToList();
            var goals = JsonDataStore.LoadFinancialGoals().ToList();

            if (investments.Any())
                PortfolioAnalysis = _aiService.AnalyzeInvestmentPortfolio(investments);
            else
                PortfolioAnalysis = "투자 데이터가 없습니다. 먼저 투자 자산을 추가해주세요.";

            if (transactions.Any())
                SpendingAnalysis = _aiService.AnalyzeSpendingPattern(transactions);
            else
                SpendingAnalysis = "거래 데이터가 없습니다. 수입과 지출을 기록해주세요.";

            BudgetRecommendation = _aiService.RecommendBudgetOptimization(budgets, transactions);
            FinancialRecommendation = _aiService.GenerateFinancialRecommendation(
                investments, transactions, bankAccounts, goals);
        }
    }
}
