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

        public List<DailyReportEntryModel> Entries { get; set; }
    }
}
