using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SmartHotelManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly UserManager<ApplicationUser> _userManager;
    public EmployeeController(IEmployeeService employeeService, UserManager<ApplicationUser> userManager)
    {
        _employeeService = employeeService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 10)
    {
        var result = await _employeeService.GetAllAsync();
        var query = result.Data.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(e => e.Email.Contains(q) || e.FirstName.Contains(q) || e.LastName.Contains(q) || e.Department.Contains(q) || e.Position.Contains(q));
        }
        var total = query.Count();
        var items = query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        ViewBag.Total = total;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Query = q;
        return View(items);
    }

    public IActionResult Create()
    {
        ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_userManager.Users, "Id", "Email");
        return View(new Employee());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_userManager.Users, "Id", "Email", employee.UserId);
            return View(employee);
        }
        var res = await _employeeService.AddAsync(employee);
        if (res.Success)
        {
            TempData["SuccessMessage"] = "Employee created.";
            return RedirectToAction(nameof(Index));
        }
        TempData["ErrorMessage"] = res.Error;
        ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_userManager.Users, "Id", "Email", employee.UserId);
        return View(employee);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var res = await _employeeService.GetByIdAsync(id);
        if (!res.Success || res.Data == null)
        {
            TempData["ErrorMessage"] = res.Error;
            return RedirectToAction(nameof(Index));
        }
        ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_userManager.Users, "Id", "Email", res.Data.UserId);
        return View(res.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Employee employee)
    {
        if (!ModelState.IsValid)
        {
            ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_userManager.Users, "Id", "Email", employee.UserId);
            return View(employee);
        }
        var res = await _employeeService.UpdateAsync(employee);
        if (res.Success)
        {
            TempData["SuccessMessage"] = "Employee updated.";
            return RedirectToAction(nameof(Index));
        }
        TempData["ErrorMessage"] = res.Error;
        ViewData["UserId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_userManager.Users, "Id", "Email", employee.UserId);
        return View(employee);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var res = await _employeeService.GetByIdAsync(id);
        if (!res.Success || res.Data == null)
        {
            TempData["ErrorMessage"] = res.Error;
            return RedirectToAction(nameof(Index));
        }
        return View(res.Data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var res = await _employeeService.DeleteAsync(id);
        TempData[res.Success ? "SuccessMessage" : "ErrorMessage"] = res.Success ? "Employee deleted." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
