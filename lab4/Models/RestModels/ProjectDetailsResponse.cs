namespace Trs.Models.RestModels;

public class ProjectDetailsResponse : ManagedProjectListResponseEntry
{
    public List<AcceptedTimeListEntry> AcceptedTime { get; set; } = new();
}
