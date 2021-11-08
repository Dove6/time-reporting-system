using System.Collections.Generic;

namespace TRS.Models.JsonModels
{
    public class Report
    {
        public string Filename;

        public bool Frozen { get; set; }
        public List<ReportEntry> Entries { get; set; } = new();
        public List<ActivitySummary> Accepted { get; set; } = new();
    }
}
