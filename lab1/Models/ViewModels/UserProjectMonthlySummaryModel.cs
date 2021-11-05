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
        [Display(Name = "Czas pracy (w minutach)")]
        public int Time { get; set; }

        [Required]
        [Display(Name = "Stan raportu")]
        public SummaryStatus Status { get; set; }

        public enum SummaryStatus
        {
            [Display(Name = "Niezgłoszony")]
            InProgress,
            [Display(Name = "Zgłoszony")]
            Declared,
            [Display(Name = "Zaakceptowany")]
            Accepted
        }
    }
}
