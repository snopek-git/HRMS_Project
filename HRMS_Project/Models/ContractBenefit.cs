using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class ContractBenefit
    {
        public int IdBenefitContract { get; set; }
        public int IdBenefit { get; set; }
        public int IdContract { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Benefit Benefit { get; set; }
        public Contract Contract { get; set; }
    }
}
