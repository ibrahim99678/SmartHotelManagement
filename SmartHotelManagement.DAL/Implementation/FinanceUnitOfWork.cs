using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;

namespace SmartHotelManagement.DAL.Implementation;

public class FinanceUnitOfWork : UnitOfWork, IFinanceUnitOfWork
{
    public IFinanceTransactionRepository FinanceTransactionRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IPaymentMethodRepository PaymentMethodRepository { get; }
    public IBudgetRepository BudgetRepository { get; }

    public FinanceUnitOfWork(SmartHotelDbContext context,
        IFinanceTransactionRepository financeTransactionRepository,
        ICategoryRepository categoryRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IBudgetRepository budgetRepository) : base(context)
    {
        FinanceTransactionRepository = financeTransactionRepository;
        CategoryRepository = categoryRepository;
        PaymentMethodRepository = paymentMethodRepository;
        BudgetRepository = budgetRepository;
    }
}
