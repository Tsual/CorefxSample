using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocket.wsp
{
    public class WsP1
    {
        private static readonly ILog log = LogHelper.GetLog(typeof(WsP1));

        public static async Task DoProcess(HttpContext context, System.Net.WebSockets.WebSocket webSocket)
        {
            string rec_str = "";
            var buffer = System.Net.WebSockets.WebSocket.CreateServerBuffer(1024);

            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                
                do
                {
                    rec_str += Encoding.UTF8.GetString(buffer).Replace("\0", "");
                } while (!result.EndOfMessage);

                log.Info("recive<<" + rec_str);
                log.Info("send<<" + rec_str.ToUpper());

                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(rec_str.ToUpper())), WebSocketMessageType.Binary, true, CancellationToken.None);
                //await webSocket.SendAsync(new ArraySegment<byte>(buffer), result.MessageType, result.EndOfMessage, CancellationToken.None);
            } while (!result.CloseStatus.HasValue);



            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
