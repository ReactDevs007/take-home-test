using System.Collections.Generic;
using System.Threading.Tasks;
using Fundo.Applications.WebApi.DTOs;
using Fundo.Applications.WebApi.Models;

namespace Fundo.Applications.WebApi.Services
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanResponse>> GetAllLoansAsync();
        Task<LoanResponse?> GetLoanByIdAsync(int id);
        Task<LoanResponse> CreateLoanAsync(CreateLoanRequest request);
        Task<LoanResponse?> MakePaymentAsync(int id, PaymentRequest request);

        Task<IEnumerable<LoanHistoryResponse>> GetLoanHistoryAsync(int loanId);
    }
} 