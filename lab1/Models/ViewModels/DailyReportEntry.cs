using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class DailyReportEntry
    {
        public int Id { get; set; }

        [Display(Name = "Kod projektu")]
        [Required]
        public string Code { get; set; }

        [Display(Name = "Kod kategorii")]
        [Required]
        public string Subcode { get; set; }

        [Display(Name = "Czas (w minutach)")]
        public int Time { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }
    }
}
