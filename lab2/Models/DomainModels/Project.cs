using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trs.Models.DomainModels;

public class Project
{
    [Key]
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public int Budget { get; set; }
    [DefaultValue(true)]
    public bool Active { get; set; }

    [ForeignKey(nameof(Manager))]
    public int ManagerId { get; set; }
    public virtual User? Manager { get; set; }
    public virtual ICollection<Category>? Categories { get; set; }
    public virtual ICollection<ReportEntry>? ReportEntries { get; set; }
    public virtual ICollection<AcceptedTime>? AcceptedTime { get; set; }
}
