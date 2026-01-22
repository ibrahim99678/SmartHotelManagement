using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Implementation
{
    public class ReservationRepository : Repository<Reservation, int, SmartHotelDbContext>, IReservationRepository
    {
        public ReservationRepository(SmartHotelDbContext context) : base(context)
        {
        }
    }
}
