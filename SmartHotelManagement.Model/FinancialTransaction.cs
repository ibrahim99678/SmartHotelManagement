using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHotelManagement.Model;

public class FinancialTransaction : Entity
{
    [Key]
    public int TransactionId { get; set; }
    public FinanceTransactionType Type { get; set; } = FinanceTransactionType.Expense;
    public DateTime Date { get; set; } = DateTime.Now;
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    [Required, StringLength(5)]
    public string Currency { get; set; } = "USD";
    public int CategoryId { get; set; }
    public int PaymentMethodId { get; set; }
    public int? ReservationId { get; set; }
    public int? GuestId { get; set; }
    public string? Description { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceRule { get; set; }
    public string? AttachmentUrl { get; set; }
}
