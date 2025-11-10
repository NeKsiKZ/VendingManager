using Xunit;
using Assert = Xunit.Assert;
using Microsoft.EntityFrameworkCore;
using VendingManager.Controllers;
using VendingManager.Data;
using VendingManager.Models;
using VendingManager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace VendingManager.Tests
{
    public class DashboardControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);
            SeedDatabase(context);
            return context;
        }

        private void SeedDatabase(ApplicationDbContext context)
        {
            context.Machines.Add(new Machine { Id = 1, Name = "Test Machine" });
            context.Products.AddRange(
                new Product { Id = 10, Name = "Cola", Price = 3.0m },
                new Product { Id = 20, Name = "Fanta", Price = 2.0m }
            );

            context.MachineSlots.Add(new MachineSlot { Id = 1, MachineId = 1, ProductId = 10, Quantity = 10, Capacity = 20 });

            context.Transactions.AddRange(
                // --- Transakcje z Września (powinny być zliczone) ---
                new Transaction { Id = 1, MachineId = 1, ProductId = 10, TransactionDate = new DateTime(2025, 9, 5), SalePrice = 3.0m }, // Cola
                new Transaction { Id = 2, MachineId = 1, ProductId = 10, TransactionDate = new DateTime(2025, 9, 10), SalePrice = 3.0m }, // Cola
                new Transaction { Id = 3, MachineId = 1, ProductId = 20, TransactionDate = new DateTime(2025, 9, 15), SalePrice = 2.0m }, // Fanta

                // --- Transakcja z Października (powinna być zignorowana) ---
                new Transaction { Id = 4, MachineId = 1, ProductId = 10, TransactionDate = new DateTime(2025, 10, 5), SalePrice = 3.0m } // Cola
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task Index_Should_CalculateStatistics_Correctly_Based_On_DateFilter()
        {
            await using var context = GetInMemoryDbContext();

            var controller = new DashboardController(context);

            var startDate = new DateTime(2025, 9, 1);
            var endDate = new DateTime(2025, 9, 30);

            var result = await controller.Index(startDate, endDate);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<DashboardViewModel>(viewResult.Model);


            Assert.Equal(8.0m, model.TotalRevenue);

            Assert.Equal(3, model.TotalTransactions);

            Assert.Equal("Cola", model.BestSellingProduct);

            Assert.Equal(2, model.BestSellingProductCount);

            Assert.Single(model.ChartLabels);
            Assert.Single(model.ChartData);
            Assert.Equal(8.0m, model.ChartData[0]);
        }
    }
}