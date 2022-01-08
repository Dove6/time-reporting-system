using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Trs.Controllers.Attributes;
using Trs.DataManager;
using Trs.Models.DomainModels;
using Trs.Models.RestModels;

namespace Trs.Controllers;

[Route("[controller]")]
public class UsersController : BaseController
{
    private ILogger<UsersController> _logger;

    public UsersController(IDataManager dataManager, IMapper mapper, ILogger<UsersController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    [ForNotLoggedInOnly]
    [HttpGet]
    public IActionResult Get()
    {
        var users = DataManager.GetAllUsers();
        return Ok(Mapper.Map<List<UserModel>>(users));
    }

    [ForLoggedInOnly]
    [HttpGet]
    [Route("current")]
    public IActionResult GetCurrent()
    {
        return Ok(LoggedInUser);
    }

    [ForNotLoggedInOnly]
    [HttpPost]
    [Route("{username}/login")]
    public IActionResult Login(string username)
    {
        var foundUser = DataManager.FindUserByName(username);
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
    [HttpPut]
    [Route("{username}")]
    public IActionResult Put(string username)
    {
        var existingUser = DataManager.FindUserByName(username);
        if (existingUser != null)
            return Conflict();
        var addedUser = new User { Name = username };
        DataManager.AddUser(addedUser);
        return CreatedAtAction(nameof(Login), new { username });
    }
}
