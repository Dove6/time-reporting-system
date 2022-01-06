using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Trs.Models.DomainModels;

[Index(nameof(OwnerId), nameof(Month), IsUnique = true)]
public class Report
{
    [RegularExpression("^\\d{4}-\\d{2}$")]
    public string Month { get; set; }
    [DefaultValue(false)]
    public bool Frozen { get; set; }

    public int OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public User? Owner { get; set; }
    public ICollection<ReportEntry>? Entries { get; set; }
    public ICollection<AcceptedTime>? AcceptedTime { get; set; }
}
