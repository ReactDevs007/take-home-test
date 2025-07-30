using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Fundo.Applications.WebApi.Data;
using Fundo.Applications.WebApi.DTOs;
using Fundo.Applications.WebApi.Models;

namespace Fundo.Applications.WebApi.Services
{
    public class LoanService : ILoanService
    {
        private readonly LoanDbContext _context;

        public LoanService(LoanDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LoanResponse>> GetAllLoansAsync()
        {
            var loans = await _context.Loans.ToListAsync();
            return loans.Select(MapToResponse);
        }

        public async Task<LoanResponse?> GetLoanByIdAsync(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            return loan != null ? MapToResponse(loan) : null;
        }

        public async Task<LoanResponse> CreateLoanAsync(CreateLoanRequest request)
        {
            var loan = new Loan
            {
                Amount = request.Amount,
                CurrentBalance = request.Amount,
                ApplicantName = request.ApplicantName,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return MapToResponse(loan);
        }

        public async Task<LoanResponse?> MakePaymentAsync(int id, PaymentRequest request)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return null;
            }

            if (loan.Status == "paid")
            {
                throw new InvalidOperationException("Cannot make payment on a loan that is already paid off");
            }

            if (request.Amount > loan.CurrentBalance)
            {
                throw new InvalidOperationException("Payment amount cannot exceed current balance");
            }

            loan.CurrentBalance -= request.Amount;
            loan.UpdatedAt = DateTime.UtcNow;

            if (loan.CurrentBalance == 0)
            {
                loan.Status = "paid";
            }

            await _context.SaveChangesAsync();

            return MapToResponse(loan);
        }

        private static LoanResponse MapToResponse(Loan loan)
        {
            return new LoanResponse
            {
                Id = loan.Id,
                Amount = loan.Amount,
                CurrentBalance = loan.CurrentBalance,
                ApplicantName = loan.ApplicantName,
                Status = loan.Status,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt
            };
        }
    }
} 