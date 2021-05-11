using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Streamia.Models;
using Streamia.Helpers;
using Streamia.ViewModels;
using System.Net;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Streamia.Models.Interfaces;
using Streamia.Models.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Streamia.Controllers
{
    public class ResellersController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IRepository<Bouquet> bouquetRepository;
        private readonly IRepository<Setting> settingRepository;
        private readonly IRepository<Recharge> rechargeRepository;

        public ResellersController
        (
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IRepository<Bouquet> bouquetRepository,
            IRepository<Setting> settingRepository,
            IRepository<Recharge> rechargeRepository
        )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.bouquetRepository = bouquetRepository;
            this.settingRepository = settingRepository;
            this.rechargeRepository = rechargeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Bouquets = await bouquetRepository.GetAll();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    AddMAG = model.AddMag,
                    AddEnigma = model.AddEnigma,
                    MonitorMagOnly = model.MonitorMagOnly,
                    MonitorEnigmaOnly = model.MonitorEnigmaOnly,
                    LockSTB = model.LockSTB,
                    Restream = model.Restream
                };

                if (model.TrialAccount)
                {
                    user.TrialAccount = true;

                    if (model.TrialDays < 1)
                    {
                        ModelState.AddModelError("TrialDays", "Trial period should be at least 1 day");
                        return View(model);
                    }

                    user.TrialDays = model.TrialDays;
                }

                user.ResellerBouquets = new List<ResellerBouquet>();

                foreach (var bouquetId in model.BouquetIds)
                {
                    user.ResellerBouquets.Add(new ResellerBouquet
                    {
                        ResellerId = user.Id,
                        BouquetId = bouquetId
                    });
                }

                var createResult = await userManager.CreateAsync(user, model.Password);

                if (createResult.Succeeded)
                {
                    var claims = new List<Claim>();

                    if (user.AddMAG)
                    {
                        claims.Add(new Claim("AddMag", "true"));
                    }

                    if (user.AddEnigma)
                    {
                        claims.Add(new Claim("AddEnigma", "true"));
                    }

                    claims.Add(new Claim("IsReseller", "true"));

                    var addClaimsResult = await userManager.AddClaimsAsync(user, claims);

                    if (addClaimsResult.Succeeded)
                    {
                        return RedirectToAction(nameof(Manage));
                    }

                    foreach (var error in addClaimsResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var resellers = await userManager.Users.ToListAsync();
            return View(resellers);
        }

        [HttpGet]
        public async Task<IActionResult> Recharge()
        {
            var settings = await settingRepository.GetById(1);
            return View(new RechargeViewModel { Setting = settings });
        }

        [HttpPost]
        public async Task<IActionResult> Recharge(RechargeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // LOGIC NEEDED TO MAKE SURE TRANSACTION EXISTS ON PAYPAL
                var user = await userManager.GetUserAsync(User);

                var recharge = new Recharge
                {
                    ResellerId = user.Id,
                    TransactionId = model.TransactionId,
                    Points = model.Points
                };

                user.Credit += model.Points;

                await rechargeRepository.Add(recharge);
                await userManager.UpdateAsync(user);

                return RedirectToAction("Dashboard", "Home");
            }
            return View(model);
        }
    }
}