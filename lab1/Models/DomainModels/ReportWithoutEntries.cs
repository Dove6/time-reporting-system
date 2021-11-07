using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class ReportWithoutEntries : IEquatable<ReportWithoutEntries>
    {
        [Required]
        public User Owner { get; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}")]
        public DateTime Month { get; }

        [Required]
        public bool Frozen { get; set; }

        [Required]
        public HashSet<AcceptedSummary> Accepted { get; set; } = new();

        public ReportWithoutEntries(User owner, DateTime month)
        {
            Owner = owner;
            Month = TruncateToMonth(month);
        }

        public bool Equals(ReportWithoutEntries other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(Owner, other.Owner) && TruncateToMonth(Month).Equals(TruncateToMonth(other.Month));
        }

        public override bool Equals(object obj)
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
            return HashCode.Combine(Owner, TruncateToMonth(Month));
        }

        private static DateTime TruncateToMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }
    }
}
