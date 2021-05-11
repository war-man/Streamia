using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streamia.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Streamia.Models.Extensions
{
    public static class SeedDataExtensions
    {
        public static void SeedUsers(UserManager<AppUser> userManager)
        {
            if (userManager.FindByEmailAsync("dev@streamia.com").Result == null)
            {
                Guid guid = Guid.NewGuid();
                string guidString = Convert.ToBase64String(guid.ToByteArray());
                guidString = guidString.Replace("=", "");
                guidString = guidString.Replace("+", "");

                var user = new AppUser
                {
                    Id = guidString,
                    UserName = "dev@streamia.com",
                    Email = "dev@streamia.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "DEV@STREAMIA.COM",
                    NormalizedUserName = "DEV@STREAMIA.COM",
                    PhoneNumberConfirmed = true,
                    PhoneNumber = "0123456789",

                };

                IdentityResult result = userManager.CreateAsync(user, "Streamia0123456789").Result;

                if (result.Succeeded)
                {
                    userManager.AddClaimAsync(user, new Claim("IsAdmin", "true")).Wait();
                }
            }
        }

        public static void SeedSetting(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasData(new Setting
            {
                Id = 1,
                PointPrice = 0.1m,
                PointsPerCreatedUser = 10,
                PayPalClientId = string.Empty
            });
        }

        public static void SeedTranscodes(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transcode>().HasData(new Transcode { 
                Id = 1, 
                Name = "Basic: H264/AAC",
                Preset = "default",
                VideoCodec = "h264",
                VideoProfile = "none",
                AudioCodec = "aac"
            });
        }

        public static void SeedCategories(this ModelBuilder modelBuilder)
        {
            string[] liveCategories = new string[]
            {
                "Science",
                "Action",
                "News"
            };

            string[] channelCategories = new string[]
            {
                "Science",
                "Action",
                "News",
                "Series",
                "Movies",
                "Documentary"
            };

            string[] moviesCategories = new string[]
            {
                "Action",
                "Adventure",
                "Comedy",
                "Crime",
                "Drama",
                "Fantasy",
                "Historical",
                "Horror",
                "Mystery",
                "Philosophical",
                "Political",
                "Saga",
                "Thriller",
                "Western",
                "Crime Thriller",
                "Disaster Thriller",
                "Psychological Thriller",
                "Techno Thriller",
                "Science Fiction",
                "Suspense",
                "Animation"
            };

            string[] seriesesCategories = new string[]
            {
                "Action",
                "Adventure",
                "Comedy",
                "Crime",
                "Drama",
                "Fantasy",
                "Historical",
                "Horror",
                "Mystery",
                "Philosophical",
                "Political",
                "Saga",
                "Thriller",
                "Western",
                "Crime Thriller",
                "Disaster Thriller",
                "Psychological Thriller",
                "Techno Thriller",
                "Science Fiction",
                "Suspense",
                "Animation",
                "Documentary",
                "Family",
                "Children",
                "Sport"
            };

            int idCounter = 1;
            List<Category> seedList = new List<Category>();

            foreach (string categoryName in liveCategories)
            {
                seedList.Add(new Category { Id = idCounter, CategoryType = CategoryType.LiveStreams, Name = categoryName });
                idCounter++;
            }

            foreach (string categoryName in channelCategories)
            {
                seedList.Add(new Category { Id = idCounter, CategoryType = CategoryType.Channels, Name = categoryName });
                idCounter++;
            }

            foreach (string categoryName in moviesCategories)
            {
                seedList.Add(new Category { Id = idCounter, CategoryType = CategoryType.Movies, Name = categoryName });
                idCounter++;
            }

            foreach (string categoryName in seriesesCategories)
            {
                seedList.Add(new Category { Id = idCounter, CategoryType = CategoryType.Serieses, Name = categoryName });
                idCounter++;
            }

            modelBuilder.Entity<Category>().HasData(seedList);
        }

    }
}
