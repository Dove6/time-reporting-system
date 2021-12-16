using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TRS.Extensions;

namespace Trs.Models.DbModels;

[Index(nameof(OwnerId), nameof(Month), IsUnique = true)]
public class Report
{
    [Key]
    public int Id { get; set; }
    [BackingField(nameof(_month)), DataType(DataType.Date)]
    public DateTime Month { get => _month; set => _month = value.TrimToMonth(); }
    private DateTime _month;
    [DefaultValue(false)]
    public bool Frozen { get; set; }

    [ForeignKey(nameof(Owner))]
    public int OwnerId { get; set; }
    public virtual User? Owner { get; set; }
    public virtual ICollection<ReportEntry>? ReportEntries { get; set; }
    public virtual ICollection<AcceptedTime>? AcceptedTime { get; set; }
}
