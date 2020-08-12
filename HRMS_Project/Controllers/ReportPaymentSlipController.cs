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
    public class ReportPaymentSlipController : ControllerBase
    {
        private IConverter _converter;
        private readonly ApplicationDbContext _context;

        public ReportPaymentSlipController(IConverter converter, ApplicationDbContext context)
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
                DocumentTitle = "Pasek wynagrodzenia",
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

            var query = from c in _context.Contract
                        where c.IdEmployee == IdEmployee && c.IdContractStatus == 1
                        select c.Salary;

            var salary = query.FirstOrDefault(); //0


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

            var sb = new StringBuilder();
            sb.AppendFormat(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header' align='center'><h1>Pasek wynagrodzenia</h1></div>
                                <table align='center'>
                                    <tr>
                                        <td><b>Wynagrodzenie zasadnicze</b></td>
                                        <td><b>{0}</b></td>
                                    </tr><tr>
                                        <td>Skł. Emerytalna (9.76%)</td>
                                        <td>{1}</td>
                                        <td rowspan='3'>{4}</td>
                                    </tr><tr>
                                        <td>Skł. Rentowa (1.5%)</td>
                                        <td>{2}</td>
                                    </tr><tr>
                                        <td>Skł. Chorobowa (2.45%)</td>
                                        <td>{3}</td>
                                    </tr><tr>
                                        <td>Podstawa wymiaru składki na ubezp. zdrowotne</td>
                                        <td><b>{5}</b></td>
                                    </tr><tr>
                                        <td>Skł. zdrowotna należna do ZUS</td>
                                        <td>{6}</td>
                                    </tr><tr>
                                        <td>Skł. zdrowotna podl. odliczeniu od zal. na podatek dochodowy</td>
                                        <td>{7}</td>
                                    </tr><tr>
                                        <td>Podstawowe koszty uzyskania przychodów</td>
                                        <td>{8}</td>
                                    </tr><tr>
                                        <td>Podstawa opodatkowania</td>
                                        <td><b>{9}</b></td>
                                    </tr><tr>
                                        <td>Zaliczka na podatek dochodowy</td>
                                        <td>{10}</td>
                                    </tr><tr>
                                        <td>Zal. na podatek dochodowy podl. wpłacie do Urzędu Skarb.</td>
                                        <td>{11}</td>
                                    </tr><tr>
                                        <td>Dodatki</td>
                                        <td>{12}</td>
                                    </tr><tr>
                                        <td><b>Wynagrodzenie netto</b></td>
                                        <td><b>{13}</b></td>
                                    </tr>", salary, //0
                                            sklEmeryt.ToString("0.00"), //1
                                            sklRent.ToString("0.00"), //2
                                            sklChor.ToString("0.00"), //3
                                            sumSklUbezp.ToString("0.00"), //4
                                            podstWymSklUbezpZdr.ToString("0.00"), //5
                                            sklZdrZUS.ToString("0.00"), //6
                                            sklZdrPodlOdliczOdZalPodatekDoch.ToString("0.00"), //7
                                            podstKosztyUzyskPrzychodu.ToString("0.00"), //8
                                            podstawaOpodatkowania.ToString("0.00"), //9
                                            zaliczkaNaPodatekDochodowy.ToString("0.00"), //10
                                            zaliczkaNaPodatekDochodowyDoUS.ToString("0.00"), //11
                                            benefitsValue.ToString("0.00"), //12
                                            NettoSalary.ToString("0.00") //13
                                            );

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
    }
}