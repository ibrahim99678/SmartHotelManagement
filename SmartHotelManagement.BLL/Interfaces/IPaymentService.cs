using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.Model;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Interfaces;

public interface IPaymentService
{
    Task<Result<int>> AddPaymentAsync(int reservationId, decimal amount, PaymentType paymentType, string? bankName);
    Task<Result<PaymentStatus>> GetReservationPaymentStatusAsync(int reservationId);
    Task<Result<decimal>> GetReservationDueAmountAsync(int reservationId);
    Task<Result<decimal>> GetMonthlySalesTotalAsync(DateTime monthStart, DateTime monthEnd);
    Task<Result<IDictionary<string, decimal>>> GetMonthlySalesByRoomTypeAsync(DateTime monthStart, DateTime monthEnd);
    Task<Result<IList<Payment>>> GetPaymentsForReservationAsync(int reservationId);
    Task<Result<IList<Payment>>> GetPaymentsForGuestAsync(int guestId);
}
