using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace VendingManager.Models
{
    public class MachineErrorLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Automat")]
        public int MachineId { get; set; }

        [ForeignKey("MachineId")]
        public Machine? Machine { get; set; }

        [Required]
        [Display(Name = "Znacznik czasu")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "Kod Błędu")]
        public string ErrorCode { get; set; } = string.Empty;

        [Display(Name = "Wiadomość")]
        public string Message { get; set; } = string.Empty;
    }
}
