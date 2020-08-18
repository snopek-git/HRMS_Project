using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using HRMS_Project.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportAllEmployeesController : ControllerBase
    {
        private IConverter _converter;
        private readonly ApplicationDbContext _context;

        public ReportAllEmployeesController(IConverter converter, ApplicationDbContext context)
        {
            _converter = converter;
            _context = context;
        }

        [HttpGet]
        public IActionResult CreateReport()
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "Raport wszystkich pracowników firmy.",
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GetHTMLString(),
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

        public string GetHTMLString()
        {
            var employees = _context.Employee.OrderBy(x => x.LastName);

            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>Raport wszystkich pracowników firmy</h1></div>
                                <table align='center'>
                                    <tr>
                                        <th>Imie</th>
                                        <th>Nazwisko</th>
                                        <th>Data urodzenia</th>
                                        <th>Email</th>
                                    </tr>");

            foreach (var emp in employees)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                  </tr>", emp.FirstName, emp.LastName, emp.BirthDate, emp.Email);
            }

            sb.Append(@"
                                </table>
                            </body>
                        </html>");

            return sb.ToString();
        }
    }
}