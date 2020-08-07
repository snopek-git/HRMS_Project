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
    public class ReportLastMonthPaymentController : ControllerBase
    {
        private IConverter _converter;
        private readonly ApplicationDbContext _context;

        public ReportLastMonthPaymentController(IConverter converter, ApplicationDbContext context)
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
                DocumentTitle = "Wypłaty wynagrodzeń za ostatni miesiąc",
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
            var query = from c in _context.Contract
                        where c.IdContractStatus == 1
                        select new
                        {
                            c.Salary,
                            IdEmployee = (from e in _context.Employee
                                          where (e.Id == c.IdEmployee)
                                          select e.Id).FirstOrDefault(),
                            Employee = (from e in _context.Employee
                                        where (e.Id == c.IdEmployee)
                                        select e.LastName + ' ' + e.FirstName).FirstOrDefault()
                        };

            var LastMonthPayments = new List<ReportLastMonthPaymentViewModel>();

            foreach (var item in query)
            {
                LastMonthPayments.Add(new ReportLastMonthPaymentViewModel
                {
                    IdEmployee = item.IdEmployee,
                    Employee = item.Employee,
                    Salary = item.Salary
                });
            }

            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>Raport urlopowy wszystkich pracowników</h1></div>
                                <table align='center'>
                                    <tr>
                                        <th>Pracownik</th>
                                        <th>Wynagrodzenie Brutto</th>
                                        <th>Dodatki</th>
                                        <th>Wypłacona kwota</th>
                                    </tr>");

            foreach (var p in LastMonthPayments)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                  </tr>", p.Employee, p.Salary, GetBenefitsValue(p.IdEmployee), GetPaymentValue(p.Salary, p.IdEmployee));
            }

            sb.Append(@"
                                </table>
                            </body>
                        </html>");

            return sb.ToString();
        }

        public decimal GetBenefitsValue(string IdEmployee)
        {
            var benefitsValue = 0.0M;

            var contract = from c in _context.Contract
                           where c.IdEmployee == IdEmployee && c.IdContractStatus == 1
                           select c.IdContract;

            var benefits = from b in _context.Benefit
                           select new
                           {
                               b.IdBenefit,
                               b.Name,
                               b.Price,
                               IsSelected = ((from cb in _context.ContractBenefit
                                              where (cb.IdContract == contract.FirstOrDefault()) & (cb.IdBenefit == b.IdBenefit)
                                              select cb).Count() > 0)
                           };

            foreach (var item in benefits)
            {
                if (item.IsSelected == true)
                {
                    benefitsValue += item.Price;
                }
            }

            return benefitsValue;
        }

        public decimal GetPaymentValue(decimal salary, string IdEmployee)
        {
            var sklEmeryt = Decimal.Multiply(salary, 0.0976M); //1
            var sklRent = Decimal.Multiply(salary, 0.015M); //2
            var sklChor = Decimal.Multiply(salary, 0.0245M); //3
            var sumSklUbezp = sklEmeryt + sklRent + sklChor; //4
            var podstWymSklUbezpZdr = salary - sumSklUbezp; //5
            var sklZdrZUS = Decimal.Multiply(podstWymSklUbezpZdr, 0.09M); //6
            var sklZdrPodlOdliczOdZalPodatekDoch = Decimal.Multiply(podstWymSklUbezpZdr, 0.0775M); //7
            decimal podstKosztyUzyskPrzychodu = 111.25M; //8
            var podstawaOpodatkowania = podstWymSklUbezpZdr - podstKosztyUzyskPrzychodu; //9
            var zaliczkaNaPodatekDochodowy = Decimal.Multiply(podstawaOpodatkowania, 0.19M) - 48.9M; //10
            var zaliczkaNaPodatekDochodowyDoUS = Decimal.Round(zaliczkaNaPodatekDochodowy - sklZdrPodlOdliczOdZalPodatekDoch); //11
            decimal benefitsValue = GetBenefitsValue(IdEmployee); //12
            var NettoSalary = salary - sumSklUbezp - zaliczkaNaPodatekDochodowyDoUS - sklZdrZUS - benefitsValue; //13

            return NettoSalary;
        }
    }
}