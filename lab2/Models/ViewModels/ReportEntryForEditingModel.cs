using Microsoft.AspNetCore.Mvc.Rendering;

namespace TRS.Models.ViewModels;

public class ReportEntryForEditingModel : ReportEntryModel
{
    public List<SelectListItem> CategorySelectList = new();
}
