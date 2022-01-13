namespace Trs.Models.RestModels;

public class ReportEntryResponse : ReportEntryUpdateRequest
{
    public string Date { get; set; }
    public string ProjectCode { get; set; }
}
