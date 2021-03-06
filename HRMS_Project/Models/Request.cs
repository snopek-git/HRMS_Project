﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models
{
    public class Request
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRequest { get; set; }
        public int RequestNumber { get; set; } //tworzony automatycznie

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime RequestDate { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

        public int IdRequestType { get; set; }
        
        public int IdRequestStatus { get; set; }

        public string IdEmployee { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane!")]
        public int Quantity { get; set; } //obliczane automatycznie: EndDate - StartDate

        public string EmployeeComment { get; set; }

        public string ManagerComment { get; set; }

        public int? AbsenceTypeRef { get; set; }

        public RequestType IdRequestTypeNavigation { get; set; }
        public RequestStatus IdRequestStatusNavigation { get; set; }
        public Employee IdEmployeeNavigation { get; set; }

        [ForeignKey("AbsenceTypeRef")]
        public AbsenceType IdAbsenceTypeNavigation { get; set; }
    }
}
