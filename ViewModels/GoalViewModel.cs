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

            using var context = new AppDbContext();
            var goal = new FinancialGoal
            {
                Title = Title,
                TargetAmount = TargetAmount,
                CurrentAmount = CurrentAmount,
                TargetDate = TargetDate,
                Category = SelectedCategory,
                Description = Description
            };

            context.FinancialGoals.Add(goal);
            context.SaveChanges();

            LoadGoals();
            ClearForm();
        }

        [RelayCommand]
        private void UpdateProgress()
        {
            if (SelectedGoal == null) return;

            using var context = new AppDbContext();
            var goal = context.FinancialGoals.Find(SelectedGoal.Id);
            if (goal != null)
            {
                goal.CurrentAmount = CurrentAmount;
                if (goal.CurrentAmount >= goal.TargetAmount)
                {
                    goal.IsCompleted = true;
                }
                context.SaveChanges();
            }

            LoadGoals();
        }

        [RelayCommand]
        private void DeleteGoal()
        {
            if (SelectedGoal == null) return;

            using var context = new AppDbContext();
            var goal = context.FinancialGoals.Find(SelectedGoal.Id);
            if (goal != null)
            {
                context.FinancialGoals.Remove(goal);
                context.SaveChanges();
            }

            LoadGoals();
        }

        [RelayCommand]
        private void LoadGoals()
        {
            using var context = new AppDbContext();
            var goalList = context.FinancialGoals
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
