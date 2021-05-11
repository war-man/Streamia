using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class Recharge : BaseEntity
    {
        public string ResellerId { get; set; }
        public string TransactionId { get; set; }
        public uint Points { get; set; }
        public DateTime TransactionDateTime { get; set; } = DateTime.Now;
        public AppUser Reseller { get; set; }
    }
}
