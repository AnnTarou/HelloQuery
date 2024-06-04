using Microsoft.AspNetCore.Mvc;

namespace HelloQuery.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult error()
        {
            return View();
        }
    }
}
