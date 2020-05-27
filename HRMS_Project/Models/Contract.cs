using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class Contract
    {
        //public Contract()
        //{
        //    ContractBenefit = new HashSet<ContractBenefit>();
        //}

        public int IdContract { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContractNumber { get; set; }
        public decimal Salary { get; set; }
        public DateTime ContractStart { get; set; }
        public DateTime ContractEnd { get; set; }
        public int IdContractType { get; set; }
        public int IdContractStatus { get; set; }
        public ICollection<ContractBenefit> ContractBenefit { get; set; }
        public string IdEmployee { get; set; } //Odwolanie do kolumn ID z tabeli AspNetUsers
        public Employee IdEmployeeNavigation { get; set; }
    }
}
