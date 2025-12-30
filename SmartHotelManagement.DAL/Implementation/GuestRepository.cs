using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Implemention;

public class GuestRepository : Repository<Guest, int, SmartHotelDbContext>, IGuestRepository
{
    public GuestRepository(SmartHotelDbContext context) : base(context)
    {
    }
}
