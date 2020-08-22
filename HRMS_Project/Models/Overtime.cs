using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class Overtime
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdOvertime { get; set; }

        public string IdEmployee { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        public int Quantity { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ToBeSettledBefore { get; set; }



        public virtual Employee IdEmployeeNavigation { get; set; }
    }
}
