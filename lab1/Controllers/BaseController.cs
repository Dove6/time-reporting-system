using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TRS.DataManager;
using TRS.Models.DomainModels;

namespace TRS.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IDataManager DataManager;
        protected readonly IMapper Mapper;

        protected User LoggedInUser
        {
            get
            {
                if (HttpContext.Items.TryGetValue("user", out var user))
                    return (User)user;
                return null;
            }
        }

        protected BaseController(IDataManager dataManager, IMapper mapper)
        {
            DataManager = dataManager;
            Mapper = mapper;
        }

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
