using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.DAL.Interfaces;

public interface IPaymentMethodRepository : IRepository<PaymentMethod, int, SmartHotelDbContext>
{
}
