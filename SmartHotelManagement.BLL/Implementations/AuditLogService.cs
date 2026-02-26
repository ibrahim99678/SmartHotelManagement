using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Implementations;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogUnitOfWork _uow;
    public AuditLogService(IAuditLogUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> LogAsync(AuditLog log)
    {
        await _uow.AuditLogRepository.AddAsync(log);
        var saved = await _uow.SaveChangesAsync();
        if (!saved) return Result<int>.FailResult("Failed to save audit log.");
        return Result<int>.SuccessResult(log.AuditLogId);
    }
}
