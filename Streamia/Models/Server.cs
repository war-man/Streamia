using Streamia.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class Server : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Ip { get; set; }

        [Required]
        [Display(Name = "Root Password")]
        public string RootPassword { get; set; }

        [Required]
        [Display(Name = "Max Clients")]
        public uint MaxClients { get; set; } = 1;

        [Display(Name = "SSH Port")]
        public uint SshPort { get; set; } = 22;

        public ServerState ServerState { get; set; } = ServerState.Configuring;

        public ICollection<StreamServer> StreamServers { get; set; }

        public ICollection<MovieServer> MovieServers { get; set; }

        public ICollection<SeriesServer> SeriesServers { get; set; }

        public ICollection<ChannelServer> ChannelServers { get; set; }
    }
}
