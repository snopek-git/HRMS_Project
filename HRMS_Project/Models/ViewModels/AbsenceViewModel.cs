﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class AbsenceViewModel
    {

        public List<AvailableAbsence> AvailableAbsence { get; set; }
        public List<AbsenceType> AbsenceType { get; set; }

    }
}
