using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TestAsync();

        }

        static void demo()
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5001/chathub")
                .Build();


            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine(user + "<<" + message);
            });

            connection.StartAsync().Wait();
            string str;
            while ((str = Console.ReadLine()) != null)
                connection.InvokeAsync("SendMessage", "user", str).Wait();
        }

        static void TestAsync()
        {
            HubConnection connection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5001/chathub")
                    .Build();
            int count = 0;

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<byte[]>("EchoBack", (array) =>
            {
                //Console.WriteLine(++count);
            });

            var send_array = new byte[4 * 1024];
            new Random().NextBytes(send_array);
            connection.StartAsync().Wait();

            var total_def = 256;
            var tasks = new Task[total_def];
            var start_time = DateTime.Now;


            //Parallel.For(0, 256 * 200, (i) =>
            //{
            //     connection.InvokeAsync("Echo", send_array).Wait();
            //});
            for (int i = 0; i < 200 * 1; i++)
            {
                var total = total_def;
                while (--total >= 0)
                    tasks[total] = connection.InvokeAsync("Echo", send_array);
                Task.WaitAll(tasks);
            }
            Console.WriteLine("fiii<<" + (DateTime.Now - start_time));
        }

    }
}
