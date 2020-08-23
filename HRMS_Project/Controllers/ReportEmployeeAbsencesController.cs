using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using HRMS_Project.Data;
using HRMS_Project.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportEmployeeAbsencesController : ControllerBase
    {
        private IConverter _converter;
        private readonly ApplicationDbContext _context;

        public ReportEmployeeAbsencesController(IConverter converter, ApplicationDbContext context)
        {
            _converter = converter;
            _context = context;
        }

        [HttpGet]
        public IActionResult CreateReport(string id)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "Raport urlopowy",
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GetHTMLString(id),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css", "ReportStyle.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Strona [page] z [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "2020 - HRMS_Project" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };


            var file = _converter.Convert(pdf);

            return File(file, "application/pdf");
        }

        public string GetHTMLString(string IdEmployee)
        {
            var absences = from r in _context.Request
                           orderby r.StartDate, r.EndDate
                           where r.IdEmployee == IdEmployee && r.IdRequestStatus == 2
                           select new
                           {
                               r.StartDate,
                               r.EndDate,
                               Status = (from s in _context.RequestStatus
                                         where (s.IdRequestStatus == r.IdRequestStatus)
                                         select s.StatusName).FirstOrDefault(),
                               Type = (from rt in _context.RequestType
                                       where (rt.IdRequestType == r.IdRequestType)
                                       select rt.RequestTypeName).FirstOrDefault()
                           };

            var requestReport = new List<ReportRequestViewModel>();

            foreach (var item in absences)
            {
                requestReport.Add(new ReportRequestViewModel
                {
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Status = item.Status,
                    Type = item.Type
                });
            }

            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header' align='center'><h1>Raport urlopowy</h1></div>
                                <table align='center'>
                                    <tr>
                                        <th>Rodzaj urlopu</th>
                                        <th>Początek</th>
                                        <th>Koniec</th>
                                        <th>Status</th>
                                    </tr>");

            foreach (var r in requestReport)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                  </tr>", r.Type, r.StartDate.ToString("yyyy-MM-dd"), r.EndDate.ToString("yyyy-MM-dd"), r.Status);
            }

            sb.Append(@"
                                </table>
                            </body>
                        </html>");

            return sb.ToString();
        }
    }
}