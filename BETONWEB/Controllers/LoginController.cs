using BETONWEB.Models.Classes;
using BETONWEB.Models.Entity;
using BETONWEB.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BETONWEB.Controllers
{
    public class LoginController : Controller
    {
        private readonly Context _context;

        public LoginController()
        {
            _context = new Context();
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(UserLogin model)
        {
            if (ModelState.IsValid)
            {             
                var bilgiler = _context.Sabit_Kullanicilar.FirstOrDefault(x => x.Kullanici == model.Kullanici && x.Sifre == model.Sifre);
                if (bilgiler != null)
                {
                    // if (model.BeniHatirla==true)
                    //{

                    //}
                    FormsAuthentication.SetAuthCookie(bilgiler.Kullanici, false);
                    Session["Kullanici"] = bilgiler.Kullanici.ToString();
                    Session["Sifre"] = bilgiler.Sifre.ToString();
                    return RedirectToAction("Index", "GeneralSells");
                }
                ModelState.AddModelError("LoginError", "Geçersiz kullanıcı adı veya şifre");
            }
           
            return View(model);          
        }
    }
}