using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Streamia.Models;
using Streamia.Models.Interfaces;

namespace Streamia.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IRepository<Setting> settingRepository;

        public SettingsController (IRepository<Setting> settingRepository)
        {
            this.settingRepository = settingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> General()
        {
            var model = await settingRepository.GetById(1);

            if (model == null)
            {
                model = await settingRepository.Add(new Setting
                {
                    Id = 1,
                    PointPrice = 0.1m,
                    PointsPerCreatedUser = 10,
                    PayPalClientId = string.Empty
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> General(Setting model)
        {
            if (ModelState.IsValid)
            {
                if (model.PointPrice < 0.1m)
                {
                    ModelState.AddModelError("PointPrice", "Price should be at least $0.1");
                    return View(model);
                }
                model.Id = 1;
                await settingRepository.Edit(model);
            }

            return RedirectToAction(nameof(General));
        }

    }
}
