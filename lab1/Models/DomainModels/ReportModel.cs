using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.DomainModels
{
    public class ReportModel
    {
        [Required]
        public bool Frozen { get; set; }

        [Required]
        public List<ReportEntryModel> Entries { get; set; }

        [Required]
        public List<AcceptedSummaryModel> Accepted { get; set; }
    }
}
