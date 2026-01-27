using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class RoomController : Controller
{

    private readonly IRoomService _roomService;
    private readonly IRoomTypeService _roomTypeService;
    private readonly IWebHostEnvironment _hostEnvironment;

    public RoomController(IRoomService roomService, IRoomTypeService roomTypeService, IWebHostEnvironment hostEnvironment)
    {
        _roomService = roomService;
        _roomTypeService = roomTypeService;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var roomsResult = await _roomService.GetAllAsync();
        return View(roomsResult.Data);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["RoomTypeId"] = new SelectList(_roomTypeService.GetAllAsync().Result.Data, "RoomTypeId", "RoomTypeName");
        var typesResult = await _roomTypeService.GetAllAsync();
        ViewBag.RoomTypeName = typesResult.Success ? typesResult.Data : new List<RoomType>();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoomRequest room, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            var typesResult = await _roomTypeService.GetAllAsync();
            ViewBag.RoomTypeName = typesResult.Success ? typesResult.Data : new List<RoomType>();
            return View(room);
        }

        if (imageFile != null && imageFile.Length > 0)
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
            var typesResult = await _roomTypeService.GetAllAsync();
            ViewBag.RoomTypeName = typesResult.Success ? typesResult.Data : new List<RoomType>();
            TempData["ErrorMessage"] = result.Error;
            return View(room);
        }
    }
}