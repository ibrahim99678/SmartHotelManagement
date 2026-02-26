using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartHotelManagement.Web.Models;
using SmartHotelManagement.BLL.Interfaces;
using System.Linq;

namespace SmartHotelManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRoomService _roomService; 
        private readonly IReservationService _reservationService;
        private readonly IPaymentService _paymentService;
        private readonly IEmployeeService _employeeService;

        public HomeController(ILogger<HomeController> logger, IRoomService roomService, IReservationService reservationService, IPaymentService paymentService, IEmployeeService employeeService)
        {
            _logger = logger;
            _roomService = roomService;
            _reservationService = reservationService;
            _paymentService = paymentService;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var todayStart = now.Date;
            var todayEnd = todayStart.AddDays(1);
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var reservationsResult = await _reservationService.GetAllAsync();
            var roomsResult = await _roomService.GetAllAsync();
            var employeesResult = await _employeeService.GetAllAsync();
            var salesTotalResult = await _paymentService.GetMonthlySalesTotalAsync(monthStart, monthEnd);
            var salesByTypeResult = await _paymentService.GetMonthlySalesByRoomTypeAsync(monthStart, monthEnd);

            var reservations = reservationsResult.Success ? reservationsResult.Data : new List<Model.Reservation>();
            var rooms = roomsResult.Success ? roomsResult.Data : new List<Model.Room>();
            var employeesCount = employeesResult.Success ? employeesResult.Data.Count : 0;
            var monthlySalesTotal = salesTotalResult.Success ? salesTotalResult.Data : 0;
            var monthlySalesByRoomType = salesByTypeResult.Success ? salesByTypeResult.Data : new Dictionary<string, decimal>();

            var todayReservations = reservations
                .Where(r => r.CheckOutDate.HasValue && todayStart < r.CheckOutDate.Value && todayEnd > r.CheckInDate)
                .OrderBy(r => r.CheckInDate)
                .ToList();

            var last7Days = todayStart.AddDays(-7);
            var last7Reservations = reservations
                .Where(r => r.CheckOutDate.HasValue && last7Days < r.CheckOutDate.Value && todayEnd > r.CheckInDate)
                .OrderByDescending(r => r.CheckInDate)
                .ToList();

            var reservedRoomIds = new HashSet<int>(todayReservations.Select(r => r.RoomId));
            var availableRoomsCount = rooms.Where(room => room.IsActive && !reservedRoomIds.Contains(room.RoomId)).Count();

            var vm = new DashboardViewModel
            {
                TodayReservations = todayReservations,
                Last7DaysReservations = last7Reservations,
                AvailableRoomsCount = availableRoomsCount,
                EmployeeCount = employeesCount,
                MonthlySalesTotal = monthlySalesTotal,
                MonthlySalesByRoomType = monthlySalesByRoomType
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
