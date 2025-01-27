using FBone.Service;
using FBone.Service.Authorize;
using Microsoft.AspNetCore.Mvc;

namespace FBone.Controllers
{
    [Authorize("Admin")]
    public class EncryptController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string decrypted, string encrypted, string act)
        {
            if (act.Equals("encrypt") && !string.IsNullOrEmpty(decrypted))
            {
                ViewBag.decr_result = decrypted;
                ViewBag.encr_result = MyEncryption.EncryptString(decrypted);
            }
            if (act.Equals("decrypt") && !string.IsNullOrEmpty(encrypted))
            {
                ViewBag.decr_result = MyEncryption.DecryptString(encrypted);
                ViewBag.encr_result = encrypted;
            }
            ViewBag.nyURL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/Crypt";
            return View();
        }
        public IActionResult Index()
        {
            ViewBag.encr_result = "";
            ViewBag.decr_result = "";
            ViewBag.nyURL = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/Crypt";
            return View();
        }
    }
}