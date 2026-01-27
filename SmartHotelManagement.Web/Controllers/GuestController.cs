using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.BLL.Mapping;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.Model;
using System.IO;

namespace SmartHotelManagement.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class GuestController : Controller
    {
        private readonly IGuestService _guestService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public GuestController(IGuestService guestService, IWebHostEnvironment webHostEnvironment)
        {
            _guestService = guestService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var guests = await _guestService.GetAllAsync();
            return View(guests.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGuestRequest guest, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(guest);

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "guests");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var fullPath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                guest.GuestImage = "/uploads/guests/" + fileName;
            }

            var result = await _guestService.AddAsync(guest);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Guest added Succesfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return View(guest);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _guestService.GetByIdAsync(id);
            if (result.Success)
            {
                // Map Guest -> UpdateGuestRequest so Edit view receives the expected model type
                var vm = result.Data.MapToUpdateGuestRequest();
                return View(vm);
            }
            else
            {
                TempData["ErrorMessage"] = result.Error ?? "An Error occured while fetching the Guest!";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateGuestRequest guest, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(guest);

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "guests");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var fullPath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                guest.GuestImage = "/uploads/guests/" + fileName;
            }

            // Map UpdateGuestRequest -> Guest and call existing service method
            var guestEntity = guest.MapToGuest();
            var updateResult = await _guestService.UpdateAsync(guestEntity);

            if (updateResult.Success)
            {
                TempData["SuccessMessage"] = "Guest Updated Successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = updateResult.Error ?? "An error occurred while updating the guest.";
                return View(guest);
            }
        }

        public async Task<IActionResult> ViewDetails(int id)
        {
            var guestResult = await _guestService.GetByIdAsync(id);

            if (!guestResult.Success || guestResult.Data == null)
            {
                TempData["ErrorMessage"] = guestResult.Error ?? "Guest not found.";
                return RedirectToAction("Index");
            }

            return View(guestResult.Data);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _guestService.DeleteAsync(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Guest Deleted Successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Error ?? "An Error occured while deleting the Guest!";
                return RedirectToAction(nameof(Index));
            }
        }

        
    }
}
