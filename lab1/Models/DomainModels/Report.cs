using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class Report : ReportWithoutEntries, IEquatable<Report>
    {
        [Required]
        public List<ReportEntry> Entries { get; set; } = new();

        public Report(User owner, DateTime month)
            : base(owner, month)
        {}

        public bool Equals(Report other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return base.Equals(other);
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
            return base.GetHashCode();
        }
    }
}
