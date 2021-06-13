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
    }
}