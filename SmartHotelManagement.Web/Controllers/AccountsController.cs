using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.Model;
using SmartHotelManagement.DAL.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHotelManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager")]
public class AccountsController : Controller
{
    private readonly IFinanceService _financeService;
    private readonly IReservationService _reservationService;
    private readonly IGuestService _guestService;
    private readonly IFinanceUnitOfWork _financeUnitOfWork;

    public AccountsController(IFinanceService financeService, IReservationService reservationService, IGuestService guestService, IFinanceUnitOfWork financeUnitOfWork)
    {
        _financeService = financeService;
        _reservationService = reservationService;
        _guestService = guestService;
        _financeUnitOfWork = financeUnitOfWork;
    }

    public async Task<IActionResult> Dashboard(DateTime? start, DateTime? end)
    {
        var s = start ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var e = end ?? s.AddMonths(1).AddDays(-1);
        var summary = await _financeService.GetSummaryAsync(s, e);
        ViewBag.Income = summary.Success ? summary.Data.income : 0;
        ViewBag.Expense = summary.Success ? summary.Data.expense : 0;
        ViewBag.Net = (ViewBag.Income - ViewBag.Expense);
        return View();
    }

    public async Task<IActionResult> Expenses(DateTime? start, DateTime? end, int? categoryId, int? reservationId, int? paymentMethodId)
    {
        var s = start ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var e = end ?? s.AddMonths(1).AddDays(-1);
        var list = await _financeService.GetTransactionsAsync(FinanceTransactionType.Expense, s, e, categoryId, reservationId, paymentMethodId);
        var categories = await _financeUnitOfWork.CategoryRepository.GetAsync(x => x, x => x.Kind == CategoryKind.Expense && x.IsActive);
        var methods = await _financeUnitOfWork.PaymentMethodRepository.GetAsync(x => x, x => x.IsActive);
        var reservations = await _reservationService.GetAllAsync();
        ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name", categoryId);
        ViewData["PaymentMethodId"] = new SelectList(methods, "PaymentMethodId", "Name", paymentMethodId);
        ViewData["ReservationId"] = new SelectList(reservations.Success ? reservations.Data : Array.Empty<Reservation>(), "ReservationId", "ReservationId", reservationId);
        ViewBag.Start = s;
        ViewBag.End = e;
        return View(list.Success ? list.Data : Array.Empty<FinancialTransaction>());
    }

    public async Task<IActionResult> Income(DateTime? start, DateTime? end, int? categoryId, int? reservationId, int? paymentMethodId)
    {
        var s = start ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var e = end ?? s.AddMonths(1).AddDays(-1);
        var list = await _financeService.GetTransactionsAsync(FinanceTransactionType.Income, s, e, categoryId, reservationId, paymentMethodId);
        var categories = await _financeUnitOfWork.CategoryRepository.GetAsync(x => x, x => x.Kind == CategoryKind.Income && x.IsActive);
        var methods = await _financeUnitOfWork.PaymentMethodRepository.GetAsync(x => x, x => x.IsActive);
        var reservations = await _reservationService.GetAllAsync();
        ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name", categoryId);
        ViewData["PaymentMethodId"] = new SelectList(methods, "PaymentMethodId", "Name", paymentMethodId);
        ViewData["ReservationId"] = new SelectList(reservations.Success ? reservations.Data : Array.Empty<Reservation>(), "ReservationId", "ReservationId", reservationId);
        ViewBag.Start = s;
        ViewBag.End = e;
        return View(list.Success ? list.Data : Array.Empty<FinancialTransaction>());
    }

    public async Task<IActionResult> Reports(DateTime? start, DateTime? end)
    {
        var s = start ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var e = end ?? s.AddMonths(1).AddDays(-1);
        var summary = await _financeService.GetSummaryAsync(s, e);
        var incomeByCat = await _financeService.GetTotalsByCategoryAsync(FinanceTransactionType.Income, s, e);
        var expenseByCat = await _financeService.GetTotalsByCategoryAsync(FinanceTransactionType.Expense, s, e);
        ViewBag.Start = s;
        ViewBag.End = e;
        ViewBag.Income = summary.Success ? summary.Data.income : 0;
        ViewBag.Expense = summary.Success ? summary.Data.expense : 0;
        ViewBag.IncomeByCategory = incomeByCat.Success ? incomeByCat.Data : new System.Collections.Generic.Dictionary<string, decimal>();
        ViewBag.ExpenseByCategory = expenseByCat.Success ? expenseByCat.Data : new System.Collections.Generic.Dictionary<string, decimal>();
        return View();
    }

