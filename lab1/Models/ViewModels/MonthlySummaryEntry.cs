using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class MonthlySummaryEntry
    {
        [Required]
        [Display(Name = "Projekt")]
        public string ProjectCode;

        [Required]
        [Display(Name = "Zadeklarowany czas (w minutach)")]
        public int Time;

        [Required]
        [Display(Name = "Zaakceptowany czas (w minutach)")]
        public int? AcceptedTime;
    }
}
