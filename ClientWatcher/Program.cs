using System;
using System.Net.WebSockets;
using System.Threading;

namespace ClientWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(11000);
            WsClient client = new WsClient(new Uri("ws://localhost:5000/ws"), event_text: (text, host) => { Console.WriteLine(DateTime.Now + "<<" + text); });
            var task = client.HostAsync();
            client.Send("join,Watcher");
            task.Wait();
        }
    }
}
