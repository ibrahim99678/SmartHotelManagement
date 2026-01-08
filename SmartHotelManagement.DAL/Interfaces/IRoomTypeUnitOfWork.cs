using SmartHotelManagement.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Interfaces
{
    public interface IRoomTypeUnitOfWork : IUnitOfWork
    {
        IRoomTypeRepository RoomTypeRepository { get; }

    }
}
