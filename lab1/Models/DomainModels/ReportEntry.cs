using System;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class ReportEntry
    {
        [Required]
        public User Owner { get; set; }

        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Kod projektu")]
        [Required]
        public string Code { get; set; }

        [Display(Name = "Kod kategorii")]
        [Required]
        public string Subcode { get; set; }

        [Display(Name = "Czas (w minutach)")]
        [Required]
        public int Time { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }
    }
}
