using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
    public class WsClient : IDisposable
    {
        public event Action<string, WsClient> DoneReciveText;
        public event Action<byte[], WsClient> DoneReciveBinary;

        public ClientWebSocket SocketInstance { get; }
        private int send_buffer_size;
        private int recive_buffer_size;

        public WsClient(Uri uri, int send_buffer_size = 4 * 1024, int recive_buffer_size = 4 * 1024,
            Action<string, WsClient> event_text = null, Action<byte[], WsClient> event_binary = null)
        {
            if (event_text != null) DoneReciveText += event_text;
            if (event_binary != null) DoneReciveBinary += event_binary;

            SocketInstance = new ClientWebSocket();
            SocketInstance.ConnectAsync(uri, CancellationToken.None).Wait();

            this.send_buffer_size = send_buffer_size;
            this.recive_buffer_size = recive_buffer_size;
        }

        /// <summary>
        /// usage:  ins.HostAsync();
        ///         ins.Send("hello");
        /// </summary>
        public async Task HostAsync()
        {
            do
            {
                var buffer = new byte[recive_buffer_size];
                var res = await SocketInstance.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                bool is_close = false;
                switch (res.MessageType)
                {
                    case WebSocketMessageType.Binary:
                        if (res.Count == 0) break;
                        var result = new byte[res.Count];
                        int index = 0;
                        do
                        {
                            Array.Copy(buffer, 0, result, index++ * recive_buffer_size, index * recive_buffer_size > res.Count ? res.Count - --index * recive_buffer_size : recive_buffer_size);
                        } while (!res.EndOfMessage);
                        DoneReciveBinary?.Invoke(result, this);
                        break;
                    case WebSocketMessageType.Text:
                        if (res.Count == 0) break;
                        var result_1 = new byte[res.Count];
                        int index_1 = 0;
                        do
                        {
                            Array.Copy(buffer, 0, result_1, index_1++ * recive_buffer_size, index_1 * recive_buffer_size > res.Count ? res.Count - --index_1 * recive_buffer_size : recive_buffer_size);
                        } while (!res.EndOfMessage);
                        DoneReciveText?.Invoke(Encoding.UTF8.GetString(result_1), this);
                        break;
                    case WebSocketMessageType.Close:
                        is_close = true;
                        break;
                }
                if (is_close) break;
            } while (!SocketInstance.CloseStatus.HasValue);
        }

        public bool Send(string text)
        {
            if (SocketInstance == null || SocketInstance.State != WebSocketState.Open) return false;
            SocketInstance.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(text)), WebSocketMessageType.Text, true, CancellationToken.None).Wait();
            return true;
        }

        public bool Send(byte[] bin)
        {
            if (SocketInstance == null || SocketInstance.State != WebSocketState.Open) return false;
            SocketInstance.SendAsync(new ArraySegment<byte>(bin), WebSocketMessageType.Binary, true, CancellationToken.None).Wait();
            return true;
        }

        public void Dispose()
        {
            SocketInstance.Dispose();
        }
    }
}
