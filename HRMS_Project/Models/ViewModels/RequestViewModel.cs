using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class RequestViewModel
    {
        public List<Request> Request { get; set; }
        public List<RequestType> RequestType { get; set; }
        public List<RequestStatus> RequestStatus { get; set; }
    }
}
