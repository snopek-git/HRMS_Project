using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Project.Models
{
    public partial class Employee : IdentityUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEmployee { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public string IdCardNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int IdJob { get; set; }
        public int? IdManager { get; set; }
        public bool IsActive { get; set; }
    }
}
