using SmartHotelManagement.BLL.Implementations;
using SmartHotelManagement.BLL.Interfaces;
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

            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // BLL
            services.AddScoped<IGuestService, GuestService>();

            return services;
        }
    }
}
