using System;
using System.ComponentModel.DataAnnotations;

namespace Fundo.Applications.WebApi.Models
{
    public class Loan
    {
        public int Id { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Current balance cannot be negative")]
        public decimal CurrentBalance { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "Applicant name cannot exceed 100 characters")]
        public string ApplicantName { get; set; } = string.Empty;
        
        [Required]
        public string Status { get; set; } = "active";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
} 