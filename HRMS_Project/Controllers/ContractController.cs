using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_Project.Data;
using HRMS_Project.Models;
using HRMS_Project.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Project.Controllers
{
    public class ContractController : Controller
    {

        private readonly UserManager<Employee> userManager;
        private readonly ApplicationDbContext _context;
        public ContractController(UserManager<Employee> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this._context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListAllContracts()
        {
            var listOfContracts = _context.Contract.Include(e => e.IdEmployeeNavigation).Include(t => t.IdContractTypeNavigation).Include(s => s.IdContractStatusNavigation);
            return View(await listOfContracts.ToListAsync());
        }

        public async Task<IActionResult> ListContracts(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Użytkownik z Id = {id} nie istnieje";
                return View("Error");
            }

            var listOfContracts = _context.Contract.Include(e => e.IdEmployeeNavigation).Include(t => t.IdContractTypeNavigation).Include(s => s.IdContractStatusNavigation)
                                                  .Where(e => e.IdEmployee == id).ToListAsync();
            return View(await listOfContracts);
        }

        [HttpGet]
        public async Task<IActionResult> ContractDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                                            .Include(c => c.IdEmployeeNavigation)
                                            .Include(c => c.IdContractStatusNavigation)
                                            .Include(c => c.IdContractTypeNavigation)
                                            .FirstOrDefaultAsync(m => m.IdContract == id);
            if (contract == null)
            {
                return NotFound();
            }

            var nettoSalary = Decimal.Multiply(contract.Salary , 0.77M);
            

            var benefits = from b in _context.Benefit
                         select new
                         {
                             b.IdBenefit,
                             b.Name,
                             b.Price,
                             IsSelected = ((from cb in _context.ContractBenefit
                                            where (cb.IdContract == id) & (cb.IdBenefit == b.IdBenefit)
                                            select cb).Count() > 0)
                         };

            decimal benefitsValue = 0.0M;

            var editContractViewModel = new EditContractViewModel();

            editContractViewModel.Contract = contract;
            editContractViewModel.NettoSalary = nettoSalary;

            var benefitCheckBox = new List<BenefitCheckBoxViewModel>();

            foreach (var item in benefits)
            {
                benefitCheckBox.Add(new BenefitCheckBoxViewModel { IdBenefit = item.IdBenefit, BenefitName = item.Name, IsSelected = item.IsSelected });
                if(item.IsSelected == true)
                {
                    benefitsValue += item.Price;
                }
            }

            editContractViewModel.Benefits = benefitCheckBox;
            editContractViewModel.BenefitsValue = benefitsValue;
            editContractViewModel.FinalSalary = nettoSalary - benefitsValue;

            return View(editContractViewModel);
        }

        [HttpGet]
        public IActionResult CreateContract()
        {
            ViewData["IdContractStatus"] = new SelectList(_context.ContractStatus, "IdContractStatus", "StatusName");
            ViewData["IdContractType"] = new SelectList(_context.ContractType, "IdContractType", "ContractTypeName");
            ViewData["IdEmployee"] = new SelectList(_context.Employee, "Id", "Email");
            //ViewData["Benefits"] = context.Benefit;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContract(Contract contract)
        {

            ViewData["IdContractStatus"] = new SelectList(_context.ContractStatus, "IdContractStatus", "StatusName");
            ViewData["IdContractType"] = new SelectList(_context.ContractType, "IdContractType", "ContractTypeName");
            ViewData["IdEmployee"] = new SelectList(_context.Employee, "Id", "Email");
            if (ModelState.IsValid)
            {
                _context.Add(contract);

                //Adding Benefits To Contract
                //foreach(int idBenefit in IdsBenefits)
                //{
                //    ContractBenefit contractBenefit = new ContractBenefit();
                //    contractBenefit.IdContract = contract.IdContract;
                //    contractBenefit.IdBenefit = idBenefit;
                //    context.ContractBenefit.Add(contractBenefit);
                //}

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListAllContracts));
            }
            return View(contract);
        }

        [HttpGet]
        public async Task<IActionResult> EditContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract.FindAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            var result = from b in _context.Benefit
                         select new
                         {
                             b.IdBenefit,
                             b.Name,
                             b.Price,
                             IsSelected = ((from cb in _context.ContractBenefit
                                            where (cb.IdContract == id) & (cb.IdBenefit == b.IdBenefit)
                                            select cb).Count() > 0)
                         };

            var editContractViewModel = new EditContractViewModel();

            /*editContractViewModel.IdContract = id.Value;
            editContractViewModel.Salary = contract.Salary;
            editContractViewModel.ContractStart = contract.ContractStart;
            editContractViewModel.ContractEnd = contract.ContractEnd;*/
            editContractViewModel.Contract = contract;

            var benefitCheckBox = new List<BenefitCheckBoxViewModel>();

            foreach (var item in result)
            {
                //var benefitCheckBoxViewModel = new BenefitCheckBoxViewModel
                //{
                //    IdBenefit = benefit.IdBenefit,
                //    BenefitName = benefit.Name
                //};
                benefitCheckBox.Add(new BenefitCheckBoxViewModel { IdBenefit = item.IdBenefit, BenefitName = item.Name, Price = item.Price, IsSelected = item.IsSelected });
            }



            ViewData["IdContractStatus"] = new SelectList(_context.ContractStatus, "IdContractStatus", "StatusName");
            ViewData["IdContractType"] = new SelectList(_context.ContractType, "IdContractType", "ContractTypeName");
            ViewData["IdEmployee"] = new SelectList(_context.Employee, "Id", "Email");
            //ViewData["Benefits"] = benefitCheckBox; //context.Benefit;

            editContractViewModel.Benefits = benefitCheckBox;

            return View(editContractViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContract(EditContractViewModel editContract)//int id, Contract contract, int[] IdsBenefits)
        {
            //var result = from b in context.Benefit
            //             select new
            //             {
            //                 b.IdBenefit,
            //                 b.Name,
            //                 IsSelected = ((from cb in context.ContractBenefit
            //                                where (cb.IdContract == id) & (cb.IdBenefit == b.IdBenefit)
            //                                select cb).Count() > 0)
            //             };

            //var benefitCheckBox = new List<BenefitCheckBoxViewModel>();

            //foreach (var item in result)
            //{
            //    benefitCheckBox.Add(new BenefitCheckBoxViewModel { IdBenefit = item.IdBenefit, BenefitName = item.Name, IsSelected = item.IsSelected });
            //}

            ViewData["IdContractStatus"] = new SelectList(_context.ContractStatus, "IdContractStatus", "StatusName");
            ViewData["IdContractType"] = new SelectList(_context.ContractType, "IdContractType", "ContractTypeName");
            ViewData["IdEmployee"] = new SelectList(_context.Employee, "Id", "Email");
            //ViewData["Benefits"] = benefitCheckBox;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(editContract.Contract);
                    //Adding Benefits To Contract
                    //foreach (int idBenefit in IdsBenefits)
                    //{
                    //    ContractBenefit contractBenefit = new ContractBenefit
                    //    {
                    //        IdContract = contract.IdContract,
                    //        IdBenefit = idBenefit
                    //    };
                    //    context.ContractBenefit.Add(contractBenefit);
                    //}

                    foreach (var item in _context.ContractBenefit)
                    {
                        if (item.IdContract == editContract.Contract.IdContract)
                        {
                            _context.Entry(item).State = EntityState.Deleted;
                        }
                    }

                    foreach (var item in editContract.Benefits)
                    {
                        if (item.IsSelected)
                        {
                            _context.ContractBenefit.Add(new ContractBenefit()
                            {
                                IdContract = editContract.Contract.IdContract,
                                IdBenefit = item.IdBenefit
                            });
                        }
                    }



                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(editContract.Contract.IdContract))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListAllContracts));
            }

            return View(editContract);
        }

        public async Task<IActionResult> DeleteContract(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contract
                .Include(c => c.IdContractStatusNavigation)
                .Include(c => c.IdContractTypeNavigation)
                .Include(c => c.IdEmployeeNavigation)
                .FirstOrDefaultAsync(m => m.IdContract == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpPost, ActionName("DeleteContract")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contract.FindAsync(id);
            _context.Contract.Remove(contract);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListAllContracts));
        }

        private bool ContractExists(int id)
        {
            return _context.Contract.Any(e => e.IdContract == id);
        }

    }
}