using Microsoft.AspNetCore.Mvc;

namespace uniexetask.web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
