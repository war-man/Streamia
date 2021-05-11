using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class ChannelServer : BaseEntity
    {
        public int ChannelId { get; set; }
        public int ServerId { get; set; }
        public int Pid { get; set; } = 0;
        public Channel Channel { get; set; }
        public Server Server { get; set; }
    }
}
