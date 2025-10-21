using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RichIZ.Services
{
    public class PDFReportService
    {
        public void GenerateMonthlyReport(string filePath, int year, int month)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            using var context = new AppDbContext();

            var transactions = context.Transactions
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .ToList();

            var investments = context.Investments.ToList();
            var bankAccounts = context.BankAccounts.ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text($"RichIZ 월간 재무 리포트 - {year}년 {month}월")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            // 요약
                            x.Item().Element(c => CreateSummarySection(c, transactions, investments, bankAccounts));

                            // 수입/지출 분석
                            x.Item().Element(c => CreateTransactionSection(c, transactions));

                            // 투자 현황
                            x.Item().Element(c => CreateInvestmentSection(c, investments));

                            // 카테고리별 지출
                            x.Item().Element(c => CreateCategorySection(c, transactions));
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("페이지 ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                            x.Span($" | 생성일: {DateTime.Now:yyyy-MM-dd HH:mm}");
                        });
                });
            })
            .GeneratePdf(filePath);
        }

        private void CreateSummarySection(IContainer container, List<Transaction> transactions, List<Investment> investments, List<BankAccount> bankAccounts)
        {
            container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("재무 요약").SemiBold().FontSize(16);

                var income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
                var expense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
                var totalBank = bankAccounts.Sum(b => b.Balance);
                var totalInvestment = investments.Sum(i => i.CurrentValue);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"총 수입: {income:N0}원").FontColor(Colors.Green.Medium);
                    row.RelativeItem().Text($"총 지출: {expense:N0}원").FontColor(Colors.Red.Medium);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"순 저축: {income - expense:N0}원").SemiBold();
                    row.RelativeItem().Text($"저축률: {(income > 0 ? (income - expense) / income * 100 : 0):N1}%");
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"은행 자산: {totalBank:N0}원");
                    row.RelativeItem().Text($"투자 자산: {totalInvestment:N0}원");
                });

                column.Item().Text($"총 자산: {totalBank + totalInvestment:N0}원").SemiBold().FontSize(14);
            });
        }

        private void CreateTransactionSection(IContainer container, List<Transaction> transactions)
        {
            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Text("거래 내역").SemiBold().FontSize(16);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // 날짜
                        columns.RelativeColumn(3); // 제목
                        columns.RelativeColumn(2); // 카테고리
                        columns.RelativeColumn(2); // 금액
                    });

                    // 헤더
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("날짜");
                        header.Cell().Element(CellStyle).Text("제목");
                        header.Cell().Element(CellStyle).Text("카테고리");
                        header.Cell().Element(CellStyle).Text("금액");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    // 데이터 (최근 15개만)
                    foreach (var t in transactions.OrderByDescending(t => t.Date).Take(15))
                    {
                        table.Cell().Element(CellStyle).Text(t.Date.ToString("MM-dd"));
                        table.Cell().Element(CellStyle).Text(t.Title);
                        table.Cell().Element(CellStyle).Text(t.Category);
                        table.Cell().Element(CellStyle).Text($"{t.Amount:N0}원")
                            .FontColor(t.Type == TransactionType.Income ? Colors.Green.Medium : Colors.Red.Medium);

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
                        }
                    }
                });
            });
        }

        private void CreateInvestmentSection(IContainer container, List<Investment> investments)
        {
            if (!investments.Any())
            {
                container.Text("투자 자산이 없습니다.").Italic();
                return;
            }

            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Text("투자 포트폴리오").SemiBold().FontSize(16);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // 자산명
                        columns.RelativeColumn(2); // 유형
                        columns.RelativeColumn(2); // 현재가치
                        columns.RelativeColumn(2); // 손익
                        columns.RelativeColumn(2); // 수익률
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("자산명");
                        header.Cell().Element(CellStyle).Text("유형");
                        header.Cell().Element(CellStyle).Text("현재가치");
                        header.Cell().Element(CellStyle).Text("손익");
                        header.Cell().Element(CellStyle).Text("수익률");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var inv in investments)
                    {
                        table.Cell().Element(CellStyle).Text(inv.Name);
                        table.Cell().Element(CellStyle).Text(inv.Type.ToString());
                        table.Cell().Element(CellStyle).Text($"{inv.CurrentValue:N0}원");
                        table.Cell().Element(CellStyle).Text($"{inv.ProfitLoss:N0}원")
                            .FontColor(inv.ProfitLoss >= 0 ? Colors.Green.Medium : Colors.Red.Medium);
                        table.Cell().Element(CellStyle).Text($"{inv.ReturnRate:N2}%");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
                        }
                    }
                });
            });
        }

        private void CreateCategorySection(IContainer container, List<Transaction> transactions)
        {
            var expenses = transactions.Where(t => t.Type == TransactionType.Expense).ToList();
            if (!expenses.Any())
            {
                container.Text("지출 내역이 없습니다.").Italic();
                return;
            }

            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Text("카테고리별 지출 분석").SemiBold().FontSize(16);

                var categoryData = expenses.GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
                    .OrderByDescending(x => x.Amount)
                    .ToList();

                var totalExpense = categoryData.Sum(x => x.Amount);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("카테고리");
                        header.Cell().Element(CellStyle).Text("금액");
                        header.Cell().Element(CellStyle).Text("비율");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        }
                    });

                    foreach (var cat in categoryData)
                    {
                        var percentage = (cat.Amount / totalExpense) * 100;
                        table.Cell().Element(CellStyle).Text(cat.Category);
                        table.Cell().Element(CellStyle).Text($"{cat.Amount:N0}원");
                        table.Cell().Element(CellStyle).Text($"{percentage:N1}%");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
                        }
                    }
                });
            });
        }
    }
}
