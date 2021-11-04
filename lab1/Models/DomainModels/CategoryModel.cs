using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class CategoryModel
    {
        [Required]
        public string Code { get; set; }
    }
}
