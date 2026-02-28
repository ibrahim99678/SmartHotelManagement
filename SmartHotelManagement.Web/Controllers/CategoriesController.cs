using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHotelManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class CategoriesController : Controller
{
    private readonly IFinanceUnitOfWork _uow;
    public CategoriesController(IFinanceUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _uow.CategoryRepository.GetAsync(x => x);
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["ParentCategoryId"] = new SelectList(await _uow.CategoryRepository.GetAsync(x => x), "CategoryId", "Name");
        return View(new Category { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category cat)
    {
        if (!ModelState.IsValid)
        {
            return await Create();
        }
        await _uow.CategoryRepository.AddAsync(cat);
        var saved = await _uow.SaveChangesAsync();
        TempData[saved ? "SuccessMessage" : "ErrorMessage"] = saved ? "Category added." : "Failed to add category.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _uow.CategoryRepository.GetByIdAsync(id);
        if (item == null) return RedirectToAction(nameof(Index));
        ViewData["ParentCategoryId"] = new SelectList(await _uow.CategoryRepository.GetAsync(x => x), "CategoryId", "Name", item.ParentCategoryId);
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category cat)
    {
        if (!ModelState.IsValid)
        {
            return await Edit(cat.CategoryId);
        }
        await _uow.CategoryRepository.UpdateAsync(cat);
        var saved = await _uow.SaveChangesAsync();
        TempData[saved ? "SuccessMessage" : "ErrorMessage"] = saved ? "Category updated." : "Failed to update category.";
        return RedirectToAction(nameof(Index));
    }
}
