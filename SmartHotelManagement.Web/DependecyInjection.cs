using SmartHotelManagement.BLL.Implementations;
using SmartHotelManagement.BLL.Interfaces;
using SmartHotelManagement.DAL.Implementation;
using SmartHotelManagement.DAL.Implemention;
using SmartHotelManagement.DAL.Interfaces;

namespace SmartHotelManagement.Web
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
          // DAL
            services.AddScoped<IGuestUnitOfWork, GuestUnitOfWork>();
            services.AddScoped<IGuestRepository, GuestRepository>();
            services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
            services.AddScoped<IRoomTypeUnitOfWork, RoomTypeUnitOfWork>();
            services.AddScoped<IRoomUnitOfWork, RoomUnitOfWork>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IReservationUnitOfWork, ReservationUnitOfWork>();
            services.AddScoped<IRoomChangeRepository, RoomChangeRepository>();
            services.AddScoped<IRoomChangeUnitOfWork, RoomChangeUnitOfWork>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentUnitOfWork, PaymentUnitOfWork>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeUnitOfWork, EmployeeUnitOfWork>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IAuditLogUnitOfWork, AuditLogUnitOfWork>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IRoomUnitOfWork, RoomUnitOfWork>();
            services.AddScoped<IFinanceTransactionRepository, FinanceTransactionRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();
            services.AddScoped<IFinanceUnitOfWork, FinanceUnitOfWork>();

            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // BLL
            services.AddScoped<IGuestService, GuestService>();           
            services.AddScoped<IRoomTypeService, RoomTypeService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IRoomChangeService, RoomChangeService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IFinanceService, FinanceService>();

            return services;
        }
    }
}
