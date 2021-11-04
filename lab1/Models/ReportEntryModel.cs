using System;
using System.ComponentModel.DataAnnotations;

namespace TRS.Models
{
    public class ReportEntryModel
    {
        public string Date { get; set; }
        public string Code { get; set; }
        public string Subcode { get; set; }
        public int Time { get; set; }
        public string Description { get; set; }
    }
}
