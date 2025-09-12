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

        public DbSet<Car> Cars { get; set; }
        public DbSet<CarAPI.Models.Person> Person { get; set; } = default!;
    }
}
