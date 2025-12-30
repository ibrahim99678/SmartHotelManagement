using Microsoft.EntityFrameworkCore;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Context
{
    public class SmartHotelDbContext : DbContext
    {
        public SmartHotelDbContext(DbContextOptions<SmartHotelDbContext>options) : base(options)
        {
        }
       public DbSet<Guest> Guests { get; set; }
    }
}
