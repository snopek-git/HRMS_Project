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
                                        .ToListAsync();

            var requestType = await _context.RequestType
                                        .ToListAsync();

            var requestStatus = await _context.RequestStatus
                                        .ToListAsync();

            var absenceType = await _context.AbsenceType
                                        .ToListAsync();

            var model = new RequestViewModel
            {
                Request = request,
                RequestType = requestType,
                RequestStatus = requestStatus,
                AbsenceType = absenceType
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PendingRequest(string id)
        {

            var employeeList = await userManager.Users.Where(e => e.IdManager == id).OrderBy(x => x.LastName).ToListAsync();

            var idList = new List<string>();

            foreach (var user in employeeList)
            {
                idList.Add(user.Id);
            }


            //dodac warunek o statusie
            var request = await _context.Request
                                   .Where(r => (idList.Contains(r.IdEmployee) && r.IdRequestStatus == 1)) //dodać requestType == 1
                                   .ToListAsync();

            var requestType = await _context.RequestType
                                        .ToListAsync();

            var requestStatus = await _context.RequestStatus
                                        .ToListAsync();

            var absenceType = await _context.AbsenceType
                            .ToListAsync();

            var model = new RequestViewModel
            {
                Request = request,
                RequestType = requestType,
                RequestStatus = requestStatus,
                AbsenceType = absenceType
            };

            return View(model);
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

            var manager = userManager.Users.First(e => e.IdManager == user.IdManager);

            var absenceType = await _context.AbsenceType.FindAsync(request.AbsenceTypeRef);

            var model = new RequestDetailsViewModel
            {
                Request = request,
                RequestStatus = requestStatus,
                RequestType = requestType,
                Manager = manager,
                AbsenceType = absenceType

            };

            return View(model);
        }

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

            var manager = userManager.Users.First(e => e.IdManager == user.IdManager);

            var absenceType = await _context.AbsenceType.FindAsync(request.AbsenceTypeRef);

            var model = new RequestDetailsViewModel
            {
                Request = request,
                RequestStatus = requestStatus,
                RequestType = requestType,
                Manager = manager,
                AbsenceType = absenceType

            };

            return View(model);
        }


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
        public IActionResult CreateRequest()
        {

            ViewData["IdAbsenceType"] = new SelectList(_context.AbsenceType, "IdAbsenceType", "AbsenceTypeName");
            //ViewData["IdAbsenceType"] = new SelectList(_context.AbsenceType)

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRequest(Request request)
        {
            ViewData["IdAbsenceType"] = new SelectList(_context.AbsenceType, "IdAbsenceType", "AbsenceTypeName");


            request.RequestNumber = _context.Request.Max(r => r.IdRequest) + 1001; //id na tym etapie jeszcze nie istnieje, dlatego max + 1001
            request.RequestDate = DateTime.Today;
            request.Quantity = (int)(request.EndDate - request.StartDate).TotalDays + 1;
            request.IdRequestType = 1;
            request.IdRequestStatus = 1;

            var numberOfDays = _context.AvailableAbsence
                                        .Where(a => (a.IdEmployee == request.IdEmployee) && (a.IdAbsenceType == request.AbsenceTypeRef))
                                        .FirstOrDefault()
                                        .AvailableDays;

            //Sprawdzanie czy pracownik ma wystarczająco dostępnych dni urlopowych

            if(numberOfDays - request.Quantity < 0)
            {
                return View("TooManyDaysRequested");
            }

            //Sprawdzanie czy urlopy na siebie nie nachodzą (sprawdzanie z wnioskami o statusie 1 lub 2)

            var listOfRequests = await _context.Request
                                               .Where(r => (r.IdEmployee == request.IdEmployee && r.IdRequestStatus < 3))
                                               .ToListAsync();

            foreach (Request r in listOfRequests)
            {
                
                bool overlap = request.StartDate < r.EndDate && r.StartDate < request.EndDate;
                
                if (overlap == true)
                {
                    return View("OverlappingRequests");
                }
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

    }
}