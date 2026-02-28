using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHotelManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class BudgetsController : Controller
{
    private readonly IFinanceUnitOfWork _uow;
    private readonly IFinanceService _financeService;
    public BudgetsController(IFinanceUnitOfWork uow, IFinanceService financeService)
    {
        _uow = uow;
        _financeService = financeService;
    }

    public async Task<IActionResult> Index()
    {
        var budgets = await _uow.BudgetRepository.GetAsync(x => x);
        var categories = await _uow.CategoryRepository.GetAsync(x => x);
        ViewBag.CategoryMap = categories.ToDictionary(c => c.CategoryId, c => c.Name);
        return View(budgets.OrderByDescending(b => b.PeriodStart));
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _uow.CategoryRepository.GetAsync(x => x, x => x.Kind == CategoryKind.Expense && x.IsActive);
        ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name");
        return View(new Budget { PeriodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), PeriodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1), Currency = "USD" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Budget budget)
    {
        if (!ModelState.IsValid)
        {
            return await Create();
        }
        await _uow.BudgetRepository.AddAsync(budget);
        var saved = await _uow.SaveChangesAsync();
        TempData[saved ? "SuccessMessage" : "ErrorMessage"] = saved ? "Budget added." : "Failed to add budget.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var b = await _uow.BudgetRepository.GetByIdAsync(id);
        if (b == null) return RedirectToAction(nameof(Index));
        var categories = await _uow.CategoryRepository.GetAsync(x => x, x => x.Kind == CategoryKind.Expense && x.IsActive);
        ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name", b.CategoryId);
        return View(b);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Budget budget)
    {
        if (!ModelState.IsValid)
        {
            return await Edit(budget.BudgetId);
        }
        await _uow.BudgetRepository.UpdateAsync(budget);
        var saved = await _uow.SaveChangesAsync();
        TempData[saved ? "SuccessMessage" : "ErrorMessage"] = saved ? "Budget updated." : "Failed to update budget.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Summary(DateTime? start, DateTime? end)
    {
        var s = start ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var e = end ?? s.AddMonths(1).AddDays(-1);
        var budgets = await _uow.BudgetRepository.GetAsync(x => x, x => x.PeriodStart >= s && x.PeriodEnd <= e);
        var categories = await _uow.CategoryRepository.GetAsync(x => x);
        var map = categories.ToDictionary(c => c.CategoryId, c => c.Name);
        var totals = await _financeService.GetTotalsByCategoryAsync(FinanceTransactionType.Expense, s, e);
        ViewBag.PeriodStart = s;
        ViewBag.PeriodEnd = e;
        ViewBag.CategoryMap = map;
        ViewBag.ExpenseTotals = totals.Success ? totals.Data : new System.Collections.Generic.Dictionary<string, decimal>();
        return View(budgets);
    }
}
