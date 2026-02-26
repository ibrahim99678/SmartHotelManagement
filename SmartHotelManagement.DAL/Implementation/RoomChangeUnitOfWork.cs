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
    public class RoomChangeUnitOfWork : UnitOfWork, IRoomChangeUnitOfWork
    {
        public RoomChangeUnitOfWork(SmartHotelDbContext context, IRoomChangeRepository roomChangeRepository) : base(context)
        {
            RoomChangeRepository = roomChangeRepository;
        }
        public IRoomChangeRepository RoomChangeRepository { get; }
    }
}
