using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Interfaces;

public interface IEmployeeService
{
    Task<Result<IList<Employee>>> GetAllAsync();
    Task<Result<Employee?>> GetByIdAsync(int id);
    Task<Result<int>> AddAsync(Employee employee);
    Task<Result<int>> UpdateAsync(Employee employee);
    Task<Result<bool>> DeleteAsync(int id);
}
