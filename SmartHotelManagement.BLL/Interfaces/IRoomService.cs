using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Interfaces
{
    public interface IRoomService
    {
        Task<Result<IList<Room>>> GetAllAsync();
        Task<Result<Room?>> GetByIdAsync(int id);
        Task<Result<int>> AddAsync(CreateRoomRequest room);
        Task<Result<int>> UpdateAsync(Room room);
        Task<Result<bool>> DeleteAsync(int id);
        
    }
}
