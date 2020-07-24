using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class RequestStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRequestStatus { get; set; }
        public string StatusName { get; set; }

        public ICollection<Request> Request { get; set; }
    }
}
