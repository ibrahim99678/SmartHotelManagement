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

            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // BLL
            services.AddScoped<IGuestService, GuestService>();           
            services.AddScoped<IRoomTypeService, RoomTypeService>();
            services.AddScoped<IRoomService, RoomService>();

            return services;
        }
    }
}
