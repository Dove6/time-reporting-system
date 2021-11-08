using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using TRS.Controllers.Attributes;
using TRS.DataManager;
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

        public IActionResult Login()
        {
            var userSelectList = new UserSelectListModel
            {
                Usernames = DataManager.GetAllUsers().Select(x => new SelectListItem(x.Name, x.Name)).ToList()
            };
            return View(userSelectList);
        }

        [HttpPost]
        public IActionResult Login(string username)
        {
            var user = DataManager.FindUserByName(username);
            if (user != null)
                LoggedInUser = user;
            return RedirectToAction("Index", "Home");
        }

        [ForLoggedInOnly]
        [HttpPost]
        public IActionResult Logout()
        {
            LoggedInUser = null;
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username)
        {
            DataManager.AddUser(new User { Name = username });
            return Login(username);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
