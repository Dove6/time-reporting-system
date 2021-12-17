using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Trs.Extensions;

namespace Trs.Models.DomainModels;

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

    public User? Owner { get; set; }
    public ICollection<ReportEntry>? ReportEntries { get; set; }
    public ICollection<AcceptedTime>? AcceptedTime { get; set; }

    [Timestamp]
    public byte[] Timestamp { get; set; }
}
