using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.DAL.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Interfaces;

public interface IReservationService
{
    Task<Result<IList<Reservation>>> GetAllAsync();
    Task<Result<Reservation?>> GetByIdAsync(int id);
    Task<Result<int>> AddAsync(CreateReservationRequest reservation);
    Task<Result<int>> UpdateAsync(Reservation reservation);
    Task<Result<bool>> DeleteAsync(int id);

}
