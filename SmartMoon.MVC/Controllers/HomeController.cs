using Microsoft.AspNetCore.Mvc;
using SmartMoon.MVC.Models;
using SmartMoon.MVC.Models.Data;
using SmartMoon.MVC.Models.ViewModels;
using System.Diagnostics;

namespace SmartMoon.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext context;

        public HomeController(ILogger<HomeController> logger,AppDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult Index()
        {
            var model = new HomePageViewModel
            {
                moneyDrawers = context.moneyDrawer.ToList()
            };
            return View(model);
        }

        public IActionResult Privacy()
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
