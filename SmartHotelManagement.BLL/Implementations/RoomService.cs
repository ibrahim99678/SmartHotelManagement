using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.BLL.Mapping;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Implementations;

public class RoomService : IRoomService
{
    private readonly IRoomUnitOfWork _roomUnitOfWork;

    public RoomService(IRoomUnitOfWork roomUnitOfWork)
    {
        _roomUnitOfWork = roomUnitOfWork;
    }

    public async Task<Result<int>> AddAsync(CreateRoomRequest room)
    {
        if (room is null)
        {
            return (Result<int>.FailResult("Room can not be null."));
        }
        try
        {
            var newRoom = room.MapToRoom();
            await _roomUnitOfWork.RoomRepository.AddAsync(newRoom);
            var saved = await _roomUnitOfWork.SaveChangesAsync();
            if (!saved)
            {
                return Result<int>.FailResult("Failed to save the room.");
            }
            return Result<int>.SuccessResult(newRoom.RoomId);
        }
        catch (Exception)
        {

            return Result<int>.FailResult("Failed to add room.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var newRoom = await _roomUnitOfWork.RoomRepository.GetByIdAsync(id);
        if (newRoom is not null)
        {
            await _roomUnitOfWork.RoomRepository.DeleteAsync(newRoom);
            return Result<bool>.SuccessResult(true);
        }

        return Result<bool>.FailResult("Room not found.");
    }



    public async Task<Result<IList<Room>>> GetAllAsync()
    {
        var rooms = await _roomUnitOfWork.RoomRepository.GetAsync(
            x => x, // selector: return the Room entity itself
            null,   // predicate: no filter
            q => q.OrderByDescending(x => x.RoomId) // order by RoomId descending
        );
        return Result<IList<Room>>.SuccessResult(rooms);
    }

    public async Task<Result<Room?>> GetByIdAsync(int id)
    {
        var room = await _roomUnitOfWork.RoomRepository.GetByIdAsync(id);
        if(room is not null)
        {
            return Result<Room?>.SuccessResult(room);
        }
        return Result<Room?>.FailResult("Room not found.");
    }

    public Task<Result<int>> UpdateAsync(Room room)
    {
        var existingRoomTask = _roomUnitOfWork.RoomRepository.GetByIdAsync(room.RoomId);
        if (existingRoomTask is not null)
        {
            var existingRoom = existingRoomTask.Result;
            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.RoomTypeId = room.RoomTypeId;
            existingRoom.FloorNo = room.FloorNo;
            existingRoom.Capacity = room.Capacity;
            existingRoom.BaseRate = room.BaseRate;
            existingRoom.Status = room.Status;
            existingRoom.RoomImage = room.RoomImage;
            existingRoom.Notes = room.Notes;
            existingRoom.IsActive = room.IsActive;
            _roomUnitOfWork.RoomRepository.UpdateAsync(existingRoom);
            var saved = _roomUnitOfWork.SaveChangesAsync().Result;
            if (!saved)
            {
                return Task.FromResult(Result<int>.FailResult("Failed to update the room."));
            }
            return Task.FromResult(Result<int>.SuccessResult(existingRoom.RoomId));
        }
        return Task.FromResult(Result<int>.FailResult("Room not found."));
    }
}
