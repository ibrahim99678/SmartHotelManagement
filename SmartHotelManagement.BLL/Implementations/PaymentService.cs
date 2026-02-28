using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IPaymentUnitOfWork _paymentUnitOfWork;
    private readonly IReservationUnitOfWork _reservationUnitOfWork;
    private readonly IRoomUnitOfWork _roomUnitOfWork;

    public PaymentService(IPaymentUnitOfWork paymentUnitOfWork, IReservationUnitOfWork reservationUnitOfWork, IRoomUnitOfWork roomUnitOfWork)
    {
        _paymentUnitOfWork = paymentUnitOfWork ?? throw new ArgumentNullException(nameof(paymentUnitOfWork));
        _reservationUnitOfWork = reservationUnitOfWork ?? throw new ArgumentNullException(nameof(reservationUnitOfWork));
        _roomUnitOfWork = roomUnitOfWork ?? throw new ArgumentNullException(nameof(roomUnitOfWork));
    }

    public async Task<Result<int>> AddPaymentAsync(int reservationId, decimal amount, PaymentType paymentType, string? bankName)
    {
        var reservation = await _reservationUnitOfWork.ReservationRepository.GetByIdAsync(reservationId);
        if (reservation is null)
        {
            return Result<int>.FailResult("Reservation not found.");
        }

        var payment = new Payment
        {
            ReservationId = reservationId,
            Amount = amount,
            PaymentType = paymentType,
            BankName = bankName,
            Status = PaymentStatus.Paid,
            PaymentDate = DateTime.Now
        };

        await _paymentUnitOfWork.PaymentRepository.AddAsync(payment);
        var saved = await _paymentUnitOfWork.SaveChangesAsync();
        if (!saved)
        {
            return Result<int>.FailResult("Failed to record payment.");
        }
        var statusCheck = await GetReservationPaymentStatusAsync(reservationId);
        if (statusCheck.Success && statusCheck.Data == PaymentStatus.Paid)
        {
            var res = await _reservationUnitOfWork.ReservationRepository.GetByIdAsync(reservationId);
            if (res != null)
            {
                res.IsCheckedOut = true;
                res.Status = "CheckedOut";
                await _reservationUnitOfWork.ReservationRepository.UpdateAsync(res);
                await _reservationUnitOfWork.SaveChangesAsync();
                var room = await _roomUnitOfWork.RoomRepository.GetByIdAsync(res.RoomId);
                if (room != null)
                {
                    room.Status = RoomStatus.Available;
                    await _roomUnitOfWork.RoomRepository.UpdateAsync(room);
                    await _roomUnitOfWork.SaveChangesAsync();
                }
            }
        }
        return Result<int>.SuccessResult(payment.PaymentId);
    }

    public async Task<Result<PaymentStatus>> GetReservationPaymentStatusAsync(int reservationId)
    {
        var reservation = await _reservationUnitOfWork.ReservationRepository.GetByIdAsync(reservationId);
        if (reservation is null)
        {
            return Result<PaymentStatus>.FailResult("Reservation not found.");
        }
        var total = reservation.TotalAmount ?? 0;
        var payments = await _paymentUnitOfWork.PaymentRepository.GetAsync(p => p.Amount, x => x.ReservationId == reservationId);
        var paid = payments.Sum();
        var status = paid >= total ? PaymentStatus.Paid : PaymentStatus.Due;
        return Result<PaymentStatus>.SuccessResult(status);
    }

    public async Task<Result<decimal>> GetReservationDueAmountAsync(int reservationId)
    {
        var reservation = await _reservationUnitOfWork.ReservationRepository.GetByIdAsync(reservationId);
        if (reservation is null)
        {
            return Result<decimal>.FailResult("Reservation not found.");
        }
        var total = reservation.TotalAmount ?? 0;
        var payments = await _paymentUnitOfWork.PaymentRepository.GetAsync(p => p.Amount, x => x.ReservationId == reservationId);
        var paid = payments.Sum();
        var due = total - paid;
        if (due < 0) due = 0;
        return Result<decimal>.SuccessResult(due);
    }

    public async Task<Result<decimal>> GetMonthlySalesTotalAsync(DateTime monthStart, DateTime monthEnd)
    {
        var payments = await _paymentUnitOfWork.PaymentRepository.GetAsync(p => p.Amount, x => x.PaymentDate >= monthStart && x.PaymentDate < monthEnd);
        return Result<decimal>.SuccessResult(payments.Sum());
    }

    public async Task<Result<IDictionary<string, decimal>>> GetMonthlySalesByRoomTypeAsync(DateTime monthStart, DateTime monthEnd)
    {
        var payments = await _paymentUnitOfWork.PaymentRepository.GetAsync(p => p, x => x.PaymentDate >= monthStart && x.PaymentDate < monthEnd);
        var result = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        foreach (var pay in payments)
        {
            var reservation = await _reservationUnitOfWork.ReservationRepository.GetByIdAsync(pay.ReservationId);
            var roomTypeName = reservation?.Room?.RoomType?.RoomTypeName ?? "Unknown";
            if (!result.ContainsKey(roomTypeName)) result[roomTypeName] = 0;
            result[roomTypeName] += pay.Amount;
        }
        return Result<IDictionary<string, decimal>>.SuccessResult(result);
    }

    public async Task<Result<IList<Payment>>> GetPaymentsForReservationAsync(int reservationId)
    {
        var items = await _paymentUnitOfWork.PaymentRepository.GetAsync(p => p, x => x.ReservationId == reservationId);
        return Result<IList<Payment>>.SuccessResult(items.OrderByDescending(p => p.PaymentDate).ToList());
    }

    public async Task<Result<IList<Payment>>> GetPaymentsForGuestAsync(int guestId)
    {
        var reservations = await _reservationUnitOfWork.ReservationRepository.GetAsync(r => r.ReservationId, x => x.GuestId == guestId);
        var resIds = reservations.ToList();
        if (resIds.Count == 0) return Result<IList<Payment>>.SuccessResult(new List<Payment>());
        var items = await _paymentUnitOfWork.PaymentRepository.GetAsync(p => p, x => resIds.Contains(x.ReservationId));
        return Result<IList<Payment>>.SuccessResult(items.OrderByDescending(p => p.PaymentDate).ToList());
    }
}
