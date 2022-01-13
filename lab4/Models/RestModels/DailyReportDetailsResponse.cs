namespace Trs.Models.RestModels;

public class DailyReportDetailsResponse : ReportDetailsResponse
{
    public Dictionary<int, ReportEntryResponse> Entries { get; set; } = new();
}
