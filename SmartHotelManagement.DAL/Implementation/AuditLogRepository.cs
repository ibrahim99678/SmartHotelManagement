using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.DAL.Implementation;

public class AuditLogRepository : Repository<AuditLog, int, SmartHotelDbContext>, IAuditLogRepository
{
    public AuditLogRepository(SmartHotelDbContext context) : base(context)
    {
    }
}
