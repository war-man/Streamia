using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords don't match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Trial Period In Days")]
        public uint TrialDays { get; set; }

        [Display(Name = "Trial Account")]
        public bool TrialAccount { get; set; }

        [Display(Name = "Add MAG Device")]
        public bool AddMag { get; set; }

        [Display(Name = "Add Enigma Device")]
        public bool AddEnigma { get; set; }

        [Display(Name = "Monitor MAG Devices Only")]
        public bool MonitorMagOnly { get; set; }

        [Display(Name = "Monitor Enigma Devices Only")]
        public bool MonitorEnigmaOnly { get; set; }

        [Display(Name = "Lock STB Device")]
        public bool LockSTB { get; set; }
        public bool Restream { get; set; }

        [Required]
        [Display(Name = "Bouquets")]
        public int[] BouquetIds { get; set; }
    }
}
