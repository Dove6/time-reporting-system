using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Trs.Models.Constants;

namespace Trs.Models.ViewModels;

public class ProjectModel : ProjectUpdatableModel
{
    [Display(Name = "Kod")]
    [Required(ErrorMessage = ErrorMessages.FieldRequired)]
    [StringLength(10, ErrorMessage = ErrorMessages.StringMaxLength)]
    public string Code { get; set; }

    [Display(Name = "Właściciel")]
    [ValidateNever]
    public string Manager { get; set; }

    [Display(Name = "Budżet pomniejszony o zaakceptowany czas")]
    [ValidateNever]
    public int BudgetLeft { get; set; }

    [Display(Name = "Aktywny")]
    [ValidateNever]
    public bool Active { get; set; }
}
