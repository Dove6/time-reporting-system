using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TRS.Models.ViewModels;

public class DailyReportModel : SummaryModel
{
    [Display(Name = "Zatwierdzony")]
    [ValidateNever]
    public bool Frozen { get; set; }

    public List<ReportEntryModel> Entries = new();
}
