using System;
using System.ComponentModel.DataAnnotations;
using TRS.Models.Constants;

namespace TRS.Models.ViewModels
{
    public class ReportEntryModel : ReportEntryUpdatableModel
    {
        public int MonthlyIndex;

        [Display(Name = "Data")]
        [Required(ErrorMessage = ErrorMessages.FieldRequired)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = ErrorMessages.FieldRequired)]
        [Display(Name = "Projekt")]
        public string Code { get; set; }
    }
}
