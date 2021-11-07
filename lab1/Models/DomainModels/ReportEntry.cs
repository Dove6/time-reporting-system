using System;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class ReportEntry : IEquatable<ReportEntry>, IComparable<ReportEntry>
    {
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Date { get; set; }

        public int IndexForDate { get; set; }

        [Display(Name = "Kod projektu")]
        [Required]
        public string Code { get; set; }

        [Display(Name = "Kod kategorii")]
        [Required]
        public string Subcode { get; set; }

        [Display(Name = "Czas (w minutach)")]
        [Required]
        public int Time { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        public bool Equals(ReportEntry other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Date.Date.Equals(other.Date.Date) && IndexForDate == other.IndexForDate;
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
            return HashCode.Combine(Date.Date, IndexForDate);
        }

        public int CompareTo(ReportEntry other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;
            var dateComparison = Date.Date.CompareTo(other.Date.Date);
            if (dateComparison != 0)
                return dateComparison;
            return IndexForDate.CompareTo(other.IndexForDate);
        }
    }
}
