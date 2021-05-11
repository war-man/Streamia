using Streamia.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class Movie : TMDBResult
    {
        [Required]
        public string Source { get; set; }

        [NotMapped]
        public int ServerId { get; set; } = 0;

        [NotMapped]
        [Display(Name = "Servers")]
        public List<int> ServerIds { get; set; }

        public ICollection<MovieServer> MovieServers { get; set; }

        public ICollection<BouquetMovie> BouquetMovies { get; set; }

        public Movie()
        {
            State = StreamState.Transcoding;
            ServerIds = new List<int>();
            MovieServers = new List<MovieServer>();
            BouquetMovies = new List<BouquetMovie>();
        }

    }
}
