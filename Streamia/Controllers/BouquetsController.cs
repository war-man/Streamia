using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;

namespace Streamia.Controllers
{
    public class BouquetsController : Controller
    {
        private readonly IRepository<Bouquet> bouquetRepository;
        private readonly IRepository<Stream> streamRepository;
        private readonly IRepository<Movie> movieRepository;
        private readonly IRepository<Series> seriesRepository;
        private readonly IRepository<Channel> channelRepository;

        public BouquetsController
        (
            IRepository<Bouquet> bouquetRepository,
            IRepository<Stream> streamRepository,
            IRepository<Movie> movieRepository,
            IRepository<Series> seriesRepository,
            IRepository<Channel> channelRepository
        )
        {
            this.bouquetRepository = bouquetRepository;
            this.streamRepository = streamRepository;
            this.movieRepository = movieRepository;
            this.seriesRepository = seriesRepository;
            this.channelRepository = channelRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            await PrepareViewBag();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Bouquet model)
        {
            if (ModelState.IsValid)
            {
                if (model.BouquetStreams.Count > 0)
                {
                    foreach (int streamId in model.StreamIds)
                    {
                        model.BouquetStreams.Add(new BouquetStream
                        {
                            BouquetId = model.Id,
                            StreamId = streamId
                        });
                    }
                }

                if (model.BouquetMovies.Count > 0)
                {
                    foreach (int movieId in model.MovieIds)
                    {
                        model.BouquetMovies.Add(new BouquetMovie
                        {
                            BouquetId = model.Id,
                            MovieId = movieId
                        });
                    }
                }

                if (model.BouquetSeries.Count > 0)
                {
                    foreach (int seriesId in model.SeriesIds)
                    {
                        model.BouquetSeries.Add(new BouquetSeries
                        {
                            BouquetId = model.Id,
                            SeriesId = seriesId
                        });
                    }
                }

                if (model.BouquetChannels.Count > 0)
                {
                    foreach (int channelId in model.ChannelIds)
                    {
                        model.BouquetChannels.Add(new BouquetChannel
                        {
                            BouquetId = model.Id,
                            ChannelId = channelId
                        });
                    }
                }

                await bouquetRepository.Add(model);

                return RedirectToAction(nameof(Manage));
            }

            await PrepareViewBag();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Bouquet bouquet = await bouquetRepository.GetById(id);

            if (bouquet == null)
            {
                return NotFound();
            }

            await PrepareViewBag();

            return View(bouquet);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Bouquet model)
        {
            if (ModelState.IsValid)
            {
                await bouquetRepository.Edit(model);

                return RedirectToAction(nameof(Manage));
            }

            await PrepareViewBag();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await bouquetRepository.Delete(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string keyword)
        {
            IEnumerable<Bouquet> data;

            if (keyword != null)
            {
                data = await bouquetRepository.Search(m => m.Name.Contains(keyword));
                ViewBag.Keyword = keyword;
            }
            else
            {
                data = await bouquetRepository.GetAll();
            }

            return View(data);
        }

        private async Task PrepareViewBag()
        {
            ViewBag.Streams = await streamRepository.GetAll();
            ViewBag.Movies = await movieRepository.GetAll();
            ViewBag.Serieses = await seriesRepository.GetAll();
            ViewBag.Channels = await channelRepository.GetAll();
        }
    }
}