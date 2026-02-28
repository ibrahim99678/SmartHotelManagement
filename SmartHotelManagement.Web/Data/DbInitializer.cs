using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartHotelManagement.DAL.Context;
using SmartHotelManagement.Model;

namespace SmartHotelManagement.Web.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {

            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = scope.ServiceProvider.GetRequiredService<SmartHotelDbContext>();


            //Create roles if they do not exist
            string[] roles = new[] { "Admin", "Manager", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            //Create a default admin user if it does not exist
            var adminEmail = "admin@smarthotel.com";
            var adminPassword = "Admin@123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed default finance categories if none exist
            if (!await db.Categories.AnyAsync())
            {
                var cats = new List<Category>
                {
                    new Category { Name = "Maintenance & Repairs", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Utilities", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Housekeeping Supplies", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Laundry", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Salaries & Benefits", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Insurance", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Licenses & Taxes", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Marketing & Commissions", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Software Subscriptions", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Payment Gateway Fees", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Furniture & Fixtures", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Training", Kind = CategoryKind.Expense, IsActive = true },
                    new Category { Name = "Room Bookings", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Upsell (Early/Late)", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Restaurant/Bar", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Room Service", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Minibar", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Spa/WELLNESS", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Conference/Banquet", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Parking", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Laundry (Income)", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Tourism Services", Kind = CategoryKind.Income, IsActive = true },
                    new Category { Name = "Penalties/Fees", Kind = CategoryKind.Income, IsActive = true }
                };
                await db.Categories.AddRangeAsync(cats);
                await db.SaveChangesAsync();
            }
            // Seed payment methods if none exist
            if (!await db.PaymentMethods.AnyAsync())
            {
                var methods = new List<PaymentMethod>
                {
                    new PaymentMethod { Name = "Cash", IsActive = true },
                    new PaymentMethod { Name = "Card", Provider = "Generic", MaskedAccount = "**** **** **** 1234", IsActive = true },
                    new PaymentMethod { Name = "Bank Transfer", Provider = "Local Bank", MaskedAccount = "ACCT ****1234", IsActive = true },
                    new PaymentMethod { Name = "Online", Provider = "Gateway", IsActive = true }
                };
                await db.PaymentMethods.AddRangeAsync(methods);
                await db.SaveChangesAsync();
            }

            // Demo: attach income transaction to a sample reservation
            var sampleReservation = await db.Reservations
                .Include(r => r.Room)
                .OrderBy(r => r.ReservationId)
                .FirstOrDefaultAsync();
            if (sampleReservation == null)
            {
                var anyGuest = await db.Guests.OrderBy(g => g.GuestId).FirstOrDefaultAsync();
                var anyRoom = await db.Rooms.OrderBy(r => r.RoomId).FirstOrDefaultAsync();
                if (anyGuest != null && anyRoom != null)
                {
                    var today = DateTime.Today;
                    sampleReservation = new Reservation
                    {
                        GuestId = anyGuest.GuestId,
                        RoomId = anyRoom.RoomId,
                        ReferenceName = $"{anyGuest.FirstName} {anyGuest.LastName}",
                        SpouseName = string.Empty,
                        CheckInDate = today,
                        CheckOutDate = today.AddDays(1),
                        StayInNight = 1,
                        TotalAmount = anyRoom.BaseRate,
                        Status = "Confirmed",
                        IsCheckedIn = false,
                        IsCheckedOut = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1
                    };
                    await db.Reservations.AddAsync(sampleReservation);
                    await db.SaveChangesAsync();
                }
            }
            if (sampleReservation != null)
            {
                var existingTx = await db.FinancialTransactions.AnyAsync(ft => ft.ReservationId == sampleReservation.ReservationId);
                if (!existingTx)
                {
                    var roomBookingsCat = await db.Categories.FirstOrDefaultAsync(c => c.Kind == CategoryKind.Income && c.Name == "Room Bookings")
                        ?? await db.Categories.FirstOrDefaultAsync(c => c.Kind == CategoryKind.Income);
                    var cashMethod = await db.PaymentMethods.FirstOrDefaultAsync(m => m.Name == "Cash")
                        ?? await db.PaymentMethods.FirstOrDefaultAsync();
                    var amount = sampleReservation.TotalAmount ?? (sampleReservation.Room?.BaseRate ?? 0);
                    if (roomBookingsCat != null && cashMethod != null && amount > 0)
                    {
                        var tx = new FinancialTransaction
                        {
                            Type = FinanceTransactionType.Income,
                            Date = DateTime.UtcNow,
                            Amount = amount,
                            Currency = "USD",
                            CategoryId = roomBookingsCat.CategoryId,
                            PaymentMethodId = cashMethod.PaymentMethodId,
                            ReservationId = sampleReservation.ReservationId,
                            Description = "Demo income linked to sample reservation"
                        };
                        await db.FinancialTransactions.AddAsync(tx);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
