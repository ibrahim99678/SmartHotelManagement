using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.BLL.Mapping;
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

    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Edit(UpdateRoomRequest room, IFormFile? imageFile)
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

        // Map UpdateRoomRequest -> Room and call existing service method

        var roomEntity = room.MapToRoom();
        var result = await _roomService.UpdateAsync(roomEntity);

        if (result.Success)
        {
            TempData["SuccessMessage"] = "Room updated Successfully.";
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

    public async Task<IActionResult> Edit(int id)
    {
        var roomResult = await _roomService.GetByIdAsync(id);
        if (roomResult.Success && roomResult.Data != null)
        {
            ViewData["RoomTypeId"] = new SelectList(_roomTypeService.GetAllAsync().Result.Data, "RoomTypeId", "RoomTypeName", roomResult.Data.RoomTypeId);
            var typesResult = await _roomTypeService.GetAllAsync();
            ViewBag.RoomTypeName = typesResult.Success ? typesResult.Data : new List<RoomType>();
            return View(roomResult.Data);
        }
        else
        {
            TempData["ErrorMessage"] = roomResult.Error;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var room = await _roomService.GetByIdAsync(id.Value);
        if (room == null)
        {
            return NotFound();
        }

        return View(room.Data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _roomService.DeleteAsync(id);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Room deleted Successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error ?? "An error occurred while deleting the room.";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var roomResult = await _roomService.GetByIdAsync(id);
        if (roomResult.Success && roomResult.Data != null)
        {
            return View(roomResult.Data);
        }
        else
        {
            TempData["ErrorMessage"] = roomResult.Error;
            return RedirectToAction(nameof(Index));
        }
    }

}
