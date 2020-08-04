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

            var model = new RequestViewModel
            {
                Request = request,
                RequestType = requestType,
                RequestStatus = requestStatus
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PendingRequest(int id)
        {

            var employeeList = await userManager.Users.Where(e => e.IdManager == id).ToListAsync();

            var idList = new List<string>();

            foreach (var user in employeeList)
            {
                idList.Add(user.Id);
            }

            var request = await _context.Request
                                   .Where(r => idList.Contains(r.IdEmployee))
                                   .ToListAsync();

            var requestType = await _context.RequestType
                                        .ToListAsync();

            var requestStatus = await _context.RequestStatus
                                        .ToListAsync();

            var model = new RequestViewModel
            {
                Request = request,
                RequestType = requestType,
                RequestStatus = requestStatus
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

            var model = new RequestDetailsViewModel
            {
                Request = request,
                RequestStatus = requestStatus,
                RequestType = requestType,
                Manager = manager
                
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

            var model = new RequestDetailsViewModel
            {
                Request = request,
                RequestStatus = requestStatus,
                RequestType = requestType,
                Manager = manager

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

    }
}