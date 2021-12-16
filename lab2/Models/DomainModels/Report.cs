using System.ComponentModel.DataAnnotations;
using TRS.Extensions;

namespace TRS.Models.DomainModels;

public class Report : IEquatable<Report>
{
    [Required]
    public string Owner { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM}")]
    public DateTime Month { get => _month; set => _month = value.TrimToMonth(); }
    private DateTime _month;

    [Required]
    public bool Frozen { get; set; }

    [Required]
    public HashSet<ReportEntry> Entries { get; set; } = new();

    [Required]
    public HashSet<AcceptedTime> Accepted { get; set; } = new();

    public bool Equals(Report? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Owner == other.Owner && Month.Equals(other.Month);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return Equals((Report)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Owner, Month);
    }
}
