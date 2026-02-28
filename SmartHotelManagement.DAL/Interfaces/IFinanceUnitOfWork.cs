using SmartHotelManagement.DAL.Core;

namespace SmartHotelManagement.DAL.Interfaces;

public interface IFinanceUnitOfWork : IUnitOfWork
{
    IFinanceTransactionRepository FinanceTransactionRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IPaymentMethodRepository PaymentMethodRepository { get; }
    IBudgetRepository BudgetRepository { get; }
}
