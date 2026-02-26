using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.BLL.Mapping;
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

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _roomTypeService.GetByIdAsync(id);
        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }
        var vm = new UpdateRoomTypeRequest
        {
            RoomTypeId = result.Data.RoomTypeId,
            RoomTypeName = result.Data.RoomTypeName,
            DefaultRate = result.Data.DefaultRate
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateRoomTypeRequest roomType)
    {
        if (!ModelState.IsValid) return View(roomType);
        var entity = roomType.MapToRoomType();
        var result = await _roomTypeService.UpdateAsync(entity);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Room Type updated Successfully.";
            return RedirectToAction(nameof(Index));
        }
        TempData["ErrorMessage"] = "Failed to update Room Type.";
        return View(roomType);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _roomTypeService.GetByIdAsync(id);
        if (!result.Success || result.Data == null)
        {
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }
        return View(result.Data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _roomTypeService.DeleteAsync(id);
        if (result.Success)
        {
            TempData["SuccessMessage"] = "Room Type deleted Successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = result.Error;
        }
        return RedirectToAction(nameof(Index));
    }
}
