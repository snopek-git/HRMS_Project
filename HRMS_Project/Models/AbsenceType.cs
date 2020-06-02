using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class AbsenceType
    {
        public AbsenceType()
        {
            AvailableAbsence = new HashSet<AvailableAbsence>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAbsenceType { get; set; }
        public string AbsenceTypeName { get; set; }

        public virtual ICollection<AvailableAbsence> AvailableAbsence { get; set; }
    }
}
