using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace VendingManager.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("MachineId")]
        public Machine? Machine { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required]
        [Display(Name = "Data transakcji")]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Cena w momencie sprzedaży")]
        public decimal SalePrice { get; set; }
    }
}
