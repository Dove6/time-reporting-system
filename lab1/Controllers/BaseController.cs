using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TRS.Controllers.Attributes;
using TRS.DataManager;
using TRS.Models.DomainModels;

namespace TRS.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IDataManager DataManager;
        protected readonly IMapper Mapper;

        private const string UsernameCookieName = "username";
        private const string UserItemName = "user";

        private User _loggedInUser;
        protected User LoggedInUser
        {
            get => _loggedInUser;
            set
            {
                _loggedInUser = value;
                if (_loggedInUser == null)
                {
                    Response.Cookies.Delete(UsernameCookieName);
                    HttpContext.Items.Remove(UserItemName);
                }
                else
                {
                    Response.Cookies.Append(UsernameCookieName,
                        _loggedInUser.Name,
                        new CookieOptions
                        {
                            MaxAge = TimeSpan.FromHours(1),
                            HttpOnly = true
                        });
                    HttpContext.Items[UserItemName] = _loggedInUser;
                }
            }
        }

        protected BaseController(IDataManager dataManager, IMapper mapper)
        {
            DataManager = dataManager;
            Mapper = mapper;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.Cookies.TryGetValue(UsernameCookieName, out var username))
                LoggedInUser = DataManager.FindUserByName(username!);

            if (LoggedInUser == null &&
                filterContext.ActionDescriptor.EndpointMetadata.OfType<ForLoggedInOnlyAttribute>().Any())
                filterContext.Result = RedirectToAction("NotLoggedIn", "Home");
        }
    }
}
