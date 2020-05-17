using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasła do siebie nie pasują.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Pierwsze imie")]
        public string FirstName { get; set; }

        [Display(Name = "Drugie imie")]
        public string SecondName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "PESEL")]
        public string Pesel { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data urodzenia")]
        public DateTime BDate { get; set; }

        [Display(Name = "Numer telefonu")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Numer karty ID")]
        public string IdCardNumber { get; set; }

        [Display(Name = "Stanowisko")]
        public int IdJob { get; set; }

        [Display(Name = "Manager")]
        public int IdManager { get; set; }

        [Display(Name = "Uprawnienia")]
        public int IdRole { get; set; }

    }
}
