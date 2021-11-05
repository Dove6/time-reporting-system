using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class ProjectListModel
    {
        [Required]
        public ProjectModel[] Activities { get; set; }
    }
}
