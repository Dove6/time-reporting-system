﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using TRS.DataManager;
using TRS.Models;
using TRS.Models.DomainModels;
using TRS.Models.ViewModels;

namespace TRS.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IDataManager _dataManager;

        public UserController(ILogger<UserController> logger, IDataManager dataManager)
        {
            _logger = logger;
            _dataManager = dataManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            var userSelectList = new UserSelectListModel
            {
                Usernames = _dataManager.GetAllUsers().Values.Select(x => new SelectListItem(x.Name, x.Name)).ToList()
            };
            return View(userSelectList);
        }

        [HttpPost]
        public IActionResult Login(string username)
        {
            var user = _dataManager.FindUserByName(username);
            if (user != null)
                Response.Cookies.Append("user",
                    JsonSerializer.Serialize(user),
                    new CookieOptions
                    {
                        MaxAge = TimeSpan.FromHours(1),
                        HttpOnly = true
                    });
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("user");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username)
        {
            _dataManager.AddUser(new UserModel { Name = username });
            return Login(username);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
