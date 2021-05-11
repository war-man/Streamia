using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public uint Credit { get; set; }
        public bool AddMAG { get; set; }
        public bool AddEnigma { get; set; }
        public bool MonitorMagOnly { get; set; }
        public bool MonitorEnigmaOnly { get; set; }
        public bool LockSTB { get; set; }
        public bool Restream { get; set; }
        public bool TrialAccount { get; set; }
        public uint TrialDays { get; set; }
        public DateTime CreationDateTime { get; set; } = DateTime.Now;
        public ICollection<ResellerBouquet> ResellerBouquets { get; set; }
    }
}
