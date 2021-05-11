using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;
using Streamia.Realtime;

namespace Streamia.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MovieStatusController : ControllerBase
    {
        private readonly IRepository<Movie> movieRepository;
        private readonly IHubContext<MovieStatusHub> hub;

        public MovieStatusController(IRepository<Movie> movieRepository, IHubContext<MovieStatusHub> hub)
        {
            this.movieRepository = movieRepository;
            this.hub = hub;
        }

        [Route("edit/{id}/{state}")]
        public async Task<IActionResult> Edit(int id, StreamState state)
        {
            var movie = await movieRepository.GetById(id);
            if (movie != null)
            {
                movie.State = state;
                await movieRepository.Edit(movie);
                await hub.Clients.All.SendAsync("UpdateSignal", new { id, state = (int) state });
            }
            return Ok();
        }
    }
}
