﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            Request = new HashSet<Request>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAbsenceType { get; set; }

        [Required (ErrorMessage = "To pole jest wymagane")]
        public string AbsenceTypeName { get; set; }

        public virtual ICollection<AvailableAbsence> AvailableAbsence { get; set; }

        public virtual ICollection<Request> Request { get; set; }
    }
}
