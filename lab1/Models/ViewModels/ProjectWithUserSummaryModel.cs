using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class ProjectWithUserSummaryModel
    {
        [Required]
        public ProjectModel Project { get; set; }

        [Required]
        public UserProjectMonthlySummaryModel[] UserSummaries { get; set; }
    }
}
