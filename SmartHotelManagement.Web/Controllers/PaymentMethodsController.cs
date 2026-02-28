using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System.Threading.Tasks;

namespace SmartHotelManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class PaymentMethodsController : Controller
{
    private readonly IFinanceUnitOfWork _uow;
    public PaymentMethodsController(IFinanceUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _uow.PaymentMethodRepository.GetAsync(x => x);
        return View(list);
    }

    public IActionResult Create()
    {
        return View(new PaymentMethod { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentMethod method)
    {
        if (!ModelState.IsValid)
        {
            return View(method);
        }
        await _uow.PaymentMethodRepository.AddAsync(method);
        var saved = await _uow.SaveChangesAsync();
        TempData[saved ? "SuccessMessage" : "ErrorMessage"] = saved ? "Payment method added." : "Failed to add payment method.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _uow.PaymentMethodRepository.GetByIdAsync(id);
        if (item == null) return RedirectToAction(nameof(Index));
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PaymentMethod method)
    {
        if (!ModelState.IsValid)
        {
            return View(method);
        }
        await _uow.PaymentMethodRepository.UpdateAsync(method);
        var saved = await _uow.SaveChangesAsync();
        TempData[saved ? "SuccessMessage" : "ErrorMessage"] = saved ? "Payment method updated." : "Failed to update payment method.";
        return RedirectToAction(nameof(Index));
    }
}
