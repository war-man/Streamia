using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Streamia.Models;
using Streamia.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Models.Contexts
{
    public class StreamiaContext : IdentityDbContext<AppUser>
    {
        public StreamiaContext (DbContextOptions<StreamiaContext> options) : base(options)
        {
        }

        public DbSet<Server> Servers { get; set; }

        public DbSet<AppUser> AdminUsers { get; set; }

        public DbSet<Stream> Streams { get; set; }

        public DbSet<Bouquet> Bouquets { get; set; }

        public DbSet<IptvUser> IptvUsers { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Channel> Channels { get; set; }

        public DbSet<Series> Serieses { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<StreamServer> StreamServers { get; set; }

        public DbSet<MovieServer> MovieServers { get; set; }

        public DbSet<SeriesServer> SeriesServers { get; set; }

        public DbSet<BouquetStream> BouquetStreams { get; set; }

        public DbSet<BouquetMovie> BouquetMovies { get; set; }

        public DbSet<BouquetSeries> BouquetSeries { get; set; }

        public DbSet<ResellerBouquet> ResellerBouquets { get; set; }

        public DbSet<Transcode> Transcodes { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Recharge> Recharges { get; set; }

        public DbSet<Case> Cases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigRelationships();
            modelBuilder.SeedSetting();
            modelBuilder.SeedCategories();
            modelBuilder.SeedTranscodes();
        }

    }
}
