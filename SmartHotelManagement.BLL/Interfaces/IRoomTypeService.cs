using Microsoft.AspNetCore.Mvc;
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
    public interface IRoomTypeService
    {
        Task<Result<IList<RoomType>>> GetAllAsync();
        Task<Result<RoomType?>> GetByIdAsync(int id);
        Task<Result<int>> AddAsync(CreateRoomTypeRequest roomType);
        Task<Result<int>> UpdateAsync(RoomType roomType);
        Task<Result<bool>> DeleteAsync(int id);

    }
}
