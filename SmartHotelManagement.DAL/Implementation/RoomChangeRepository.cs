using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.Model;
using SmartHotelManagement.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Implementation;

public class RoomChangeRepository : Repository<RoomChange, int, SmartHotelDbContext>, IRoomChangeRepository
{
    public RoomChangeRepository(SmartHotelDbContext context) : base(context)
    {
    }

}
