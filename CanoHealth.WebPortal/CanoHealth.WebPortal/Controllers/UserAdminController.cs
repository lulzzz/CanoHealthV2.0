using AutoMapper;
using CanoHealth.WebPortal.CommonTools.ExtensionMethods;
using CanoHealth.WebPortal.Core.Domain;
using CanoHealth.WebPortal.Persistance;
using CanoHealth.WebPortal.Services.Email;
using CanoHealth.WebPortal.ViewModels;
using CanoHealth.WebPortal.ViewModels.Account;
using CanoHealth.WebPortal.ViewModels.Admin;
using Elmah;
using IdentitySample.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
//using CanoHealth.WebPortal.ViewModels;

namespace IdentitySample.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class UsersAdminController : Controller
    {
        public UsersAdminController()
        {

        }

        public UsersAdminController(ApplicationUserManager userManager,
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
            private set
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

        #region Telerik

        public ActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult ReadUsers([DataSourceRequest] DataSourceRequest request)
        {
            var result = UserManager.Users.ToList()
                .Convert();
            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser([DataSourceRequest] DataSourceRequest request,
            UserFormViewModel userFormViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userInDb = await UserManager.FindByEmailAsync(userFormViewModel.Email);
                    if (userInDb != null)
                    {
                        ModelState.AddModelError("Email", "Duplicate user. Please try again.");
                        return Json(new[] { userFormViewModel }.ToDataSourceResult(request, ModelState));
                    }

                    var user = userFormViewModel.Convert();
                    var adminresult = await UserManager.CreateAsync(user, userFormViewModel.Password);

                    //If User was successfully created 
                    if (adminresult.Succeeded)
                    {
                        //Assign roles to the current user
                        var selectedRoles = userFormViewModel.Roles.Select(r => r.Name).ToArray();
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            return Json(new[] { userFormViewModel }.ToDataSourceResult(request, ModelState));
                        }
                        //Grant access to corporations
                        using (var unitOfWork = new UnitOfWork(new ApplicationDbContext()))
                        {
                            var corporationAccess = userFormViewModel.Corporations
                            .Select(uca => new UserCorporationAccess
                            {
                                AccessId = Guid.NewGuid(),
                                UserId = user.Id,
                                CorporationId = uca.CorporationId
                            }).ToList();
                            unitOfWork.UserCorporationAccessRepository
                                .AddRange(corporationAccess);
                            unitOfWork.Complete();
                        }
                        userFormViewModel.Id = user.Id;

                        var code = await UserManager
                                    .GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");

                        string body = string.Format(CultureInfo.InvariantCulture,
                                        @"<center>
                                    <h3> Hi {0} !</h3>
                                    <div> Congratulations on your new CanoHealth Credentialing account! Getting set up with CanoHealth Credentialing is quick and easy.You can get integrated in minutes. </div>
                                    <div> Your User is: {0} </div>
                                    <div> Your Password is: {2} </div>
                                    <div> Let's confirm your account by clicking on the following link. </div>
                                    <div> <a href='{1}' target='_top'>Confirm</a></div>                                    
                                    </center>",
                                            user.Email, callbackUrl, userFormViewModel.Password);

                        var email = new Emails
                        {
                            To = new List<string> { user.Email },
                            Subject = "Confirm your account.",
                            Body = body
                        };

                        await new CustomEmailService().SendSmtpEmailAsync(email);
                    }
                    else
                    {
                        ModelState.AddModelError("", adminresult.Errors.First());
                        return Json(new[] { userFormViewModel }.ToDataSourceResult(request, ModelState));
                    }
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again.");
                }
            }

            return Json(new[] { userFormViewModel }.ToDataSourceResult(request, ModelState));
        }

        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUser([DataSourceRequest] DataSourceRequest request,
            UserFormViewModel userFormViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userInDb = await UserManager.FindByEmailAsync(userFormViewModel.Email);
                    if (userInDb != null && userInDb.Id != userFormViewModel.Id)
                    {
                        ModelState.AddModelError("Email", "Duplicate user. Please try again.");
                        return Json(new[] { userFormViewModel }.ToDataSourceResult(request, ModelState));
                    }

                    var user = await UserManager.FindByIdAsync(userFormViewModel.Id);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "User not found.");
                        return Json(new[] { userFormViewModel }.ToDataSourceResult(request, ModelState));
                    }

                    user.UserName = userFormViewModel.Email;
                    user.Email = userFormViewModel.Email;
                    user.FirstName = userFormViewModel.FirstName;
                    user.LastName = userFormViewModel.LastName;
                    user.Active = userFormViewModel.Active;

                    var userRoles = await UserManager.GetRolesAsync(user.Id);

                    var selectedRole = userFormViewModel.Roles.Select(n => n.Name).ToArray() ?? new string[] { };

                    var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return Json(new[] { userFormViewModel }.ToDataSourceResult(request, ModelState));
                    }
                    result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First());
                        return Json(new[] { userFormViewModel }.ToDataSourceResult(request, ModelState));
                    }

                    using (var unitOfWork = new UnitOfWork(new ApplicationDbContext()))
                    {
                        var selectedCorporation = unitOfWork.UserCorporationAccessRepository
                            .GetCorporationAccessByUser(user.Id)
                            .Select(Mapper.Map<Corporation, CorporationViewModel>)
                            .ToList();
                        var corporationByParam = userFormViewModel.Corporations
                            .ToList();
                        var corporationToInsert = corporationByParam.Except(selectedCorporation)
                            .ToList();
                        unitOfWork.UserCorporationAccessRepository
                            .AddRange(corporationToInsert.Select(x => new UserCorporationAccess
                            {
                                AccessId = Guid.NewGuid(),
                                UserId = user.Id,
                                CorporationId = x.CorporationId
                            }));

                        var corporationToDelete = selectedCorporation.Except(corporationByParam)
                            .ToList();
                        foreach (var item in corporationToDelete)
                        {
                            var toDelete = unitOfWork.UserCorporationAccessRepository
                                .SingleOrDefault(x => x.UserId == user.Id &&
                                                      x.CorporationId == item.CorporationId);
                            unitOfWork.UserCorporationAccessRepository.Remove(toDelete);
                        }
                        unitOfWork.Complete();
                    }
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("", "We are sorry, but something went wrong. Please try again.");
                }
            }
            return Json(new[] { userFormViewModel }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region MVC5
        // GET: /Users/
        public async Task<ActionResult> IndexOriginal()
        {
            return View(await UserManager.Users.ToListAsync());
        }

        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                    var code = await UserManager
                                    .GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");

                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");

            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
        #endregion       
    }
}