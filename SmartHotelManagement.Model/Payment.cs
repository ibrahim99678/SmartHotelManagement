using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Model;

public class Payment
{
    public int PaymentId { get; set; }

    public int ReservationId { get; set; }
    public decimal Amount { get; set; }

    public PaymentType PaymentType { get; set; }
    public string? BankName { get; set; }

    public PaymentStatus Status { get; set; }
    public DateTime PaymentDate { get; set; }
}
