using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IRoomChangeService _roomChangeService;
        private readonly IPaymentService _paymentService;
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;

        public ReservationsController(IReservationService reservationService, IRoomChangeService roomChangeService, IPaymentService paymentService, IGuestService guestService, IRoomService roomService)
        {
            _reservationService = reservationService;
            _roomChangeService = roomChangeService;
            _paymentService = paymentService;
            _guestService = guestService;
            _roomService = roomService;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var listResult = await _reservationService.GetAllAsync();
            return View(listResult.Success ? listResult.Data : new List<Reservation>());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationResult = await _reservationService.GetByIdAsync(id.Value);
            if (!reservationResult.Success || reservationResult.Data == null)
            {
                return NotFound();
            }

            var reservation = reservationResult.Data;
            var statusResult = await _paymentService.GetReservationPaymentStatusAsync(reservation.ReservationId);
            var dueResult = await _paymentService.GetReservationDueAmountAsync(reservation.ReservationId);
            if (statusResult.Success)
                ViewData["PaymentStatus"] = statusResult.Data;
            if (dueResult.Success)
                ViewData["DueAmount"] = dueResult.Data;
            var paymentsResult = await _paymentService.GetPaymentsForReservationAsync(reservation.ReservationId);
            if (paymentsResult.Success)
                ViewData["Payments"] = paymentsResult.Data;
            var guestPayments = await _paymentService.GetPaymentsForGuestAsync(reservation.GuestId);
            if (guestPayments.Success)
                ViewData["GuestPayments"] = guestPayments.Data;
            var guestReservations = await _reservationService.GetAllAsync();
            ViewData["GuestReservations"] = guestReservations.Success
                ? guestReservations.Data.Where(r => r.GuestId == reservation.GuestId && r.ReservationId != reservation.ReservationId)
                    .OrderByDescending(r => r.CheckInDate).Take(5).ToList()
                : new List<Reservation>();

            var roomsResult = await _roomService.GetAllAsync();
            var reservationsResult = await _reservationService.GetAllAsync();
            var allRooms = roomsResult.Success ? roomsResult.Data : new List<Room>();
            var allReservations = reservationsResult.Success ? reservationsResult.Data : new List<Reservation>();
            var reservedRoomIds = new HashSet<int>(
                allReservations
                    .Where(r => r.ReservationId != reservation.ReservationId && r.CheckOutDate.HasValue
                        && reservation.CheckInDate < r.CheckOutDate.Value
                        && reservation.CheckOutDate > r.CheckInDate)
                    .Select(r => r.RoomId)
            );
            var availableRooms = allRooms
                .Where(room => room.IsActive && room.RoomId != reservation.RoomId && !reservedRoomIds.Contains(room.RoomId))
                .OrderBy(r => r.RoomNumber)
                .ToList();
            ViewData["AvailableRooms"] = new SelectList(availableRooms, "RoomId", "RoomNumber");
            var roomMap = allRooms.ToDictionary(r => r.RoomId, r => r.RoomNumber);
            ViewBag.RoomMap = roomMap;
            var changes = await _roomChangeService.GetHistoryAsync(reservation.ReservationId);
            ViewData["RoomChanges"] = changes;

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            var guests = _guestService.GetAllAsync().Result.Data;
            var rooms = _roomService.GetAllAsync().Result.Data;
            ViewData["GuestId"] = new SelectList(guests, "GuestId", "FirstName");
            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "RoomNumber");
            ViewBag.Rooms = rooms;
            var roomTypes = rooms?.Where(r => r.RoomType != null)
                                  .Select(r => r.RoomType!)
                                  .GroupBy(rt => rt.RoomTypeId)
                                  .Select(g => g.First())
                                  .OrderBy(rt => rt.RoomTypeName)
                                  .ToList() ?? new List<RoomType>();
            ViewData["RoomTypeId"] = new SelectList(roomTypes, "RoomTypeId", "RoomTypeName");
            var statusItems = Enum.GetValues(typeof(SmartHotelManagement.Model.RoomStatus))
                                  .Cast<SmartHotelManagement.Model.RoomStatus>()
                                  .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });
            ViewBag.StatusList = statusItems;
            ViewBag.RoomRates = rooms?.Select(r => new { r.RoomId, Rate = r.BaseRate }).ToList();
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                var req = new SmartHotelManagement.Contract.Request.CreateReservationRequest
                {
                    GuestId = reservation.GuestId,
                    RoomId = reservation.RoomId,
                    ReferenceName = reservation.ReferenceName ?? "",
                    ReferencePhone = reservation.ReferencePhone?.ToString(),
                    SpouseName = reservation.SpouseName ?? "",
                    CheckInDate = reservation.CheckInDate,
                    CheckOutDate = reservation.CheckOutDate,
                    StayInNight = reservation.StayInNight,
                    TotalAmount = reservation.TotalAmount ?? 0,
                    Status = reservation.Status ?? "Booked",
                    IsCheckedIn = reservation.IsCheckedIn,
                    IsCheckedOut = reservation.IsCheckedOut
                };
                var result = await _reservationService.AddAsync(req);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Reservation created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = result.Error;
            }
            var guests = _guestService.GetAllAsync().Result.Data;
            var rooms = _roomService.GetAllAsync().Result.Data;
            ViewData["GuestId"] = new SelectList(guests, "GuestId", "FirstName", reservation.GuestId);
            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "RoomNumber", reservation.RoomId);
            ViewData["Rooms"] = null;
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationResult = await _reservationService.GetByIdAsync(id.Value);
            if (!reservationResult.Success || reservationResult.Data == null)
            {
                return NotFound();
            }
            var reservation = reservationResult.Data;
            var guests = _guestService.GetAllAsync().Result.Data;
            var rooms = _roomService.GetAllAsync().Result.Data;
            ViewData["GuestId"] = new SelectList(guests, "GuestId", "FirstName", reservation.GuestId);
            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "RoomNumber", reservation.RoomId);
            var statusItems = Enum.GetValues(typeof(SmartHotelManagement.Model.RoomStatus))
                                  .Cast<SmartHotelManagement.Model.RoomStatus>()
                                  .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() });
            ViewBag.StatusList = statusItems;
            var statusResult = await _paymentService.GetReservationPaymentStatusAsync(reservation.ReservationId);
            var dueResult = await _paymentService.GetReservationDueAmountAsync(reservation.ReservationId);
            if (statusResult.Success)
                ViewData["PaymentStatus"] = statusResult.Data;
            if (dueResult.Success)
                ViewData["DueAmount"] = dueResult.Data;
            var paymentsResult = await _paymentService.GetPaymentsForReservationAsync(reservation.ReservationId);
            if (paymentsResult.Success)
                ViewData["Payments"] = paymentsResult.Data;
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _reservationService.UpdateAsync(reservation);
                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Error;
                    var guests = _guestService.GetAllAsync().Result.Data;
                    var rooms = _roomService.GetAllAsync().Result.Data;
                    ViewData["GuestId"] = new SelectList(guests, "GuestId", "FirstName", reservation.GuestId);
                    ViewData["RoomId"] = new SelectList(rooms, "RoomId", "RoomNumber", reservation.RoomId);
                    return View(reservation);
                }
                return RedirectToAction(nameof(Index));
            }
            var g2 = _guestService.GetAllAsync().Result.Data;
            var r2 = _roomService.GetAllAsync().Result.Data;
            ViewData["GuestId"] = new SelectList(g2, "GuestId", "FirstName", reservation.GuestId);
            ViewData["RoomId"] = new SelectList(r2, "RoomId", "RoomNumber", reservation.RoomId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservationResult = await _reservationService.GetByIdAsync(id.Value);
            if (!reservationResult.Success || reservationResult.Data == null)
            {
                return NotFound();
            }

            return View(reservationResult.Data);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _reservationService.DeleteAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Error;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _reservationService.GetByIdAsync(id).Result.Success;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRoom(int reservationId, int newRoomId, string reason)
        {
            try
            {
                await _roomChangeService.ChangeRoomAsync(reservationId, newRoomId, reason);
                TempData["SuccessMessage"] = "Room changed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id = reservationId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPayment(int reservationId, decimal amount, PaymentType paymentType, string? bankName)
        {
            var result = await _paymentService.AddPaymentAsync(reservationId, amount, paymentType, bankName);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Payment recorded successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
            }
            return RedirectToAction(nameof(Details), new { id = reservationId });
        }

        [HttpGet]
        public async Task<IActionResult> PaymentStatus(int id)
        {
            var statusResult = await _paymentService.GetReservationPaymentStatusAsync(id);
            var dueResult = await _paymentService.GetReservationDueAmountAsync(id);
            if (!statusResult.Success || !dueResult.Success)
            {
                TempData["ErrorMessage"] = statusResult.Success ? dueResult.Error : statusResult.Error;
                return RedirectToAction(nameof(Details), new { id });
            }
            ViewData["PaymentStatus"] = statusResult.Data;
            ViewData["DueAmount"] = dueResult.Data;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteReservation(int id)
        {
            var reservationResult = await _reservationService.GetByIdAsync(id);
            if (!reservationResult.Success || reservationResult.Data == null)
            {
                TempData["ErrorMessage"] = reservationResult.Error;
                return RedirectToAction(nameof(Index));
            }
            var reservation = reservationResult.Data;
            var dueResult = await _paymentService.GetReservationDueAmountAsync(id);
            if (!dueResult.Success)
            {
                TempData["ErrorMessage"] = dueResult.Error;
                return RedirectToAction(nameof(Details), new { id });
            }
            var due = dueResult.Data;
            if (due > 0)
            {
                var payRes = await _paymentService.AddPaymentAsync(id, due, PaymentType.Cash, null);
                if (!payRes.Success)
                {
                    TempData["ErrorMessage"] = payRes.Error;
                    return RedirectToAction(nameof(Details), new { id });
                }
            }
            reservation.IsCheckedOut = true;
            reservation.Status = "CheckedOut";
            var upd = await _reservationService.UpdateAsync(reservation);
            if (!upd.Success)
            {
                TempData["ErrorMessage"] = upd.Error;
                return RedirectToAction(nameof(Details), new { id });
            }
            return RedirectToAction(nameof(Invoice), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
        {
            var reservationResult = await _reservationService.GetByIdAsync(id);
            if (!reservationResult.Success || reservationResult.Data == null)
            {
                TempData["ErrorMessage"] = reservationResult.Error;
                return RedirectToAction(nameof(Index));
            }
            var reservation = reservationResult.Data;
            var dueResult = await _paymentService.GetReservationDueAmountAsync(id);
            var due = dueResult.Success ? dueResult.Data : 0;
            var total = reservation.TotalAmount ?? 0;
            var paid = total - due;
            ViewBag.Total = total;
            ViewBag.Paid = paid < 0 ? 0 : paid;
            ViewBag.Due = due < 0 ? 0 : due;
            return View(reservation);
        }
    }
}
