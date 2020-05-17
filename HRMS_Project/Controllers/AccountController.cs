using HRMS_Project.Data;
using HRMS_Project.Models;
using HRMS_Project.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Employee> userManager;
        private readonly SignInManager<Employee> signInManager;
        private readonly ApplicationDbContext context;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<Employee> userManager, SignInManager<Employee> signInManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["IdJob"] = new SelectList(context.Job, "IdJob", "JobName");
            ViewData["IdManager"] = new SelectList(context.Employee, "IdManager", "LastName");
            ViewData["IdRole"] = new SelectList(roleManager.Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Employee
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    SecondName = model.SecondName,
                    LastName = model.LastName,
                    Pesel = model.Pesel,
                    BirthDate = model.BDate,
                    PhoneNumber = model.PhoneNumber,
                    IdCardNumber = model.IdCardNumber,
                    IdJob = model.IdJob
                    //IdManager = model.IdManager
                    

                };
                var result = await userManager.CreateAsync(user, model.Password);                

                //ViewData["IdJob"] = new SelectList(context.Job, "IdJob", "JobName", model.IdJob);
                //ViewData["IdManager"] = new SelectList(context.Employee, "IdEmployee", "EmailAddress", model.IdManager);
                //ViewData["IdRole"] = new SelectList(context.Role, "IdRole", "RoleName", employee.IdRole);

                if (result.Succeeded)
                {
                    //await signInManager.SignInAsync(user, isPersistent: false); //Logowanie od razu po rejestracji

                    var role = await roleManager.FindByIdAsync(model.IdRole.ToString());
                    var roleResult = userManager.AddToRoleAsync(user, role.Name);

                    return RedirectToAction("index", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

    }
}
