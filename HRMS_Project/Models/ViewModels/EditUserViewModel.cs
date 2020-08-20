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

        [EmailAddress]
        [Required(ErrorMessage = "To pole jest wymagane")]
        public string Email { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        public string FirstName { get; set; }

        public string SecondName { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        public string Pesel { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime BDate { get; set; }

        public string PhoneNumber { get; set; }

        public string IdCardNumber { get; set; }

        public int IdJob { get; set; }

        public string IdManager { get; set; }

        public int IdEmployee { get; set; }

    }
}
