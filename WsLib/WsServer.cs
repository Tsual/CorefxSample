using log4net;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
    public class WsServer : IDisposable
    {
        private static readonly ILog log = LogHelper.GetLog(typeof(WsServer));

        public event Action<string, WsServer> DoneReciveText;
        public event Action<byte[], WsServer> DoneReciveBinary;
        public event Action<WsServer> SocketStateAbnormal;

        public WebSocket SocketInstance { get; }
        private int send_buffer_size;
        private int recive_buffer_size;

        public WsServer(WebSocket SocketInstance, int send_buffer_size = 4 * 1024,
            int recive_buffer_size = 4 * 1024, bool enable_log = true,
            Action<string, WsServer> event_text = null, Action<byte[], WsServer> event_binary = null, Action<WsServer> event_state_abnormal = null)
        {
            if (enable_log)
            {
                DoneReciveText += (str, host) => { log.Info("recive<<text<<" + str); };
                DoneReciveBinary += (bin, host) => { log.Info("recive<<binary<<@" + bin.GetHashCode()); };
            }

            if (event_text != null) DoneReciveText += event_text;
            if (event_binary != null) DoneReciveBinary += event_binary;
            if (event_state_abnormal != null) SocketStateAbnormal += event_state_abnormal;

            this.SocketInstance = SocketInstance;
            this.send_buffer_size = send_buffer_size;
            this.recive_buffer_size = recive_buffer_size;
        }

        /// <summary>
        /// usage:  ins.HostAsync();
        ///         ins.Send("hello");
        /// </summary>
        public async Task HostAsync()
        {
            try
            {
                do
                {
                    var buffer = new byte[recive_buffer_size];
                    var res = await SocketInstance.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
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
                            return;
                    }
                } while (!SocketInstance.CloseStatus.HasValue);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return;
            }
        }

        public bool Send(string text)
        {
            if (SocketInstance == null || SocketInstance.State != WebSocketState.Open) {
                SocketStateAbnormal?.Invoke(this);
                return false;
            }
            SocketInstance.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(text)), WebSocketMessageType.Text, true, CancellationToken.None).Wait();
            return true;
        }

        public bool Send(byte[] bin)
        {
            if (SocketInstance == null || SocketInstance.State != WebSocketState.Open)
            {
                SocketStateAbnormal?.Invoke(this);
                return false;
            }
            SocketInstance.SendAsync(new ArraySegment<byte>(bin), WebSocketMessageType.Binary, true, CancellationToken.None).Wait();
            return true;
        }

        public void Dispose()
        {
            SocketInstance.Dispose();
        }
    }
}
