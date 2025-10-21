using Newtonsoft.Json;
using RichIZ.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RichIZ.Data
{
    public class JsonDataStore
    {
        private static readonly string DataFolder;
        private static readonly string TransactionsFile;
        private static readonly string InvestmentsFile;
        private static readonly string BudgetsFile;
        private static readonly string BankAccountsFile;
        private static readonly string FinancialGoalsFile;
        private static readonly string LicensesFile;
        private static readonly string AIAnalysesFile;

        static JsonDataStore()
        {
            DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RichIZ");
            Directory.CreateDirectory(DataFolder);

            TransactionsFile = Path.Combine(DataFolder, "transactions.json");
            InvestmentsFile = Path.Combine(DataFolder, "investments.json");
            BudgetsFile = Path.Combine(DataFolder, "budgets.json");
            BankAccountsFile = Path.Combine(DataFolder, "bankaccounts.json");
            FinancialGoalsFile = Path.Combine(DataFolder, "financialgoals.json");
            LicensesFile = Path.Combine(DataFolder, "licenses.json");
            AIAnalysesFile = Path.Combine(DataFolder, "aianalyses.json");
        }

        // Transactions
        public static List<Transaction> LoadTransactions()
        {
            return LoadData<Transaction>(TransactionsFile);
        }

        public static void SaveTransactions(List<Transaction> transactions)
        {
            SaveData(TransactionsFile, transactions);
        }

        // Investments
        public static List<Investment> LoadInvestments()
        {
            return LoadData<Investment>(InvestmentsFile);
        }

        public static void SaveInvestments(List<Investment> investments)
        {
            SaveData(InvestmentsFile, investments);
        }

        // Budgets
        public static List<Budget> LoadBudgets()
        {
            return LoadData<Budget>(BudgetsFile);
        }

        public static void SaveBudgets(List<Budget> budgets)
        {
            SaveData(BudgetsFile, budgets);
        }

        // BankAccounts
        public static List<BankAccount> LoadBankAccounts()
        {
            return LoadData<BankAccount>(BankAccountsFile);
        }

        public static void SaveBankAccounts(List<BankAccount> accounts)
        {
            SaveData(BankAccountsFile, accounts);
        }

        // FinancialGoals
        public static List<FinancialGoal> LoadFinancialGoals()
        {
            return LoadData<FinancialGoal>(FinancialGoalsFile);
        }

        public static void SaveFinancialGoals(List<FinancialGoal> goals)
        {
            SaveData(FinancialGoalsFile, goals);
        }

        // Licenses
        public static List<License> LoadLicenses()
        {
            return LoadData<License>(LicensesFile);
        }

        public static void SaveLicenses(List<License> licenses)
        {
            SaveData(LicensesFile, licenses);
        }

        // AIAnalyses
        public static List<AIAnalysis> LoadAIAnalyses()
        {
            return LoadData<AIAnalysis>(AIAnalysesFile);
        }

        public static void SaveAIAnalyses(List<AIAnalysis> analyses)
        {
            SaveData(AIAnalysesFile, analyses);
        }

        // Generic helpers
        private static List<T> LoadData<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new List<T>();
                }

                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        private static void SaveData<T>(string filePath, List<T> data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch
            {
                // 저장 실패 시 무시
            }
        }

        // Helper methods for adding/updating/deleting
        public static void AddTransaction(Transaction transaction)
        {
            var list = LoadTransactions();
            transaction.Id = list.Any() ? list.Max(t => t.Id) + 1 : 1;
            list.Add(transaction);
            SaveTransactions(list);
        }

        public static void UpdateTransaction(Transaction transaction)
        {
            var list = LoadTransactions();
            var index = list.FindIndex(t => t.Id == transaction.Id);
            if (index >= 0)
            {
                list[index] = transaction;
                SaveTransactions(list);
            }
        }

        public static void DeleteTransaction(int id)
        {
            var list = LoadTransactions();
            list.RemoveAll(t => t.Id == id);
            SaveTransactions(list);
        }

        public static void AddInvestment(Investment investment)
        {
            var list = LoadInvestments();
            investment.Id = list.Any() ? list.Max(i => i.Id) + 1 : 1;
            list.Add(investment);
            SaveInvestments(list);
        }

        public static void UpdateInvestment(Investment investment)
        {
            var list = LoadInvestments();
            var index = list.FindIndex(i => i.Id == investment.Id);
            if (index >= 0)
            {
                list[index] = investment;
                SaveInvestments(list);
            }
        }

        public static void DeleteInvestment(int id)
        {
            var list = LoadInvestments();
            list.RemoveAll(i => i.Id == id);
            SaveInvestments(list);
        }

        public static void AddBankAccount(BankAccount account)
        {
            var list = LoadBankAccounts();
            account.Id = list.Any() ? list.Max(b => b.Id) + 1 : 1;
            list.Add(account);
            SaveBankAccounts(list);
        }

        public static void UpdateBankAccount(BankAccount account)
        {
            var list = LoadBankAccounts();
            var index = list.FindIndex(b => b.Id == account.Id);
            if (index >= 0)
            {
                list[index] = account;
                SaveBankAccounts(list);
            }
        }

        public static void DeleteBankAccount(int id)
        {
            var list = LoadBankAccounts();
            list.RemoveAll(b => b.Id == id);
            SaveBankAccounts(list);
        }

        public static void AddBudget(Budget budget)
        {
            var list = LoadBudgets();
            budget.Id = list.Any() ? list.Max(b => b.Id) + 1 : 1;
            list.Add(budget);
            SaveBudgets(list);
        }

        public static void UpdateBudget(Budget budget)
        {
            var list = LoadBudgets();
            var index = list.FindIndex(b => b.Id == budget.Id);
            if (index >= 0)
            {
                list[index] = budget;
                SaveBudgets(list);
            }
        }

        public static void DeleteBudget(int id)
        {
            var list = LoadBudgets();
            list.RemoveAll(b => b.Id == id);
            SaveBudgets(list);
        }

        public static void AddFinancialGoal(FinancialGoal goal)
        {
            var list = LoadFinancialGoals();
            goal.Id = list.Any() ? list.Max(g => g.Id) + 1 : 1;
            list.Add(goal);
            SaveFinancialGoals(list);
        }

        public static void UpdateFinancialGoal(FinancialGoal goal)
        {
            var list = LoadFinancialGoals();
            var index = list.FindIndex(g => g.Id == goal.Id);
            if (index >= 0)
            {
                list[index] = goal;
                SaveFinancialGoals(list);
            }
        }

        public static void DeleteFinancialGoal(int id)
        {
            var list = LoadFinancialGoals();
            list.RemoveAll(g => g.Id == id);
            SaveFinancialGoals(list);
        }

        public static void AddLicense(License license)
        {
            var list = LoadLicenses();
            license.Id = list.Any() ? list.Max(l => l.Id) + 1 : 1;
            list.Add(license);
            SaveLicenses(list);
        }

        public static void UpdateLicense(License license)
        {
            var list = LoadLicenses();
            var index = list.FindIndex(l => l.Id == license.Id);
            if (index >= 0)
            {
                list[index] = license;
                SaveLicenses(list);
            }
        }

        public static void AddAIAnalysis(AIAnalysis analysis)
        {
            var list = LoadAIAnalyses();
            analysis.Id = list.Any() ? list.Max(a => a.Id) + 1 : 1;
            list.Add(analysis);
            SaveAIAnalyses(list);
        }
    }
}
