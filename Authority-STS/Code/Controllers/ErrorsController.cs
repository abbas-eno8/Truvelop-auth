using Microsoft.AspNetCore.Mvc;

namespace AuthoritySTS.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}