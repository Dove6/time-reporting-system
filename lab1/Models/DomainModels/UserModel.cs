using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class UserModel
    {
        [Required]
        public string Name { get; set; }
    }
}
