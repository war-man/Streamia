using Microsoft.EntityFrameworkCore;
using Streamia.Models.Contexts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models.Validation
{
    public class UsernameUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (StreamiaContext) validationContext.GetService(typeof(StreamiaContext));
            var entity = context.IptvUsers.SingleOrDefault(m => m.Username == value.ToString());
            if (entity != null)
            {
                return new ValidationResult($"{value} already exists");
            }
            return ValidationResult.Success;
        }
    }
}
