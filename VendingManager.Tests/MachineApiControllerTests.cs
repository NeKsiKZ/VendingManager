using Xunit;
using Assert = Xunit.Assert;
using Microsoft.EntityFrameworkCore;
using VendingManager.Controllers;
using VendingManager.Data;
using VendingManager.Models;
using VendingManager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.SignalR;
using VendingManager.Hubs;

namespace VendingManager.Tests
{
    public class MachineApiControllerTests
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
            context.Products.Add(new Product { Id = 10, Name = "Cola", Price = 3.5m });

            context.MachineSlots.Add(new MachineSlot
            {
                Id = 1,
                MachineId = 1,
                ProductId = 10,
                Quantity = 5,
                Capacity = 20
            });

            context.SaveChanges();
        }

        private Mock<IHubContext<DashboardHub>> GetMockHubContext()
        {
            var mockClients = new Mock<IClientProxy>();

            var mockHubContext = new Mock<IHubContext<DashboardHub>>();

            mockHubContext.Setup(h => h.Clients.All)
                          .Returns(mockClients.Object);

            return mockHubContext;
        }


        [Fact]
        public async Task RecordSale_When_Slot_Is_Valid_Should_Decrease_Quantity_And_Create_Transaction()
        {
            await using var context = GetInMemoryDbContext();

            var mockHubContext = GetMockHubContext();

            var controller = new MachineApiController(context, mockHubContext.Object);

            var saleRequest = new SaleRequestDto { ProductId = 10 };
            int machineId = 1;

            var result = await controller.RecordSale(machineId, saleRequest);

            var actionResult = Assert.IsType<CreatedAtActionResult>(result);

            var transaction = Assert.IsType<Transaction>(actionResult.Value);


            var slotAfterSale = await context.MachineSlots.FindAsync(1);
            Assert.NotNull(slotAfterSale);
            Assert.Equal(4, slotAfterSale.Quantity);

            var transactionInDb = await context.Transactions.FindAsync(transaction.Id);
            Assert.NotNull(transactionInDb);

            Assert.Equal(10, transactionInDb.ProductId);
            Assert.Equal(3.5m, transactionInDb.SalePrice);
        }
    }
}