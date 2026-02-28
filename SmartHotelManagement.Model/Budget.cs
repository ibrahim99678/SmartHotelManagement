using System;

namespace SmartHotelManagement.Model;

public class Budget : Entity
{
    public int BudgetId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}
