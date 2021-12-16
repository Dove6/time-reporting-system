using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TRS.Models.ViewModels;

public class SummaryModel
{
    public List<ProjectTimeSummaryEntry> ProjectTimeSummaries = new();

    [Display(Name = "Czas (w minutach) razem")]
    [ValidateNever]
    public int TotalTime { get; set; }

    [Display(Name = "Zaakceptowany czas (w minutach) razem")]
    [ValidateNever]
    public int TotalAcceptedTime { get; set; }
}
