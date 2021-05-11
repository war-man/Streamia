using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class MovieServer : BaseEntity
    {
        public int MovieId { get; set; }
        public int ServerId { get; set; }
        public int Pid { get; set; } = 0;
        public Movie Movie { get; set; }
        public Server Server { get; set; }
    }
}
