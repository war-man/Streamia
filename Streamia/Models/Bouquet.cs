using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class Bouquet : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public uint Points { get; set; }

        [NotMapped]
        [Display(Name= "Streams")]
        public int[] StreamIds { get; set; }

        [NotMapped]
        [Display(Name = "Movies")]
        public int[] MovieIds { get; set; }

        [NotMapped]
        [Display(Name = "Serieses")]
        public int[] SeriesIds { get; set; }

        [NotMapped]
        [Display(Name = "Channels")]
        public int[] ChannelIds { get; set; }

        public ICollection<BouquetStream> BouquetStreams { get; set; }
        public ICollection<BouquetMovie> BouquetMovies { get; set; }
        public ICollection<BouquetSeries> BouquetSeries { get; set; }
        public ICollection<BouquetChannel> BouquetChannels { get; set; }
        public ICollection<ResellerBouquet> ResellerBouquets { get; set; }

        public Bouquet()
        {
            BouquetStreams = new List<BouquetStream>();
            BouquetMovies = new List<BouquetMovie>();
            BouquetSeries = new List<BouquetSeries>();
            BouquetChannels = new List<BouquetChannel>();
        }
    }
}
