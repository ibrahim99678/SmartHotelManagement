using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.Web.Controllers;

public class RoomController : Controller
{
    private readonly IRoomService _roomService;
    private readonly IWebHostEnvironment _hostEnvironment;

    public RoomController(IRoomService roomService, IWebHostEnvironment hostEnvironment)
    {
        _roomService = roomService;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var roomsResult = await _roomService.GetAllAsync();
        return View(roomsResult.Data);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken] 
    public async Task<IActionResult> Create(CreateRoomRequest room, IFormFile? imageFile)
    {
        if(!ModelState.IsValid) return View(room);

        if(imageFile!=null && imageFile.Length>0)
        {
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "rooms");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var fullPath = Path.Combine(uploadsFolder, fileName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            room.RoomImage = "/uploads/rooms/" + fileName;
        }

        var result = await _roomService.AddAsync(room);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Room added Successfully.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
            return View(room);
        }
    }
}