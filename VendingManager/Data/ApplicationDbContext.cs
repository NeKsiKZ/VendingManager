using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using VendingManager.Models;

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

            var seedDate = new DateTime(2025, 11, 20, 12, 0, 0);

            modelBuilder.Entity<Machine>().HasData(
                new Machine { Id = 1, Name = "Automat POLITECHNIKA", Location = "Hol wejściowy, Budynek A", Status = "Online", LastContact = seedDate.AddMinutes(-5), Latitude = 53.1169, Longitude = 23.1465 },
                new Machine { Id = 2, Name = "Automat OPERA", Location = "Opera, Główne wejście", Status = "Online", LastContact = seedDate.AddMinutes(-15), Latitude = 53.1300, Longitude = 23.1502 },
                new Machine { Id = 3, Name = "Automat DWORZEC PKP", Location = "Wejście od strony parkingu", Status = "Offline", LastContact = seedDate.AddHours(-48), Latitude = 53.1322, Longitude = 23.1356 },
                new Machine { Id = 4, Name = "Automat GALERIA ALFA", Location = "Poziom +1, Food Court", Status = "Online", LastContact = seedDate.AddMinutes(-2), Latitude = 53.1245, Longitude = 23.1679 },
                new Machine { Id = 5, Name = "Automat GALERIA JUROWIECKA", Location = "Galeria, Główne wejście", Status = "Online", LastContact = seedDate.AddMinutes(-10), Latitude = 53.1362, Longitude = 23.1633 },
                new Machine { Id = 6, Name = "Automat GALERIA BIAŁA", Location = "Przy sklepie rossmann", Status = "Online", LastContact = seedDate.AddMinutes(-30), Latitude = 53.1233, Longitude = 23.1790 },
                new Machine { Id = 7, Name = "Automat RYNEK KOŚCIUSZKI", Location = "Główny hol budynku", Status = "Online", LastContact = seedDate.AddMinutes(-1), Latitude = 53.1324, Longitude = 23.1587 },
                new Machine { Id = 8, Name = "Automat GALERIA ZIELONE WZGÓRZA", Location = "Główne wejście", Status = "Online", LastContact = seedDate.AddMinutes(-12), Latitude = 53.1238, Longitude = 23.0976 },
                new Machine { Id = 9, Name = "Automat STADION MIEJSKI", Location = "Trybuna VIP", Status = "Maintenance", LastContact = seedDate.AddHours(-3), Latitude = 53.1061, Longitude = 23.1500 },
                new Machine { Id = 10, Name = "Automat ZESPÓŁ SZKÓŁ ELEKTRYCZNYCH", Location = "Główne wejście, Parter", Status = "Online", LastContact = seedDate.AddMinutes(-4), Latitude = 53.1528, Longitude = 23.1550 },
                new Machine { Id = 11, Name = "Automat DWORZEC PKS", Location = "Peron 1", Status = "Offline", LastContact = seedDate.AddDays(-5), Latitude = 53.1337, Longitude = 23.1353 },
                new Machine { Id = 12, Name = "Automat KOMENDA POLICJI", Location = "Poziom +2, Korytarz", Status = "Online", LastContact = seedDate.AddMinutes(-8), Latitude = 53.1358, Longitude = 23.1702 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Cola 0.5L", Description = "Napój gazowany", Price = 4.50m },
                new Product { Id = 2, Name = "Fanta 0.5L", Description = "Napój gazowany", Price = 4.50m },
                new Product { Id = 3, Name = "Woda Niegazowana", Description = "Źródlana", Price = 2.50m },
                new Product { Id = 4, Name = "Monster Energy", Description = "Napój energetyczny", Price = 7.00m },
                new Product { Id = 5, Name = "Baton Czekoladowy", Description = "Z orzechami", Price = 3.00m },
                new Product { Id = 6, Name = "Chipsy Solone", Description = "Mała paczka", Price = 3.50m }
            );

            var slots = new List<MachineSlot>();
            int slotIdCounter = 1;

            void AddSlotsForMachine(int machineId, int[] productIds, int[] quantities)
            {
                for (int i = 0; i < productIds.Length; i++)
                {
                    slots.Add(new MachineSlot
                    {
                        Id = slotIdCounter++,
                        MachineId = machineId,
                        ProductId = productIds[i],
                        Capacity = 20,
                        Quantity = quantities[i]
                    });
                }
            }

            AddSlotsForMachine(1, new[] { 1, 2, 3, 4 }, new[] { 5, 8, 2, 15 });
            AddSlotsForMachine(2, new[] { 1, 2, 5, 6 }, new[] { 18, 19, 20, 15 });
            AddSlotsForMachine(3, new[] { 1, 3, 4 }, new[] { 1, 2, 0 });
            AddSlotsForMachine(4, new[] { 1, 2, 3, 4, 5, 6 }, new[] { 10, 10, 10, 10, 5, 5 });
            AddSlotsForMachine(5, new[] { 4, 5 }, new[] { 12, 15 });
            AddSlotsForMachine(6, new[] { 3, 1 }, new[] { 5, 10 });
            AddSlotsForMachine(7, new[] { 1, 2, 3, 5 }, new[] { 20, 15, 5, 10 });
            AddSlotsForMachine(8, new[] { 4, 5, 6 }, new[] { 18, 20, 15 });
            AddSlotsForMachine(9, new[] { 1, 3 }, new[] { 0, 0 });
            AddSlotsForMachine(10, new[] { 3, 5, 6 }, new[] { 10, 10, 10 });
            AddSlotsForMachine(11, new[] { 1, 2, 4 }, new[] { 2, 2, 1 });
            AddSlotsForMachine(12, new[] { 1, 2, 3, 4, 5, 6 }, new[] { 20, 20, 20, 20, 20, 20 });

            modelBuilder.Entity<MachineSlot>().HasData(slots);

            var transactions = new List<Transaction>();
            int transIdCounter = 1;
            var random = new Random(12345);

            var productPrices = new Dictionary<int, decimal>
            {
                { 1, 4.50m }, { 2, 4.50m }, { 3, 2.50m }, { 4, 7.00m }, { 5, 3.00m }, { 6, 3.50m }
            };

            int[] machinesWithTraffic = {
                1, 1,
                2, 2,
                3,
                4, 4, 4,
                5, 5,
                6,
                7, 7, 7,
                8, 8,
                10, 10,
                12
            };

            DateTime startDate = new DateTime(2025, 1, 1);
            DateTime endDate = seedDate;

            for (var day = startDate; day <= endDate; day = day.AddDays(1))
            {
                int dailyTransactions = random.Next(2, 12);

                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    dailyTransactions += random.Next(4, 8);
                }

                for (int i = 0; i < dailyTransactions; i++)
                {
                    int machineId = machinesWithTraffic[random.Next(machinesWithTraffic.Length)];
                    int productId = random.Next(1, 7);

                    int hour = random.Next(8, 23);
                    int minute = random.Next(0, 60);

                    transactions.Add(new Transaction
                    {
                        Id = transIdCounter++,
                        MachineId = machineId,
                        ProductId = productId,
                        TransactionDate = new DateTime(day.Year, day.Month, day.Day, hour, minute, 0),
                        SalePrice = productPrices[productId]
                    });
                }
            }

            modelBuilder.Entity<Transaction>().HasData(transactions);
        }
    }
}