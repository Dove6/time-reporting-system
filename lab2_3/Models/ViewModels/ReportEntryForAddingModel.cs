using Microsoft.AspNetCore.Mvc.Rendering;

namespace Trs.Models.ViewModels;

public class ReportEntryForAddingModel : ReportEntryModel
{
    public List<SelectListItem> ProjectSelectList = new();
    public Dictionary<string, List<SelectListItem>> ProjectCategorySelectList = new();
}
