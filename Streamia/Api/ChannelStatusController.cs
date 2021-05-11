using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Renci.SshNet;
using Streamia.Helpers;
using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;
using Streamia.Realtime;

namespace Streamia.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ChannelStatusController : ControllerBase
    {
        private readonly IRepository<Channel> channelRepository;
        private readonly IRepository<Server> serverRepository;
        private readonly IHubContext<ChannelStatusHub> hub;

        public ChannelStatusController
        (
            IRepository<Channel> channelRepository,
            IRepository<Server> serverRepository,
            IHubContext<ChannelStatusHub> hub
        )
        {
            this.channelRepository = channelRepository;
            this.serverRepository = serverRepository;
            this.hub = hub;
        }

        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var channel = await channelRepository.GetById(id, new string[] { "ChannelServers" });

            if (channel != null)
            {
                if (channel.SourceTranscodedCount < channel.SourceCount)
                {
                    channel.SourceTranscodedCount++;
                }

                if (channel.SourceCount == channel.SourceTranscodedCount)
                {
                    var server = await serverRepository.GetById(channel.ChannelServers.FirstOrDefault().ServerId);
                    ThreadPool.QueueUserWorkItem(queue => {
                        try
                        {
                            string input = $"/var/hls/{channel.StreamKey}/sources/RESOLUTION/source_list.txt";
                            string output = $"/var/hls/{channel.StreamKey}";
                            List<string> transcoders = FFMPEGCommand.ChannelCommand(input, output);
                            var client = new SshClient(server.Ip, "root", server.RootPassword);
                            client.Connect();

                            foreach (var transcoder in transcoders)
                            {
                                var cmd = client.CreateCommand($"nohup {transcoder} >/dev/null 2>&1 & echo $!");
                                var result = cmd.Execute();
                                int pid = int.Parse(result);
                                client.RunCommand($"disown -h {pid}");
                            }

                            client.Disconnect();
                            client.Dispose();
                        } 
                        catch
                        {

                        }
                    });
                    channel.State = StreamState.Live;
                    await hub.Clients.All.SendAsync("UpdateSignal", new { id, state = (int) StreamState.Live });
                } 

                await channelRepository.Edit(channel);
            }

            return Ok();
        }

        [Route("edit/{id}/{state}")]
        public async Task<IActionResult> Edit(int id, StreamState state)
        {
            var channel = await channelRepository.GetById(id);

            if (channel != null)
            {
                channel.State = state;
                await channelRepository.Edit(channel);
                await hub.Clients.All.SendAsync("UpdateSignal", new { id, state = (int) state });
            }

            return Ok();
        }
    }
}
