using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.DAL.Core;
using SmartHotelManagement.DAL.Interfaces;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.DAL.Implementation;

public class CategoryRepository : Repository<Category, int, SmartHotelDbContext>, ICategoryRepository
{
    public CategoryRepository(SmartHotelDbContext context) : base(context)
    {
    }
}
