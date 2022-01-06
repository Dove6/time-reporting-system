using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trs.Models.DomainModels;

public class AcceptedTime
{
    [Range(0, int.MaxValue)]
    public int Time { get; set; }

    public string ProjectCode { get; set; } = "";
    public int OwnerId { get; set; }
    public string ReportMonth { get; set; } = "";

    [ForeignKey(nameof(OwnerId))]
    public User? Owner { get; set; }
    [ForeignKey($"{nameof(OwnerId)}, {nameof(ReportMonth)}")]
    public Report? Report { get; set; }
    [ForeignKey(nameof(ProjectCode))]
    public Project? Project { get; set; }

    [Timestamp]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public byte[] Timestamp { get; set; }
}
