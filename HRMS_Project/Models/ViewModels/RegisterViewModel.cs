using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "To pole jest wymagane")]
        [EmailAddress]
        [Display(Name = "Adres E-mail:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [StringLength(100, ErrorMessage = "Pole {0} musi posiadać przynajmniej {2} i maksymalnie {1} znaków", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło:")]
        public string Password { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło:")]
        [Compare("Password", ErrorMessage = "Hasła do siebie nie pasują.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [Display(Name = "Pierwsze imie:")]
        public string FirstName { get; set; }

        [Display(Name = "Drugie imie:")]
        public string SecondName { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [Display(Name = "Nazwisko:")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [Display(Name = "PESEL:")]
        public string Pesel { get; set; }

        [Required(ErrorMessage = "To pole jest wymagane")]
        [DataType(DataType.Date, ErrorMessage = "Niepoprawna data")]
        [Display(Name = "Data urodzenia:")]
        public DateTime BDate { get; set; }

        [Display(Name = "Numer telefonu:")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Numer karty ID:")]
        public string IdCardNumber { get; set; }

        [Display(Name = "Stanowisko:")]
        public int IdJob { get; set; }

        [Display(Name = "Manager:")]
        public int IdManager { get; set; }

        [Display(Name = "Uprawnienia:")]
        public string IdRole { get; set; }

    }
}
