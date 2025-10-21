using Microsoft.EntityFrameworkCore;
using RichIZ.Models;
using System;
using System.IO;

namespace RichIZ.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<FinancialGoal> FinancialGoals { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<AIAnalysis> AIAnalyses { get; set; }

        public AppDbContext()
        {
            // 데이터베이스가 없으면 자동으로 생성
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(folder, "RichIZ");
            Directory.CreateDirectory(path);
            var dbPath = Path.Combine(path, "richiz.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Transaction 설정
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");

            // Investment 설정
            modelBuilder.Entity<Investment>()
                .Property(i => i.PurchasePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Investment>()
                .Property(i => i.CurrentPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Investment>()
                .Property(i => i.Quantity)
                .HasColumnType("decimal(18,8)");

            // Budget 설정
            modelBuilder.Entity<Budget>()
                .Property(b => b.Amount)
                .HasColumnType("decimal(18,2)");

            // BankAccount 설정
            modelBuilder.Entity<BankAccount>()
                .Property(b => b.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BankAccount>()
                .Property(b => b.InterestRate)
                .HasColumnType("decimal(5,2)");

            // FinancialGoal 설정
            modelBuilder.Entity<FinancialGoal>()
                .Property(g => g.TargetAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<FinancialGoal>()
                .Property(g => g.CurrentAmount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
