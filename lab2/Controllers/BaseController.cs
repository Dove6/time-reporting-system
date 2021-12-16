using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TRS.Controllers.Attributes;
using TRS.Controllers.Constants;
using TRS.DataManager;
using TRS.Extensions;
using TRS.Models.DomainModels;

namespace TRS.Controllers;

public abstract class BaseController : Controller
{
    protected readonly IDataManager DataManager;
    protected readonly IMapper Mapper;

    private const string UsernameCookieKey = "username";
    private const string UserViewDataKey = "user";
    private const string DateViewDataKey = "date";
    private const string ErrorTempDataKey = "error";
    private const string DateQueryKey = "date";
    private const string ErrorModelStateKey = "error";

    private User? _loggedInUser;
    protected User? LoggedInUser
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

    protected RedirectToActionResult RedirectToActionWithError(string actionName, string error)
    {
        TempData[ErrorTempDataKey] = error;
        return RedirectToAction(actionName);
    }

    protected RedirectToActionResult RedirectToActionWithError(string actionName, object routeValues, string error)
    {
        TempData[ErrorTempDataKey] = error;
        return RedirectToAction(actionName, routeValues);
    }

    private RedirectToActionResult RedirectToActionWithError(string actionName, string controllerName, string error)
    {
        TempData[ErrorTempDataKey] = error;
        return RedirectToAction(actionName, controllerName);
    }

    protected RedirectToActionResult RedirectToActionWithError(string actionName, string controllerName, object routeValues, string error)
    {
        TempData[ErrorTempDataKey] = error;
        return RedirectToAction(actionName, controllerName, routeValues);
    }

    private DateTime ParseRouteDate()
    {
        if (Request.Query.TryGetValue(DateQueryKey, out var dateRouteValue) && (string)dateRouteValue != null)
            if (DateTime.TryParseExact((string)dateRouteValue, DateTimeExtensions.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                return parsedDate;
        return DateTime.Today;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (Request.Cookies.TryGetValue(UsernameCookieKey, out var username))
            LoggedInUser = DataManager.FindUserByName(username!);

        if (LoggedInUser == null && filterContext.ActionDescriptor.EndpointMetadata.OfType<ForLoggedInOnlyAttribute>().Any())
            filterContext.Result = RedirectToActionWithError("NotLoggedIn", "Home", ErrorMessages.HasToBeLoggedIn);
        if (LoggedInUser != null && filterContext.ActionDescriptor.EndpointMetadata.OfType<ForNotLoggedInOnlyAttribute>().Any())
            filterContext.Result = RedirectToActionWithError("Index", "Home", ErrorMessages.MustNotBeLoggedIn);

        RequestedDate = ParseRouteDate();

        if (TempData.ContainsKey(ErrorTempDataKey))
            ModelState.AddModelError(ErrorModelStateKey, (string)TempData[ErrorTempDataKey]!);
    }
}
