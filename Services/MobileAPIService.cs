using Newtonsoft.Json;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RichIZ.Services
{
    /// <summary>
    /// 간단한 HTTP 리스너를 통한 모바일 앱 연동 API
    /// </summary>
    public class MobileAPIService
    {
        private HttpListener? _listener;
        private bool _isRunning;
        private const int PORT = 8888;

        public void Start()
        {
            if (_isRunning) return;

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{PORT}/");
            _listener.Prefixes.Add($"http://+:{PORT}/"); // 외부 접근 허용
            _listener.Start();
            _isRunning = true;

            Task.Run(() => HandleRequests());
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            _listener?.Close();
        }

        private async Task HandleRequests()
        {
            while (_isRunning && _listener != null)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    await ProcessRequest(context);
                }
                catch
                {
                    // 리스너 종료 시 예외 무시
                }
            }
        }

        private async Task ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            // CORS 헤더 추가
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

            try
            {
                var path = request.Url?.AbsolutePath ?? "/";

                object? result = path switch
                {
                    "/api/transactions" => GetTransactions(),
                    "/api/investments" => GetInvestments(),
                    "/api/summary" => GetSummary(),
                    "/api/goals" => GetGoals(),
                    _ => new { error = "Not found" }
                };

                var json = JsonConvert.SerializeObject(result);
                var buffer = Encoding.UTF8.GetBytes(json);

                response.ContentType = "application/json";
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(new { error = ex.Message });
                var buffer = Encoding.UTF8.GetBytes(error);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer);
            }
            finally
            {
                response.OutputStream.Close();
            }
        }

        private object GetTransactions()
        {
            using var context = new AppDbContext();
            var transactions = context.Transactions
                .OrderByDescending(t => t.Date)
                .Take(50)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Amount,
                    Date = t.Date.ToString("yyyy-MM-dd"),
                    Type = t.Type.ToString(),
                    t.Category
                })
                .ToList();

            return new { success = true, data = transactions };
        }

        private object GetInvestments()
        {
            using var context = new AppDbContext();
            var investments = context.Investments
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    Type = i.Type.ToString(),
                    i.CurrentValue,
                    i.ProfitLoss,
                    i.ReturnRate
                })
                .ToList();

            return new { success = true, data = investments };
        }

        private object GetSummary()
        {
            using var context = new AppDbContext();

            var now = DateTime.Now;
            var monthlyTransactions = context.Transactions
                .Where(t => t.Date.Year == now.Year && t.Date.Month == now.Month)
                .ToList();

            var income = monthlyTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var expense = monthlyTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            var totalInvestment = context.Investments.Sum(i => i.CurrentValue);
            var totalBank = context.BankAccounts.Sum(b => b.Balance);

            return new
            {
                success = true,
                data = new
                {
                    monthlyIncome = income,
                    monthlyExpense = expense,
                    netSavings = income - expense,
                    totalInvestment,
                    totalBank,
                    totalAssets = totalBank + totalInvestment
                }
            };
        }

        private object GetGoals()
        {
            using var context = new AppDbContext();
            var goals = context.FinancialGoals
                .Where(g => !g.IsCompleted)
                .Select(g => new
                {
                    g.Id,
                    g.Title,
                    g.TargetAmount,
                    g.CurrentAmount,
                    g.Progress,
                    g.DaysRemaining,
                    Category = g.Category.ToString()
                })
                .ToList();

            return new { success = true, data = goals };
        }

        public int GetPort() => PORT;
        public bool IsRunning() => _isRunning;
    }
}
