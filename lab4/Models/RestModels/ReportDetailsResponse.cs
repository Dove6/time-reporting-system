namespace Trs.Models.RestModels;

public class ReportDetailsResponse
{
    public bool Frozen { get; set; }
    public List<ProjectTimeSummaryEntry> ProjectTimeSummaries { get; set; }
}
