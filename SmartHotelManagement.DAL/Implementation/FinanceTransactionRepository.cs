using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.DAL.Implementation;

public class FinanceTransactionRepository : Repository<FinancialTransaction, int, SmartHotelDbContext>, IFinanceTransactionRepository
{
    public FinanceTransactionRepository(SmartHotelDbContext context) : base(context)
    {
    }
}
