using System.Collections.Generic;

namespace TRS.Models.JsonModels
{
    public class ReportModel
    {
        public bool Frozen { get; set; }
        public List<ReportEntryModel> Entries { get; set; }
        public List<AcceptedSummaryModel> Accepted { get; set; }
    }
}
