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
    public class RestockingControllerTests
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
            context.Machines.Add(new Machine { Id = 1, Name = "Test Machine 1" });
            context.Products.Add(new Product { Id = 10, Name = "Cola", Price = 3.5m });
            context.Products.Add(new Product { Id = 20, Name = "Fanta", Price = 3.0m });
            context.Products.Add(new Product { Id = 30, Name = "Woda", Price = 2.0m });

            context.MachineSlots.AddRange(
                // Przypadek 1: Powyżej progu (50% zapełnienia) -> NIE POWINIEN SIĘ POJAWIĆ
                new MachineSlot { Id = 1, MachineId = 1, ProductId = 10, Quantity = 10, Capacity = 20 },

                // Przypadek 2: Poniżej progu (20% zapełnienia) -> POWINIEN SIĘ POJAWIĆ
                new MachineSlot { Id = 2, MachineId = 1, ProductId = 20, Quantity = 4, Capacity = 20 },

                // Przypadek 3: Dokładnie na progu (25% zapełnienia) -> NIE POWINIEN SIĘ POJAWIĆ
                new MachineSlot { Id = 3, MachineId = 1, ProductId = 30, Quantity = 5, Capacity = 20 }
            );

            context.SaveChanges();
        }

        [Fact]
        public async Task Index_Should_Return_Only_Products_Below_25_Percent()
        {
            await using var context = GetInMemoryDbContext();

            var controller = new RestockingController(context);


            var result = await controller.Index();


            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<RestockingViewModel>(viewResult.Model);

            Assert.Single(model.ShoppingList);

            Assert.Single(model.RouteList);

            Assert.Equal("Fanta", model.ShoppingList.First().ProductName);

            Assert.Equal(16, model.ShoppingList.First().TotalNeeded);
        }
    }
}