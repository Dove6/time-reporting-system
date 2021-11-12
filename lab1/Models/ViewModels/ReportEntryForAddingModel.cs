using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TRS.Models.ViewModels
{
    public class ReportEntryForAddingModel : ReportEntryModel
    {
        public List<SelectListItem> ProjectSelectList = new();
        public Dictionary<string, List<SelectListItem>> ProjectCategorySelectList = new();
    }
}
