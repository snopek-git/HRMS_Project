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
            ViewData["IdEmployee"] = new SelectList(_context.Employee, "Id", "Email");

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

        [HttpGet]
        public async Task<IActionResult> EditAvailableAbsence(int id)
        {

            var availableAbsence = await _context.AvailableAbsence.FindAsync(id);
            
            if(availableAbsence == null)
            {
                return NotFound();
            }

            var absenceType = await _context.AbsenceType.ToListAsync();

            var model = new EditAvailableAbsenceViewModel
            {
                AvailableAbsence = availableAbsence,
                AbsenceType = absenceType
            };

            return View(model);
        }
    }
}