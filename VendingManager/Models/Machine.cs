using System.ComponentModel.DataAnnotations;

namespace VendingManager.Models
{
    public class Machine
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa maszyny jest wymagana.")]
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = "Brak lokalizacji";
        public string Status { get; set; } = "Offline";
		public DateTime LastContact {  get; set; }

		[Display(Name = "Szerokość geogr. (Latitude)")]
		public double Latitude { get; set; }

		[Display(Name = "Długość geogr. (Longitude)")]
		public double Longitude { get; set; }

		[Display(Name = "Tryb Serwisowy")]
		public bool IsUnderMaintenance { get; set; } = false;

		public virtual ICollection<MachineSlot> Slots { get; set; } = new List<MachineSlot>();
	}
}
