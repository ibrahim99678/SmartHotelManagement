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
    public class RoomUnitOfWork : UnitOfWork, IRoomUnitOfWork
    {
        public RoomUnitOfWork(SmartHotelDbContext context, IRoomRepository roomRepository) : base(context)
        {
            RoomRepository = roomRepository;
        }

        public IRoomRepository RoomRepository { get; }

       
    }
}
