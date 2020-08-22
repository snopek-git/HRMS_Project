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
using Microsoft.EntityFrameworkCore;

namespace HRMS_Project.Controllers
{
    public class AvailableAbsenceController : Controller
    {

        private readonly UserManager<Employee> userManager;
        private readonly ApplicationDbContext _context;

        public AvailableAbsenceController(UserManager<Employee> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this._context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> ListAvailableAbsence()
        {

            var availableAbsence = await _context.AvailableAbsence
                                        .ToListAsync();

            var absenceType = await _context.AbsenceType
                                        .ToListAsync();

            var model = new AbsenceViewModel
            {
                AvailableAbsence = availableAbsence,
                AbsenceType = absenceType
            };

            return View(model);

        }

        //get available abasence of Employee [per type of absence]
        [HttpGet]
        public async Task<IActionResult> Summary(string id)
        {

            var availableAbsence = await _context.AvailableAbsence
                                        .Where(a => a.IdEmployee == id)
                                        .ToListAsync();

            var absenceType = await _context.AbsenceType
                                        .ToListAsync();

            var model = new AbsenceViewModel
            {
                AvailableAbsence = availableAbsence,
                AbsenceType = absenceType
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateAvailableAbsence()
        {
            ViewData["IdAbsenceType"] = new SelectList(_context.AbsenceType, "IdAbsenceType", "AbsenceTypeName");
            //ViewData["IdEmployee"] = new SelectList(_context.Employee, "Id", "Email");
            var EmpQuery = _context.Employee.OrderBy(x => x.LastName).Select(x => new
            {
                Id = x.Id,
                FullName = x.LastName + " " + x.FirstName
            });

            ViewData["IdEmployee"] = new SelectList(EmpQuery, "Id", "FullName");


            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAvailableAbsence(AvailableAbsence availableAbsence)
        {
            ViewData["IdAbsenceType"] = new SelectList(_context.AbsenceType, "IdAbsenceType", "AbsenceTypeName");
            ViewData["IdEmployee"] = new SelectList(_context.Employee, "Id", "Email");

            if (ModelState.IsValid)
            {
                _context.Add(availableAbsence);

                await _context.SaveChangesAsync();
                return RedirectToAction("ListAvailableAbsence");
            }

            return View(availableAbsence);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteAvailableAbsence(int id)
        {
            var availableAbsence = await _context.AvailableAbsence.FindAsync(id);

            if(availableAbsence == null)
            {
                return NotFound();
            }
            else
            {
                _context.AvailableAbsence.Remove(availableAbsence);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListAvailableAbsence");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetAvailableAbsence(int id)
        {
            var availableAbsence = await _context.AvailableAbsence.FindAsync(id);

            availableAbsence.AvailableDays = 0;
            availableAbsence.UsedAbsence = 0;

            if (availableAbsence == null)
            {
                return NotFound();
            }
            else
            {
                _context.AvailableAbsence.Update(availableAbsence);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListAvailableAbsence");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditAvailableAbsence(int id)
        {

            var availableAbsence = await _context.AvailableAbsence.FindAsync(id);

            if (availableAbsence == null)
            {
                return NotFound();
            }

            var absenceType = await _context.AbsenceType.FindAsync(availableAbsence.IdAbsenceType);

            var model = new EditAvailableAbsenceViewModel
            {
                AvailableAbsence = availableAbsence,
                AbsenceType = absenceType
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAvailableAbsence(int id, AvailableAbsence availableAbsence)
        {
            if (id != availableAbsence.IdAvailableAbsence)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(availableAbsence);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvailableAbsenceExists(availableAbsence.IdAvailableAbsence))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListAvailableAbsence));
            }
            return View(availableAbsence);
        }

        private bool AvailableAbsenceExists(int id)
        {
            return _context.AvailableAbsence.Any(e => e.IdAvailableAbsence == id);
        }
    }
}