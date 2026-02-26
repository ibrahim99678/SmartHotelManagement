using SmartHotelManagement.DAL.Core;

namespace SmartHotelManagement.DAL.Interfaces;

public interface IEmployeeUnitOfWork : IUnitOfWork
{
    IEmployeeRepository EmployeeRepository { get; }
}
