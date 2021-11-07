using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class DailyReportModel
    {
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public bool Frozen { get; set; }

        public List<DailyReportEntry> Entries { get; set; }

        public Dictionary<string, int> ProjectTimeSummary { get; set; }

        public int TotalDailyTime { get; set; }
    }
}
