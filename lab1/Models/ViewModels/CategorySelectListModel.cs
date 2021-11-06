using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TRS.Models.ViewModels
{
    public class CategorySelectListModel
    {
        public string CategoryCode;

        public List<SelectListItem> CategoryCodes { get; set; }
    }
}
