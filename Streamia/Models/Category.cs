using Streamia.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [Display(Name = "Type")]
        public CategoryType CategoryType { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
