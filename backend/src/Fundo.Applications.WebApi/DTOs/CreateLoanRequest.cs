using System.ComponentModel.DataAnnotations;

namespace Fundo.Applications.WebApi.DTOs
{
    public class CreateLoanRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "Applicant name cannot exceed 100 characters")]
        public string ApplicantName { get; set; } = string.Empty;
    }
} 