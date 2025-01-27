using FBone.Service;
using Microsoft.AspNetCore.Mvc;

namespace FBone.Controllers
{    
    public class CryptController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string decrypted, string encrypted, string act)
        {
            if (act.Equals("encrypt") && decrypted.Length != 0)
            {
                ViewBag.decr_result = decrypted;
                ViewBag.encr_result = MyEncryption.EncryptString(decrypted);
            }
            return View();
        }
        public IActionResult Index()
        {
            ViewBag.encr_result = "";
            ViewBag.decr_result = "";
            return View();
        }
    }
}