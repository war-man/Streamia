using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models.ViewModels
{
    public class RechargeViewModel
    {
        [Required]
        [Display(Name = "How many points do you need")]
        public uint Points { get; set; }
        public string TransactionId { get; set; }
        public Setting Setting { get; set; }
    }
}
