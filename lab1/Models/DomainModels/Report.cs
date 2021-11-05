using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class Report : IEquatable<Report>
    {
        [Required]
        public User Owner { get; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public DateTime Month { get; }

        [Required]
        public bool Frozen { get; set; }

        [Required]
        public List<ReportEntry> Entries { get; } = new();

        [Required]
        public HashSet<AcceptedSummary> Accepted { get; } = new();

        public Report(User owner, DateTime month)
        {
            Owner = owner;
            Month = TruncateToMonth(month);
        }

        public bool Equals(Report other)
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
