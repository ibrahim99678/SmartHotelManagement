using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Implementation;

public class PaymentRepository : Repository<Payment, int, SmartHotelDbContext>, IPaymentRepository
{
    public PaymentRepository(SmartHotelDbContext context) : base(context)
    {
    }

}
