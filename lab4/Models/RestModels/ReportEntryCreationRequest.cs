using System.ComponentModel.DataAnnotations;

namespace Trs.Models.RestModels;

public class ReportEntryCreationRequest
{
    public string ProjectCode { get; set; } = "";
    public string CategoryCode { get; set; } = "";
    [Range(1, int.MaxValue)]
    public int Time { get; set; }
    public string Description { get; set; } = "";
}
