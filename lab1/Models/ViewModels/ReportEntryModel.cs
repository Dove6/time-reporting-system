using System;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class ReportEntryModel
    {
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Kod projektu")]
        [RegularExpression("^[a-z0-9-]+$")]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Kod kategorii")]
        [RegularExpression("^[a-z0-9-]*$")]
        [StringLength(20)]
        public string Subcode { get; set; } = "";

        [Display(Name = "Czas (w minutach)")]
        [Range(0, 31 * 24 * 60)]
        public int Time { get; set; }

        [Required]
        [Display(Name = "Opis")]
        [StringLength(200)]
        public string Description { get; set; } = "";
    }
}
