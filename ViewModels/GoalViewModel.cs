using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RichIZ.ViewModels
{
    public partial class GoalViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<FinancialGoal> goals = new();

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private decimal targetAmount;

        [ObservableProperty]
        private decimal currentAmount;

        [ObservableProperty]
        private DateTime targetDate = DateTime.Now.AddYears(1);

        [ObservableProperty]
        private GoalCategory selectedCategory = GoalCategory.Other;

        [ObservableProperty]
        private string? description;

        [ObservableProperty]
        private FinancialGoal? selectedGoal;

        public GoalViewModel()
        {
            LoadGoals();
        }

        [RelayCommand]
        private void AddGoal()
        {
            if (string.IsNullOrWhiteSpace(Title) || TargetAmount <= 0)
                return;

            // JSON DataStore 사용
            var goal = new FinancialGoal
            {
                Title = Title,
                TargetAmount = TargetAmount,
                CurrentAmount = CurrentAmount,
                TargetDate = TargetDate,
                Category = SelectedCategory,
                Description = Description
            };

            JsonDataStore.LoadFinancialGoals().Add(goal);
            // 자동 저장됨

            LoadGoals();
            ClearForm();
        }

        [RelayCommand]
        private void UpdateProgress()
        {
            if (SelectedGoal == null) return;

            // JSON DataStore 사용
            var goal = JsonDataStore.LoadFinancialGoals().FirstOrDefault(g => g.Id == SelectedGoal.Id);
            if (goal != null)
            {
                goal.CurrentAmount = CurrentAmount;
                if (goal.CurrentAmount >= goal.TargetAmount)
                {
                    goal.IsCompleted = true;
                }
                // 자동 저장됨
            }

            LoadGoals();
        }

        [RelayCommand]
        private void DeleteGoal()
        {
            if (SelectedGoal == null) return;

            // JSON DataStore 사용
            var goal = JsonDataStore.LoadFinancialGoals().FirstOrDefault(g => g.Id == SelectedGoal.Id);
            if (goal != null)
            {
                JsonDataStore.LoadFinancialGoals().Remove(goal);
                // 자동 저장됨
            }

            LoadGoals();
        }

        [RelayCommand]
        private void LoadGoals()
        {
            // JSON DataStore 사용
            var goalList = JsonDataStore.LoadFinancialGoals()
                .OrderBy(g => g.IsCompleted)
                .ThenBy(g => g.TargetDate)
                .ToList();

            Goals.Clear();
            foreach (var goal in goalList)
            {
                Goals.Add(goal);
            }
        }

        private void ClearForm()
        {
            Title = string.Empty;
            TargetAmount = 0;
            CurrentAmount = 0;
            TargetDate = DateTime.Now.AddYears(1);
            Description = null;
        }
    }
}
