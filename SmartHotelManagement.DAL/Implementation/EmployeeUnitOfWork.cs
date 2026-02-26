using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;

namespace SmartHotelManagement.DAL.Implementation;

public class EmployeeUnitOfWork : UnitOfWork, IEmployeeUnitOfWork
{
    public IEmployeeRepository EmployeeRepository { get; }

    public EmployeeUnitOfWork(SmartHotelDbContext context, IEmployeeRepository employeeRepository) : base(context)
    {
        EmployeeRepository = employeeRepository;
    }
}
