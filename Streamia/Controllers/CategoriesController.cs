using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Streamia.Helpers;
using Streamia.Models;
using Streamia.Models.Interfaces;

namespace Streamia.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IRepository<Category> categoryRepository;

        public CategoriesController(IRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category model)
        {
            if (ModelState.IsValid)
            {
                await categoryRepository.Add(model);
                return RedirectToAction(nameof(Manage));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await categoryRepository.GetById(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                await categoryRepository.Edit(model);
                return RedirectToAction(nameof(Manage));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                await categoryRepository.Delete(id);
            }
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Manage(int? page)
        {
            //IEnumerable<Category> data;

            //if (keyword != null)
            //{
            //    data = await categoryRepository.Search(m => m.Name.Contains(keyword));
            //    ViewBag.Keyword = keyword;
            //} 
            //else
            //{
            //    data = await categoryRepository.GetAll();
            //}
            var pager = new Pager<Category>(categoryRepository, page, 10);
            var data = await pager.GetPaginatedList();
            ViewBag.PaginationData = pager.GetPaginationData();
            return View(data);
        }

    }
}