using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class ProjectModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Manager { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Budget { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public List<CategoryModel> Subactivities { get; set; }
    }
}
