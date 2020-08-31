using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_Project.Data;
using HRMS_Project.Models;
using HRMS_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HRMS_Project.Controllers
{
    [Authorize(Roles = "PracownikHR, Administrator")]
    public class AdministrationController : Controller
    {

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<Employee> employeeUserManager;
        private readonly ApplicationDbContext context;
        public AdministrationController (RoleManager<IdentityRole> roleManager, UserManager<Employee> employeeUserManager, ApplicationDbContext context)
        {
            this.roleManager = roleManager;
            this.employeeUserManager = employeeUserManager;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //=============================================//
        //================ Role ===============//
        //=============================================//

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {

            //Business -> Services do uzupelnienia, zeby zastapic ten kod??

            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if(role == null)
            {
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach(var employee in employeeUserManager.Users.OrderBy(x => x.LastName))
            {
                if(await employeeUserManager.IsInRoleAsync(employee, role.Name))
                {
                    model.Users.Add(employee.LastName + " " + employee.FirstName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                //ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in employeeUserManager.Users.OrderBy(x => x.LastName))
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.LastName + " " + user.FirstName
                };

                if (await employeeUserManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            model.OrderBy(x => x.UserName);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await employeeUserManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await employeeUserManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await employeeUserManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await employeeUserManager.IsInRoleAsync(user, role.Name))
                {
                    result = await employeeUserManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }


        //=============================================//
        //================ Pracownicy ==============//
        //=============================================//
        //[HttpGet]
        public IActionResult ListUsers()
        {
            var users = employeeUserManager.Users;
            return View(users.OrderBy(x => x.LastName));
        }

        [HttpGet]
        public async Task<IActionResult> ListUsers(string searchString, string sortOrder, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            int pageSize = 10;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var query = from e in context.Employee select e;
            if(!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(e => e.LastName.Contains(searchString) || e.FirstName.Contains(searchString) || e.Email.Contains(searchString));
            }

            return View(await PaginatedList<Employee>.CreateAsync(query.OrderBy(e => e.LastName).AsNoTracking(), pageNumber ?? 1, pageSize));
            //query.OrderBy(x => x.LastName).AsNoTracking().ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await employeeUserManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Użytkownik z Id = {id} nie istnieje";
                return View("NotFound");
            }

            //var userClaims = await employeeUserManager.GetClaimsAsync(user);

            var userRoles = await employeeUserManager.GetRolesAsync(user);

            ViewData["IdJob"] = new SelectList(context.Job.OrderBy(x => x.JobName), "IdJob", "JobName");

            var query = from e in context.Employee
                        where e.IdJob == 1
                        orderby e.LastName
                        select new
                        {
                            IdManager = e.Id,
                            Name = e.LastName + " " + e.FirstName
                        };

            ViewData["IdManager"] = new SelectList(query, "IdManager", "Name");

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                LastName = user.LastName,
                Pesel = user.Pesel,
                BDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber,
                IdCardNumber = user.IdCardNumber,
                IdJob = user.IdJob,
                IdManager = user.IdManager,
                Roles = userRoles
            };



            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await employeeUserManager.FindByIdAsync(model.Id);

            ViewData["IdJob"] = new SelectList(context.Job.OrderBy(x => x.JobName), "IdJob", "JobName");

            var query = from e in context.Employee
                        where e.IdJob == 1
                        orderby e.LastName
                        select new
                        {
                            IdManager = e.IdEmployee,
                            Name = e.LastName + ' ' + e.FirstName
                        };

            ViewData["IdManager"] = new SelectList(query, "IdManager", "Name");

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Pracownik z Id = {model.Id} nie istnieje";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.SecondName = model.SecondName;
                user.LastName = model.LastName;
                user.Pesel = model.Pesel;
                user.BirthDate = model.BDate;
                user.PhoneNumber = model.PhoneNumber;
                user.IdCardNumber = model.IdCardNumber;
                user.IdJob = model.IdJob;
                user.IdEmployee = model.IdEmployee;
                user.IdManager = model.IdManager;

                var result = await employeeUserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await employeeUserManager.FindByIdAsync(id);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"Użytkownik o Id = {id} nie istenieje";
                return View("NotFound");
            } else
            {
                var result = await employeeUserManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                return View("ListUsers");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Rola z ID = {id} nie istnieje";
                return View("NotFound");
            }
            else
            {
                var result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListRoles");
            }
        }


        //=============================================//
        //================ Benefity ==============//
        //=============================================//

        public async Task<IActionResult> ListBenefits()
        {
            return View(await context.Benefit.OrderBy(x => x.Name).ToListAsync());
        }

        public IActionResult CreateBenefit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBenefit(Benefit benefit)
        {
            if (ModelState.IsValid)
            {
                context.Add(benefit);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(ListBenefits));
            }
            return View(benefit);
        }

        public async Task<IActionResult> EditBenefit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefit = await context.Benefit.FindAsync(id);
            if (benefit == null)
            {
                return NotFound();
            }
            return View(benefit);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBenefit(int id, Benefit benefit)
        {
            if (id != benefit.IdBenefit)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(benefit);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BenefitExists(benefit.IdBenefit))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListBenefits));
            }
            return View(benefit);
        }

        public async Task<IActionResult> DeleteBenefit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var benefit = await context.Benefit
                .FirstOrDefaultAsync(m => m.IdBenefit == id);
            if (benefit == null)
            {
                return NotFound();
            }

            return View(benefit);
        }

        // POST: Benefits/Delete/5
        [HttpPost, ActionName("DeleteBenefit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var benefit = await context.Benefit.FindAsync(id);
            context.Benefit.Remove(benefit);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(ListBenefits));
        }

        private bool BenefitExists(int id)
        {
            return context.Benefit.Any(e => e.IdBenefit == id);
        }
        public IActionResult Reports()
        {
            return View();
        }
    }
}