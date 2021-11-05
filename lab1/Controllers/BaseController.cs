using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TRS.Models.DomainModels;

namespace TRS.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.Cookies.TryGetValue("user", out var encodedUser))
            {
                var loggedInUser = JsonSerializer.Deserialize<User>(encodedUser);
                HttpContext.Items.Add("user", loggedInUser);
            }
        }
    }
}
