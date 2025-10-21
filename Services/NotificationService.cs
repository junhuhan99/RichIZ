using RichIZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RichIZ.Services
{
    public class NotificationService
    {
        public class Notification
        {
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public NotificationType Type { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }

        public enum NotificationType
        {
            Info,
            Warning,
            Alert,
            Success
        }

        /// <summary>
        /// 예산 초과 알림 생성
        /// </summary>
        public List<Notification> CheckBudgetAlerts(List<Budget> budgets, List<Transaction> transactions)
        {
            var notifications = new List<Notification>();
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            foreach (var budget in budgets.Where(b => b.Month == currentMonth && b.Year == currentYear))
            {
                var spent = transactions
                    .Where(t => t.Date.Month == currentMonth &&
                               t.Date.Year == currentYear &&
                               t.Category == budget.Category &&
                               t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);

                var percentage = budget.Amount > 0 ? (spent / budget.Amount) * 100 : 0;

                if (percentage >= 100)
                {
                    notifications.Add(new Notification
                    {
                        Title = "예산 초과 경고",
                        Message = $"'{budget.Category}' 예산을 {percentage:N0}% 초과했습니다!",
                        Type = NotificationType.Alert
                    });
                }
                else if (percentage >= 90)
                {
                    notifications.Add(new Notification
                    {
                        Title = "예산 임박 경고",
                        Message = $"'{budget.Category}' 예산의 {percentage:N0}%를 사용했습니다.",
                        Type = NotificationType.Warning
                    });
                }
            }

            return notifications;
        }

        /// <summary>
        /// 목표 달성 알림 생성
        /// </summary>
        public List<Notification> CheckGoalAlerts(List<FinancialGoal> goals)
        {
            var notifications = new List<Notification>();

            foreach (var goal in goals.Where(g => !g.IsCompleted))
            {
                // 목표 기한이 임박한 경우
                if (goal.DaysRemaining <= 30 && goal.DaysRemaining > 0)
                {
                    var progressNeeded = goal.TargetAmount - goal.CurrentAmount;
                    notifications.Add(new Notification
                    {
                        Title = "목표 기한 임박",
                        Message = $"'{goal.Title}' 목표까지 {goal.DaysRemaining}일 남았습니다. (남은 금액: {progressNeeded:N0}원)",
                        Type = NotificationType.Warning
                    });
                }

                // 목표 달성한 경우
                if (goal.Progress >= 100)
                {
                    notifications.Add(new Notification
                    {
                        Title = "목표 달성!",
                        Message = $"축하합니다! '{goal.Title}' 목표를 달성했습니다!",
                        Type = NotificationType.Success
                    });
                }
            }

            return notifications;
        }

        /// <summary>
        /// 투자 손실 알림
        /// </summary>
        public List<Notification> CheckInvestmentAlerts(List<Investment> investments)
        {
            var notifications = new List<Notification>();

            foreach (var investment in investments)
            {
                // 20% 이상 손실
                if (investment.ReturnRate <= -20)
                {
                    notifications.Add(new Notification
                    {
                        Title = "투자 손실 경고",
                        Message = $"'{investment.Name}'이(가) {investment.ReturnRate:N2}% 손실 중입니다. 검토가 필요합니다.",
                        Type = NotificationType.Alert
                    });
                }
                // 50% 이상 수익
                else if (investment.ReturnRate >= 50)
                {
                    notifications.Add(new Notification
                    {
                        Title = "높은 수익률",
                        Message = $"'{investment.Name}'이(가) {investment.ReturnRate:N2}% 수익을 기록했습니다! 일부 매도를 고려해보세요.",
                        Type = NotificationType.Success
                    });
                }
            }

            return notifications;
        }

        /// <summary>
        /// 라이센스 만료 알림
        /// </summary>
        public Notification? CheckLicenseAlert(License? license)
        {
            if (license == null || license.IsExpired)
                return new Notification
                {
                    Title = "라이센스 만료",
                    Message = "라이센스가 만료되었습니다. 갱신이 필요합니다.",
                    Type = NotificationType.Alert
                };

            var daysRemaining = (license.ExpiryDate - DateTime.Now).Days;

            if (daysRemaining <= 7 && daysRemaining > 0)
                return new Notification
                {
                    Title = "라이센스 만료 임박",
                    Message = $"라이센스가 {daysRemaining}일 후 만료됩니다.",
                    Type = NotificationType.Warning
                };

            return null;
        }

        /// <summary>
        /// 모든 알림 수집
        /// </summary>
        public List<Notification> GetAllNotifications(
            List<Budget> budgets,
            List<Transaction> transactions,
            List<FinancialGoal> goals,
            List<Investment> investments,
            License? license)
        {
            var notifications = new List<Notification>();

            notifications.AddRange(CheckBudgetAlerts(budgets, transactions));
            notifications.AddRange(CheckGoalAlerts(goals));
            notifications.AddRange(CheckInvestmentAlerts(investments));

            var licenseAlert = CheckLicenseAlert(license);
            if (licenseAlert != null)
                notifications.Add(licenseAlert);

            return notifications.OrderByDescending(n => n.CreatedAt).ToList();
        }
    }
}
