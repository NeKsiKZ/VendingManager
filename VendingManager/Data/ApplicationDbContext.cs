using Microsoft.EntityFrameworkCore;
using VendingManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace VendingManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MachineSlot> MachineSlots { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var seedDate = new DateTime(2025, 10, 20, 12, 0, 0);

            modelBuilder.Entity<Machine>().HasData(
                new Machine
                {
                    Id = 1,
                    Name = "Automat GŁÓWNY",
                    Location = "Hol wejściowy, Budynek A",
                    Status = "Online",
                    LastContact = seedDate.AddMinutes(-5)
                },
                new Machine
                {
                    Id = 2,
                    Name = "Automat PIĘTRO 2",
                    Location = "Korytarz przy windach",
                    Status = "Offline",
                    LastContact = seedDate.AddHours(-8)
                }
            );
        }
    }
}
