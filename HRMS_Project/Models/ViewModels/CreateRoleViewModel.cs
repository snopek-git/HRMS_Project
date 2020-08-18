using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS_Project.Models.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required (ErrorMessage = "To pole jest wymagane")]
        public string RoleName { get; set; }
    }
}
