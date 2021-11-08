using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TRS.Models.ViewModels
{
    public class ReportEntryForAddingModel
    {
        public ReportEntryModel ReportEntry { get; set; }
        public List<SelectListItem> ProjectSelectList { get; set; }
        public Dictionary<string, List<SelectListItem>> ProjectCategorySelectList { get; set; }
    }
}
