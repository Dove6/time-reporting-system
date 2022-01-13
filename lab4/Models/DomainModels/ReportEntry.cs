using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trs.Models.DomainModels;

public class ReportEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [RegularExpression("^\\d{2}$")]
    public string DayOfMonth { get; set; }
    [Range(1, int.MaxValue)]
    public int Time { get; set; }
    [DefaultValue("")]
    public string Description { get; set; } = "";

    public string ProjectCode { get; set; } = "";
    public string CategoryCode { get; set; } = "";
    public int OwnerId { get; set; }
    public string ReportMonth { get; set; } = "";

    [ForeignKey(nameof(ProjectCode))]
    public Project? Project { get; set; }
    [ForeignKey($"{nameof(ProjectCode)}, {nameof(CategoryCode)}")]
    public Category? Category { get; set; }
    [ForeignKey(nameof(OwnerId))]
    public User? Owner { get; set; }
    [ForeignKey($"{nameof(OwnerId)}, {nameof(ReportMonth)}")]
    public Report? Report { get; set; }
}
