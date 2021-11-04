using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TRS.Models.ViewModels
{
    public class UserSelectListModel
    {
        public string Username;

        public List<SelectListItem> Usernames { get; set; }
    }
}
