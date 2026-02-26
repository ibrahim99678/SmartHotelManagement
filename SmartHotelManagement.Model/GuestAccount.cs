using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.Model;

public class GuestAccount
{
    public int GuestAccountId { get; set; }

    public int GuestId { get; set; }

    public decimal TotalBill { get; set; }
    public decimal PaidAmount { get; set; }

    public decimal DueAmount => TotalBill - PaidAmount;

    public bool IsCleared => DueAmount <= 0;
}

