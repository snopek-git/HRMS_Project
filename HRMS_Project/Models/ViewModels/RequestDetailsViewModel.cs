using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class RequestDetailsViewModel
    {
        public Request Request { get; set; }
        public RequestType RequestType { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public Employee Manager { get; set; }
        public AbsenceType AbsenceType { get; set; }
        public List<AvailableAbsence> AvailableAbsence { get; set; }
        public Overtime Overtime { get; set; }
    }
}
