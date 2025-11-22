namespace VendingManager.ViewModels
{
	public class DailyRevenueDto
	{
		public string Date { get; set; } = string.Empty;
		public decimal Revenue { get; set; }
		public int TransactionCount { get; set; }
	}

	public class ProductPopularityDto
	{
		public string ProductName { get; set; } = string.Empty;
		public int UnitsSold { get; set; }
		public decimal TotalRevenue { get; set; }
	}

	public class MachinePerformanceDto
	{
		public string MachineName { get; set; } = string.Empty;
		public int TotalTransactions { get; set; }
		public decimal TotalRevenue { get; set; }
		public double AverageTransactionValue { get; set; }
	}
}