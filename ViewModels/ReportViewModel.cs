using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace RichIZ.ViewModels
{
    public partial class ReportViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ISeries> incomeSeries = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> expenseSeries = new();

        [ObservableProperty]
        private ObservableCollection<ISeries> categoryPieSeries = new();

        [ObservableProperty]
        private int selectedYear = DateTime.Now.Year;

        [ObservableProperty]
        private int selectedMonth = DateTime.Now.Month;

        public ReportViewModel()
        {
            LoadChartData();
        }

        [RelayCommand]
        private void LoadChartData()
        {
            LoadMonthlyTrend();
            LoadCategoryBreakdown();
        }

        private void LoadMonthlyTrend()
        {
            // JSON DataStore 사용

            var incomeData = new List<decimal>();
            var expenseData = new List<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var monthlyTransactions = JsonDataStore.LoadTransactions()
                    .Where(t => t.Date.Year == SelectedYear && t.Date.Month == month)
                    .ToList();

                var income = monthlyTransactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Amount);

                var expense = monthlyTransactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                incomeData.Add(income);
                expenseData.Add(expense);
            }

            IncomeSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<decimal>
                {
                    Values = incomeData,
                    Name = "수입",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 3 }
                }
            };

            ExpenseSeries = new ObservableCollection<ISeries>
            {
                new LineSeries<decimal>
                {
                    Values = expenseData,
                    Name = "지출",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 }
                }
            };
        }

        private void LoadCategoryBreakdown()
        {
            // JSON DataStore 사용

            var expenses = JsonDataStore.LoadTransactions()
                .Where(t => t.Date.Year == SelectedYear &&
                           t.Date.Month == SelectedMonth &&
                           t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                .ToList();

            var pieSeries = new ObservableCollection<ISeries>();
            foreach (var item in expenses)
            {
                pieSeries.Add(new PieSeries<decimal>
                {
                    Values = new[] { item.Amount },
                    Name = item.Category
                });
            }

            CategoryPieSeries = pieSeries;
        }
    }
}
