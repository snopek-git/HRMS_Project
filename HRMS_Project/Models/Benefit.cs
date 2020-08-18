using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class Benefit
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdBenefit { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        public string Name { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        public decimal Price { get; set; }

        public ICollection<ContractBenefit> ContractBenefit { get; set; }
    }
}
