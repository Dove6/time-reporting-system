using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TRS.Models.ViewModels
{
    public class ProjectSelectListModel
    {
        public string ProjectCode;

        public List<SelectListItem> ProjectCodes { get; set; }
    }
}
