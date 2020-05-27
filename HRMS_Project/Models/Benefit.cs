using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class Benefit
    {
        public int IdBenefit { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public ICollection<ContractBenefit> ContractBenefit { get; set; }
    }
}
