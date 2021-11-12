using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TRS.Models.ViewModels
{
    public class UserSelectListModel : UserModel
    {
        public List<SelectListItem> Usernames = new();
    }
}
