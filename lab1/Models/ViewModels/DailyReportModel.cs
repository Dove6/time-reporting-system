using System;
using System.ComponentModel.DataAnnotations;
using TRS.Models.DomainModels;

namespace TRS.Models.ViewModels
{
    public class DailyReportModel
    {
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public Report Report { get; set; }
    }
}
