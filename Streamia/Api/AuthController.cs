using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;

namespace Streamia.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<IptvUser> iptvUserRepository;
        private readonly IRepository<Stream> streamRepository;
        private readonly IRepository<Movie> movieRepository;

        public AuthController(
            IRepository<IptvUser> iptvUserRepository,
            IRepository<Stream> streamRepository,
            IRepository<Movie> movieRepository
        )
        {
            this.iptvUserRepository = iptvUserRepository;
            this.streamRepository = streamRepository;
            this.movieRepository = movieRepository;
        }

        [Route("authenticate/{username}/{password}/{categoryType}/{sourceId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(string username, string password, CategoryType categoryType, int sourceId)
        {
            // we should activate user subscriotion on first use
            // we should monitor user connection
            // we should check if user subscription is still running
            // we should load balance connections <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            if (!await iptvUserRepository.Exists(m => m.Username.Equals(username) && m.Password.Equals(password)))
            {
                return NotFound();
            }
            string url = null;
            if (categoryType == CategoryType.LiveStreams)
            {
                var stream = await streamRepository.GetById(sourceId, new string[] { "StreamServers", "StreamServers.Server" });
                foreach (var server in stream.StreamServers)
                {
                    url = $"http://{server.Server.Ip}/hls/{stream.StreamKey}/index.m3u8";
                    break;
                }
            } 
            else if (categoryType == CategoryType.Movies)
            {
                var movie = await movieRepository.GetById(sourceId, new string[] { "MovieServers", "MovieServers.Server" });
                if (movie.TranscodeId == 0)
                {
                    return Redirect(movie.Source);
                }
                foreach (var server in movie.MovieServers)
                {
                    url = $"http://{server.Server.Ip}/hls/{movie.StreamKey}/index.m3u8";
                    break;
                }
            }
            return Redirect(url);
        }
    }
}
