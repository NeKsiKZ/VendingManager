using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using VendingManager.Models;
using Microsoft.EntityFrameworkCore;


namespace VendingManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MachineSlot> MachineSlots { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MachineErrorLog> MachineErrorLogs { get; set; }

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
                    LastContact = seedDate.AddMinutes(-5),
                    Latitude = 53.117015,
                    Longitude = 23.146449
				},
                new Machine
                {
                    Id = 2,
                    Name = "Automat PIĘTRO 2",
                    Location = "Korytarz przy windach",
                    Status = "Offline",
                    LastContact = seedDate.AddHours(-8),
                    Latitude = 53.133826,
					Longitude = 23.134977
				}
            );

            modelBuilder.Entity<Product>().HasData(
            new Product { Id = 2, Name = "Fanta (Test)", Description = "Seed Data", Price = 3.00m },
            new Product { Id = 3, Name = "Cola (Test)", Description = "Seed Data", Price = 3.50m },
            new Product { Id = 4, Name = "Woda (Test)", Description = "Seed Data", Price = 3m },
            new Product { Id = 10, Name = "Monster (Test)", Description = "Seed Data", Price = 7m }
);

            modelBuilder.Entity<Transaction>().HasData(
                // --- Transakcje z Października 2025 ---
                new Transaction { Id = 101, MachineId = 1, ProductId = 3, TransactionDate = new DateTime(2025, 10, 5), SalePrice = 3.50m },
                new Transaction { Id = 102, MachineId = 2, ProductId = 2, TransactionDate = new DateTime(2025, 10, 10), SalePrice = 3.00m },
                new Transaction { Id = 103, MachineId = 1, ProductId = 4, TransactionDate = new DateTime(2025, 10, 15), SalePrice = 3.00m },

                // --- Transakcje z Września 2025 ---
                new Transaction { Id = 104, MachineId = 1, ProductId = 3, TransactionDate = new DateTime(2025, 9, 5), SalePrice = 3.50m },
                new Transaction { Id = 105, MachineId = 1, ProductId = 10, TransactionDate = new DateTime(2025, 9, 6), SalePrice = 3.50m },
                new Transaction { Id = 106, MachineId = 2, ProductId = 4, TransactionDate = new DateTime(2025, 9, 10), SalePrice = 3.50m },
                new Transaction { Id = 107, MachineId = 1, ProductId = 2, TransactionDate = new DateTime(2025, 9, 20), SalePrice = 3.00m },
                new Transaction { Id = 133, MachineId = 2, ProductId = 4, TransactionDate = new DateTime(2025, 9, 11), SalePrice = 3.50m },

                // --- Transakcje z Sierpnia 2025 ---
                new Transaction { Id = 108, MachineId = 2, ProductId = 4, TransactionDate = new DateTime(2025, 8, 10), SalePrice = 3.50m },
                new Transaction { Id = 109, MachineId = 2, ProductId = 10, TransactionDate = new DateTime(2025, 8, 15), SalePrice = 3.00m },
                new Transaction { Id = 131, MachineId = 2, ProductId = 10, TransactionDate = new DateTime(2025, 8, 15), SalePrice = 3.00m },
                new Transaction { Id = 134, MachineId = 2, ProductId = 3, TransactionDate = new DateTime(2025, 8, 13), SalePrice = 3.50m },
                new Transaction { Id = 132, MachineId = 2, ProductId = 2, TransactionDate = new DateTime(2025, 8, 7), SalePrice = 3.50m },

                // --- Transakcje z Lipca 2025 ---
                new Transaction { Id = 110, MachineId = 1, ProductId = 2, TransactionDate = new DateTime(2025, 7, 5), SalePrice = 3.00m },
                new Transaction { Id = 111, MachineId = 2, ProductId = 3, TransactionDate = new DateTime(2025, 7, 8), SalePrice = 3.50m },
                new Transaction { Id = 112, MachineId = 1, ProductId = 4, TransactionDate = new DateTime(2025, 7, 10), SalePrice = 3.50m },
                new Transaction { Id = 113, MachineId = 2, ProductId = 10, TransactionDate = new DateTime(2025, 7, 15), SalePrice = 3.00m },
                new Transaction { Id = 114, MachineId = 1, ProductId = 2, TransactionDate = new DateTime(2025, 7, 20), SalePrice = 3.00m },
                new Transaction { Id = 115, MachineId = 1, ProductId = 3, TransactionDate = new DateTime(2025, 7, 25), SalePrice = 3.50m },
                new Transaction { Id = 116, MachineId = 2, ProductId = 4, TransactionDate = new DateTime(2025, 7, 28), SalePrice = 3.50m },

                // --- Transakcje z Czerwca 2025 ---
                new Transaction { Id = 117, MachineId = 1, ProductId = 10, TransactionDate = new DateTime(2025, 6, 2), SalePrice = 3.00m },
                new Transaction { Id = 118, MachineId = 2, ProductId = 2, TransactionDate = new DateTime(2025, 6, 5), SalePrice = 3.00m },
                new Transaction { Id = 119, MachineId = 1, ProductId = 3, TransactionDate = new DateTime(2025, 6, 7), SalePrice = 3.50m },
                new Transaction { Id = 120, MachineId = 2, ProductId = 4, TransactionDate = new DateTime(2025, 6, 11), SalePrice = 3.50m },
                new Transaction { Id = 121, MachineId = 1, ProductId = 10, TransactionDate = new DateTime(2025, 6, 15), SalePrice = 3.00m },
                new Transaction { Id = 122, MachineId = 2, ProductId = 2, TransactionDate = new DateTime(2025, 6, 20), SalePrice = 3.00m },
                new Transaction { Id = 123, MachineId = 1, ProductId = 3, TransactionDate = new DateTime(2025, 6, 25), SalePrice = 3.50m },
                new Transaction { Id = 124, MachineId = 2, ProductId = 10, TransactionDate = new DateTime(2025, 6, 29), SalePrice = 3.00m },

                // --- Transakcje z Maja 2025 ---
                new Transaction { Id = 125, MachineId = 1, ProductId = 4, TransactionDate = new DateTime(2025, 5, 5), SalePrice = 3.50m },
                new Transaction { Id = 126, MachineId = 2, ProductId = 2, TransactionDate = new DateTime(2025, 5, 10), SalePrice = 3.00m },
                new Transaction { Id = 127, MachineId = 1, ProductId = 3, TransactionDate = new DateTime(2025, 5, 15), SalePrice = 3.50m },
                new Transaction { Id = 128, MachineId = 2, ProductId = 10, TransactionDate = new DateTime(2025, 5, 20), SalePrice = 3.00m },
                new Transaction { Id = 129, MachineId = 1, ProductId = 4, TransactionDate = new DateTime(2025, 5, 25), SalePrice = 3.50m },
                new Transaction { Id = 130, MachineId = 2, ProductId = 2, TransactionDate = new DateTime(2025, 5, 28), SalePrice = 3.00m }
            );
        }
    }
}
