using RichIZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RichIZ.Services
{
    public class ExcelService
    {
        /// <summary>
        /// 거래 내역을 CSV로 내보내기
        /// </summary>
        public void ExportTransactionsToCSV(List<Transaction> transactions, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("날짜,제목,유형,카테고리,금액,설명");

            foreach (var t in transactions)
            {
                sb.AppendLine($"{t.Date:yyyy-MM-dd},{t.Title},{t.Type},{t.Category},{t.Amount},\"{t.Description}\"");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 투자 내역을 CSV로 내보내기
        /// </summary>
        public void ExportInvestmentsToCSV(List<Investment> investments, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("자산명,유형,수량,매입가,현재가,총투자금,현재가치,손익,수익률");

            foreach (var i in investments)
            {
                sb.AppendLine($"{i.Name},{i.Type},{i.Quantity},{i.PurchasePrice},{i.CurrentPrice},{i.TotalInvested},{i.CurrentValue},{i.ProfitLoss},{i.ReturnRate:N2}%");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 은행 계좌를 CSV로 내보내기
        /// </summary>
        public void ExportBankAccountsToCSV(List<BankAccount> accounts, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("은행명,계좌번호,계좌유형,잔액,금리,별칭");

            foreach (var a in accounts)
            {
                sb.AppendLine($"{a.BankName},{a.AccountNumber},{a.AccountType},{a.Balance},{a.InterestRate},\"{a.AccountNickname}\"");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// CSV에서 거래 내역 가져오기
        /// </summary>
        public List<Transaction> ImportTransactionsFromCSV(string filePath)
        {
            var transactions = new List<Transaction>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            // 첫 줄은 헤더이므로 건너뜀
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var parts = lines[i].Split(',');
                    if (parts.Length >= 5)
                    {
                        var transaction = new Transaction
                        {
                            Date = DateTime.Parse(parts[0]),
                            Title = parts[1],
                            Type = Enum.Parse<TransactionType>(parts[2]),
                            Category = parts[3],
                            Amount = decimal.Parse(parts[4]),
                            Description = parts.Length > 5 ? parts[5].Trim('"') : null
                        };
                        transactions.Add(transaction);
                    }
                }
                catch
                {
                    // 잘못된 행은 건너뜀
                    continue;
                }
            }

            return transactions;
        }

        /// <summary>
        /// 전체 데이터를 CSV로 내보내기
        /// </summary>
        public void ExportAllDataToFolder(string folderPath,
            List<Transaction> transactions,
            List<Investment> investments,
            List<BankAccount> bankAccounts,
            List<Budget> budgets,
            List<FinancialGoal> goals)
        {
            Directory.CreateDirectory(folderPath);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            ExportTransactionsToCSV(transactions, Path.Combine(folderPath, $"Transactions_{timestamp}.csv"));
            ExportInvestmentsToCSV(investments, Path.Combine(folderPath, $"Investments_{timestamp}.csv"));
            ExportBankAccountsToCSV(bankAccounts, Path.Combine(folderPath, $"BankAccounts_{timestamp}.csv"));

            // 예산 내보내기
            var budgetSb = new StringBuilder();
            budgetSb.AppendLine("카테고리,금액,월,년도");
            foreach (var b in budgets)
            {
                budgetSb.AppendLine($"{b.Category},{b.Amount},{b.Month},{b.Year}");
            }
            File.WriteAllText(Path.Combine(folderPath, $"Budgets_{timestamp}.csv"), budgetSb.ToString(), Encoding.UTF8);

            // 목표 내보내기
            var goalSb = new StringBuilder();
            goalSb.AppendLine("제목,목표금액,현재금액,목표일,카테고리,완료여부");
            foreach (var g in goals)
            {
                goalSb.AppendLine($"{g.Title},{g.TargetAmount},{g.CurrentAmount},{g.TargetDate:yyyy-MM-dd},{g.Category},{g.IsCompleted}");
            }
            File.WriteAllText(Path.Combine(folderPath, $"Goals_{timestamp}.csv"), goalSb.ToString(), Encoding.UTF8);
        }
    }
}
