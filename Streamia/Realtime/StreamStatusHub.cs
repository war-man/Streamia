using Microsoft.AspNetCore.SignalR;
using Renci.SshNet;
using Streamia.Helpers;
using Streamia.Models;
using Streamia.Models.Enums;
using Streamia.Models.Interfaces;
using Streamia.Realtime.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamia.Realtime
{
    public class StreamStatusHub : Hub, IState<StreamServer>
    {
        private readonly IRepository<Stream> streamRepository;

        public StreamStatusHub(IRepository<Stream> streamRepository)
        {
            this.streamRepository = streamRepository;
        }

        public async Task Update(int sourceId, StreamState state)
        {
            var stream = await streamRepository.GetById(sourceId, new string[] { "StreamServers", "StreamServers.Server", "Transcode" });

            if (stream == null)
            {
                return;
            }

            if (stream.State == state)
            {
                return;
            }

            if (state == StreamState.Live)
            {
                var options = new Dictionary<string, string>
                {
                    { "hls_time", "4" },
                    { "hls_playlist_type", "event" }
                };
                if (!stream.Record)
                {
                    options.Add("hls_flags", "delete_segments");
                }
                string transcode = FFMPEGCommand.MakeCommand(stream.Transcode, stream.Source, $"/var/hls/{stream.StreamKey}", options);
                string prepareCommand = $"mkdir /var/hls/{stream.StreamKey}";
                prepareCommand += $" && cd /var/hls/{stream.StreamKey}";
                prepareCommand += $" && mkdir 1080p 720p 480p 360p";
                stream.StreamServers = (List<StreamServer>) Start((IList<StreamServer>)stream.StreamServers, prepareCommand, transcode);
            } 
            else if (state == StreamState.Stopped)
            {
                string prepareCommand = $"rm -rf /var/hls/{stream.StreamKey}";
                stream.StreamServers = (List<StreamServer>) Stop((IList<StreamServer>) stream.StreamServers, prepareCommand);
            }

            stream.State = state;
            await streamRepository.Edit(stream);
            await Clients.All.SendAsync("Update", new { sourceId, state = (int) state });
        }

        public IList<StreamServer> Start(IList<StreamServer> servers, string preCommand, string command)
        {
            for (int i = 0; i < servers.Count; i++)
            {
                try
                {
                    var client = new SshClient(servers[i].Server.Ip, "root", servers[i].Server.RootPassword);
                    client.Connect();
                    client.RunCommand(preCommand);
                    var cmd = client.CreateCommand($"nohup {command} >/dev/null 2>&1 & echo $!");
                    var result = cmd.Execute();
                    int pid = int.Parse(result);
                    client.RunCommand($"disown -h {pid}");
                    client.Disconnect();
                    client.Dispose();
                    servers[i].Pid = pid;
                }
                catch
                {
                    throw new Exception($"Failed to start on server {servers[i].Server.Name}@{servers[i].Server.Ip}");
                }
            }
            return servers;
        }

        public IList<StreamServer> Stop(IList<StreamServer> servers, string preCommand)
        {
            for (int i = 0; i < servers.Count; i++)
            {
                try
                {
                    var client = new SshClient(servers[i].Server.Ip, "root", servers[i].Server.RootPassword);
                    client.Connect();
                    client.RunCommand($"kill -9 {servers[i].Pid}");
                    client.RunCommand(preCommand);
                    client.Disconnect();
                    client.Dispose();
                    servers[i].Pid = 0;
                }
                catch
                {
                    throw new Exception($"Failed to stop on server {servers[i].Server.Name}@{servers[i].Server.Ip}");
                }
            }
            return servers;
        }
        
    }
}
