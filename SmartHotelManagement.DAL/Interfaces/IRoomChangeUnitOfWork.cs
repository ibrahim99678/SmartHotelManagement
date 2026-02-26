using SmartHotelManagement.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Interfaces
{
    public interface IRoomChangeUnitOfWork : IUnitOfWork
    {
        IRoomChangeRepository RoomChangeRepository { get; }
    }
}
