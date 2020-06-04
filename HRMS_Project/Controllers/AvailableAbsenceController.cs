using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_Project.Data;
using HRMS_Project.Models;
using HRMS_Project.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            var model = await _context.AvailableAbsence.ToListAsync();
            return View(model);
        }


        //get available abasence of Employee [per type of absence]
        [HttpGet]
        public async Task<IActionResult> Summary(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Użytkownik z Id = {id} nie istnieje";
                return View("Error");
            }

            var availableAbsence = await _context.AvailableAbsence
                                        .Where(a => a.IdEmployee == id)
                                        .ToListAsync();

            var absenceType = await _context.AbsenceType
                                        .ToListAsync();

            var model = new AbsenceViewModel
            {
                Employee = user,
                AvailableAbsence = availableAbsence,
                AbsenceType = absenceType
            };

            return View(model);

        }

    }
}