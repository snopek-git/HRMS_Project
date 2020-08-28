using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRMS_Project.Data;
using HRMS_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Project.Controllers
{
    public class OvertimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OvertimeController (ApplicationDbContext context)
        {
            this._context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeOvertime(string id)
        {

            var overtime = await _context.Overtime
                                        .Where(a => a.IdEmployee == id)
                                        .OrderByDescending(a => a.ToBeSettledBefore)
                                        .ToListAsync();


            //dodac tworzenie nowego Overtime jesli zaczyna sie nowy kwartal
            DateTime today = DateTime.Today;
            DateTime maxDateOvertime = overtime.OrderByDescending(a => a.ToBeSettledBefore)
                                               .First()
                                               .ToBeSettledBefore;


            if(maxDateOvertime != null && today > maxDateOvertime)
            {
                Overtime newQuarterOvertime = new Overtime
                {
                    IdEmployee = id,
                    Quantity = 0,
                    ToBeSettledBefore = QuarterEndDate()
            };

                _context.Add(newQuarterOvertime);
                await _context.SaveChangesAsync();

                overtime = await _context.Overtime
                                        .Where(a => a.IdEmployee == id)
                                        .OrderByDescending(a => a.ToBeSettledBefore)
                                        .ToListAsync();
            }



            //zaktualozwac overtime

            return View(overtime);
        }

        public DateTime QuarterEndDate()
        {

            int quarterNumber = (DateTime.Now.Month - 1) / 3 + 1;
            DateTime firstDayOfQuarter = new DateTime(DateTime.Now.Year, (quarterNumber - 1) * 3 + 1, 1);
            DateTime lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
            DateTime date = lastDayOfQuarter;
            return date;
        }
    }
}