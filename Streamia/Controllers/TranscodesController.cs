using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Streamia.Models;
using Streamia.Models.Interfaces;

namespace Streamia.Controllers
{
    public class TranscodesController : Controller
    {
        private readonly IRepository<Transcode> transcodeRepository;

        public TranscodesController(IRepository<Transcode> transcodeRepository)
        {
            this.transcodeRepository = transcodeRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Transcode());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Transcode model)
        {
            if (ModelState.IsValid)
            {
                await transcodeRepository.Add(model);
                return RedirectToAction(nameof(Manage));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await transcodeRepository.GetById(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Transcode model)
        {
            if (ModelState.IsValid)
            {
                await transcodeRepository.Edit(model);
                return RedirectToAction(nameof(Manage));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await transcodeRepository.Delete(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string keyword)
        {
            IEnumerable<Transcode> data;
            if (keyword != null)
            {
                data = await transcodeRepository.Search(m => m.Name.Contains(keyword));
                ViewBag.Keyword = keyword;
            }
            else
            {
                data = await transcodeRepository.GetAll();
            }
            return View(data);
        }

    }
}
