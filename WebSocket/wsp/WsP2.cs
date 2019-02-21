using log4net;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.Text;

namespace WebSocket.wsp
{
    public class WsP2
    {
        private static readonly ILog log = LogHelper.GetLog(typeof(WsP2));
        public static Dictionary<string, WsServer> WsServers { get; set; } = new Dictionary<string, WsServer>();

        //join,amber
        //send,msg
        public static async Task DoProcess(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            string name = "";
            var host = new WsServer(webSocket, event_text: (text, client) =>
            {
                if (text.StartsWith("join") && !WsServers.ContainsValue(client))
                {
                    name = text.Substring(5);
                    WsServers.Add(name, client);
                }
                else if (text.StartsWith("send") && WsServers.ContainsValue(client))
                {
                    foreach (var tar_client in from t in WsServers where t.Value != client select t)
                        tar_client.Value.Send(name + "<<" + text.Substring(5));
                }
            });
            await host.HostAsync();
            if (!string.IsNullOrEmpty(name))
                WsServers.Remove(name);
        }

    }
}
