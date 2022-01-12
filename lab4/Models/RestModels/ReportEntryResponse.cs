using System.ComponentModel.DataAnnotations;

namespace Trs.Models.RestModels;

public class ReportEntryResponse : ReportEntryUpdateRequest
{
    public int Id { get; set; }
    public string Date { get; set; }
    public string ProjectCode { get; set; }
}
