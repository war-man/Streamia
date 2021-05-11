using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Streamia.Models;
using Streamia.Models.Contexts;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;

namespace Streamia.Controllers
{
    public class StreamsController : Controller
    {
        private readonly IRepository<Stream> streamRepository;
        private readonly IRepository<Bouquet> bouquetRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<Server> serverRepository;
        private readonly IRepository<StreamServer> streamServersRepository;
        private readonly IRepository<BouquetStream> bouquetStreamsRepository;
        private readonly IRepository<Transcode> transcodeRepository;

        public StreamsController(
            IRepository<Stream> streamRepository,
            IRepository<Bouquet> bouquetRepository,
            IRepository<Category> categoryRepository,
            IRepository<Server> serverRepository,
            IRepository<StreamServer> streamServersRepository,
            IRepository<BouquetStream> bouquetStreamsRepository,
            IRepository<Transcode> transcodeRepository
        )
        {
            this.streamRepository = streamRepository;
            this.bouquetRepository = bouquetRepository;
            this.categoryRepository = categoryRepository;
            this.serverRepository = serverRepository;
            this.streamServersRepository = streamServersRepository;
            this.bouquetStreamsRepository = bouquetStreamsRepository;
            this.transcodeRepository = transcodeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            Stream model = new Stream();
            await PrepareViewBag();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Stream model)
        {
            if (ModelState.IsValid)
            {
                foreach (var serverId in model.ServerIds)
                {
                    model.StreamServers.Add(new StreamServer { 
                        ServerId = serverId, 
                        StreamId = model.Id
                    });
                }

                foreach (var bouquetId in model.BouquetIds)
                {
                    model.BouquetStreams.Add(new BouquetStream
                    {
                        BouquetId = bouquetId,
                        StreamId = model.Id
                    });
                }

                if (model.TranscodeId == 0)
                {
                    model.TranscodeId = null;
                    model.State = StreamState.Live;
                }

                await streamRepository.Add(model);

                return RedirectToAction(nameof(Manage));
            }

            await PrepareViewBag();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            await PrepareViewBag();

            var stream = await streamRepository.GetById(id, new string[] { "BouquetStreams", "StreamServers" });

            if(stream == null)
            {
                return NotFound();
            }

            stream.BouquetIds = new List<int>();
            stream.ServerIds = new List<int>();

            foreach(var bouquet in stream.BouquetStreams)
            {
                stream.BouquetIds.Add(bouquet.BouquetId);
            }

            foreach (var server in stream.StreamServers)
            {
                stream.ServerIds.Add(server.ServerId);
            }

            return View(stream);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Stream model)
        {
            if (ModelState.IsValid)
            {
                await streamServersRepository.Delete(m => m.StreamId == model.Id);
                await bouquetStreamsRepository.Delete(m => m.StreamId == model.Id);

                foreach (var serverId in model.ServerIds)
                {
                    model.StreamServers.Add(new StreamServer
                    {
                        ServerId = serverId,
                        StreamId = model.Id
                    });
                }

                foreach (var bouquetId in model.BouquetIds)
                {
                    model.BouquetStreams.Add(new BouquetStream
                    {
                        BouquetId = bouquetId,
                        StreamId = model.Id
                    });
                }

                if (model.TranscodeId == 0)
                {
                    model.State = StreamState.Live;
                }

                await streamServersRepository.Add(model.StreamServers);
                await bouquetStreamsRepository.Add(model.BouquetStreams);
                await streamRepository.Edit(model);

                return RedirectToAction(nameof(Manage));
            }
            await PrepareViewBag();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await streamServersRepository.Delete(m => m.StreamId == id);
            await bouquetStreamsRepository.Delete(m => m.StreamId == id);
            await streamRepository.Delete(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var dataToLoad = new string[]
            {
                "Category",
                "BouquetStreams",
                "BouquetStreams.Bouquet",
                "StreamServers",
                "StreamServers.Server"
            };
            var stream = await streamRepository.GetById(id, dataToLoad);

            if (stream == null)
            {
                return NotFound();
            }

            return View(stream);
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var result = await streamRepository.GetAll(new string[] { "Category" });
            return View(result);
        }

        private async Task PrepareViewBag()
        {
            ViewBag.Categories = await categoryRepository.Search(m => m.CategoryType == CategoryType.LiveStreams);
            ViewBag.Servers = await serverRepository.Search(m => m.ServerState == ServerState.Online);
            ViewBag.Bouquets = await bouquetRepository.GetAll();
            ViewBag.TranscodeProfiles = await transcodeRepository.GetAll();
        }
    }
}