using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Interfaces;

public interface IGuestService
{
    Task<Result<IList<Guest>>> GetAllAsync();
    Task<Result<Guest?>> GetByIdAsync(int id);
    Task<Result<int>> AddAsync(CreateGuestRequest guest);
    Task<Result<int>> UpdateAsync(Guest guest);
    Task<Result<bool>> DeleteAsync(int id);   
}
