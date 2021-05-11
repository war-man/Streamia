using Streamia.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class IptvUser : BaseEntity
    {
        [Required]
        [UsernameUnique]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public uint Connections { get; set; }

        [Required]
        [Display(Name = "Subscription duration in days")]
        public uint DaysToExpire { get; set; }

        public DateTime? Expiration { get; set; } = null;

        public string Notes { get; set; }

        [Required]
        [Display(Name = "Bouquet")]
        public int BouquetId { get; set; }

        public bool Banned { get; set; }

        public Bouquet Bouquet { get; set; }
    }
}
