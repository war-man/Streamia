using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Realtime.Interfaces
{
    public interface IRemoteConnection
    {
        public IDictionary<string, SshClient> ConnectionsList { get; set; }
    }
}
