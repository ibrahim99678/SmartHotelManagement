using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Implementation
{
    public class RoomTypeUnitOfWork : UnitOfWork, IRoomTypeUnitOfWork
    {
        public RoomTypeUnitOfWork(SmartHotelDbContext context, IRoomTypeRepository roomTypeRepository) : base(context)
        {
            RoomTypeRepository = roomTypeRepository;
        }

        public IRoomTypeRepository RoomTypeRepository { get; }
      
    }
}
