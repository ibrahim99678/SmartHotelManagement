using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartHotelManagement.Model;
namespace SmartHotelManagement.DAL.Context
{
    public class SmartHotelDbContext :IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public SmartHotelDbContext(DbContextOptions<SmartHotelDbContext> options) : base(options)
        {
        }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<GuestAccount> GuestAccounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RoomChange> RoomChanges { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<FinancialTransaction> FinancialTransactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Budget> Budgets { get; set; }


        //DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder<SmartHotelDbContext>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();


            modelBuilder.Entity<Room>()
                .Property(r => r.BaseRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RoomType>()
                .Property(rt => rt.DefaultRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalAmount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Reservation>();

            modelBuilder.Entity<RoomChange>()
                .Property(rc => rc.Reason)
                .HasMaxLength(500);

            modelBuilder.Entity<FinancialTransaction>()
                .HasKey(ft => ft.TransactionId);
            modelBuilder.Entity<FinancialTransaction>()
                .Property(ft => ft.Amount)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Budget>()
                .Property(b => b.Amount)
                .HasColumnType("decimal(18,2)");

            
        }
    }
}
