using System.ComponentModel.DataAnnotations;
using Trs.Models.Constants;

namespace Trs.Models.ViewModels;

public class ReportEntryModel : ReportEntryUpdatableModel
{
    public int Id;

    [Display(Name = "Data")]
    [Required(ErrorMessage = ErrorMessages.FieldRequired)]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = ErrorMessages.FieldRequired)]
    [Display(Name = "Projekt")]
    public string Code { get; set; }
}
