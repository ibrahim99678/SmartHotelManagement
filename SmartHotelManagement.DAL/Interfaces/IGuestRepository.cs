using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Interfaces;

public interface IGuestRepository 
        :IRepository<Guest, int, SmartHotelDbContext>
{
}
