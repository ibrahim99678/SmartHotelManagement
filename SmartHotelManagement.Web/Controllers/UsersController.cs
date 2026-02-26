using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHotelManagement.Model;
using SmartHotelManagement.BLL.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHotelManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAuditLogService _auditLogService;
    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAuditLogService auditLogService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _auditLogService = auditLogService;
    }

    public IActionResult Index(string? q, int page = 1, int pageSize = 10)
    {
        var query = _userManager.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(u => u.Email.Contains(q) || u.FirstName.Contains(q) || u.LastName.Contains(q));
        }
        var total = query.Count();
        var items = query.OrderBy(u => u.Email).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        ViewBag.Total = total;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Query = q;
        return View(items);
    }

    public async Task<IActionResult> ManageRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        var roles = _roleManager.Roles.ToList();
        var userRoles = await _userManager.GetRolesAsync(user);
        ViewData["Roles"] = roles;
        ViewData["UserRoles"] = userRoles;
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoles(string id, string[] selectedRoles)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        var currentRoles = await _userManager.GetRolesAsync(user);
        var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
        var toAdd = selectedRoles.Except(currentRoles).Intersect(allRoles).ToList();
        var toRemove = currentRoles.Except(selectedRoles).Intersect(allRoles).ToList();
        var addRes = await _userManager.AddToRolesAsync(user, toAdd);
        var remRes = await _userManager.RemoveFromRolesAsync(user, toRemove);
        if (addRes.Succeeded && remRes.Succeeded)
        {
            TempData["SuccessMessage"] = "Roles updated.";
            var adminId = _userManager.GetUserId(User) ?? string.Empty;
            var details = $"Added: {string.Join(",", toAdd)}; Removed: {string.Join(",", toRemove)}";
            await _auditLogService.LogAsync(new AuditLog
            {
                AdminUserId = adminId,
                TargetUserId = user.Id,
                Action = "UpdateRoles",
                Details = details
            });
        }
        else
        {
            TempData["ErrorMessage"] = "Failed to update roles.";
        }
        return RedirectToAction(nameof(ManageRoles), new { id });
    }
}
