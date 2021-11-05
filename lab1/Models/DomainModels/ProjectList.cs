using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class ProjectList
    {
        [Required]
        public HashSet<Project> Activities { get; set; }
    }
}
