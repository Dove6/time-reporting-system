using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TRS.Models.ViewModels
{
    public class ReportEntryForEditingModel
    {
        public ReportEntryModel ReportEntry { get; set; }
        public List<SelectListItem> CategorySelectList { get; set; }
    }
}
