using HRMS_Project.Data;
using HRMS_Project.Models;
using HRMS_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Controllers
{
    [Authorize(Roles = "PracownikHR, Administrator")]
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
            ViewData["IdJob"] = new SelectList(context.Job.OrderBy(x => x.JobName), "IdJob", "JobName");
            //ViewData["IdManager"] = new SelectList(context.Employee, "IdEmployee", "LastName");
            ViewData["IdRole"] = new SelectList(roleManager.Roles, "Id", "Name");

            var query = from e in context.Employee
                        where e.IdJob == 1
                        orderby e.LastName
                        select new
                        {
                            IdManager = e.Id,
                            Name = e.LastName + " " + e.FirstName
                        };

            ViewData["IdManager"] = new SelectList(query, "IdManager", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["IdJob"] = new SelectList(context.Job, "IdJob", "JobName");
            ViewData["IdRole"] = new SelectList(roleManager.Roles, "Id", "Name");

            var query = from e in context.Employee
                        where e.IdJob == 1
                        orderby e.LastName
                        select new
                        {
                            IdManager = e.Id,
                            Name = e.LastName + ' ' + e.FirstName
                        };

            ViewData["IdManager"] = new SelectList(query, "IdManager", "Name");


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
                    IdJob = model.IdJob,
                    IdManager = model.IdManager
                    

                };
                var result = await userManager.CreateAsync(user, model.Password);                

                if (result.Succeeded)
                {
                    //await signInManager.SignInAsync(user, isPersistent: false); //Logowanie od razu po rejestracji

                    var role = await roleManager.FindByIdAsync(model.IdRole);
                    var roleResult = await userManager.AddToRoleAsync(user, role.Name);

                    return RedirectToAction("index", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {            
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {                
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {                    
                    return RedirectToAction("index", "home");
                }

                ModelState.AddModelError("", "Błędna próba logowania");
                
            }

            return View(model);
        }

    }
}
