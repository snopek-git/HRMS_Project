using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class ContractBenefit
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdBenefitContract { get; set; }
        public int IdBenefit { get; set; }
        public int IdContract { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Benefit Benefit { get; set; }
        public Contract Contract { get; set; }
    }
}
