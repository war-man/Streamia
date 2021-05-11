using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class ResellerBouquet : BaseEntity
    {
        public string ResellerId { get; set; }
        public int BouquetId { get; set; }
        public AppUser Reseller { get; set; }
        public Bouquet Bouquet { get; set; }
    }
}
