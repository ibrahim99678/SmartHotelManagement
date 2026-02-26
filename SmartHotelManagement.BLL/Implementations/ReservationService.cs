using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.BLL.Mapping;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationUnitOfWork _reservationUnitOfWork;
        private readonly IRoomUnitOfWork _roomUnitOfWork;

        public ReservationService(IReservationUnitOfWork reservationUnitOfWork, IRoomUnitOfWork roomUnitOfWork)
        {
            _reservationUnitOfWork = reservationUnitOfWork;
            _roomUnitOfWork = roomUnitOfWork;
        }

        public async Task<Result<int>> AddAsync(CreateReservationRequest reservation)
        {
            if (reservation == null)
            {
                return Result<int>.FailResult("Reservation cannot be null");
            }
            if (!reservation.CheckOutDate.HasValue || reservation.CheckOutDate.Value <= reservation.CheckInDate)
            {
                return Result<int>.FailResult("Check-out must be after check-in.");
            }
            try
            {
                var nights = reservation.StayInNight.HasValue && reservation.StayInNight.Value > 0
                    ? reservation.StayInNight.Value
                    : (reservation.CheckOutDate.Value.Date - reservation.CheckInDate.Date).Days;
                reservation.StayInNight = nights;

                var overlap = await _reservationUnitOfWork.ReservationRepository.IsExistsAsync(x =>
                    x.RoomId == reservation.RoomId &&
                    reservation.CheckInDate < x.CheckOutDate &&
                    reservation.CheckOutDate > x.CheckInDate);
                if (overlap)
                {
                    return Result<int>.FailResult("Selected room is already booked for the given dates.");
                }

                var newReservation = reservation.MapToReservation();
                var room = await _roomUnitOfWork.RoomRepository.GetByIdAsync(newReservation.RoomId);
                var rate = room != null
                    ? (room.BaseRate > 0 ? room.BaseRate : (room.RoomType?.DefaultRate ?? 0))
                    : 0;
                newReservation.TotalAmount = rate * nights;
                await _reservationUnitOfWork.ReservationRepository.AddAsync(newReservation);
                await _reservationUnitOfWork.SaveChangesAsync();
                return Result<int>.SuccessResult(newReservation.ReservationId);
            }
            catch (Exception)
            {
                return Result<int>.FailResult("An error occurred while adding the reservation");
            }
        }
      
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var entity = await _reservationUnitOfWork.ReservationRepository.GetByIdAsync(id);
            if (entity is null)
            {
                return Result<bool>.FailResult("Reservation not found.");
            }
            await _reservationUnitOfWork.ReservationRepository.DeleteAsync(entity);
            var saved = await _reservationUnitOfWork.SaveChangesAsync();
            return saved ? Result<bool>.SuccessResult(true) : Result<bool>.FailResult("Failed to delete reservation.");
        }

        public async Task<Result<IList<Reservation>>> GetAllAsync()
        {
            var list = await _reservationUnitOfWork.ReservationRepository.GetAsync(r => r,
                include: q => q.Include(x => x.Guest)
                               .Include(x => x.Room)
                               .ThenInclude(r => r.RoomType));
            return Result<IList<Reservation>>.SuccessResult(list);
        }

        public async Task<Result<Reservation?>> GetByIdAsync(int id)
        {
            var item = await _reservationUnitOfWork.ReservationRepository.GetFirstOrDefaultAsync(r => r,
                predicate: x => x.ReservationId == id,
                include: q => q.Include(x => x.Guest).Include(x => x.Room));
            if (item is null)
            {
                return Result<Reservation?>.FailResult("Reservation not found.");
            }
            return Result<Reservation?>.SuccessResult(item);
        }

        public async Task<Result<int>> UpdateAsync(Reservation reservation)
        {
            if (!reservation.CheckOutDate.HasValue || reservation.CheckOutDate.Value <= reservation.CheckInDate)
            {
                return Result<int>.FailResult("Check-out must be after check-in.");
            }
            var existing = await _reservationUnitOfWork.ReservationRepository.GetByIdAsync(reservation.ReservationId);
            if (existing is null)
            {
                return Result<int>.FailResult("Reservation not found.");
            }
            var nights = reservation.StayInNight.HasValue && reservation.StayInNight.Value > 0
                ? reservation.StayInNight.Value
                : (reservation.CheckOutDate.Value.Date - reservation.CheckInDate.Date).Days;

            var overlap = await _reservationUnitOfWork.ReservationRepository.IsExistsAsync(x =>
                x.RoomId == reservation.RoomId &&
                x.ReservationId != reservation.ReservationId &&
                reservation.CheckInDate < x.CheckOutDate &&
                reservation.CheckOutDate > x.CheckInDate);
            if (overlap)
            {
                return Result<int>.FailResult("Selected room is already booked for the given dates.");
            }
            existing.GuestId = reservation.GuestId;
            existing.RoomId = reservation.RoomId;
            existing.ReferenceName = reservation.ReferenceName;
            existing.ReferencePhone = reservation.ReferencePhone;
            existing.SpouseName = reservation.SpouseName;
            existing.CheckInDate = reservation.CheckInDate;
            existing.CheckOutDate = reservation.CheckOutDate;
            existing.StayInNight = nights;
            var room = await _roomUnitOfWork.RoomRepository.GetByIdAsync(existing.RoomId);
            var rate = room != null
                ? (room.BaseRate > 0 ? room.BaseRate : (room.RoomType?.DefaultRate ?? 0))
                : 0;
            existing.TotalAmount = rate * nights;
            existing.Status = reservation.Status;
            existing.IsCheckedIn = reservation.IsCheckedIn;
            existing.IsCheckedOut = reservation.IsCheckedOut;
            await _reservationUnitOfWork.ReservationRepository.UpdateAsync(existing);
            var saved = await _reservationUnitOfWork.SaveChangesAsync();
            if (!saved)
            {
                return Result<int>.FailResult("Failed to update reservation.");
            }
            return Result<int>.SuccessResult(existing.ReservationId);
        }
    }
}
