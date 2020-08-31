using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using HRMS_Project.Data;
using HRMS_Project.Models;
using HRMS_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Project.Controllers
{
    public class RequestController : Controller
    {
        private readonly UserManager<Employee> userManager;
        private readonly ApplicationDbContext _context;

        public RequestController(UserManager<Employee> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this._context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ListRequest(string id)
        {

            var request = await _context.Request
                                        .Where(a => a.IdEmployee == id)
                                        .Include(a => a.IdEmployeeNavigation)
                                        .Include(a => a.IdRequestStatusNavigation)
                                        .Include(a => a.IdRequestTypeNavigation)
                                        .Include(a => a.IdAbsenceTypeNavigation)
                                        .OrderByDescending(r => r.IdRequest)
                                        .ToListAsync();

            return View(request);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> PendingRequest(string id)
        {

            var employeeList = await userManager.Users
                                                .Where(e => e.IdManager == id)
                                                .OrderBy(x => x.LastName)
                                                .ToListAsync();

            var idList = new List<string>();

            foreach (var user in employeeList)
            {
                idList.Add(user.Id);
            }


            var request = await _context.Request
                            .Where(r => (idList.Contains(r.IdEmployee) && r.IdRequestStatus == 1))
                            .Include(a => a.IdEmployeeNavigation)
                            .Include(a => a.IdRequestStatusNavigation)
                            .Include(a => a.IdRequestTypeNavigation)
                            .Include(a => a.IdAbsenceTypeNavigation)
                            .OrderByDescending(r => r.IdRequest)
                            .ToListAsync();

            return View(request);
        }

        [Authorize(Roles = "PracownikHR, Administrator")]
        [HttpGet]
        public async Task<IActionResult> AllPendingRequests(string id)
        {

            var request = await _context.Request
                            .Where(r => (r.IdRequestStatus == 1 && r.IdEmployee != id))
                            .Include(a => a.IdEmployeeNavigation)
                            .Include(a => a.IdRequestStatusNavigation)
                            .Include(a => a.IdRequestTypeNavigation)
                            .Include(a => a.IdAbsenceTypeNavigation)
                            .OrderByDescending(r => r.IdRequest)
                            .ToListAsync();

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> RequestDetails(int id)
        {

            var request = await _context.Request.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            var requestType = await _context.RequestType.FindAsync(request.IdRequestType);

            var requestStatus = await _context.RequestStatus.FindAsync(request.IdRequestStatus);

            var user = await userManager.FindByIdAsync(request.IdEmployee);

            Employee manager;

            if (UserWithoutManagerCheck(user.Id))
            {
                manager = user;
            }
            else
            {
                manager = userManager.Users.First(e => e.Id == user.IdManager);
            }

            var absenceType = await _context.AbsenceType.FindAsync(request.AbsenceTypeRef);

            var availableAbsence = await _context.AvailableAbsence
                                                 .Where(a => a.IdEmployee == request.IdEmployee)
                                                 .ToListAsync();

            var overtime = _context.Overtime
                                   .Where(a => a.IdEmployee == request.IdEmployee)
                                   .OrderByDescending(a => a.ToBeSettledBefore)
                                   .FirstOrDefault();

            var model = new RequestDetailsViewModel
            {
                Request = request,
                RequestStatus = requestStatus,
                RequestType = requestType,
                Manager = manager,
                AbsenceType = absenceType,
                AvailableAbsence = availableAbsence,
                Overtime = overtime
            };

            return View(model);
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> PendingRequestDetails(int id)
        {

            var request = await _context.Request.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            var requestType = await _context.RequestType.FindAsync(request.IdRequestType);

            var requestStatus = await _context.RequestStatus.FindAsync(request.IdRequestStatus);

            var user = await userManager.FindByIdAsync(request.IdEmployee);

            Employee manager;

            if (UserWithoutManagerCheck(user.Id))
            {
                manager = user;
            }
            else
            {
                manager = userManager.Users.First(e => e.Id == user.IdManager);
            }

            var absenceType = await _context.AbsenceType.FindAsync(request.AbsenceTypeRef);

            var availableAbsence = await _context.AvailableAbsence
                                                 .Where(a => a.IdEmployee == request.IdEmployee)
                                                 .ToListAsync();

            var overtime = _context.Overtime
                                   .Where(a => a.IdEmployee == request.IdEmployee)
                                   .OrderByDescending(a => a.ToBeSettledBefore)
                                   .FirstOrDefault();

            var model = new RequestDetailsViewModel
            {
                Request = request,
                RequestStatus = requestStatus,
                RequestType = requestType,
                Manager = manager,
                AbsenceType = absenceType,
                AvailableAbsence = availableAbsence,
                Overtime = overtime

            };

            return View(model);
        }

        [Authorize(Roles = "PracownikHR, Administrator")]
        [HttpPost]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Request.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }
            else
            {
                _context.Request.Remove(request);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListRequest", new { id = request.IdEmployee });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateRequest(string id)
        {
            var userAbsenceType = await _context.AvailableAbsence
                                         .Where(a => a.IdEmployee == id)
                                         .Select(a => a.IdAbsenceType)
                                         .ToListAsync();



            ViewData["IdAbsenceType"] = new SelectList(_context.AbsenceType.Where(a => userAbsenceType.Contains(a.IdAbsenceType)), "IdAbsenceType", "AbsenceTypeName");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRequest(Request request)
        {

            request.RequestNumber = _context.Request.Max(r => r.IdRequest) + 1001;
            request.RequestDate = DateTime.Today;
            request.Quantity = (int)GetBusinessDays(request.StartDate, request.EndDate);
            request.IdRequestType = 1;

            //Sprawdzanie czy pracownik ma managera. Jeśli nie, jego wniosek będzie automatycznie akceptowany.

            if (UserWithoutManagerCheck(request.IdEmployee))
            {
                request.IdRequestStatus = 2;
            }
            else
            {
                request.IdRequestStatus = 1;
            }
            

            var numberOfDays = _context.AvailableAbsence
                                        .Where(a => (a.IdEmployee == request.IdEmployee) && (a.IdAbsenceType == request.AbsenceTypeRef))
                                        .FirstOrDefault()
                                        .AvailableDays;


            //Sprawdzanie czy pracownik ma wystarczająco dostępnych dni urlopowych

            if (numberOfDays - request.Quantity < 0)
            {
                return View("TooManyDaysRequested");
            }

            //Sprawdzanie czy urlopy na siebie nie nachodzą (sprawdzanie z wnioskami o statusie 1 lub 2)

            var listOfRequests = await _context.Request
                                               .Where(r => (r.IdEmployee == request.IdEmployee && r.IdRequestStatus < 3))
                                               .ToListAsync();

            if (RequestOverlapCheck(listOfRequests, request) == true)
            {
                return View("OverlappingRequests");
            }

            if (ModelState.IsValid)
            {
                _context.Add(request);

                await _context.SaveChangesAsync();
                return RedirectToAction("ListRequest", new { id = request.IdEmployee });
            }

            return View(request);
        }

        [HttpGet]
        public IActionResult CreateOvertimeRequest()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateOvertimeRequest(Request request)
        {

            request.RequestNumber = _context.Request.Max(r => r.IdRequest) + 1001;
            request.RequestDate = DateTime.Today;
            request.IdRequestType = 2;

            //Sprawdzanie czy pracownik ma managera. Jeśli nie, jego wniosek będzie automatycznie akceptowany.
            if (UserWithoutManagerCheck(request.IdEmployee))
            {
                request.IdRequestStatus = 2;
            }
            else
            {
                request.IdRequestStatus = 1;
            }

            //Sprawdzanie czy wnioski nadgodzin na siebie nie nachodzą (sprawdzanie z wnioskami o statusie 1 lub 2)

            var listOfOvertimeRequests = await _context.Request
                                               .Where(r => (r.IdEmployee == request.IdEmployee && r.IdRequestStatus < 3))
                                               .ToListAsync();

            if (RequestOverlapCheck(listOfOvertimeRequests, request) == true)
            {
                return View("OverlappingRequests");
            }

            if (ModelState.IsValid)
            {
                _context.Add(request);

                await _context.SaveChangesAsync();
                return RedirectToAction("ListRequest", new { id = request.IdEmployee });
            }

            return View(request);
        }


        [HttpGet]
        public IActionResult CreateTakeOvertimeRequest()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateTakeOvertimeRequest(Request request)
        {

            request.RequestNumber = _context.Request.Max(r => r.IdRequest) + 1001;
            request.RequestDate = DateTime.Today;
            request.IdRequestType = 3;
            request.EndDate = request.StartDate;

            //Sprawdzanie czy pracownik ma managera. Jeśli nie, jego wniosek będzie automatycznie akceptowany.
            if (UserWithoutManagerCheck(request.IdEmployee))
            {
                request.IdRequestStatus = 2;
            }
            else
            {
                request.IdRequestStatus = 1;
            }


            //Sprawdzanie czy pracownik ma wystarczająco nadgodzin
            var currentOvertime = _context.Overtime
                                          .Where(o => (o.IdEmployee == request.IdEmployee))
                                          .OrderByDescending(o => o.ToBeSettledBefore)
                                          .First()
                                          .Quantity;

            if (request.Quantity > currentOvertime)
            {
                return View("TooMuchOvertimeRequested");
            }

            //Sprawdzanie czy wnioski nadgodzin na siebie nie nachodzą (sprawdzanie z wnioskami o statusie 1 lub 2)

            var listOfOvertimeRequests = await _context.Request
                                               .Where(r => (r.IdEmployee == request.IdEmployee && r.IdRequestStatus < 3))
                                               .ToListAsync();


            if (RequestOverlapCheck(listOfOvertimeRequests, request) == true)
            {
                return View("OverlappingRequests");
            }

            if (ModelState.IsValid)
            {
                _context.Add(request);

                await _context.SaveChangesAsync();
                return RedirectToAction("ListRequest", new { id = request.IdEmployee });
            }

            return View(request);
        }

        public IActionResult TooManyDaysRequested()
        {
            return View();
        }

        public IActionResult OverlappingRequests()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var request = await _context.Request.FindAsync(id);

            request.IdRequestStatus = 4;

            if (request == null)
            {
                return NotFound();
            }
            else
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
                return View();
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.Request.FindAsync(id);

            request.IdRequestStatus = 2;

            if (request == null)
            {
                return NotFound();
            }
            else
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
                return View();
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> DeclineRequest(int id)
        {
            var request = await _context.Request.FindAsync(id);

            request.IdRequestStatus = 3;

            if (request == null)
            {
                return NotFound();
            }
            else
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
                return View();
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> DeclineReason(int id)
        {
            var request = await _context.Request.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }
            else
            {
                return View(request);
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> DeclineReason(int id, Request request)
        {

            if (id != request.IdRequest)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (request == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DeclineRequest", new { id = request.IdRequest});
            }
            return View(request);
        }

        //------------------------METODY POMOCNICZE

        public static double GetBusinessDays(DateTime startD, DateTime endD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if (endD.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
            if (startD.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

            return calcBusinessDays;

        }

        public static bool RequestOverlapCheck(List<Request> requestList, Request request)
        {
            foreach (Request r in requestList)
            {

                bool overlap = request.StartDate <= r.EndDate && r.StartDate <= request.EndDate;

                if (overlap == true)
                {
                    return true;
                }
            }

            return false;
        }

        public bool UserWithoutManagerCheck(string id)
        {
            var usersWithoutManager =   userManager.Users
                                               .Where(e => e.IdManager == null)
                                               .Select(e => e.Id)
                                               .ToList();

            if (usersWithoutManager.Contains(id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}