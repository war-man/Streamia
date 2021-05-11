using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;

namespace Streamia.Controllers
{
    public class IptvUsersController : Controller
    {
        public IEnumerable<Bouquet> Bouquets { get; set; }

        private readonly IRepository<IptvUser> iptvRepository;
        private readonly IRepository<Bouquet> bouquetRepository;
        private readonly IRepository<Setting> settingRepository;
        private readonly UserManager<AppUser> userManager;

        public IptvUsersController
        (
            IRepository<IptvUser> iptvRepository,
            IRepository<Bouquet> bouquetRepository,
            IRepository<Setting> settingRepository,
            UserManager<AppUser> userManager
        )
        {
            this.iptvRepository = iptvRepository;
            this.bouquetRepository = bouquetRepository;
            this.settingRepository = settingRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Bouquets = await bouquetRepository.GetAll();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IptvUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                if (((ClaimsIdentity)User.Identity).HasClaim("IsReseller", "true"))
                {
                    var settings = await settingRepository.GetById(1);
                    uint totalCharge = settings.PointsPerCreatedUser;
                    var bouquet = await bouquetRepository.GetById(model.BouquetId);
                    totalCharge += bouquet.Points;

                    if (totalCharge > user.Credit)
                    {
                        ModelState.AddModelError(string.Empty, "Your credit is not enough to create this user");
                        ViewBag.Bouquets = await bouquetRepository.GetAll();
                        return View(model);
                    }

                    user.Credit -= totalCharge;

                    var result = await userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
                
                await iptvRepository.Add(model);
                return RedirectToAction(nameof(Manage));
            }
            ViewBag.Bouquets = await bouquetRepository.GetAll();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await iptvRepository.Delete(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string keyword)
        {
            IEnumerable<IptvUser> data;
            if (keyword != null)
            {
                data = await iptvRepository.Search(m => m.Username.Contains(keyword));
                ViewBag.Keyword = keyword;
            }
            else
            {
                data = await iptvRepository.GetAll(new string[] { "Bouquet" });
            }
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Ban(int id)
        {
            var user = await iptvRepository.GetById(id);
            if (user != null)
            {
                user.Banned = !user.Banned;
                await iptvRepository.Edit(user);
            }
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var user = await iptvRepository.GetById(id, new string[] { 
                "Bouquet",
                "Bouquet.BouquetStreams",
                "Bouquet.BouquetStreams.Stream",
                "Bouquet.BouquetMovies",
                "Bouquet.BouquetMovies.Movie"
            });
            var m3u8Builder = new StringBuilder();
            m3u8Builder.AppendLine("#EXTM3U");
            foreach (var bouquetStream in user.Bouquet.BouquetStreams)
            {
                m3u8Builder.AppendLine($"#EXTINF:-1,{bouquetStream.Stream.Name}");
                m3u8Builder.AppendLine($"{Request.Scheme}://{Request.Host}/api/auth/authenticate/{user.Username}/{user.Password}/{CategoryType.LiveStreams}/{bouquetStream.Stream.Id}");
            }
            foreach (var bouquetMovie in user.Bouquet.BouquetMovies)
            {
                m3u8Builder.AppendLine($"#EXTINF:-1,{bouquetMovie.Movie.Name}");
                m3u8Builder.AppendLine($"{Request.Scheme}://{Request.Host}/api/auth/authenticate/{user.Username}/{user.Password}/{CategoryType.Movies}/{bouquetMovie.Movie.Id}");
            }
            return File(Encoding.UTF8.GetBytes(m3u8Builder.ToString()), "text/m3u8", "iptv-playlist.m3u8");
        }
    }
}