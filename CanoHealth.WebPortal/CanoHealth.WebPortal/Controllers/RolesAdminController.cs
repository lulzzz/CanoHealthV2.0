using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.ViewModels.Admin;
using Elmah;
using IdentitySample.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class RolesAdminController : Controller
    {
        public RolesAdminController()
        {
        }

        public RolesAdminController(ApplicationUserManager userManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Roles/
        public ActionResult IndexOriginal()
        {
            return View(RoleManager.Roles);
        }

        //
        // GET: /Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(roleViewModel.Name, roleViewModel.Active);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            RoleViewModel roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(roleModel);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                role.Active = roleModel.Active;
                await RoleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View();
        }

        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return HttpNotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                else
                {
                    result = await RoleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        #region Telerik

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReadActiveRoles([DataSourceRequest] DataSourceRequest request)
        {
            var roles = RoleManager.Roles
                .Where(r => r.Active)
                .Select(r => new RoleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Active = r.Active
                })
                .ToList();
            return Json(roles.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadAllRoles([DataSourceRequest] DataSourceRequest request)
        {
            var roles = RoleManager.Roles
               .Select(r => new RoleViewModel
               {
                   Id = r.Id,
                   Name = r.Name,
                   Active = r.Active
               })
               .ToList();
            return Json(roles.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CreateRole([DataSourceRequest] DataSourceRequest request, RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var roleInDb = await RoleManager.FindByNameAsync(roleViewModel.Name);
                    if (roleInDb != null)
                    {
                        ModelState.AddModelError("Name", "Duplicate Role. Please try again.");
                        return Json(new[] { roleViewModel }.ToDataSourceResult(request, ModelState));
                    }

                    var role = new ApplicationRole(roleViewModel.Name, roleViewModel.Active);
                    var roleresult = await RoleManager.CreateAsync(role);
                    if (!roleresult.Succeeded)
                    {
                        ModelState.AddModelError("", roleresult.Errors.First());
                        return Json(new[] { roleViewModel }.ToDataSourceResult(request, ModelState));
                    }
                    roleViewModel.Id = role.Id;
                }
                catch (System.Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again.");
                }
            }
            return Json(new[] { roleViewModel }.ToDataSourceResult(request, ModelState));
        }

        public async Task<ActionResult> UpdateRole([DataSourceRequest] DataSourceRequest request, RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var roleInDb = await RoleManager.FindByNameAsync(roleViewModel.Name);
                    if (roleInDb != null && roleInDb.Id != roleViewModel.Id)
                    {
                        ModelState.AddModelError("Name", "Duplicate Role. Please try again.");
                        return Json(new[] { roleViewModel }.ToDataSourceResult(request, ModelState));
                    }

                    var role = await RoleManager.FindByIdAsync(roleViewModel.Id);
                    if (role == null)
                    {
                        ModelState.AddModelError("", "Role not found.");
                        return Json(new[] { roleViewModel }.ToDataSourceResult(request, ModelState));
                    }
                    role.Name = roleViewModel.Name;
                    role.Active = roleViewModel.Active;
                    await RoleManager.UpdateAsync(role);
                }
                catch (System.Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again.");
                }
            }
            return Json(new[] { roleViewModel }.ToDataSourceResult(request, ModelState));
        }

        #endregion
    }
}