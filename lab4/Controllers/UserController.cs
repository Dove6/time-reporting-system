using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Trs.Controllers.Attributes;
using Trs.DataManager;
using Trs.Models.DomainModels;
using Trs.Models.ViewModels;

namespace Trs.Controllers;

[Route("[controller]")]
public class UserController : BaseController
{
    private ILogger<UserController> _logger;

    public UserController(IDataManager dataManager, IMapper mapper, ILogger<UserController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    [ForLoggedInOnly]
    public IActionResult Current()
    {
        return Ok(LoggedInUser);
    }

    [ForNotLoggedInOnly]
    [HttpGet]
    public IActionResult Index()
    {
        var users = DataManager.GetAllUsers();
        return Ok(Mapper.Map<List<UserModel>>(users));
    }

    [ForNotLoggedInOnly]
    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromBody] UserModel user)
    {
        var foundUser = DataManager.FindUserByName(user.Name);
        if (foundUser == null)
            return NotFound();
        LoggedInUser = foundUser;
        return Ok();
    }

    [ForLoggedInOnly]
    [HttpPost]
    [Route("logout")]
    public IActionResult Logout()
    {
        LoggedInUser = null;
        return Ok();
    }

    [ForNotLoggedInOnly]
    [HttpPost]
    [Route("register")]
    public IActionResult Register([FromBody] UserModel user)
    {
        var existingUser = DataManager.FindUserByName(user.Name);
        if (existingUser != null)
            return Conflict();
        DataManager.AddUser(new User { Name = user.Name });
        return Login(user);
    }
}
