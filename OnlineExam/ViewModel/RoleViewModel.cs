using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineExam.ViewModel
{
    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string RoleName { get; set; }
    }
}