using Microsoft.AspNetCore.Mvc;

namespace FBone.Controllers
{
    public class AccessController : Controller
    {        
        public ActionResult Denied()
        {
            return View();
        }
    }
}