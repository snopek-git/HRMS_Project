using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Roles = new List<string>();
        }

        public string Id { get; set; }

        public IList<string> Roles { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string LastName { get; set; }

        public string Pesel { get; set; }

        public DateTime BDate { get; set; }

        public string PhoneNumber { get; set; }

        public string IdCardNumber { get; set; }

        public int IdJob { get; set; }

        //public int IdManager { get; set; }

    }
}
