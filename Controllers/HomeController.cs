using Dermainsight.Models;
using Dermainsight.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Logging;
using System.Text;

namespace Dermainsight.Controllers
{
    public class HomeController : Controller
    {
        private static List<Users> users = new List<Users>();
        private readonly FastApiService _fastApiService;

        public HomeController(FastApiService fastApiService)
        {

            _fastApiService = fastApiService;
        }

        public IActionResult Login()
        {            
            return View();
        }

        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> LoginPOST(String userName, String userPassword)
        {
            Users? user = await _fastApiService.getUserByUsername(userName, userPassword);

            if (user != null)
            {
                CurrentUser.activeUser = user;
                return RedirectToAction("Index", "MainMenu");
            }

            ViewBag.Errormessage = "Kullanıcı bilgileri hatalı veya eksik.";

            return View();
        }

        public IActionResult Register()
        {
            return View(new Users());
        }

        [HttpPost, ActionName("Register")]
        public async Task<IActionResult> RegisterPOST(Users user, string pass, string password_repeat)
        {
            if(!pass.Equals(password_repeat))
            {
                ViewBag.Errormessage = "Girdiğiniz şifreler uyuşmamaktadır.";
                return View();
            }
            user.Password = pass;

            var result = await _fastApiService.CreateUser(user);


            return RedirectToAction("Login");
        }
    }
}
