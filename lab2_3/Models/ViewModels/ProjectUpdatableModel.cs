using System.ComponentModel.DataAnnotations;
using Trs.Models.Constants;

namespace Trs.Models.ViewModels;

public class ProjectUpdatableModel
{
    [Display(Name = "Nazwa")]
    [Required(ErrorMessage = ErrorMessages.FieldRequired)]
    public string Name { get; set; }

    [Display(Name = "Budżet czasowy")]
    [Required(ErrorMessage = ErrorMessages.FieldRequired)]
    public int Budget { get; set; }

    [Display(Name = "Kategorie")]
    [DataType(DataType.MultilineText)]
    public string? Categories { get; set; }
}
