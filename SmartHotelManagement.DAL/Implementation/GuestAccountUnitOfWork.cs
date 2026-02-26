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
    public class GuestAccountUnitOfWork : UnitOfWork, IGuestAccountUnitOfWork
    {
        public IGuestAccountRepository GuestAccountRepository { get; }

        public GuestAccountUnitOfWork(SmartHotelDbContext context, IGuestAccountRepository guestAccountRepository) : base(context)
        {
            GuestAccountRepository = guestAccountRepository;
        }
    }
}
