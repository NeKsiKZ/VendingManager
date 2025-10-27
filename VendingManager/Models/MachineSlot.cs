using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingManager.Models
{
    public class MachineSlot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Automat")]
        public int MachineId { get; set; }

        [Required]
        [Display(Name = "Produkt")]
        public int ProductId { get; set; }

        [ForeignKey("MachineId")]
        public Machine? Machine { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Ilość musi być w zakresie {1}-{2}.")]
        [Display(Name = "Aktualna ilość")]
        public int Quantity { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Pojemność musi być w zakresie {1}-{2}.")]
        [Display(Name = "Pojemność slotu")]
        public int Capacity { get; set; }
    }
}
