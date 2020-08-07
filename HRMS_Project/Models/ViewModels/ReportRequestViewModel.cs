using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class ReportRequestViewModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Employee;
        public string Status;
        public string Type;
    }
}
