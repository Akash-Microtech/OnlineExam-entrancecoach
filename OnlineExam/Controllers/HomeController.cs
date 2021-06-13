using OnlineExam.Models;
using OnlineExam.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OnlineExam.Controllers
{
    public class HomeController : Controller
    {
        private DB db = new DB();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            if (loginViewModel.UserName == null && loginViewModel.Password == null)
            {
                ModelState.AddModelError("", "Please fill all mandatory fields");
                return View(loginViewModel);
            }

            User user = db.Users.Where(u => u.UserName == loginViewModel.UserName && u.Password == loginViewModel.Password).FirstOrDefault();

            if (user != null)
            {
                var Ticket = new FormsAuthenticationTicket(loginViewModel.UserName, true, 3000);
                string Encrypt = FormsAuthentication.Encrypt(Ticket);
                var Cookie = new HttpCookie(FormsAuthentication.FormsCookieName, Encrypt);
                Cookie.Expires = DateTime.Now.AddHours(3000);
                Cookie.HttpOnly = true;
                Response.Cookies.Add(Cookie);

                if(user.RoleId == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if(user.RoleId == 2)
                {
                    return RedirectToAction("Index", "Student");
                }
                else if(user.RoleId == 3)
                {
                    return RedirectToAction("Index", "Teacher");
                }
                else if (user.RoleId == 4)
                {
                    return RedirectToAction("Index", "Dtp");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(loginViewModel);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                CreatedDate = DateTime.Now,
                RoleId = model.RoleId
            };

            var data = db.Users.Where(d => d.Email == user.Email || d.UserName == user.UserName).FirstOrDefault();

            if(data != null)
            {
                ModelState.AddModelError("", "Email or Username already exists");
                return View(model);
            }

            if (ModelState.IsValid)
            {                
                db.Users.Add(user);
                await db.SaveChangesAsync();
                ViewBag.StatusMessage = "Registration Succesfully Completed";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult NotFound()
        {
            return View();
        }
    }
}