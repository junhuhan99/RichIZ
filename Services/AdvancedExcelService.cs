using ClosedXML.Excel;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RichIZ.Services
{
    public class AdvancedExcelService
    {
        /// <summary>
        /// 전체 데이터를 Excel XLSX로 내보내기
        /// </summary>
        public void ExportAllDataToExcel(string filePath)
        {
            using var workbook = new XLWorkbook();
            // JSON DataStore 사용

            // 거래 내역 시트
            var transactionsSheet = workbook.Worksheets.Add("거래내역");
            var transactions = JsonDataStore.LoadTransactions().OrderByDescending(t => t.Date).ToList();
            CreateTransactionsSheet(transactionsSheet, transactions);

            // 투자 포트폴리오 시트
            var investmentsSheet = workbook.Worksheets.Add("투자포트폴리오");
            var investments = JsonDataStore.LoadInvestments().ToList();
            CreateInvestmentsSheet(investmentsSheet, investments);

            // 은행 계좌 시트
            var bankAccountsSheet = workbook.Worksheets.Add("은행계좌");
            var bankAccounts = JsonDataStore.LoadBankAccounts().ToList();
            CreateBankAccountsSheet(bankAccountsSheet, bankAccounts);

            // 예산 시트
            var budgetsSheet = workbook.Worksheets.Add("예산");
            var budgets = JsonDataStore.LoadBudgets().ToList();
            CreateBudgetsSheet(budgetsSheet, budgets);

            // 재무 목표 시트
            var goalsSheet = workbook.Worksheets.Add("재무목표");
            var goals = JsonDataStore.LoadFinancialGoals().ToList();
            CreateGoalsSheet(goalsSheet, goals);

            // 요약 시트
            var summarySheet = workbook.Worksheets.Add("요약");
            CreateSummarySheet(summarySheet, transactions, investments, bankAccounts);

            // 요약 시트를 맨 앞으로
            summarySheet.Position = 1;

            workbook.SaveAs(filePath);
        }

        private void CreateTransactionsSheet(IXLWorksheet sheet, List<Transaction> transactions)
        {
            // 헤더
            sheet.Cell(1, 1).Value = "날짜";
            sheet.Cell(1, 2).Value = "제목";
            sheet.Cell(1, 3).Value = "유형";
            sheet.Cell(1, 4).Value = "카테고리";
            sheet.Cell(1, 5).Value = "금액";
            sheet.Cell(1, 6).Value = "설명";

            // 헤더 스타일
            var headerRange = sheet.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // 데이터
            for (int i = 0; i < transactions.Count; i++)
            {
                var t = transactions[i];
                int row = i + 2;

                sheet.Cell(row, 1).Value = t.Date.ToString("yyyy-MM-dd");
                sheet.Cell(row, 2).Value = t.Title;
                sheet.Cell(row, 3).Value = t.Type.ToString();
                sheet.Cell(row, 4).Value = t.Category;
                sheet.Cell(row, 5).Value = t.Amount;
                sheet.Cell(row, 6).Value = t.Description ?? "";

                // 수입/지출별 색상
                if (t.Type == TransactionType.Income)
                    sheet.Cell(row, 5).Style.Font.FontColor = XLColor.Green;
                else
                    sheet.Cell(row, 5).Style.Font.FontColor = XLColor.Red;
            }

            sheet.Columns().AdjustToContents();
        }

        private void CreateInvestmentsSheet(IXLWorksheet sheet, List<Investment> investments)
        {
            // 헤더
            var headers = new[] { "자산명", "유형", "수량", "매입가", "현재가", "총투자금", "현재가치", "손익", "수익률(%)" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = sheet.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

            // 데이터
            for (int i = 0; i < investments.Count; i++)
            {
                var inv = investments[i];
                int row = i + 2;

                sheet.Cell(row, 1).Value = inv.Name;
                sheet.Cell(row, 2).Value = inv.Type.ToString();
                sheet.Cell(row, 3).Value = inv.Quantity;
                sheet.Cell(row, 4).Value = inv.PurchasePrice;
                sheet.Cell(row, 5).Value = inv.CurrentPrice;
                sheet.Cell(row, 6).Value = inv.TotalInvested;
                sheet.Cell(row, 7).Value = inv.CurrentValue;
                sheet.Cell(row, 8).Value = inv.ProfitLoss;
                sheet.Cell(row, 9).Value = inv.ReturnRate;

                // 손익 색상
                if (inv.ProfitLoss >= 0)
                    sheet.Cell(row, 8).Style.Font.FontColor = XLColor.Green;
                else
                    sheet.Cell(row, 8).Style.Font.FontColor = XLColor.Red;
            }

            sheet.Columns().AdjustToContents();
        }

        private void CreateBankAccountsSheet(IXLWorksheet sheet, List<BankAccount> accounts)
        {
            var headers = new[] { "은행명", "계좌번호", "유형", "잔액", "금리(%)", "별칭", "생성일" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = sheet.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightYellow;

            for (int i = 0; i < accounts.Count; i++)
            {
                var acc = accounts[i];
                int row = i + 2;

                sheet.Cell(row, 1).Value = acc.BankName;
                sheet.Cell(row, 2).Value = acc.AccountNumber;
                sheet.Cell(row, 3).Value = acc.AccountType.ToString();
                sheet.Cell(row, 4).Value = acc.Balance;
                sheet.Cell(row, 5).Value = acc.InterestRate;
                sheet.Cell(row, 6).Value = acc.AccountNickname ?? "";
                sheet.Cell(row, 7).Value = acc.CreatedDate.ToString("yyyy-MM-dd");
            }

            sheet.Columns().AdjustToContents();
        }

        private void CreateBudgetsSheet(IXLWorksheet sheet, List<Budget> budgets)
        {
            var headers = new[] { "카테고리", "금액", "월", "년도" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = sheet.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightCoral;

            for (int i = 0; i < budgets.Count; i++)
            {
                var b = budgets[i];
                int row = i + 2;

                sheet.Cell(row, 1).Value = b.Category;
                sheet.Cell(row, 2).Value = b.Amount;
                sheet.Cell(row, 3).Value = b.Month;
                sheet.Cell(row, 4).Value = b.Year;
            }

            sheet.Columns().AdjustToContents();
        }

        private void CreateGoalsSheet(IXLWorksheet sheet, List<FinancialGoal> goals)
        {
            var headers = new[] { "제목", "목표금액", "현재금액", "진행률(%)", "목표일", "남은일수", "카테고리", "완료여부" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
            }

            var headerRange = sheet.Range(1, 1, 1, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightPink;

            for (int i = 0; i < goals.Count; i++)
            {
                var g = goals[i];
                int row = i + 2;

                sheet.Cell(row, 1).Value = g.Title;
                sheet.Cell(row, 2).Value = g.TargetAmount;
                sheet.Cell(row, 3).Value = g.CurrentAmount;
                sheet.Cell(row, 4).Value = g.Progress;
                sheet.Cell(row, 5).Value = g.TargetDate.ToString("yyyy-MM-dd");
                sheet.Cell(row, 6).Value = g.DaysRemaining;
                sheet.Cell(row, 7).Value = g.Category.ToString();
                sheet.Cell(row, 8).Value = g.IsCompleted ? "완료" : "진행중";
            }

            sheet.Columns().AdjustToContents();
        }

        private void CreateSummarySheet(IXLWorksheet sheet, List<Transaction> transactions, List<Investment> investments, List<BankAccount> bankAccounts)
        {
            sheet.Cell(1, 1).Value = "RichIZ 자산 요약";
            sheet.Cell(1, 1).Style.Font.FontSize = 18;
            sheet.Cell(1, 1).Style.Font.Bold = true;

            sheet.Cell(2, 1).Value = $"작성일: {DateTime.Now:yyyy-MM-dd HH:mm}";

            // 총 자산
            sheet.Cell(4, 1).Value = "총 자산 현황";
            sheet.Cell(4, 1).Style.Font.Bold = true;
            sheet.Cell(4, 1).Style.Fill.BackgroundColor = XLColor.LightGray;

            var totalBank = bankAccounts.Sum(b => b.Balance);
            var totalInvestment = investments.Sum(i => i.CurrentValue);
            var totalAssets = totalBank + totalInvestment;

            sheet.Cell(5, 1).Value = "은행 자산:";
            sheet.Cell(5, 2).Value = totalBank;
            sheet.Cell(5, 2).Style.NumberFormat.Format = "#,##0";

            sheet.Cell(6, 1).Value = "투자 자산:";
            sheet.Cell(6, 2).Value = totalInvestment;
            sheet.Cell(6, 2).Style.NumberFormat.Format = "#,##0";

            sheet.Cell(7, 1).Value = "총 자산:";
            sheet.Cell(7, 2).Value = totalAssets;
            sheet.Cell(7, 2).Style.NumberFormat.Format = "#,##0";
            sheet.Cell(7, 2).Style.Font.Bold = true;

            // 이번 달 수입/지출
            sheet.Cell(9, 1).Value = "이번 달 수입/지출";
            sheet.Cell(9, 1).Style.Font.Bold = true;
            sheet.Cell(9, 1).Style.Fill.BackgroundColor = XLColor.LightGray;

            var thisMonth = transactions.Where(t => t.Date.Month == DateTime.Now.Month && t.Date.Year == DateTime.Now.Year).ToList();
            var income = thisMonth.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var expense = thisMonth.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            sheet.Cell(10, 1).Value = "총 수입:";
            sheet.Cell(10, 2).Value = income;
            sheet.Cell(10, 2).Style.NumberFormat.Format = "#,##0";
            sheet.Cell(10, 2).Style.Font.FontColor = XLColor.Green;

            sheet.Cell(11, 1).Value = "총 지출:";
            sheet.Cell(11, 2).Value = expense;
            sheet.Cell(11, 2).Style.NumberFormat.Format = "#,##0";
            sheet.Cell(11, 2).Style.Font.FontColor = XLColor.Red;

            sheet.Cell(12, 1).Value = "순 저축:";
            sheet.Cell(12, 2).Value = income - expense;
            sheet.Cell(12, 2).Style.NumberFormat.Format = "#,##0";
            sheet.Cell(12, 2).Style.Font.Bold = true;

            sheet.Columns().AdjustToContents();
        }

        /// <summary>
        /// Excel에서 거래 내역 가져오기
        /// </summary>
        public List<Transaction> ImportTransactionsFromExcel(string filePath)
        {
            var transactions = new List<Transaction>();

            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet("거래내역") ?? workbook.Worksheet(1);

            var rows = worksheet.RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                try
                {
                    var transaction = new Transaction
                    {
                        Date = DateTime.Parse(row.Cell(1).GetString()),
                        Title = row.Cell(2).GetString(),
                        Type = Enum.Parse<TransactionType>(row.Cell(3).GetString()),
                        Category = row.Cell(4).GetString(),
                        Amount = row.Cell(5).GetValue<decimal>(),
                        Description = row.Cell(6).GetString()
                    };
                    transactions.Add(transaction);
                }
                catch
                {
                    continue; // Skip invalid rows
                }
            }

            return transactions;
        }
    }
}
