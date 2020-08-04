using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_Project.Data;
using HRMS_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Project.Controllers
{
    public class AbsenceTypeController : Controller
    {

        private readonly UserManager<Employee> userManager;
        private readonly ApplicationDbContext _context;

        public AbsenceTypeController(UserManager<Employee> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this._context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateAbsenceType()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAbsenceType(AbsenceType absenceType)
        {

            if (ModelState.IsValid)
            {
                _context.Add(absenceType);

                await _context.SaveChangesAsync();
                return RedirectToAction("ListAbsenceType");
            }


            return View(absenceType);
        }


        [HttpGet]
        public async Task<ActionResult> ListAbsenceType()
        {
            var model = await _context.AbsenceType.ToListAsync();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteAbsenceType(int id)
        {
            var absenceType = await _context.AbsenceType.FindAsync(id);

            if (absenceType == null)
            {
                return NotFound();
            }
            else
            {
                _context.AbsenceType.Remove(absenceType);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListAbsenceType");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditAbsenceType(int id)
        {

            var model = await _context.AbsenceType.FindAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAbsenceType(int id, AbsenceType absenceType)
        {
            if (id != absenceType.IdAbsenceType)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(absenceType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbsenceTypeExists(absenceType.IdAbsenceType))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListAbsenceType));
            }
            return View(absenceType);
        }

        private bool AbsenceTypeExists(int id)
        {
            return _context.AbsenceType.Any(e => e.IdAbsenceType == id);
        }

    }
}