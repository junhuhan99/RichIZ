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
        }
    }
}
