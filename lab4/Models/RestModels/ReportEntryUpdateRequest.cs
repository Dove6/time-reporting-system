using System.ComponentModel.DataAnnotations;

namespace Trs.Models.RestModels;

public class ReportEntryUpdateRequest
{
    public string CategoryCode { get; set; }
    [Range(1, int.MaxValue)]
    public int Time { get; set; }
    public string Description { get; set; }
}
