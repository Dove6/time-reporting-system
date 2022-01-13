namespace Trs.Models.RestModels;

public class ProjectTimeSummaryEntry
{
    public string ProjectCode { get; set; } = "";
    public int Time { get; set; }
    public int? AcceptedTime { get; set; }
}
