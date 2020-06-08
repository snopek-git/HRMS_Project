using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class ContractType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdContractType { get; set; }
        public string ContractTypeName { get; set; }

        public ICollection<Contract> Contract { get; set; }
    }
}
