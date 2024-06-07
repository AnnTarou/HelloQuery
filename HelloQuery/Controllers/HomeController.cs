using System.Diagnostics;
using HelloQuery.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HelloQuery.Controllers
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
            // セッションからセッションIDを取得
            var sessionId = HttpContext.Session.GetString("SessionId");

            // セッションIDが存在しない場合はCookieをチェック
            if (sessionId == null)
            {
                HttpContext.Request.Cookies.TryGetValue("SessionId", out sessionId);
            }

            User user = null;

            // セッションIDが取得できた場合、Jsonを変換してユーザーを取得
            if (sessionId != null)
            {
                var userJson = HttpContext.Session.GetString(sessionId);
                if (userJson != null)
                {
                    user = JsonConvert.DeserializeObject<User>(userJson);
                }
            }

            // もしセッションもしくはクッキーがあればマイページへ
            if (user != null)
            {
                return RedirectToAction("Index", "Lessons");
            }
            // セッションもしくはクッキーがなければHomeを表示
            else
            {
                return View();
            }
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
