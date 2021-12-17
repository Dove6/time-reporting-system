using Microsoft.AspNetCore.Mvc.Rendering;

namespace Trs.Models.ViewModels;

public class UserSelectListModel : UserModel
{
    public List<SelectListItem> Usernames = new();
}
