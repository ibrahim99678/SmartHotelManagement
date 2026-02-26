using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Implementations;

public class RoomChangeService : IRoomChangeService
{
    private readonly IReservationUnitOfWork _reservationUnitOfWork;
    private readonly IRoomUnitOfWork _roomUnitOfWork;
    private readonly IRoomChangeUnitOfWork _roomChangeUnitOfWork;

    public RoomChangeService(
        IReservationUnitOfWork reservationUnitOfWork,
        IRoomUnitOfWork roomUnitOfWork,
        IRoomChangeUnitOfWork roomChangeUnitOfWork)
    {
        _reservationUnitOfWork = reservationUnitOfWork ?? throw new ArgumentNullException(nameof(reservationUnitOfWork));
        _roomUnitOfWork = roomUnitOfWork ?? throw new ArgumentNullException(nameof(roomUnitOfWork));
        _roomChangeUnitOfWork = roomChangeUnitOfWork ?? throw new ArgumentNullException(nameof(roomChangeUnitOfWork));
    }

    public async Task ChangeRoomAsync(int reservationId, int newRoomId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required.", nameof(reason));

        var reservation = await _reservationUnitOfWork.ReservationRepository.GetByIdAsync(reservationId);

        if (reservation == null)
            throw new InvalidOperationException($"Reservation {reservationId} not found.");

        var newRoom = await _roomUnitOfWork.RoomRepository.GetByIdAsync(newRoomId);

        if (newRoom == null || newRoom.Status != RoomStatus.Available)
            throw new InvalidOperationException("New room not available.");

        var oldRoomId = reservation.RoomId;
        var oldRoom = await _roomUnitOfWork.RoomRepository.GetByIdAsync(oldRoomId);
        if (oldRoom != null)
        {
            oldRoom.Status = RoomStatus.Available;
            await _roomUnitOfWork.RoomRepository.UpdateAsync(oldRoom);
        }

        newRoom.Status = RoomStatus.Occupied;
        await _roomUnitOfWork.RoomRepository.UpdateAsync(newRoom);
        reservation.RoomId = newRoomId;
        await _reservationUnitOfWork.ReservationRepository.UpdateAsync(reservation);

        await _roomChangeUnitOfWork.RoomChangeRepository.AddAsync(new RoomChange
        {
            ReservationId = reservationId,
            OldRoomId = oldRoomId,
            NewRoomId = newRoomId,
            Reason = reason
        });

        await _roomChangeUnitOfWork.SaveChangesAsync();
    }
}


