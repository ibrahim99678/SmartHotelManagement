using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.DAL.Interfaces;

public interface IFinanceTransactionRepository : IRepository<FinancialTransaction, int, SmartHotelDbContext>
{
}
