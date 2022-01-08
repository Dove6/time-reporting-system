using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Trs.Controllers.Attributes;
using Trs.DataManager;
using Trs.Models.DomainModels;

namespace Trs.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : Controller
{
    protected readonly IDataManager DataManager;
    protected readonly IMapper Mapper;

    private const string UsernameCookieKey = "username";

    private User? _loggedInUser;
    protected User? LoggedInUser
    {
        get => _loggedInUser;
        set
        {
            _loggedInUser = value;
            if (_loggedInUser == null)
                Response.Cookies.Delete(UsernameCookieKey);
            else
                Response.Cookies.Append(UsernameCookieKey,
                    _loggedInUser.Name,
                    new CookieOptions
                    {
                        MaxAge = TimeSpan.FromHours(1),
                        HttpOnly = true
                    });
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

        if (LoggedInUser == null && filterContext.ActionDescriptor.EndpointMetadata.OfType<ForLoggedInOnlyAttribute>().Any())
            filterContext.Result = Unauthorized();
        if (LoggedInUser != null && filterContext.ActionDescriptor.EndpointMetadata.OfType<ForNotLoggedInOnlyAttribute>().Any())
            filterContext.Result = Forbid();
    }
}
