using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class ReportEntryUpdateModel
    {
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
