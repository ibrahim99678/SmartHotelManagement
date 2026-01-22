using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Implementation;

public class ReservationUnitOfWork : UnitOfWork, IReservationUnitOfWork
{
    public IReservationRepository ReservationRepository { get; }

    public ReservationUnitOfWork(SmartHotelDbContext context, IReservationRepository reservationRepository)
        : base(context)
    {
        ReservationRepository = reservationRepository;
    }
}
