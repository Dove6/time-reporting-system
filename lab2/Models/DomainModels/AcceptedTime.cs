using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels;

public class AcceptedTime : IEquatable<AcceptedTime>
{
    [Required] public string Code { get; set; }

    public int Time { get; set; }

    public bool Equals(AcceptedTime? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Code == other.Code;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return Equals((AcceptedTime)obj);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}
