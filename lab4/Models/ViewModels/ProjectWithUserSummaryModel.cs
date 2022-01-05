namespace Trs.Models.ViewModels;

public class ProjectWithUserSummaryModel : ProjectModel
{
    public List<ProjectWithUserSummaryEntry> UserSummaries = new();
}
