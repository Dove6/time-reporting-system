using System.Collections.Generic;

namespace TRS.Models
{
    public class MonthlyReportModel
    {
        public bool Frozen { get; set; }
        public List<ReportEntryModel> Entries { get; set; }
        public List<AcceptedSummaryModel> Accepted { get; set; }
    }
}
