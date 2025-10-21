using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingManager.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nazwa produktu jest wymagana.")]
        [Display(Name = "Nazwa Produktu")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Opis")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cena jest wymagana.")]
        [Range(typeof(decimal), "0,01", "1000,00", ErrorMessage = "Cena musi być w zakresie od {1} do {2}.")]
        [Display(Name = "Cena")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
    }
}
