using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class ProjectWithUsersModel
    {
        [Required]
        public ProjectModel Project { get; set; }

        [Required]
        public string[] Users { get; set; }
    }
}
