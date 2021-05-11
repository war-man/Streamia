﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name ="Confirm password")]
        [Compare("Password" , ErrorMessage ="Passwords don't match")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
