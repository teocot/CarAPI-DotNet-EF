using CarAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAPI.Data
{
    public class CarDbContext : DbContext
    {
        public CarDbContext(DbContextOptions<CarDbContext> options)
    : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-many: Person → Purchases
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Buyer)
                .WithMany(b => b.Purchases)
                .HasForeignKey(p => p.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-one: Purchase → Car
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Car)
                .WithOne(c => c.Purchase)
                .HasForeignKey<Purchase>(p => p.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
