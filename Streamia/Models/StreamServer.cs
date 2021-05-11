using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class StreamServer : BaseEntity
    {
        public int StreamId { get; set; }
        public int ServerId { get; set; }
        public int Pid { get; set; } = 0;
        public Stream Stream { get; set; }
        public Server Server { get; set; }
    }
}