    public async Task<IActionResult> AddExpense()
    {
        var categories = await _financeUnitOfWork.CategoryRepository.GetAsync(x => x, x => x.Kind == CategoryKind.Expense && x.IsActive);
        var methods = await _financeUnitOfWork.PaymentMethodRepository.GetAsync(x => x, x => x.IsActive);
        var reservations = await _reservationService.GetAllAsync();
        var guests = await _guestService.GetAllAsync();
        ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name");
        ViewData["PaymentMethodId"] = new SelectList(methods, "PaymentMethodId", "Name");
        ViewData["ReservationId"] = new SelectList(reservations.Success ? reservations.Data : Array.Empty<Reservation>(), "ReservationId", "ReservationId");
        ViewData["GuestId"] = new SelectList(guests.Success ? guests.Data : Array.Empty<Guest>(), "GuestId", "FirstName");
        return View(new FinancialTransaction { Date = DateTime.Now, Currency = "USD" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpense(FinancialTransaction tx)
    {
        if (!ModelState.IsValid)
        {
            return await AddExpense();
        }
        var res = await _financeService.AddExpenseAsync(tx);
        TempData[res.Success ? "SuccessMessage" : "ErrorMessage"] = res.Success ? "Expense added." : res.Error;
        return RedirectToAction(nameof(Expenses));
    }

    public async Task<IActionResult> AddIncome()
    {
        var categories = await _financeUnitOfWork.CategoryRepository.GetAsync(x => x, x => x.Kind == CategoryKind.Income && x.IsActive);
        var methods = await _financeUnitOfWork.PaymentMethodRepository.GetAsync(x => x, x => x.IsActive);
        var reservations = await _reservationService.GetAllAsync();
        var guests = await _guestService.GetAllAsync();
        ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name");
        ViewData["PaymentMethodId"] = new SelectList(methods, "PaymentMethodId", "Name");
        ViewData["ReservationId"] = new SelectList(reservations.Success ? reservations.Data : Array.Empty<Reservation>(), "ReservationId", "ReservationId");
        ViewData["GuestId"] = new SelectList(guests.Success ? guests.Data : Array.Empty<Guest>(), "GuestId", "FirstName");
        return View(new FinancialTransaction { Date = DateTime.Now, Currency = "USD" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddIncome(FinancialTransaction tx)
    {
        if (!ModelState.IsValid)
        {
            return await AddIncome();
        }
        var res = await _financeService.AddIncomeAsync(tx);
        TempData[res.Success ? "SuccessMessage" : "ErrorMessage"] = res.Success ? "Income added." : res.Error;
        return RedirectToAction(nameof(Income));
    }

    public async Task<IActionResult> RecurringTemplates()
    {
        var templates = await _financeUnitOfWork.FinanceTransactionRepository.GetAsync(x => x, x => x.IsRecurring);
        return View(templates);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateRecurring()
    {
        var templates = await _financeUnitOfWork.FinanceTransactionRepository.GetAsync(x => x, x => x.IsRecurring);
        var now = DateTime.Now;
        var start = new DateTime(now.Year, now.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        foreach (var t in templates)
        {
            var exists = await _financeUnitOfWork.FinanceTransactionRepository.GetAsync(x => x,
                x => x.IsRecurring && x.CategoryId == t.CategoryId && x.PaymentMethodId == t.PaymentMethodId &&
                     x.Type == t.Type && x.Amount == t.Amount && x.Date >= start && x.Date <= end);
            if (exists.Count == 0)
            {
                var tx = new FinancialTransaction
                {
                    Type = t.Type,
                    Date = now,
                    Amount = t.Amount,
                    Currency = t.Currency,
                    CategoryId = t.CategoryId,
                    PaymentMethodId = t.PaymentMethodId,
                    ReservationId = t.ReservationId,
                    GuestId = t.GuestId,
                    Description = $"Recurring: {t.Description}",
                    IsRecurring = true,
                    RecurrenceRule = t.RecurrenceRule
                };
                await _financeUnitOfWork.FinanceTransactionRepository.AddAsync(tx);
            }
        }
        var saved = await _financeUnitOfWork.SaveChangesAsync();
        TempData[saved ? "SuccessMessage" : "ErrorMessage"] = saved ? "Recurring entries generated for current period." : "Failed to generate recurring entries.";
        return RedirectToAction(nameof(RecurringTemplates));
    }
}
