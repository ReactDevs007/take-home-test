using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Fundo.Applications.WebApi.Data;
using Fundo.Applications.WebApi.Services;
using Fundo.Applications.WebApi.DTOs;
using Fundo.Applications.WebApi.Models;

namespace Fundo.Services.Tests.Unit
{
    public class LoanServiceTests : IDisposable
    {
        private readonly LoanDbContext _context;
        private readonly LoanService _loanService;

        public LoanServiceTests()
        {
            var options = new DbContextOptionsBuilder<LoanDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LoanDbContext(options);
            _loanService = new LoanService(_context);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var loans = new List<Loan>
            {
                new Loan
                {
                    Id = 1,
                    Amount = 1000m,
                    CurrentBalance = 500m,
                    ApplicantName = "John Doe",
                    Status = "active",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Loan
                {
                    Id = 2,
                    Amount = 2000m,
                    CurrentBalance = 0m,
                    ApplicantName = "Jane Smith",
                    Status = "paid",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };

            _context.Loans.AddRange(loans);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllLoansAsync_ShouldReturnAllLoans()
        {
            // Act
            var result = await _loanService.GetAllLoansAsync();

            // Assert
            var loans = result.ToList();
            Assert.Equal(2, loans.Count);
            Assert.Contains(loans, l => l.ApplicantName == "John Doe");
            Assert.Contains(loans, l => l.ApplicantName == "Jane Smith");
        }

        [Fact]
        public async Task GetLoanByIdAsync_WithValidId_ShouldReturnLoan()
        {
            // Act
            var result = await _loanService.GetLoanByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result.ApplicantName);
            Assert.Equal(1000m, result.Amount);
            Assert.Equal(500m, result.CurrentBalance);
        }

        [Fact]
        public async Task GetLoanByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _loanService.GetLoanByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateLoanAsync_WithValidRequest_ShouldCreateLoan()
        {
            // Arrange
            var request = new CreateLoanRequest
            {
                Amount = 5000m,
                ApplicantName = "Bob Johnson"
            };

            // Act
            var result = await _loanService.CreateLoanAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5000m, result.Amount);
            Assert.Equal(5000m, result.CurrentBalance); // Initially equal to amount
            Assert.Equal("Bob Johnson", result.ApplicantName);
            Assert.Equal("active", result.Status);

            // Verify it was saved to database
            var savedLoan = await _context.Loans.FindAsync(result.Id);
            Assert.NotNull(savedLoan);
            Assert.Equal("Bob Johnson", savedLoan.ApplicantName);
        }

        [Fact]
        public async Task MakePaymentAsync_WithValidPayment_ShouldUpdateBalance()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { Amount = 200m };

            // Act
            var result = await _loanService.MakePaymentAsync(1, paymentRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(300m, result.CurrentBalance); // 500 - 200
            Assert.Equal("active", result.Status);

            // Verify in database
            var updatedLoan = await _context.Loans.FindAsync(1);
            Assert.Equal(300m, updatedLoan.CurrentBalance);
        }

        [Fact]
        public async Task MakePaymentAsync_WithFullPayment_ShouldMarkAsPaid()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { Amount = 500m }; // Full remaining balance

            // Act
            var result = await _loanService.MakePaymentAsync(1, paymentRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0m, result.CurrentBalance);
            Assert.Equal("paid", result.Status);

            // Verify in database
            var updatedLoan = await _context.Loans.FindAsync(1);
            Assert.Equal("paid", updatedLoan.Status);
        }

        [Fact]
        public async Task MakePaymentAsync_WithExcessivePayment_ShouldThrowException()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { Amount = 600m }; // More than current balance

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _loanService.MakePaymentAsync(1, paymentRequest));
            
            Assert.Equal("Payment amount cannot exceed current balance", exception.Message);
        }

        [Fact]
        public async Task MakePaymentAsync_OnPaidLoan_ShouldThrowException()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { Amount = 100m };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _loanService.MakePaymentAsync(2, paymentRequest)); // Loan ID 2 is already paid
            
            Assert.Equal("Cannot make payment on a loan that is already paid off", exception.Message);
        }

        [Fact]
        public async Task MakePaymentAsync_WithInvalidLoanId_ShouldReturnNull()
        {
            // Arrange
            var paymentRequest = new PaymentRequest { Amount = 100m };

            // Act
            var result = await _loanService.MakePaymentAsync(999, paymentRequest);

            // Assert
            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
} 