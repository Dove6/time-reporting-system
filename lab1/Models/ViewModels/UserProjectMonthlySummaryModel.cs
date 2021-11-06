using System;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class UserProjectMonthlySummaryModel
    {
        [Required]
        [Display(Name = "Nazwa użytkownika")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Miesiąc")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}")]
        public DateTime Month { get; set; }

        [Required]
        [Display(Name = "Zadeklarowany czas pracy (w minutach)")]
        public int DeclaredTime { get; set; }

        [Required]
        [Display(Name = "Zaakceptowany czas pracy (w minutach)", Prompt = "Uzupełnij...")]
        public int? AcceptedTime { get; set; }
    }
}
