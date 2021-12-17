using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Trs.Controllers.Attributes;
using Trs.Controllers.Constants;
using Trs.DataManager;
using Trs.Models.DomainModels;
using Trs.Models.ViewModels;

namespace Trs.Controllers;

public class UserController : BaseController
{
    private ILogger<UserController> _logger;

    public UserController(IDataManager dataManager, IMapper mapper, ILogger<UserController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    [ForLoggedInOnly]
    public IActionResult Index()
    {
        return View();
    }

    private void FillSelectListsInLoginModel(UserSelectListModel loginModel)
    {
        var usernames = DataManager.GetAllUsers().Select(x => new SelectListItem(x.Name, x.Name)).ToList();
        loginModel.Usernames = usernames;
    }

    [ForNotLoggedInOnly]
    public IActionResult Login()
    {
        var model = new UserSelectListModel();
        FillSelectListsInLoginModel(model);
        return View(model);
    }

    [ForNotLoggedInOnly]
    [HttpPost]
    public IActionResult Login(UserModel user)
    {
        if (!ModelState.IsValid)
            goto InvalidModelState;
        var foundUser = DataManager.FindUserByName(user.Name);
        if (foundUser == null)
            ModelState.AddModelError(nameof(user.Name), ErrorMessages.GetUserNotFoundMessage(user.Name));
        if (!ModelState.IsValid)
            goto InvalidModelState;
        LoggedInUser = foundUser;
        return RedirectToAction("Index", "Home");

        InvalidModelState:
        var loginModel = Mapper.Map<UserSelectListModel>(user);
        FillSelectListsInLoginModel(loginModel);
        return View(loginModel);
    }

    [ForLoggedInOnly]
    [HttpPost]
    public IActionResult Logout()
    {
        LoggedInUser = null;
        return RedirectToAction("NotLoggedIn", "Home");
    }

    [ForNotLoggedInOnly]
    public IActionResult Register()
    {
        return View();
    }

    [ForNotLoggedInOnly]
    [HttpPost]
    public IActionResult Register(UserModel user)
    {
        if (!ModelState.IsValid)
            return Register();
        var existingUser = DataManager.FindUserByName(user.Name);
        if (existingUser != null)
            ModelState.AddModelError(nameof(user.Name), ErrorMessages.GetUserAlreadyExistingMessage(user.Name));
        if (!ModelState.IsValid)
            return Register();
        DataManager.AddUser(new User { Name = user.Name });
        return Login(user);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
