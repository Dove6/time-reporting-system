using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TRS.Models.ViewModels;

public class ProjectTimeSummaryEntry
{
    [Display(Name = "Projekt")]
    [ValidateNever]
    public string ProjectCode { get; set; }

    [Display(Name = "Zadeklarowany czas (w minutach)")]
    [ValidateNever]
    public int Time { get; set; }

    [Display(Name = "Zaakceptowany czas (w minutach)")]
    [ValidateNever]
    public int? AcceptedTime { get; set; }
}