using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TRS.Models.ViewModels
{
    public class ProjectWithUserSummaryEntry
    {
        [Display(Name = "Nazwa użytkownika")]
        [ValidateNever]
        public string Username { get; set; }

        [Display(Name = "Miesiąc")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}")]
        [ValidateNever]
        public DateTime Month { get; set; }

        [Display(Name = "Zadeklarowany czas pracy (w minutach)")]
        [ValidateNever]
        public int DeclaredTime { get; set; }

        [Display(Name = "Zaakceptowany czas pracy (w minutach)", Prompt = "Uzupełnij...")]
        [ValidateNever]
        public int? AcceptedTime { get; set; }
    }
}
