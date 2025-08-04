using System;

namespace Fundo.Applications.WebApi.DTOs
{
    public class LoanHistoryResponse
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrentBalance { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime SnapshotDate { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public decimal? PaymentAmount { get; set; }
    }
}