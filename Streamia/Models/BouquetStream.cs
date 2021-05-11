using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class BouquetStream : BaseEntity
    {
        public int BouquetId { get; set; }
        public int StreamId { get; set; }
        public Bouquet Bouquet { get; set; }
        public Stream Stream { get; set; }
    }
}
