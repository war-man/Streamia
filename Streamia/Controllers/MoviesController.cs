using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Streamia.Helpers;
using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;

namespace Streamia.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IRepository<Movie> movieRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<Bouquet> bouquetRepository;
        private readonly IRepository<Server> serverRepository;
        private readonly IRepository<MovieServer> movieServerRepository;
        private readonly IRepository<BouquetMovie> bouquetMovieRepository;
        private readonly IRepository<Transcode> transcodeRepository;

        public MoviesController(
            IRepository<Movie> movieRepository, 
            IRepository<Category> categoryRepository,
            IRepository<Bouquet> bouquetRepository,
            IRepository<Server> serverRepository,
            IRepository<MovieServer> movieServerRepository,
            IRepository<BouquetMovie> bouquetMovieRepository,
            IRepository<Transcode> transcodeRepository
        )
        {
            this.movieRepository = movieRepository;
            this.categoryRepository = categoryRepository;
            this.bouquetRepository = bouquetRepository;
            this.serverRepository = serverRepository;
            this.movieServerRepository = movieServerRepository;
            this.bouquetMovieRepository = bouquetMovieRepository;
            this.transcodeRepository = transcodeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            await PrepareViewBag();
            return View(new Movie());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Movie model)
        {
            if (ModelState.IsValid)
            {
                if (model.TranscodeId == 0 && !Uri.IsWellFormedUriString(model.Source, UriKind.Absolute))
                {
                    ModelState.AddModelError("TranscodeId", "Streaming directly is not allowed with local server files");
                    await PrepareViewBag();
                    return View(model);
                }

                foreach (var bouquetId in model.BouquetIds)
                {
                    model.BouquetMovies.Add(new BouquetMovie
                    {
                        BouquetId = bouquetId,
                        MovieId = model.Id
                    });
                }

                if (model.TranscodeId == 0)
                {
                    model.TranscodeId = null;
                    model.State = StreamState.Ready;
                } 
                else
                {
                    if (model.ServerId > 0)
                    {
                        model.ServerIds.Add(model.ServerId);
                    }
                    
                    if (model.ServerIds.Count == 0)
                    {
                        ModelState.AddModelError("ServerIds", "Please select one server to transcode from external link");
                        await PrepareViewBag();
                        return View(model);
                    }

                    foreach (var serverId in model.ServerIds)
                    {
                        model.MovieServers.Add(new MovieServer
                        {
                            MovieId = model.Id,
                            ServerId = serverId
                        });
                    }
                }
                
                await movieRepository.Add(model);

                if (model.TranscodeId > 0)
                {
                    var transcodeProfile = await transcodeRepository.GetById((int) model.TranscodeId);
                    var server = await serverRepository.GetById(model.ServerId);
                    var host = $"{Request.Scheme}://{Request.Host}";
                    var callbackUrl = $"{host}/api/moviestatus/edit/{model.Id}/STATE";
                    ThreadPool.QueueUserWorkItem(queue => Transcode(model, transcodeProfile, server, callbackUrl));
                }

                return RedirectToAction(nameof(Manage));
            }
            await PrepareViewBag();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await movieServerRepository.Delete(m => m.MovieId == id);
            await bouquetMovieRepository.Delete(m => m.MovieId == id);
            await movieRepository.Delete(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var model = await movieRepository.GetAll(new string[] { "Category" });
            return View(model);
        }

        private async Task PrepareViewBag()
        {
            ViewBag.Categories = await categoryRepository.Search(m => m.CategoryType == CategoryType.Movies);
            ViewBag.Servers = await serverRepository.Search(m => m.ServerState == ServerState.Online);
            ViewBag.Bouquets = await bouquetRepository.GetAll();
            ViewBag.TranscodeProfiles = await transcodeRepository.GetAll();
        }

        private async void Transcode(Movie movie, Transcode transcodeProfile, Server server, string callbackUrl)
        {
            var client = new SshClient(server.Ip, "root", server.RootPassword);
            try
            {
                string successCallbackUrl = callbackUrl.Replace("STATE", StreamState.Ready.ToString());
                var options = new Dictionary<string, string>
                {
                    { "hls_time", "4" },
                    { "hls_playlist_type", "vod" }
                };
                string transcoder = FFMPEGCommand.MakeCommand(transcodeProfile, movie.Source, $"/var/hls/{movie.StreamKey}", options);
                string prepareCommand = $"mkdir /var/hls/{movie.StreamKey}";
                prepareCommand += $" && cd /var/hls/{movie.StreamKey}";
                prepareCommand += $" && mkdir 1080p 720p 480p 360p";

                client.Connect();
                client.RunCommand(prepareCommand);
                var cmd = client.CreateCommand($"nohup sh -c '{transcoder} && curl -i -X GET {successCallbackUrl}' >/dev/null 2>&1 & echo $!");
                var result = cmd.Execute();
                int pid = int.Parse(result);
                client.RunCommand($"disown -h {pid}");
                client.Disconnect();
                client.Dispose();
            }
            catch (Exception)
            {
                string failCallbackUrl = callbackUrl.Replace("STATE", StreamState.Error.ToString());
                var httpClient = new HttpClient();
                await httpClient.GetAsync(failCallbackUrl);
            }
        }
    }
}