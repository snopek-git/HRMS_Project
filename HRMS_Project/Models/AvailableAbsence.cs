using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class AvailableAbsence
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAvailableAbsence { get; set; }
        
        [Required(ErrorMessage = "To pole jest wymagane")]
        public int AvailableDays { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        public int UsedAbsence { get; set; }
        public int IdAbsenceType { get; set; }
        public string IdEmployee { get; set; }

        public virtual AbsenceType IdAbsenceTypeNavigation { get; set; }
        public virtual Employee IdEmployeeNavigation { get; set; }

    }
}
