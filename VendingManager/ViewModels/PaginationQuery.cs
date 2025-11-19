namespace VendingManager.ViewModels
{
	public class PaginationQuery
	{
		public string? SearchPhrase { get; set; }
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 5;
		public string? SortBy { get; set; }
		public bool IsDescending { get; set; } = false;
	}
}
