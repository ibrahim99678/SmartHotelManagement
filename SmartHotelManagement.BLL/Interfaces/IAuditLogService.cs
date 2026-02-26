using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.Model;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Interfaces;

public interface IAuditLogService
{
    Task<Result<int>> LogAsync(AuditLog log);
}
