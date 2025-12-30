using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.Contract.Request;
using SmartHotelManagement.Model;
using System.IO;

namespace SmartHotelManagement.Web.Controllers
{
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
            if(!ModelState.IsValid) return View(guest);

            if(imageFile !=null && imageFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/guests");
                // Directory.GetCurrentDirectory(uploads); // This line is not needed
                
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var fullPath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                guest.GuestImage = "/uploads/guest/" + fileName;
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
        

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _guestService.DeleteAsync(id);
            if(result.Success)
            {
                TempData["SuccessMessage"] = "Guest Deleted Successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.Error ?? "An Error occured while deleting the Guest!";
                return RedirectToAction(nameof(Index));
            }

            //ModelState.AddModelError(string.Empty,result.Error??"An error occured while delete the guest");
            //return BadRequest();
        }
   
    }


}
