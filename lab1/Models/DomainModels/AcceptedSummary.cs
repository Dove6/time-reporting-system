using System;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class AcceptedSummary : IEquatable<AcceptedSummary>
    {
        [Required]
        public string Code { get; }

        [Required]
        public int Time { get; set; }

        public AcceptedSummary(string code)
        {
            Code = code;
        }

        public bool Equals(AcceptedSummary other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Code == other.Code;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((AcceptedSummary)obj);
        }

        public override int GetHashCode()
        {
            return Code?.GetHashCode() ?? 0;
        }
    }
}
