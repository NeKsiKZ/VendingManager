namespace VendingManager.ViewModels
{
    public class MachineStatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = "Brak lokalizacji";
        public string Status { get; set; } = "Offline";
        public DateTime LastContact { get; set; }
        
        public int FillPercentage { get; set; }
        public bool IsStockLow { get; set; }
    }
}
