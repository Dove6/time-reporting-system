using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Trs.Controllers.Attributes;
using Trs.DataManager;
using Trs.Models.DomainModels;
using Trs.Models.RestModels;

namespace Trs.Controllers;

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
    [HttpGet("current")]
    public IActionResult GetCurrent()
    {
        return Ok(Mapper.Map<UserModel>(LoggedInUser));
    }

    [ForNotLoggedInOnly]
    [HttpPost("{username}/login")]
    public IActionResult Login(string username)
    {
        var foundUser = DataManager.FindUserByName(username);
        if (foundUser == null)
            return NotFound();
        LoggedInUser = foundUser;
        return Ok();
    }

    [ForLoggedInOnly]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        LoggedInUser = null;
        return Ok();
    }

    [ForNotLoggedInOnly]
    [HttpPut("{username}")]
    public IActionResult Put(string username)
    {
        var existingUser = DataManager.FindUserByName(username);
        if (existingUser != null)
            return Conflict();
        var addedUser = new User { Name = username };
        DataManager.AddUser(addedUser);
        LoggedInUser = addedUser;
        return Created($"/api/users/{username}/login", Mapper.Map<UserModel>(addedUser));
    }
}
