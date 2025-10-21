using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RichIZ.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string currentView = "Dashboard";

        [ObservableProperty]
        private decimal totalAssets;

        [ObservableProperty]
        private decimal monthlyIncome;

        [ObservableProperty]
        private decimal monthlyExpense;

        [ObservableProperty]
        private decimal investmentValue;

        public MainViewModel()
        {
            LoadDashboardData();
        }

        [RelayCommand]
        private void NavigateTo(string view)
        {
            CurrentView = view;
        }

        private void LoadDashboardData()
        {
            // JSON DataStore 사용
            var now = DateTime.Now;

            // 이번 달 수입/지출
            var monthlyTransactions = JsonDataStore.LoadTransactions()
                .Where(t => t.Date.Year == now.Year && t.Date.Month == now.Month)
                .ToList();

            MonthlyIncome = monthlyTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            MonthlyExpense = monthlyTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            // 투자 자산 가치
            var investments = JsonDataStore.LoadInvestments().ToList();
            InvestmentValue = investments.Sum(i => i.CurrentValue);

            // 총 자산
            TotalAssets = MonthlyIncome - MonthlyExpense + InvestmentValue;
        }

        public void RefreshData()
        {
            LoadDashboardData();
        }
    }
}
