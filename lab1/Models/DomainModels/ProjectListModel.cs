using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class ProjectListModel
    {
        [Required]
        public List<ProjectModel> Activities { get; set; }
    }
}
