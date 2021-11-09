using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TRS.Controllers.Attributes;
using TRS.DataManager;
using TRS.Extensions;
using TRS.Models.DomainModels;

namespace TRS.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IDataManager DataManager;
        protected readonly IMapper Mapper;

        private const string UsernameCookieKey = "username";
        private const string UserViewDataKey = "user";
        private const string DateViewDataKey = "date";
        private const string DateQueryKey = "date";

        private User _loggedInUser;
        protected User LoggedInUser
        {
            get => _loggedInUser;
            set
            {
                _loggedInUser = value;
                if (_loggedInUser == null)
                {
                    Response.Cookies.Delete(UsernameCookieKey);
                    ViewData.Remove(UserViewDataKey);
                }
                else
                {
                    Response.Cookies.Append(UsernameCookieKey,
                        _loggedInUser.Name,
                        new CookieOptions
                        {
                            MaxAge = TimeSpan.FromHours(1),
                            HttpOnly = true
                        });
                    ViewData[UserViewDataKey] = _loggedInUser;
                }
            }
        }

        private DateTime _requestedDate;
        protected DateTime RequestedDate
        {
            get => _requestedDate;
            private set
            {
                _requestedDate = value.Date;
                ViewData[DateViewDataKey] = _requestedDate;
            }
        }

        protected BaseController(IDataManager dataManager, IMapper mapper)
        {
            DataManager = dataManager;
            Mapper = mapper;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.Cookies.TryGetValue(UsernameCookieKey, out var username))
                LoggedInUser = DataManager.FindUserByName(username!);

            if (Request.Query.TryGetValue(DateQueryKey, out var dateRouteValue) && (string)dateRouteValue != null)
                RequestedDate = DateTime.ParseExact((string)dateRouteValue, DateTimeExtensions.DateFormat, CultureInfo.InvariantCulture);
            else
                RequestedDate = DateTime.Today;

            if (LoggedInUser == null &&
                filterContext.ActionDescriptor.EndpointMetadata.OfType<ForLoggedInOnlyAttribute>().Any())
                filterContext.Result = RedirectToAction("NotLoggedIn", "Home");
        }
    }
}
