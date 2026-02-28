using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHotelManagement.BLL.Implementations;

public class FinanceService : IFinanceService
{
    private readonly IFinanceUnitOfWork _uow;
    public FinanceService(IFinanceUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> AddExpenseAsync(FinancialTransaction tx)
    {
        tx.Type = FinanceTransactionType.Expense;
        await _uow.FinanceTransactionRepository.AddAsync(tx);
        var saved = await _uow.SaveChangesAsync();
        if (!saved) return Result<int>.FailResult("Failed to add expense.");
        return Result<int>.SuccessResult(tx.TransactionId);
    }

    public async Task<Result<int>> AddIncomeAsync(FinancialTransaction tx)
    {
        tx.Type = FinanceTransactionType.Income;
        await _uow.FinanceTransactionRepository.AddAsync(tx);
        var saved = await _uow.SaveChangesAsync();
        if (!saved) return Result<int>.FailResult("Failed to add income.");
        return Result<int>.SuccessResult(tx.TransactionId);
    }

    public async Task<Result<IList<FinancialTransaction>>> GetExpensesAsync(DateTime start, DateTime end)
    {
        var items = await _uow.FinanceTransactionRepository.GetAsync(x => x, x => x.Type == FinanceTransactionType.Expense && x.Date >= start && x.Date <= end);
        return Result<IList<FinancialTransaction>>.SuccessResult(items);
    }

    public async Task<Result<IList<FinancialTransaction>>> GetIncomeAsync(DateTime start, DateTime end)
    {
        var items = await _uow.FinanceTransactionRepository.GetAsync(x => x, x => x.Type == FinanceTransactionType.Income && x.Date >= start && x.Date <= end);
        return Result<IList<FinancialTransaction>>.SuccessResult(items);
    }

    public async Task<Result<(decimal income, decimal expense)>> GetSummaryAsync(DateTime start, DateTime end)
    {
        var income = await _uow.FinanceTransactionRepository.GetAsync(x => x.Amount, x => x.Type == FinanceTransactionType.Income && x.Date >= start && x.Date <= end);
        var expense = await _uow.FinanceTransactionRepository.GetAsync(x => x.Amount, x => x.Type == FinanceTransactionType.Expense && x.Date >= start && x.Date <= end);
        return Result<(decimal income, decimal expense)>.SuccessResult((income.Sum(), expense.Sum()));
    }

    public async Task<Result<IDictionary<string, decimal>>> GetTotalsByCategoryAsync(FinanceTransactionType type, DateTime start, DateTime end)
    {
        var items = await _uow.FinanceTransactionRepository.GetAsync(x => x, x => x.Type == type && x.Date >= start && x.Date <= end);
        var categories = await _uow.CategoryRepository.GetAsync(x => x);
        var map = categories.ToDictionary(c => c.CategoryId, c => c.Name);
        var totals = new Dictionary<string, decimal>();
        foreach (var tx in items)
        {
            var name = map.TryGetValue(tx.CategoryId, out var n) ? n : "Uncategorized";
            if (!totals.ContainsKey(name)) totals[name] = 0;
            totals[name] += tx.Amount;
        }
        return Result<IDictionary<string, decimal>>.SuccessResult(totals);
    }

    public async Task<Result<IList<FinancialTransaction>>> GetTransactionsAsync(FinanceTransactionType type, DateTime start, DateTime end, int? categoryId, int? reservationId, int? paymentMethodId)
    {
        var items = await _uow.FinanceTransactionRepository.GetAsync(x => x,
            x => x.Type == type
                 && x.Date >= start && x.Date <= end
                 && (!categoryId.HasValue || x.CategoryId == categoryId)
                 && (!reservationId.HasValue || x.ReservationId == reservationId)
                 && (!paymentMethodId.HasValue || x.PaymentMethodId == paymentMethodId));
        return Result<IList<FinancialTransaction>>.SuccessResult(items);
    }
}
