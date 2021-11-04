using System;
using System.Text.Json.Serialization;

namespace TRS.Models
{
    public class ReportEntryModel
    {
        [JsonConverter(typeof(DateJsonConverter))]
        public DateTime Date { get; set; }
        public string Code { get; set; }
        public string Subcode { get; set; }
        public int Time { get; set; }
        public string Description { get; set; }
    }
}
