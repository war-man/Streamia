using Streamia.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class Stream : StreamBase
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Source { get; set; }

        [Display(Name = "Enable PTS")]
        public bool EnablePts { get; set; } = true;

        [Display(Name = "Enable RTMP")]
        public bool EnableRtmp { get; set; }

        [Display(Name = "Record?")]
        public bool Record { get; set; }

        [Display(Name = "Start now")]
        public bool StartNow { get; set; }

        [Display(Name = "Delay in minutes")]
        public int Delay { get; set; } = 0;

        public ICollection<StreamServer> StreamServers { get; set; }
        public ICollection<BouquetStream> BouquetStreams { get; set; }

        [NotMapped]
        [Required]
        [Display(Name = "Servers")]
        public List<int> ServerIds { get; set; }

        public Stream()
        {
            State = StreamState.Stopped;
            StreamServers = new List<StreamServer>();
            BouquetStreams = new List<BouquetStream>();
        }
    }
}
