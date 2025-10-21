using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RichIZ.ViewModels
{
    public partial class InvestmentViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Investment> investments = new();

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private InvestmentType selectedType = InvestmentType.Stock;

        [ObservableProperty]
        private decimal quantity;

        [ObservableProperty]
        private decimal purchasePrice;

        [ObservableProperty]
        private decimal currentPrice;

        [ObservableProperty]
        private DateTime purchaseDate = DateTime.Now;

        [ObservableProperty]
        private string? notes;

        [ObservableProperty]
        private Investment? selectedInvestment;

        [ObservableProperty]
        private decimal totalInvested;

        [ObservableProperty]
        private decimal totalCurrentValue;

        [ObservableProperty]
        private decimal totalProfitLoss;

        public InvestmentViewModel()
        {
            LoadInvestments();
        }

        [RelayCommand]
        private void AddInvestment()
        {
            if (string.IsNullOrWhiteSpace(Name) || Quantity <= 0 || PurchasePrice <= 0)
                return;

            // JSON DataStore 사용
            var investment = new Investment
            {
                Name = Name,
                Type = SelectedType,
                Quantity = Quantity,
                PurchasePrice = PurchasePrice,
                CurrentPrice = CurrentPrice > 0 ? CurrentPrice : PurchasePrice,
                PurchaseDate = PurchaseDate,
                LastUpdated = DateTime.Now,
                Notes = Notes
            };

            JsonDataStore.LoadInvestments().Add(investment);
            // 자동 저장됨

            LoadInvestments();
            ClearForm();
        }

        [RelayCommand]
        private void UpdatePrice()
        {
            if (SelectedInvestment == null || CurrentPrice <= 0) return;

            // JSON DataStore 사용
            var investment = JsonDataStore.LoadInvestments().FirstOrDefault(i => i.Id == SelectedInvestment.Id);
            if (investment != null)
            {
                investment.CurrentPrice = CurrentPrice;
                investment.LastUpdated = DateTime.Now;
                // 자동 저장됨
            }

            LoadInvestments();
        }

        [RelayCommand]
        private void DeleteInvestment()
        {
            if (SelectedInvestment == null) return;

            // JSON DataStore 사용
            var investment = JsonDataStore.LoadInvestments().FirstOrDefault(i => i.Id == SelectedInvestment.Id);
            if (investment != null)
            {
                JsonDataStore.LoadInvestments().Remove(investment);
                // 자동 저장됨
            }

            LoadInvestments();
        }

        [RelayCommand]
        private void LoadInvestments()
        {
            // JSON DataStore 사용
            var items = JsonDataStore.LoadInvestments().ToList();

            Investments.Clear();
            foreach (var item in items)
            {
                Investments.Add(item);
            }

            CalculateTotals();
        }

        private void CalculateTotals()
        {
            TotalInvested = Investments.Sum(i => i.TotalInvested);
            TotalCurrentValue = Investments.Sum(i => i.CurrentValue);
            TotalProfitLoss = TotalCurrentValue - TotalInvested;
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Quantity = 0;
            PurchasePrice = 0;
            CurrentPrice = 0;
            PurchaseDate = DateTime.Now;
            Notes = null;
        }
    }
}
