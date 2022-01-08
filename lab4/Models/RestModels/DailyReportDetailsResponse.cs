namespace Trs.Models.RestModels;

public class DailyReportDetailsResponse : ReportDetailsResponse
{
    public List<ReportEntryResponse> Entries { get; set; }
}
