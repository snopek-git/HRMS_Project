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
    [Authorize]
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

            foreach(var employee in employeeUserManager.Users)
            {
                if(await employeeUserManager.IsInRoleAsync(employee, role.Name))
                {
                    model.Users.Add(employee.UserName);
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

            foreach (var user in employeeUserManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
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
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = employeeUserManager.Users;
            return View(users);
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
                //IdManager = (int)user.IdManager,
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await employeeUserManager.FindByIdAsync(model.Id);

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
                //IdManager = (int)user.IdManager,

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

        //=============================================//
        //================ Umowy ==============//
        //=============================================//
        //[HttpGet]
        //public async Task<IActionResult> ListContracts()
        //{
        //    var listOfContracts = context.Contract.Include(e => e.IdEmployeeNavigation).Include(t => t.IdContractTypeNavigation).Include(s => s.IdContractStatusNavigation);
        //    return View(await listOfContracts.ToListAsync());
        //}

        //[HttpGet]
        //public async Task<IActionResult> ContractDetails(int ? id)
        //{
        //    if(id == null)
        //    {
        //        return NotFound();
        //    }

        //    var contract = await context.Contract
        //                                    .Include(c => c.IdEmployeeNavigation)
        //                                    .Include(c => c.IdContractStatusNavigation)
        //                                    .Include(c => c.IdContractTypeNavigation)
        //                                    .FirstOrDefaultAsync(m => m.IdContract == id);
        //    if(contract == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(contract);
        //}

        //[HttpGet]
        //public IActionResult CreateContract()
        //{
        //    ViewData["IdContractStatus"] = new SelectList(context.ContractStatus, "IdContractStatus", "StatusName");
        //    ViewData["IdContractType"] = new SelectList(context.ContractType, "IdContractType", "ContractTypeName");
        //    ViewData["IdEmployee"] = new SelectList(context.Employee, "Id", "Email");
        //    //ViewData["Benefits"] = context.Benefit;

        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateContract(Contract contract)
        //{

        //    ViewData["IdContractStatus"] = new SelectList(context.ContractStatus, "IdContractStatus", "StatusName");
        //    ViewData["IdContractType"] = new SelectList(context.ContractType, "IdContractType", "ContractTypeName");
        //    ViewData["IdEmployee"] = new SelectList(context.Employee, "Id", "Email");
        //    if (ModelState.IsValid)
        //    {
        //        context.Add(contract);

        //        //Adding Benefits To Contract
        //        //foreach(int idBenefit in IdsBenefits)
        //        //{
        //        //    ContractBenefit contractBenefit = new ContractBenefit();
        //        //    contractBenefit.IdContract = contract.IdContract;
        //        //    contractBenefit.IdBenefit = idBenefit;
        //        //    context.ContractBenefit.Add(contractBenefit);
        //        //}

        //        await context.SaveChangesAsync();
        //        return RedirectToAction(nameof(ListContracts));
        //    }
        //    return View(contract);
        //}

        //[HttpGet]
        //public async Task<IActionResult> EditContract(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var contract = await context.Contract.FindAsync(id);

        //    if(contract == null)
        //    {
        //        return NotFound();
        //    }

        //    var result = from b in context.Benefit
        //                 select new
        //                 {
        //                     b.IdBenefit,
        //                     b.Name,
        //                     IsSelected = ((from cb in context.ContractBenefit
        //                                    where(cb.IdContract == id) & (cb.IdBenefit == b.IdBenefit)
        //                                    select cb).Count() > 0)
        //                 };

        //    var editContractViewModel = new EditContractViewModel();

        //    /*editContractViewModel.IdContract = id.Value;
        //    editContractViewModel.Salary = contract.Salary;
        //    editContractViewModel.ContractStart = contract.ContractStart;
        //    editContractViewModel.ContractEnd = contract.ContractEnd;*/
        //    editContractViewModel.Contract = contract;

        //    var benefitCheckBox = new List<BenefitCheckBoxViewModel>();

        //    foreach (var item in result)
        //    {
        //        //var benefitCheckBoxViewModel = new BenefitCheckBoxViewModel
        //        //{
        //        //    IdBenefit = benefit.IdBenefit,
        //        //    BenefitName = benefit.Name
        //        //};
        //        benefitCheckBox.Add(new BenefitCheckBoxViewModel{ IdBenefit = item.IdBenefit, BenefitName = item.Name, IsSelected = item.IsSelected});
        //    }



        //    ViewData["IdContractStatus"] = new SelectList(context.ContractStatus, "IdContractStatus", "StatusName");
        //    ViewData["IdContractType"] = new SelectList(context.ContractType, "IdContractType", "ContractTypeName");
        //    ViewData["IdEmployee"] = new SelectList(context.Employee, "Id", "Email");
        //    //ViewData["Benefits"] = benefitCheckBox; //context.Benefit;

        //    editContractViewModel.Benefits = benefitCheckBox;

        //    return View(editContractViewModel);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditContract(EditContractViewModel editContract)//int id, Contract contract, int[] IdsBenefits)
        //{
        //    //var result = from b in context.Benefit
        //    //             select new
        //    //             {
        //    //                 b.IdBenefit,
        //    //                 b.Name,
        //    //                 IsSelected = ((from cb in context.ContractBenefit
        //    //                                where (cb.IdContract == id) & (cb.IdBenefit == b.IdBenefit)
        //    //                                select cb).Count() > 0)
        //    //             };

        //    //var benefitCheckBox = new List<BenefitCheckBoxViewModel>();

        //    //foreach (var item in result)
        //    //{
        //    //    benefitCheckBox.Add(new BenefitCheckBoxViewModel { IdBenefit = item.IdBenefit, BenefitName = item.Name, IsSelected = item.IsSelected });
        //    //}

        //    ViewData["IdContractStatus"] = new SelectList(context.ContractStatus, "IdContractStatus", "StatusName");
        //    ViewData["IdContractType"] = new SelectList(context.ContractType, "IdContractType", "ContractTypeName");
        //    ViewData["IdEmployee"] = new SelectList(context.Employee, "Id", "Email");
        //    //ViewData["Benefits"] = benefitCheckBox;

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            context.Update(editContract.Contract);
        //            //Adding Benefits To Contract
        //            //foreach (int idBenefit in IdsBenefits)
        //            //{
        //            //    ContractBenefit contractBenefit = new ContractBenefit
        //            //    {
        //            //        IdContract = contract.IdContract,
        //            //        IdBenefit = idBenefit
        //            //    };
        //            //    context.ContractBenefit.Add(contractBenefit);
        //            //}

        //            foreach(var item in context.ContractBenefit)
        //            {
        //                if(item.IdContract == editContract.Contract.IdContract)
        //                {
        //                    context.Entry(item).State = EntityState.Deleted;
        //                }
        //            }

        //            foreach (var item in editContract.Benefits)
        //            {
        //                if (item.IsSelected)
        //                {
        //                    context.ContractBenefit.Add(new ContractBenefit()
        //                    {
        //                        IdContract = editContract.Contract.IdContract,
        //                        IdBenefit = item.IdBenefit
        //                    });
        //                }
        //            }



        //            await context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ContractExists(editContract.Contract.IdContract))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(ListContracts));
        //    }

        //    return View(editContract);
        //}

        //public async Task<IActionResult> DeleteContract(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var contract = await context.Contract
        //        .Include(c => c.IdContractStatusNavigation)
        //        .Include(c => c.IdContractTypeNavigation)
        //        .Include(c => c.IdEmployeeNavigation)
        //        .FirstOrDefaultAsync(m => m.IdContract == id);
        //    if (contract == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(contract);
        //}

        //[HttpPost, ActionName("DeleteContract")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var contract = await context.Contract.FindAsync(id);
        //    context.Contract.Remove(contract);
        //    await context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ContractExists(int id)
        //{
        //    return context.Contract.Any(e => e.IdContract == id);
        //}
    }
}