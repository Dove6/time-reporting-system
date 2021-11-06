using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TRS.Models.DomainModels;

namespace TRS.Models.ViewModels
{
    public class ReportEntryForEditingModel
    {
        public ReportEntry ReportEntry { get; set; }
        public List<SelectListItem> CategorySelectList { get; set; }
    }
}
