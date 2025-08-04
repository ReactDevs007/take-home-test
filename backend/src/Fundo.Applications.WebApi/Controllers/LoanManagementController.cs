using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fundo.Applications.WebApi.DTOs;
using Fundo.Applications.WebApi.Services;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoanManagementController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoanManagementController> _logger;

        public LoanManagementController(ILoanService loanService, ILogger<LoanManagementController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        /// <returns>List of all loans</returns>
        [HttpGet("loans")]
        public async Task<ActionResult<IEnumerable<LoanResponse>>> GetAllLoans()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                
                _logger.LogInformation("User {UserId} ({Username}) requested all loans", userId, username);
                
                var loans = await _loanService.GetAllLoansAsync();
                
                _logger.LogInformation("Successfully retrieved {LoanCount} loans for user {UserId}", loans.Count(), userId);
                
                return Ok(loans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving loans for user {UserId}", 
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "An error occurred while retrieving loans");
            }
        }

        /// <param name="id">Loan ID</param>
        /// <returns>Loan details</returns>
        [HttpGet("loans/{id}")]
        public async Task<ActionResult<LoanResponse>> GetLoan(int id)
        {
            try
            {
                var loan = await _loanService.GetLoanByIdAsync(id);
                if (loan == null)
                {
                    return NotFound($"Loan with ID {id} not found");
                }

                return Ok(loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving loan {LoanId}", id);
                return StatusCode(500, "An error occurred while retrieving the loan");
            }
        }

        /// <param name="request">Loan creation request</param>
        /// <returns>Created loan</returns>
        [HttpPost("loans")]
        public async Task<ActionResult<LoanResponse>> CreateLoan([FromBody] CreateLoanRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var loan = await _loanService.CreateLoanAsync(request);
                return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating loan for {ApplicantName}", request.ApplicantName);
                return StatusCode(500, "An error occurred while creating the loan");
            }
        }

        /// <param name="id">Loan ID</param>
        /// <param name="request">Payment request</param>
        /// <returns>Updated loan details</returns>
        [HttpPost("loans/{id}/payment")]
        public async Task<ActionResult<LoanResponse>> MakePayment(int id, [FromBody] PaymentRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                
                _logger.LogInformation("User {UserId} ({Username}) making payment of {PaymentAmount} on loan {LoanId}", 
                    userId, username, request.Amount, id);

                var updatedLoan = await _loanService.MakePaymentAsync(id, request);
                if (updatedLoan == null)
                {
                    _logger.LogWarning("Payment failed - Loan {LoanId} not found for user {UserId}", id, userId);
                    return NotFound($"Loan with ID {id} not found");
                }

                _logger.LogInformation("Payment successful - User {UserId} paid {PaymentAmount} on loan {LoanId}, new balance: {NewBalance}", 
                    userId, request.Amount, id, updatedLoan.CurrentBalance);

                return Ok(updatedLoan);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid payment operation for loan {LoanId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing payment for loan {LoanId}", id);
                return StatusCode(500, "An error occurred while processing the payment");
            }
        }

        /// <param name="id">Loan ID</param>
        /// <returns>Loan history</returns>
        [HttpGet("loans/{id}/history")]
        public async Task<ActionResult<IEnumerable<LoanHistoryResponse>>> GetLoanHistory(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                
                _logger.LogInformation("User {UserId} ({Username}) requested history for loan {LoanId}", userId, username, id);
                
                // First check if the loan exists
                var loan = await _loanService.GetLoanByIdAsync(id);
                if (loan == null)
                {
                    _logger.LogWarning("Loan history requested for non-existent loan {LoanId} by user {UserId}", id, userId);
                    return NotFound($"Loan with ID {id} not found");
                }
                
                var history = await _loanService.GetLoanHistoryAsync(id);
                
                _logger.LogInformation("Successfully retrieved {HistoryCount} history records for loan {LoanId} for user {UserId}", 
                    history.Count(), id, userId);
                
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving loan history for loan {LoanId}", id);
                return StatusCode(500, "An error occurred while retrieving the loan history");
            }
        }
    }
}