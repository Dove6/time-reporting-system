using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class ProjectList
    {
        [Required]
        public List<Project> Projects { get; set; }
    }
}
