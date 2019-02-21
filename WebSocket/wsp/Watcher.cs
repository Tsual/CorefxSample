using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocket.wsp
{
    public class Watcher
    {
        private readonly static ILog log = LogHelper.GetLog(typeof(Watcher));

        public string Name { get; set; }
        public System.Net.WebSockets.WebSocket Socket { get; set; }
        public Queue<string> SendQueue { get; set; } = new Queue<string>();

        public event Action<string, string> DoneReciveMessage = (name, msg) => log.Info("recive<<" + name + "<<" + msg);
        //public event Action<string, string> DoneJoin = (name, msg) => log.Info("join<<" + name + "<<" + msg);
        public event Action<string> DoneQuit = (name) => log.Info("quit<<" + name);

        public Watcher(System.Net.WebSockets.WebSocket Socket)
        {
            this.Socket = Socket;
        }

        public async Task DoWork()
        {
            WebSocketReceiveResult result;
            do
            {
                string rec_str = "";
                var buffer = System.Net.WebSockets.WebSocket.CreateServerBuffer(4 * 1024);
                result = await Socket.ReceiveAsync(buffer, CancellationToken.None);

                do
                {
                    rec_str += Encoding.UTF8.GetString(buffer).Replace("\0", "");
                } while (!result.EndOfMessage);

                


            } while (!result.CloseStatus.HasValue);
            DoneQuit(Name);
        }

        public void Send(string msg)
        {

        }
    }
}
