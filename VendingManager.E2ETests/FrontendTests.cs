using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace VendingManager.E2ETests
{
	[Parallelizable(ParallelScope.Self)]
	[TestFixture]
	public class FrontendTests : PageTest
	{
		private const string AppUrl = "http://localhost:5173";

		[Test]
		public async Task Homepage_Should_Load_And_Show_Machines()
		{
			await Page.GotoAsync(AppUrl);

			await Expect(Page.Locator("h1")).ToContainTextAsync("VendingManager");

			await Expect(Page.Locator(".leaflet-container")).ToBeVisibleAsync();

			await Expect(Page.Locator(".card").Nth(1)).ToBeVisibleAsync(new() { Timeout = 10000 });
			var count = await Page.Locator(".card").CountAsync();
			Assert.That(count, Is.GreaterThan(1), "Powinna załadować się mapa i przynajmniej jedna maszyna z API");
		}

		[Test]
		public async Task ShoppingCart_Should_Open_When_Clicked()
		{
			await Page.GotoAsync(AppUrl);

			await Page.GetByRole(AriaRole.Button, new() { Name = "🛒" }).ClickAsync();

			await Expect(Page.GetByText("Twój Koszyk")).ToBeVisibleAsync();
		}
	}
}