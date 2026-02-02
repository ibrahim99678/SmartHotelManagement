using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartHotelManagement.Web.Models;
using SmartHotelManagement.BLL.Interfaces;

namespace SmartHotelManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRoomService _roomService; // inject this via DI

        public HomeController(ILogger<HomeController> logger, IRoomService roomService)
        {
            _logger = logger;
            _roomService = roomService;
        }

        public async Task<IActionResult> Index()
        {
            var roomsResult = await _roomService.GetAllAsync();
            var rooms = roomsResult.Success && roomsResult.Data != null
                ? roomsResult.Data.Select(r => new RoomViewModel {
                      Id = r.RoomId,
                      Title = r.BaseRate.ToString("C"),
                      ImageUrl = r.RoomImage,
                      IsNew = r.IsActive,

                    // map other fields as needed
                }).ToList()
                : new List<RoomViewModel>();

            return View(rooms);
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
