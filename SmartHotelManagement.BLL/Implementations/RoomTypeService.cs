using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.BLL.Mapping;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.DAL.Implemention;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Implementations;

public class RoomTypeService : IRoomTypeService
{
    private readonly IRoomTypeUnitOfWork _roomTypeUnitOfWork;

    public RoomTypeService(IRoomTypeUnitOfWork roomTypeUnitOfWork)
    {
        _roomTypeUnitOfWork = roomTypeUnitOfWork;
    }

    public async Task<Result<int>> AddAsync(CreateRoomTypeRequest roomType)
    {
        if (roomType is null)
        {
            return Result<int>.FailResult("Room Type can not be null.");
        }
        try
        {
            var newRoomType = roomType.MapToRoomType();
            await _roomTypeUnitOfWork.RoomTypeRepository.AddAsync(newRoomType);
            var saved = await _roomTypeUnitOfWork.SaveChangesAsync();
            if (!saved)
            {
                return Result<int>.FailResult("Failed to save the Room Type.");
            }
            return Result<int>.SuccessResult(newRoomType.RoomTypeId);
        }
        catch (Exception)
        {
            return Result<int>.FailResult("An Error Occured While adding the Room Type.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var roomType = await _roomTypeUnitOfWork.RoomTypeRepository.GetByIdAsync(id);
        if (roomType is not null)
        {
            await _roomTypeUnitOfWork.RoomTypeRepository.DeleteAsync(roomType);
            return Result<bool>.SuccessResult(true);
        }
        
        return Result<bool>.FailResult("Room Type not found.");
    }

    public async Task<Result<IList<RoomType>>> GetAllAsync()
    {
        var roomTypes = await _roomTypeUnitOfWork.RoomTypeRepository.GetAsync(
            x => x, // selector: return the RoomType entity itself
            null,   // predicate: no filter
            q => q.OrderByDescending(x => x.RoomTypeId) // order by RoomTypeId descending
        );
        return Result<IList<RoomType>>.SuccessResult(roomTypes);
    }

    public async Task<Result<RoomType?>> GetByIdAsync(int id)
    {
        var roomType = await _roomTypeUnitOfWork.RoomTypeRepository.GetByIdAsync(id);
        if (roomType is not null)
        {
            return Result<RoomType?>.SuccessResult(roomType);
        }
        return Result<RoomType?>.FailResult("Room Type not found.");

    }

    public async Task<Result<int>> UpdateAsync(RoomType roomType)
    {
        var existingRoomType = await _roomTypeUnitOfWork.RoomTypeRepository.GetByIdAsync(roomType.RoomTypeId);
        if (existingRoomType is null)
        {
            existingRoomType.RoomTypeName = roomType.RoomTypeName;
            existingRoomType.DefaultRate = roomType.DefaultRate;
            await _roomTypeUnitOfWork.RoomTypeRepository.UpdateAsync(existingRoomType);
            await _roomTypeUnitOfWork.SaveChangesAsync();      
        }
        return Result<int>.FailResult("Room Type not found.");
    }
}
