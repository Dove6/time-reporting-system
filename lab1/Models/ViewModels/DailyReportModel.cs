using System.Collections.Generic;

namespace TRS.Models.ViewModels
{
    public class DailyReportModel
    {
        public bool Frozen { get; set; }

        public List<DailyReportEntry> Entries { get; set; }

        public Dictionary<string, int> ProjectTimeSummary { get; set; }

        public int TotalDailyTime { get; set; }
    }
}
