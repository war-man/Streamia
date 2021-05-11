using Renci.SshNet;
using Streamia.Realtime.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Realtime.Containers
{
    public class SshContainer : IRemoteConnection
    {
        public IDictionary<string, SshClient> ConnectionsList { get; set; }

        public SshContainer()
        {
            ConnectionsList = new Dictionary<string, SshClient>();
        }

    }
}
