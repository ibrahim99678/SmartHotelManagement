using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class RoomTypeController : Controller
{
    private readonly IRoomTypeService _roomTypeService;


    public RoomTypeController(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    public async Task<IActionResult> Index()
    {
        var roomTypes = await _roomTypeService.GetAllAsync();
        return View(roomTypes.Data);
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoomTypeRequest roomType)
    {
       if(!ModelState.IsValid) return View(roomType);
        var result = await _roomTypeService.AddAsync(roomType);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Room Type added Successfully.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
            return View(roomType);
        }

    }
}