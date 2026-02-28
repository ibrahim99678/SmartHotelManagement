using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.DAL.Implementation;

public class PaymentMethodRepository : Repository<PaymentMethod, int, SmartHotelDbContext>, IPaymentMethodRepository
{
    public PaymentMethodRepository(SmartHotelDbContext context) : base(context)
    {
    }
}
