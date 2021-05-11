using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
    public class ChannelsController : Controller
    {
        private readonly IRepository<Channel> channelRepository;
        private readonly IRepository<Bouquet> bouquetRepository;
        private readonly IRepository<Server> serverRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<Transcode> transcodeRepository;
        private readonly IRepository<BouquetChannel> bouquetChannelRepository;
        private readonly IRepository<ChannelServer> channelServerRepository;

        public ChannelsController(
            IRepository<Channel> channelRepository,
            IRepository<Bouquet> bouquetRepository,
            IRepository<Server> serverRepository,
            IRepository<Category> categoryRepository,
            IRepository<Transcode> transcodeRepository,
            IRepository<BouquetChannel> bouquetChannelRepository,
            IRepository<ChannelServer> channelServerRepository
        )
        {
            this.channelRepository = channelRepository;
            this.bouquetRepository = bouquetRepository;
            this.serverRepository = serverRepository;
            this.categoryRepository = categoryRepository;
            this.transcodeRepository = transcodeRepository;
            this.bouquetChannelRepository = bouquetChannelRepository;
            this.channelServerRepository = channelServerRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            await PrepareViewBag();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Channel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SourcePath.Length == 0)
                {
                    ModelState.AddModelError("SourcePath", "Please select a folder from one of your servers");
                    await PrepareViewBag();
                    return View(model);
                }

                foreach (var bouquetId in model.BouquetIds)
                {
                    model.BouquetChannels.Add(new BouquetChannel
                    {
                        ChannelId = model.Id,
                        BouquetId = bouquetId
                    });
                }

                model.ChannelServers.Add(new ChannelServer
                {
                    ChannelId = model.Id,
                    ServerId = (int) model.ServerId
                });

                model.SourceCount = model.SourcePath.Length;
                model.State = StreamState.Transcoding;

                await channelRepository.Add(model);

                var transcodeProfile = await transcodeRepository.GetById((int)model.TranscodeId);
                var server = await serverRepository.GetById((int) model.ServerId);
                var host = $"{Request.Scheme}://{Request.Host}";
                var callbackUrl = $"{host}/api/channelstatus/edit/{model.Id}/STATE";
                ThreadPool.QueueUserWorkItem(queue => Transcode(model, transcodeProfile, server, callbackUrl));

                return RedirectToAction(nameof(Manage));
            }

            await PrepareViewBag();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await bouquetChannelRepository.Delete(m => m.ChannelId == id);
            await channelServerRepository.Delete(m => m.ChannelId == id);
            await channelRepository.Delete(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            return View(await channelRepository.GetAll(new string[] { "Category" }));
        }

        private async Task PrepareViewBag()
        {
            ViewBag.Servers = await serverRepository.Search(m => m.ServerState == ServerState.Online);
            ViewBag.Categories = await categoryRepository.Search(m => m.CategoryType == CategoryType.Channels);
            ViewBag.Bouquets = await bouquetRepository.GetAll();
            ViewBag.TranscodeProfiles = await transcodeRepository.GetAll();
        }

        private async void Transcode(Channel channel, Transcode transcodeProfile, Server server, string callbackUrl)
        {
            var client = new SshClient(server.Ip, "root", server.RootPassword);
            try
            {
                string successCallbackUrl = callbackUrl.Replace("/STATE", string.Empty);
                string streamDirectory = $"/var/hls/{channel.StreamKey}";
                string prepareCommand = $"mkdir {streamDirectory}";
                string[] resolutions = new string[]
                {
                    "1080p",
                    "720p",
                    "480p",
                    "360p"
                };

                prepareCommand += $" && cd {streamDirectory}";
                prepareCommand += " && mkdir 1080p 720p 480p 360p sources";
                prepareCommand += " && cd sources";
                prepareCommand += " && mkdir 1080p 720p 480p 360p";
                    
                foreach (string resolution in resolutions)
                {
                    StringBuilder sourceList = new StringBuilder();

                    for (int i = 0; i < channel.SourcePath.Length; i++)
                    {
                        sourceList.Append($"file '{streamDirectory}/sources/{resolution}/{i}.ts'\n");
                    }

                    prepareCommand += $" && printf \"{sourceList}\" > {streamDirectory}/sources/{resolution}/source_list.txt";
                }

                client.Connect();
                client.RunCommand(prepareCommand);

                for (int i = 0; i < channel.SourcePath.Length; i++)
                {
                    var options = new Dictionary<string, string>
                    {
                        { "custom_output", string.Empty },
                        { "custom_output_1080p", $"{streamDirectory}/sources/1080p/{i}.ts" },
                        { "custom_output_720p", $"{streamDirectory}/sources/720p/{i}.ts" },
                        { "custom_output_480p", $"{streamDirectory}/sources/480p/{i}.ts" },
                        { "custom_output_360p", $"{streamDirectory}/sources/360p/{i}.ts" }
                    };
                    string transcoder = FFMPEGCommand.MakeCommand(transcodeProfile, channel.SourcePath[i], string.Empty, options);
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
