using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class CategoryModel
    {
        [Required]
        public string Code { get; set; }
    }
}
