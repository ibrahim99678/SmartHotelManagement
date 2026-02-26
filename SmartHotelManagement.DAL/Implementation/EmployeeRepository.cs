using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.DAL.Implementation;

public class EmployeeRepository : Repository<Employee, int, SmartHotelDbContext>, IEmployeeRepository
{
    public EmployeeRepository(SmartHotelDbContext context) : base(context)
    {
    }
}
