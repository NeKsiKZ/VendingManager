namespace VendingManager.ViewModels
{
    public class RestockingViewModel
    {
        public List<ShoppingListItem> ShoppingList { get; set; } = new List<ShoppingListItem>();
        public List<RestockRouteItem> RouteList { get; set; } = new List<RestockRouteItem>();
    }

    public class ShoppingListItem
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalNeeded { get; set; }
    }

    public class RestockRouteItem
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public string MachineLocation { get; set; } = string.Empty;
        public List<SlotToRestock> SlotsToRestock { get; set; } = new List<SlotToRestock>();
    }

    public class SlotToRestock
    {
        public string ProductName { get; set; } = string.Empty;
        public int CurrentQuantity { get; set; }
        public int Capacity { get; set; }
        public int AmountToRefill { get; set; }
    }
}
