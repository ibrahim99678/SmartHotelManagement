using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Context
{
    public class SmartHotelDbContext :IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public SmartHotelDbContext(DbContextOptions<SmartHotelDbContext> options) : base(options)
        {
        }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        //DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder<SmartHotelDbContext>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();


            modelBuilder.Entity<Room>()
                .Property(r => r.BaseRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RoomType>()
                .Property(rt => rt.DefaultRate)
                .HasColumnType("decimal(18,2)");
        }
    }
}
