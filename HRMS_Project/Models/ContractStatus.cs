using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class ContractStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdContractStatus { get; set; }
        public string StatusName { get; set; }

        public ICollection<Contract> Contract { get; set; }
    }
}
