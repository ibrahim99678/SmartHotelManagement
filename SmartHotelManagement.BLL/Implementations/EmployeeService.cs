using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeUnitOfWork _uow;
    public EmployeeService(IEmployeeUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> AddAsync(Employee employee)
    {
        if (employee == null) return Result<int>.FailResult("Employee cannot be null.");
        await _uow.EmployeeRepository.AddAsync(employee);
        var saved = await _uow.SaveChangesAsync();
        if (!saved) return Result<int>.FailResult("Failed to save employee.");
        return Result<int>.SuccessResult(employee.EmployeeId);
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _uow.EmployeeRepository.GetByIdAsync(id);
        if (entity == null) return Result<bool>.FailResult("Employee not found.");
        await _uow.EmployeeRepository.DeleteAsync(entity);
        var saved = await _uow.SaveChangesAsync();
        return saved ? Result<bool>.SuccessResult(true) : Result<bool>.FailResult("Failed to delete employee.");
    }

    public async Task<Result<IList<Employee>>> GetAllAsync()
    {
        var list = await _uow.EmployeeRepository.GetAsync(x => x, orderby: q => q.OrderBy(e => e.LastName));
        return Result<IList<Employee>>.SuccessResult(list);
    }

    public async Task<Result<Employee?>> GetByIdAsync(int id)
    {
        var entity = await _uow.EmployeeRepository.GetByIdAsync(id);
        if (entity == null) return Result<Employee?>.FailResult("Employee not found.");
        return Result<Employee?>.SuccessResult(entity);
    }

    public async Task<Result<int>> UpdateAsync(Employee employee)
    {
        var existing = await _uow.EmployeeRepository.GetByIdAsync(employee.EmployeeId);
        if (existing == null) return Result<int>.FailResult("Employee not found.");
        existing.FirstName = employee.FirstName;
        existing.LastName = employee.LastName;
        existing.Email = employee.Email;
        existing.PhoneNumber = employee.PhoneNumber;
        existing.Department = employee.Department;
        existing.Position = employee.Position;
        existing.HireDate = employee.HireDate;
        existing.IsActive = employee.IsActive;
        existing.UserId = employee.UserId;
        await _uow.EmployeeRepository.UpdateAsync(existing);
        var saved = await _uow.SaveChangesAsync();
        if (!saved) return Result<int>.FailResult("Failed to update employee.");
        return Result<int>.SuccessResult(existing.EmployeeId);
    }
}
