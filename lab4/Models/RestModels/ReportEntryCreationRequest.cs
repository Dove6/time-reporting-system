namespace Trs.Models.RestModels;

public class ReportEntryCreationRequest
{
    public string ProjectCode { get; set; }
    public string CategoryCode { get; set; }
    public int Time { get; set; }
    public string Description { get; set; }
}
