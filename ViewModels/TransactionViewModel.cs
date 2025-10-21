using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RichIZ.ViewModels
{
    public partial class TransactionViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Transaction> transactions = new();

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private DateTime date = DateTime.Now;

        [ObservableProperty]
        private TransactionType selectedType = TransactionType.Expense;

        [ObservableProperty]
        private string category = string.Empty;

        [ObservableProperty]
        private string? description;

        [ObservableProperty]
        private Transaction? selectedTransaction;

        public ObservableCollection<string> Categories { get; } = new()
        {
            "식비", "교통", "쇼핑", "공과금", "월세", "통신비",
            "의료", "교육", "문화생활", "급여", "부수입", "기타"
        };

        public TransactionViewModel()
        {
            LoadTransactions();
        }

        [RelayCommand]
        private void AddTransaction()
        {
            if (string.IsNullOrWhiteSpace(Title) || Amount <= 0 || string.IsNullOrWhiteSpace(Category))
                return;

            using var context = new AppDbContext();
            var transaction = new Transaction
            {
                Title = Title,
                Amount = Amount,
                Date = Date,
                Type = SelectedType,
                Category = Category,
                Description = Description
            };

            context.Transactions.Add(transaction);
            context.SaveChanges();

            LoadTransactions();
            ClearForm();
        }

        [RelayCommand]
        private void DeleteTransaction()
        {
            if (SelectedTransaction == null) return;

            using var context = new AppDbContext();
            context.Transactions.Remove(SelectedTransaction);
            context.SaveChanges();

            LoadTransactions();
        }

        [RelayCommand]
        private void LoadTransactions()
        {
            using var context = new AppDbContext();
            var items = context.Transactions
                .OrderByDescending(t => t.Date)
                .ToList();

            Transactions.Clear();
            foreach (var item in items)
            {
                Transactions.Add(item);
            }
        }

        private void ClearForm()
        {
            Title = string.Empty;
            Amount = 0;
            Date = DateTime.Now;
            Category = string.Empty;
            Description = null;
        }
    }
}
