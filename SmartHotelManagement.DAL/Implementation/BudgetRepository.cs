using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.DAL.Implementation;

public class BudgetRepository : Repository<Budget, int, SmartHotelDbContext>, IBudgetRepository
{
    public BudgetRepository(SmartHotelDbContext context) : base(context)
    {
    }
}
