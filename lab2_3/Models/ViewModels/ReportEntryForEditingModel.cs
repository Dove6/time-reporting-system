using Microsoft.AspNetCore.Mvc.Rendering;

namespace Trs.Models.ViewModels;

public class ReportEntryForEditingModel : ReportEntryModel
{
    public List<SelectListItem> CategorySelectList = new();
}
