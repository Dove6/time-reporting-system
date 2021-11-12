using System.Collections.Generic;

namespace TRS.Models.ViewModels
{
    public class ProjectWithUserSummaryModel : ProjectModel
    {
        public List<ProjectWithUserSummaryEntry> UserSummaries = new();
    }
}
