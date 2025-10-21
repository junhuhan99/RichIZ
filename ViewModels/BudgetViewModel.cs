using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RichIZ.ViewModels
{
    public partial class BudgetViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BudgetItem> budgetItems = new();

        [ObservableProperty]
        private string category = string.Empty;

        [ObservableProperty]
        private decimal budgetAmount;

        [ObservableProperty]
        private int selectedMonth = DateTime.Now.Month;

        [ObservableProperty]
        private int selectedYear = DateTime.Now.Year;

        [ObservableProperty]
        private decimal totalBudget;

        [ObservableProperty]
        private decimal totalSpent;

        [ObservableProperty]
        private decimal remaining;

        public ObservableCollection<string> Categories { get; } = new()
        {
            "식비", "교통", "쇼핑", "공과금", "월세", "통신비",
            "의료", "교육", "문화생활", "기타"
        };

        public BudgetViewModel()
        {
            LoadBudgets();
        }

        [RelayCommand]
        private void AddBudget()
        {
            if (string.IsNullOrWhiteSpace(Category) || BudgetAmount <= 0)
                return;

            // JSON DataStore 사용

            // 같은 카테고리와 월이 있는지 확인
            var existing = JsonDataStore.LoadBudgets()
                .FirstOrDefault(b => b.Category == Category &&
                               b.Month == SelectedMonth &&
                               b.Year == SelectedYear);

            if (existing != null)
            {
                existing.Amount = BudgetAmount;
            }
            else
            {
                var budget = new Budget
                {
                    Category = Category,
                    Amount = BudgetAmount,
                    Month = SelectedMonth,
                    Year = SelectedYear
                };
                JsonDataStore.LoadBudgets().Add(budget);
            }

            // 자동 저장됨
            LoadBudgets();
            ClearForm();
        }

        [RelayCommand]
        private void LoadBudgets()
        {
            // JSON DataStore 사용
            var budgets = JsonDataStore.LoadBudgets()
                .Where(b => b.Month == SelectedMonth && b.Year == SelectedYear)
                .ToList();

            var transactions = JsonDataStore.LoadTransactions()
                .Where(t => t.Date.Month == SelectedMonth &&
                           t.Date.Year == SelectedYear &&
                           t.Type == TransactionType.Expense)
                .ToList();

            BudgetItems.Clear();
            foreach (var budget in budgets)
            {
                var spent = transactions
                    .Where(t => t.Category == budget.Category)
                    .Sum(t => t.Amount);

                BudgetItems.Add(new BudgetItem
                {
                    Category = budget.Category,
                    BudgetAmount = budget.Amount,
                    SpentAmount = spent,
                    Remaining = budget.Amount - spent,
                    PercentageUsed = budget.Amount > 0 ? (spent / budget.Amount) * 100 : 0
                });
            }

            TotalBudget = BudgetItems.Sum(b => b.BudgetAmount);
            TotalSpent = BudgetItems.Sum(b => b.SpentAmount);
            Remaining = TotalBudget - TotalSpent;
        }

        private void ClearForm()
        {
            Category = string.Empty;
            BudgetAmount = 0;
        }
    }

    public class BudgetItem
    {
        public string Category { get; set; } = string.Empty;
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal Remaining { get; set; }
        public decimal PercentageUsed { get; set; }
    }
}
