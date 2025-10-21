using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RichIZ.ViewModels
{
    public partial class BankAccountViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BankAccount> bankAccounts = new();

        [ObservableProperty]
        private string bankName = string.Empty;

        [ObservableProperty]
        private string accountNumber = string.Empty;

        [ObservableProperty]
        private AccountType selectedAccountType = AccountType.Checking;

        [ObservableProperty]
        private decimal balance;

        [ObservableProperty]
        private decimal interestRate;

        [ObservableProperty]
        private string? accountNickname;

        [ObservableProperty]
        private BankAccount? selectedAccount;

        [ObservableProperty]
        private decimal totalBalance;

        public BankAccountViewModel()
        {
            LoadAccounts();
        }

        [RelayCommand]
        private void AddAccount()
        {
            if (string.IsNullOrWhiteSpace(BankName) || string.IsNullOrWhiteSpace(AccountNumber))
                return;

            using var context = new AppDbContext();
            var account = new BankAccount
            {
                BankName = BankName,
                AccountNumber = AccountNumber,
                AccountType = SelectedAccountType,
                Balance = Balance,
                InterestRate = InterestRate,
                AccountNickname = AccountNickname
            };

            context.BankAccounts.Add(account);
            context.SaveChanges();

            LoadAccounts();
            ClearForm();
        }

        [RelayCommand]
        private void UpdateBalance()
        {
            if (SelectedAccount == null) return;

            using var context = new AppDbContext();
            var account = context.BankAccounts.Find(SelectedAccount.Id);
            if (account != null)
            {
                account.Balance = Balance;
                account.LastUpdated = DateTime.Now;
                context.SaveChanges();
            }

            LoadAccounts();
        }

        [RelayCommand]
        private void DeleteAccount()
        {
            if (SelectedAccount == null) return;

            using var context = new AppDbContext();
            var account = context.BankAccounts.Find(SelectedAccount.Id);
            if (account != null)
            {
                context.BankAccounts.Remove(account);
                context.SaveChanges();
            }

            LoadAccounts();
        }

        [RelayCommand]
        private void LoadAccounts()
        {
            using var context = new AppDbContext();
            var accounts = context.BankAccounts.ToList();

            BankAccounts.Clear();
            foreach (var account in accounts)
            {
                BankAccounts.Add(account);
            }

            TotalBalance = BankAccounts.Sum(a => a.Balance);
        }

        private void ClearForm()
        {
            BankName = string.Empty;
            AccountNumber = string.Empty;
            Balance = 0;
            InterestRate = 0;
            AccountNickname = null;
        }
    }
}
