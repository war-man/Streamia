using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Renci.SshNet;
using Streamia.Models;
using Streamia.Models.Interfaces;
using Streamia.Realtime.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Streamia.Realtime
{
    public class DirectoryBrowserHub : Hub
    {
        private readonly IRepository<Server> serverRepository;
        private readonly IRemoteConnection remoteConnection;

        public DirectoryBrowserHub(IRepository<Server> serverRepository, IRemoteConnection remoteConnection)
        {
            this.serverRepository = serverRepository;
            this.remoteConnection = remoteConnection;
        }

        /*
         
            DONT FORGET TO ADD USER ID AS IDENTIFIER AMONG SERVER ID
            TO AVOID MULTI AGENT DATABASE SYSTEM CONFLICT <<< Ayman
         
         */

        public async Task ListServerDirectory(int id, string path)
        {
            string directoryList;
            SshClient sshClient;
            Server server = null;
            if (!remoteConnection.ConnectionsList.ContainsKey($"{id}"))
            {
                server = await serverRepository.GetById(id);
                if (server == null)
                {
                    return;
                }
                sshClient = new SshClient(server.Ip, "root", server.RootPassword);
                remoteConnection.ConnectionsList.Add(id.ToString(), sshClient);
            }
            else
            {
                sshClient = remoteConnection.ConnectionsList[$"{id}"];
            }
            try
            {
                if (!sshClient.IsConnected)
                {
                    sshClient.Connect();
                }
                var command = remoteConnection.ConnectionsList[$"{id}"].CreateCommand($"cd {path} && ls | egrep -i '\\.mp4$|\\.mkv$|\\.flv$|\\.avi$|\\.mpg|\\.mov$' ; ls -d */");
                command.Execute();
                directoryList = command.Result;
            }
            catch
            {
                throw new Exception($"Failed to connect to {server.Ip ?? "server"}");
            }
            await Clients.All.SendAsync("DirectoryList", new { directoryList });
        }

        public void DisposeConnection(int id)
        {
            try
            {
                if (remoteConnection.ConnectionsList.ContainsKey(id.ToString()))
                {
                    remoteConnection.ConnectionsList[id.ToString()].Disconnect();
                    remoteConnection.ConnectionsList[id.ToString()].Dispose();
                    remoteConnection.ConnectionsList.Remove(id.ToString());
                }
            } 
            catch
            {
                throw new Exception("Failed to dispose current connection");
            }
        }
    }
}
