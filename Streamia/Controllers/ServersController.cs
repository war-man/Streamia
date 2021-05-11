using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;
using Streamia.Realtime;

namespace Streamia.Controllers
{
    public class ServersController : Controller
    {
        private readonly IRepository<Server> serverRepository;

        public ServersController(IRepository<Server> serverRepository)
        {
            this.serverRepository = serverRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Server());
        }

        [HttpPost]
        public async Task<IActionResult> Add(Server model)
        {
            if (ModelState.IsValid)
            {
                var server = await serverRepository.Add(model);
                var host = $"{Request.Scheme}://{Request.Host}";
                var hostUrl = $"{host}/api/serverstatus/edit/{server.Id}/{ServerState.Offline}";

                ThreadPool.QueueUserWorkItem(queue => RunCommand(server, host, hostUrl));

                return RedirectToAction(nameof(Manage));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var Server = await serverRepository.GetById(id);
            if (Server == null)
            {
                return NotFound();
            }
            return View(Server);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Server model)
        {
            if (ModelState.IsValid)
            {
                await serverRepository.Edit(model);
                return RedirectToAction(nameof(Manage));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await serverRepository.Delete(id);
            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var data = await serverRepository.GetById(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string keyword)
        {
            IEnumerable<Server> data;
            if (keyword != null)
            {
                data = await serverRepository.Search(m => m.Name.Contains(keyword) || m.Ip.Contains(keyword));
                ViewBag.Keyword = keyword;
            }
            else
            {
                data = await serverRepository.GetAll();
            }
            return View(data);
        }

        private async void RunCommand(Server server, string host, string hostUrl)
        {
            var client = new SshClient(server.Ip, "root", server.RootPassword);

            string serverCommands = await System.IO.File.ReadAllTextAsync("InstallScripts/server");
            string nginxConfig = await System.IO.File.ReadAllTextAsync("InstallScripts/nginx-config");
            string nginxService = await System.IO.File.ReadAllTextAsync("InstallScripts/nginx-service");
            string streamiaService = await System.IO.File.ReadAllTextAsync("InstallScripts/streamia-service");
            nginxConfig = nginxConfig.Replace("MAX_CLIENTS", server.MaxClients.ToString());
            streamiaService = streamiaService.Replace("DOMAIN", host);
            streamiaService = streamiaService.Replace("SERVER_ID", server.Id.ToString());
            serverCommands = serverCommands.Replace("NGINX_CONFIG", nginxConfig);
            serverCommands = serverCommands.Replace("NGINX_SERVICE", nginxService);
            serverCommands = serverCommands.Replace("DOMAIN", host);
            serverCommands = serverCommands.Replace("STREAMIA_SERVICE", streamiaService);
            serverCommands = serverCommands.Replace("SERVER_ID", server.Id.ToString());
            try
            {
                client.Connect();
                client.RunCommand(serverCommands);
                client.Disconnect();
                client.Dispose();
            }
            catch
            {
                var httpClient = new HttpClient();
                await httpClient.GetAsync(hostUrl);
            }
        }
    }
}