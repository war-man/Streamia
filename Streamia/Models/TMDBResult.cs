using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models
{
    public class TMDBResult : StreamBase
    {
        [Required]
        public string Name { get; set; }
        public string Overview { get; set; }

        [Display(Name = "Poster URL")]
        public virtual string PosterUrl { get; set; }
        public virtual string Cast { get; set; }
        public virtual string Director { get; set; }
        public virtual string Gener { get; set; }

        [Display(Name = "Release Date")]
        public string ReleaseDate { get; set; }
        public virtual int Runtime { get; set; } = 0;
        public float Rating { get; set; } = 0;
    }
}
