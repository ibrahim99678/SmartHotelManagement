using SmartHotelManagement.DAL.Core;

namespace SmartHotelManagement.DAL.Interfaces;

public interface IAuditLogUnitOfWork : IUnitOfWork
{
    IAuditLogRepository AuditLogRepository { get; }
}
