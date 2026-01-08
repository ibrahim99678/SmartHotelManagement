using Microsoft.EntityFrameworkCore;
using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Implementation;

public class RoomTypeRepository 
    : Repository<RoomType, int, SmartHotelDbContext>, 
    IRoomTypeRepository
{
    public RoomTypeRepository(SmartHotelDbContext dbContext) : base(dbContext)
    {
    }
   
}
