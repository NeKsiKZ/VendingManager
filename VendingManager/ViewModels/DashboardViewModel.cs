using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace VendingManager.ViewModels
{
    public class DashboardViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Data od")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data do")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Całkowity przychód")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Liczba transakcji")]
        public int TotalTransactions { get; set; }

        [Display(Name = "Najlepszy produkt")]
        public string BestSellingProduct { get; set; } = "Brak";

        [Display(Name = "Sprzedano (szt.)")]
        public int BestSellingProductCount { get; set; }

        public List<MachineStatusViewModel> MachineStatuses { get; set; } = new List<MachineStatusViewModel>();
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<decimal> ChartData { get; set; } = new List<decimal>();
    }
}
