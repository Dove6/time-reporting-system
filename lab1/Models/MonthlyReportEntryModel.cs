using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TRS.Models
{
    public class MonthlyReportEntryModel
    {
        [JsonConverter(typeof(DateJsonConverter))]
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Kod projektu")]
        [Required]
        public string Code { get; set; }

        [Display(Name = "Kod kategorii")]
        [Required]
        public string Subcode { get; set; }

        [Display(Name = "Czas (w minutach)")]
        public int Time { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }
    }
}
