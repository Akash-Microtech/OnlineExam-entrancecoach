using OnlineExam.Models;
using OnlineExam.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private DB db = new DB();

        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(RoleViewModel roleViewModel)
        {
            var role = new UserRole
            {
                RoleName = roleViewModel.RoleName
            };

            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                await db.SaveChangesAsync();
                ViewBag.StatusMessage = "Role Succesfully Completed";
                return RedirectToAction("RoleList");
            }

            return View(role);
        }

        public async Task<ActionResult> RoleList()
        {
            return View(await db.Roles.ToListAsync());
        }

        public async Task<ActionResult> UserAccounts()
        {
            var users = await db.Users.ToListAsync();
            foreach (var item in users)
            {
                item.Role = db.Roles.Where(r => r.RoleId == item.RoleId).FirstOrDefault();
            }
            return View(users);
        }

        public ActionResult AddUser()
        {
            RegisterViewModel model = new RegisterViewModel();
            model.Roles = db.Roles.ToList();
            model.RoleId = 0;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(RegisterViewModel model)
        {
            string alpha = null;

            if(model.RoleId == 1)
            {
                alpha = "ECA";
            }
            else if (model.RoleId == 2)
            {
                alpha = "ECS";
            }
            else if (model.RoleId == 3)
            {
                alpha = "ECT";
            }
            else if (model.RoleId == 4)
            {
                alpha = "ECD";
            }
            else
            {
                alpha = "ECC";
            }

            Random random = new Random();
            int unique = random.Next(10000, 99999);
            int y = DateTime.Now.Year;
            int m = DateTime.Now.Month;
            var userId = alpha + y + m + unique;

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                UserId = userId,
                CreatedDate = DateTime.Now,
                RoleId = model.RoleId
            };

            var data = db.Users.Where(d => d.Email == user.Email || d.UserName == user.UserName).FirstOrDefault();

            if (data != null)
            {
                ModelState.AddModelError("", "Email or Username already exists");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                ViewBag.StatusMessage = "User Created Succesfully.";
                return RedirectToAction("UserAccounts");
            }

            return View(model); ;
        }
    }
}