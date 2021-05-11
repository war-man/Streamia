using Streamia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class BaseEntity : IEntity
    {
        public int Id { get; set; }
    }
}
