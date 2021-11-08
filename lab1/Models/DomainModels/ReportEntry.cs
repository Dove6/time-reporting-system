using System;
using System.ComponentModel.DataAnnotations;
using TRS.Extensions;

namespace TRS.Models.DomainModels
{
    public class ReportEntry : IEquatable<ReportEntry>
    {
        public int MonthlyIndex { get; set; }

        public DateTime Date { get => _date; set => _date = value.Date; }
        public DateTime Month => _date.TrimToMonth();
        private DateTime _date;

        [Required]
        public string Code { get; set; }

        [Required]
        public string Subcode { get; set; } = "";

        public int Time { get; set; }

        [Required]
        public string Description { get; set; } = "";

        public bool Equals(ReportEntry other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Month.Equals(other.Month) && MonthlyIndex == other.MonthlyIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((ReportEntry)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Month, MonthlyIndex);
        }
    }
}
