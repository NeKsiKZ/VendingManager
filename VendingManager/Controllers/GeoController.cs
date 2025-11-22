using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingManager.Data;
using VendingManager.ViewModels;
using VendingManager.Filters;

namespace VendingManager.Controllers
{
	[ServiceFilter(typeof(ApiKeyAuthFilter))]
	[Route("api/[controller]")]
	[ApiController]
	public class GeoController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public GeoController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: api/geo/nearest?lat=53.1325&lon=23.1688&radiusKm=5
		[HttpGet("nearest")]
		public async Task<ActionResult<IEnumerable<NearestMachineDto>>> GetNearestMachines(
			[FromQuery] double lat,
			[FromQuery] double lon,
			[FromQuery] double radiusKm = 10)
		{
			var machines = await _context.Machines
				.Where(m => m.Latitude != 0 && m.Longitude != 0)
				.ToListAsync();

			var nearestMachines = machines
				.Select(m => new NearestMachineDto
				{
					Id = m.Id,
					Name = m.Name,
					Location = m.Location,
					Latitude = m.Latitude,
					Longitude = m.Longitude,
					DistanceKm = CalculateHaversineDistance(lat, lon, m.Latitude, m.Longitude)
				})
				.Where(m => m.DistanceKm <= radiusKm)
				.OrderBy(m => m.DistanceKm)
				.ToList();

			return Ok(nearestMachines);
		}

		// === Haversine formula ===
		private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
		{
			var R = 6371;
			var dLat = ToRadians(lat2 - lat1);
			var dLon = ToRadians(lon2 - lon1);

			var a =
				Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
				Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			var distance = R * c;

			return Math.Round(distance, 2);
		}

		private double ToRadians(double angle)
		{
			return Math.PI * angle / 180.0;
		}
	}
}