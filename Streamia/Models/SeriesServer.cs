using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class SeriesServer : BaseEntity
    {
        public int SeriesId { get; set; }
        public int ServerId { get; set; }
        public int Pid { get; set; } = 0;
        public Series Series { get; set; }
        public Server Server { get; set; }
    }
}
