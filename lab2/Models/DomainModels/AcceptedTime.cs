using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trs.Models.DomainModels;

public class AcceptedTime
{
    [Range(0, int.MaxValue)]
    public int Time { get; set; }

    [ForeignKey(nameof(Report))]
    public int ReportId { get; set; }
    [ForeignKey(nameof(Project))]
    public string ProjectCode { get; set; } = "";

    public Report? Report { get; set; }
    public Project? Project { get; set; }
}
