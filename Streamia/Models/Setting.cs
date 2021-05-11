using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class Setting : BaseEntity
    {
        [Display(Name = "Point Price")]
        [Required]
        public decimal PointPrice { get; set; }

        [Required]
        [Range(1, uint.MaxValue)]
        [Display(
            Name = "Points Per Created User", 
            Description = "How many points to be charged from reseller credit for each user reseller creates, EX: 10 points per created user"
        )]
        public uint PointsPerCreatedUser { get; set; }

        [Required]
        [Display(Name = "PayPal Client Id")]
        public string PayPalClientId { get; set; }
    }
}
