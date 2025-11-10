using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplicatoin3.Domain.ViewModels.LoginAndRegistration;


namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult SiteInformation()
        {
            return View();
        }
       
        
            public IActionResult Index()
            {
                return View();
            }

            public IActionResult Contacts()
            {
                return View();
            }
            public IActionResult Privacy()
            {
                return View();
            }
          [HttpPost]
          public IActionResult Login([FromBody] LoginViewModel model)
          {
            if (ModelState.IsValid)
            {
                return Ok(model);
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

            return BadRequest(errors);
          }
        [HttpPost]
        public IActionResult Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(errors);
            }
            return Ok(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
