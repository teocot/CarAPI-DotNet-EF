using CarAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CarAPI.Data
{
    public class AppDbContext : DbContext
    {
        internal IEnumerable Persons;

        public AppDbContext(DbContextOptions<AppDbContext> options)
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
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-one: Purchase → Car
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Car)
                .WithOne(c => c.Purchase)
                .HasForeignKey<Purchase>(p => p.CarId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
