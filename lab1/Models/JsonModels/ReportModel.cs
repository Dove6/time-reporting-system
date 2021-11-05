using System.Collections.Generic;

namespace TRS.Models.JsonModels
{
    public class ReportModel
    {
        public string Filename;

        public bool Frozen { get; set; }
        public List<ReportEntry> Entries { get; set; }
        public List<AcceptedSummary> Accepted { get; set; }
    }
}
