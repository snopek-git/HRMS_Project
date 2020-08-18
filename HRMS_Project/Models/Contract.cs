using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class Contract
    {
        public Contract()
        {
            ContractBenefit = new HashSet<ContractBenefit>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdContract { get; set; }
        public int ContractNumber { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime ContractStart { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
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
        public Employee IdEmployeeNavigation { get; set; }

        //Benefity
        public ICollection<ContractBenefit> ContractBenefit { get; set; }
    }
}
