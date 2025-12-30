using Microsoft.EntityFrameworkCore;
using SmartHotelManagement.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotelManagement.DAL.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        protected readonly SmartHotelDbContext _context;
        public UnitOfWork(SmartHotelDbContext context)
        {
            _context = context;
        }

        public bool SaveChanges()
        {
           return _context.SaveChanges() > 0;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
  
        public void Rollback()
        {
            _context.ChangeTracker.Entries()
                .ToList()
                .ForEach(x => x.ReloadAsync());
        }
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
