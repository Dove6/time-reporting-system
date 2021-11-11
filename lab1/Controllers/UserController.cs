﻿using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using TRS.Controllers.Attributes;
using TRS.Controllers.Constants;
using TRS.DataManager;
using TRS.DataManager.Exceptions;
using TRS.Models.DomainModels;
using TRS.Models.ViewModels;

namespace TRS.Controllers
{
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

        [ForNotLoggedInOnly]
        public IActionResult Login()
        {
            var userSelectList = new UserSelectListModel
            {
                Usernames = DataManager.GetAllUsers().Select(x => new SelectListItem(x.Name, x.Name)).ToList()
            };
            return View(userSelectList);
        }

        [ForNotLoggedInOnly]
        [HttpPost]
        public IActionResult Login(string username)
        {
            var user = DataManager.FindUserByName(username);
            if (user == null)
                return RedirectToActionWithError(ErrorMessages.GetUserNotFoundMessage(username));
            LoggedInUser = user;
            return RedirectToAction("Index", "Home");
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
        public IActionResult Register(string username)
        {
            try
            {
                DataManager.AddUser(new User { Name = username });
            }
            catch (AlreadyExistingException)
            {
                return RedirectToActionWithError(ErrorMessages.GetUserAlreadyExistingMessage(username));
            }
            return Login(username);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
