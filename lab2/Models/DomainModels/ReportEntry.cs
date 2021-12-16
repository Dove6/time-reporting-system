using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trs.Models.DomainModels;

public class ReportEntry
{
    [Key]
    public int Id { get; set; }
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    [Range(1, int.MaxValue)]
    public int Time { get; set; }
    [DefaultValue("")]
    public string Description { get; set; } = "";

    [ForeignKey(nameof(Project))]
    public string ProjectCode { get; set; } = "";
    public virtual Project? Project { get; set; }
    [ForeignKey(nameof(Category))]
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    [ForeignKey(nameof(Report))]
    public int ReportId { get; set; }
    public virtual Report? Report { get; set; }
}
