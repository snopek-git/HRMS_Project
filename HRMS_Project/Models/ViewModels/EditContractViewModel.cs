using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class EditContractViewModel
    {
        /*public int IdContract { get; set; }
        public decimal Salary { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ContractStart { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ContractEnd { get; set; }

        //Typ Umowy
        public int IdContractType { get; set; }
        public ContractType IdContractTypeNavigation { get; set; }

        //Status Umowy
        public int IdContractStatus { get; set; }
        public ContractStatus IdContractStatusNavigation { get; set; }

        //Dla kogo umowa
        public string IdEmployee { get; set; } //Odwolanie do kolumn ID z tabeli AspNetUsers
        public Employee IdEmployeeNavigation { get; set; }*/

        public Contract Contract { get; set; }

        public List<BenefitCheckBoxViewModel> Benefits { get; set; }
    }
}
