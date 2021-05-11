using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Streamia.Models;
using Streamia.Models.Interfaces;

namespace Streamia.Controllers
{
    public class CasesController : Controller
    {
        private readonly IRepository<Case> caseRepository;
        private readonly UserManager<AppUser> userManager;

        public CasesController(IRepository<Case> caseRepository, UserManager<AppUser> userManager)
        {
            this.caseRepository = caseRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "Reseller")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Reseller")]
        public async Task<IActionResult> Add(Case model)
        {
            if (ModelState.IsValid)
            {
                model.ResellerId = userManager.GetUserId(User);
                await caseRepository.Add(model);
                return RedirectToAction("Dashboard", "Home");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var cases = await caseRepository.GetAll(new string[] { "Reseller" });
            return View(cases);
        }

    }
}
