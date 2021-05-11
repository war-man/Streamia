using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Renci.SshNet;
using Streamia.Helpers;
using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;

namespace Streamia.Controllers
{
    public class SeriesesController : Controller
    {
        private readonly IRepository<Series> seriesRepository;
        private readonly IRepository<Bouquet> bouquetRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<Server> serverRepository;
        private readonly IRepository<SeriesServer> seriesServerRepository;
        private readonly IRepository<BouquetSeries> bouquetSeriesRepository;
        private readonly IRepository<Transcode> transcodeRepository;

        public SeriesesController(
            IRepository<Series> seriesRepository,
            IRepository<Bouquet> bouquetRepository,
            IRepository<Category> categoryRepository,
            IRepository<Server> serverRepository,
            IRepository<SeriesServer> seriesServerRepository,
            IRepository<BouquetSeries> bouquetSeriesRepository,
            IRepository<Transcode> transcodeRepository
        )
        {
            this.seriesRepository = seriesRepository;
            this.bouquetRepository = bouquetRepository;
            this.categoryRepository = categoryRepository;
            this.serverRepository = serverRepository;
            this.seriesServerRepository = seriesServerRepository;
            this.bouquetSeriesRepository = bouquetSeriesRepository;
            this.transcodeRepository = transcodeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            Series model = new Series();
            await PrepareViewBag();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Series model)
        {
            if (ModelState.IsValid)
            {
                if (model.ServerId == 0)
                {
                    ModelState.AddModelError(string.Empty, "Please browse at least 1 Episode");
                    await PrepareViewBag();
                    return View(model);
                }

                foreach (var bouquetId in model.BouquetIds)
                {
                    model.BouquetSeries.Add(new BouquetSeries
                    {
                        BouquetId = bouquetId,
                        SeriesId = model.Id
                    });
                }

                model.SeriesServers.Add(new SeriesServer
                {
                    SeriesId = model.Id,
                    ServerId = model.ServerId
                });

                for (int i = 0; i < model.Episodes.Count; i++)
                {
                    model.Episodes[i].SeriesId = model.Id;
                }

                model.SourceCount = model.Episodes.Count;

                await seriesRepository.Add(model);

                var transcodeProfile = await transcodeRepository.GetById((int) model.TranscodeId);
                var server = await serverRepository.GetById(model.ServerId);
                var host = $"{Request.Scheme}://{Request.Host}";
                var callbackUrl = $"{host}/api/moviestatus/edit/{model.Id}/STATE";
                ThreadPool.QueueUserWorkItem(queue => Transcode(model, transcodeProfile, server, callbackUrl));

                return RedirectToAction(nameof(Manage));
            }
            await PrepareViewBag();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await seriesServerRepository.Delete(m => m.SeriesId == id);
            await bouquetSeriesRepository.Delete(m => m.SeriesId == id);
            await seriesRepository.Delete(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var model = await seriesRepository.GetAll(new string[] { "Category" });
            return View(model);
        }

        private async Task PrepareViewBag()
        {
            ViewBag.Categories = await categoryRepository.Search(m => m.CategoryType == CategoryType.Serieses);
            ViewBag.Servers = await serverRepository.Search(m => m.ServerState == ServerState.Online);
            ViewBag.Bouquets = await bouquetRepository.GetAll();
            ViewBag.TranscodeProfiles = await transcodeRepository.GetAll();
        }

        private async void Transcode(Series series, Transcode transcodeProfile, Server server, string callbackUrl)
        {
            var client = new SshClient(server.Ip, "root", server.RootPassword);
            try
            {
                string successCallbackUrl = callbackUrl.Replace("STATE", string.Empty);
                string streamDirectory = $"/var/hls/{series.StreamKey}";
                var options = new Dictionary<string, string>
                {
                    { "hls_time", "4" },
                    { "hls_playlist_type", "vod" }
                };

                string prepareCommand = $"mkdir {streamDirectory}";
                prepareCommand += $" && cd {streamDirectory}";
                prepareCommand += $" && mkdir 1080p 720p 480p 360p";

                client.Connect();
                client.RunCommand(prepareCommand);

                foreach (var episode in series.Episodes)
                {
                    string transcoder = FFMPEGCommand.MakeCommand(transcodeProfile, episode.Source, streamDirectory, options);
                    var cmd = client.CreateCommand($"nohup sh -c '{transcoder} && curl -i -X GET {successCallbackUrl}' >/dev/null 2>&1 & echo $!");
                    var result = cmd.Execute();
                    int pid = int.Parse(result);
                    client.RunCommand($"disown -h {pid}");
                }

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
