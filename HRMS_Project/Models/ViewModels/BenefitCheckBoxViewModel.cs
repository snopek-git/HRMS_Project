using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class BenefitCheckBoxViewModel
    {
        public int IdBenefit { get; set; }
        public string BenefitName { get; set; }
        public bool IsSelected { get; set; }

    }
}
