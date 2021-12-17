using System.ComponentModel.DataAnnotations;
using Trs.Models.Constants;

namespace Trs.Models.ViewModels;

public class ReportEntryUpdatableModel
{
    [Display(Name = "Kategoria")]
    public string? Subcode { get; set; }

    [Display(Name = "Czas (w minutach)")]
    [Required(ErrorMessage = ErrorMessages.FieldRequired)]
    [Range(0, 31 * 24 * 60, ErrorMessage = "{0} musi mieścić się w zakresie od {1} do {2} włącznie.")]
    public int Time { get; set; }

    [Display(Name = "Opis")]
    [StringLength(200, ErrorMessage = "{0} może mieć maksymalnie {1} znaków długości.")]
    public string? Description { get; set; }

    public byte[] Timestamp { get; set; }
}
