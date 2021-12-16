using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Trs.Models.ViewModels;

public class MonthlySummaryModel : SummaryModel
{
    [Display(Name = "Miesiąc")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM}")]
    [DataType(DataType.Date)]
    public DateTime Month { get; set; }

    [Display(Name = "Zatwierdzony")]
    [ValidateNever]
    public bool Frozen { get; set; }
}
