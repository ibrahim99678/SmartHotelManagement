using Microsoft.EntityFrameworkCore;
using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Implemention;

public class GuestUnitOfWork : UnitOfWork, IGuestUnitOfWork
{
    public GuestUnitOfWork(SmartHotelDbContext context, IGuestRepository guestRepository) : base(context)
    {
        GuestRepository = guestRepository;
    }

    public IGuestRepository GuestRepository { get; } 
}
