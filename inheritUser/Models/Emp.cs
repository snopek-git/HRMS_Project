using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace inheritUser.Models
{
    public partial class Emp : IdentityUser
    {
        public int IdEmployee { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public string IdCardNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int? IdManager { get; set; }
        public bool IsActive { get; set; }
    }
}
