using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class RequestType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRequestType { get; set; }
        public string RequestTypeName { get; set; }

        public ICollection<Request> Request { get; set; }
    }
}
