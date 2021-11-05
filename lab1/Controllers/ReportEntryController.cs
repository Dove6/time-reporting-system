using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TRS.Models;

namespace TRS.Controllers
{
    public class ReportEntryController : BaseController
    {
        private readonly ILogger<ReportEntryController> _logger;

        public ReportEntryController(ILogger<ReportEntryController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
