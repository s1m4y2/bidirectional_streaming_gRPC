using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Car>().HasData(
                new Car { Id = 1, CarId = "car1", Status = "Idle" },
                new Car { Id = 2, CarId = "car2", Status = "Idle" },
                new Car { Id = 3, CarId = "car3", Status = "Idle" }
            );
        }
    }

}
