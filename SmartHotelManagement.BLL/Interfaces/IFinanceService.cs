using SmartHotelManagement.BLL.ErrorModel;
using SmartHotelManagement.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace SmartHotelManagement.BLL.Interfaces;

public interface IFinanceService
{
    Task<Result<int>> AddExpenseAsync(FinancialTransaction tx);
    Task<Result<int>> AddIncomeAsync(FinancialTransaction tx);
    Task<Result<IList<FinancialTransaction>>> GetExpensesAsync(DateTime start, DateTime end);
    Task<Result<IList<FinancialTransaction>>> GetIncomeAsync(DateTime start, DateTime end);
    Task<Result<(decimal income, decimal expense)>> GetSummaryAsync(DateTime start, DateTime end);
    Task<Result<IDictionary<string, decimal>>> GetTotalsByCategoryAsync(FinanceTransactionType type, DateTime start, DateTime end);
    Task<Result<IList<FinancialTransaction>>> GetTransactionsAsync(FinanceTransactionType type, DateTime start, DateTime end, int? categoryId, int? reservationId, int? paymentMethodId);
}
