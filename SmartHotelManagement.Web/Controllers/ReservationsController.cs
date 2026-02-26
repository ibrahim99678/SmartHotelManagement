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

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            var guests = _guestService.GetAllAsync().Result.Data;
            var rooms = _roomService.GetAllAsync().Result.Data;
            ViewData["GuestId"] = new SelectList(guests, "GuestId", "FirstName");
            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "RoomNumber");
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
    }
}
