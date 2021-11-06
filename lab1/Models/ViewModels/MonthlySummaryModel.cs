using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models.ViewModels
{
    public class MonthlySummaryModel
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}")]
        [DataType(DataType.Date)]
        public DateTime Month { get; set; }
        public bool Frozen { get; set; }
        public IEnumerable<MonthlySummaryEntry> PerProject { get; set; }
        public int TotalTime { get; set; }
        public int TotalAcceptedTime { get; set; }
    }
}
