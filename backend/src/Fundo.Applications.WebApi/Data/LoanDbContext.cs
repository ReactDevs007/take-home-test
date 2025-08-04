using System;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Fundo.Applications.WebApi.Models;

namespace Fundo.Applications.WebApi.Data
{
    public class LoanDbContext : DbContext
    {
        public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
        {
        }

        public DbSet<Loan> Loans { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LoanHistory> LoanHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for money values
            modelBuilder.Entity<Loan>()
                .Property(l => l.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Loan>()
                .Property(l => l.CurrentBalance)
                .HasPrecision(18, 2);

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Seed users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@loanmanagement.com",
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = HashPassword("admin123"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30),
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Username = "user1",
                    Email = "user1@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    PasswordHash = HashPassword("user123"),
                    Role = "User",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-15),
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    Username = "user2",
                    Email = "user2@example.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    PasswordHash = HashPassword("user123"),
                    Role = "User",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10),
                    IsActive = true
                }
            );

            // Seed loans
            modelBuilder.Entity<Loan>().HasData(
                new Loan
                {
                    Id = 1,
                    Amount = 25000.00m,
                    CurrentBalance = 18750.00m,
                    ApplicantName = "John Doe",
                    Status = "active",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Loan
                {
                    Id = 2,
                    Amount = 15000.00m,
                    CurrentBalance = 0.00m,
                    ApplicantName = "Jane Smith",
                    Status = "paid",
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Loan
                {
                    Id = 3,
                    Amount = 50000.00m,
                    CurrentBalance = 32500.00m,
                    ApplicantName = "Robert Johnson",
                    Status = "active",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Loan
                {
                    Id = 4,
                    Amount = 10000.00m,
                    CurrentBalance = 0.00m,
                    ApplicantName = "Emily Williams",
                    Status = "paid",
                    CreatedAt = DateTime.UtcNow.AddDays(-45),
                    UpdatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new Loan
                {
                    Id = 5,
                    Amount = 75000.00m,
                    CurrentBalance = 72000.00m,
                    ApplicantName = "Michael Brown",
                    Status = "active",
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );

            // Seed loan histories
            modelBuilder.Entity<LoanHistory>().HasData(
                new LoanHistory
                {
                    Id = 1,
                    Amount = 25000.00m,
                    ApplicantName = "John Doe",
                    Status = "active",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    SnapshotDate = DateTime.UtcNow.AddDays(-20),
                    ChangeType = "created"
                },
                new LoanHistory
                {
                    Id = 2,
                    Amount = 25000.00m,
                    ApplicantName = "John Doe",
                    Status = "active",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    SnapshotDate = DateTime.UtcNow.AddDays(-10),
                    ChangeType = "created"
                },
                new LoanHistory
                {
                    Id = 3,
                    Amount = 25000.00m,
                    ApplicantName = "John Doe",
                    Status = "active",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    SnapshotDate = DateTime.UtcNow.AddDays(-20),
                    ChangeType = "created"
                },
                new LoanHistory
                {
                    Id = 4,
                    Amount = 25000.00m,
                    ApplicantName = "John Doe",
                    Status = "active",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    SnapshotDate = DateTime.UtcNow.AddDays(-20),
                    ChangeType = "created"
                }
            );
        }

        private static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            var hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }
    }
} 