using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Test3.Models;

namespace Test3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
          return View();
            
        }

        
        [Authorize(Policy = "CartAccessPolicy")]
        [HttpGet]
        public IActionResult Cart()
        {
            return View();

        }

        [HttpGet]
        [Authorize(Policy = "AdminPanelAccessPolicy")]
        public IActionResult AdminPanel()
        {
            return View();

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Policy = "UserDataAccessPolicy")]
        public IActionResult UserData()
        {
            return View();

        }
        
        [Authorize(Policy = "UserCartAccessPolicy")]
        public IActionResult UserCart()
        {
            return View();

        }
        

    }
}
