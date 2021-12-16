using System.ComponentModel.DataAnnotations;
using Trs.Models.Constants;

namespace Trs.Models.ViewModels;

public class UserModel
{
    [Required(ErrorMessage = ErrorMessages.FieldRequired)]
    [Display(Name = "Nazwa")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = ErrorMessages.StringMinMaxLength)]
    public string Name { get; set; }
}
