using Dermainsight.Models;
using Dermainsight.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dermainsight.Controllers
{
    public class MainMenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Logout()
        {
            CurrentUser.activeUser = null;
            return RedirectToAction("Login", "Home");
        }
    }
}
