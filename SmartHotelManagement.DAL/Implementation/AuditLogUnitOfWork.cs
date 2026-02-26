using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;

namespace SmartHotelManagement.DAL.Implementation;

public class AuditLogUnitOfWork : UnitOfWork, IAuditLogUnitOfWork
{
    public IAuditLogRepository AuditLogRepository { get; }

    public AuditLogUnitOfWork(SmartHotelDbContext context, IAuditLogRepository auditLogRepository) : base(context)
    {
        AuditLogRepository = auditLogRepository;
    }
}
