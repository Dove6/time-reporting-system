using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TRS.Models.DomainModels;

namespace TRS.Models.ViewModels
{
    public class ReportEntryForAddingModel
    {
        public ReportEntry ReportEntry { get; set; }
        public List<SelectListItem> ProjectSelectList { get; set; }
        public Dictionary<string, List<SelectListItem>> ProjectCategorySelectList { get; set; }
    }
}
