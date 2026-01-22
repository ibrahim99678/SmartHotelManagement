using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.BLL.Mapping;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.DAL.Migrations;
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

        public ReservationService(IReservationUnitOfWork reservationUnitOfWork)
        {
            _reservationUnitOfWork = reservationUnitOfWork;
        }

        public async Task<Result<int>> AddAsync(CreateReservationRequest reservation)
        {
            if (reservation == null)
            {
                return Result<int>.FailResult("Reservation cannot be null");
            }
            try
            {
                var newReservation = reservation.MapToReservation();
                //await _reservationUnitOfWork.ReservationRepository.AddAsync(newReservation);
                await _reservationUnitOfWork.SaveChangesAsync();
                return Result<int>.SuccessResult(newReservation.ReservationId);
            }
            catch (Exception)
            {
                return Result<int>.FailResult("An error occurred while adding the reservation");
            }
        }
      
        public Task<Result<bool>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IList<Reservation>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<Reservation?>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<int>> UpdateAsync(Reservation reservation)
        {
            throw new NotImplementedException();
        }
    }
}
