using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class ProjectModel
    {
        [Required]
        [Display(Name = "Kod")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Właściciel")]
        public string Manager { get; set; }

        [Required]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Budżet czasowy")]
        public int Budget { get; set; }

        [Required]
        [Display(Name = "Budżet pomniejszony o zaakceptowany czas")]
        public int BudgetLeft { get; set; }

        [Required]
        [Display(Name = "Aktywny")]
        public bool Active { get; set; }

        [Required]
        [Display(Name = "Kategorie")]
        [DataType(DataType.MultilineText)]
        public string Subactivities { get; set; }
    }
}
