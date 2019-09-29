using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatbotServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new AppConfiguration("appsettings.json");
            var server = new ChatServer(configuration);
            var cts = new CancellationTokenSource();

            var task = Task.Run(async () => await server.RunAsync(cts.Token));

            bool stop = false;
            while (!stop)
            {
                stop = (Console.ReadLine() == "stop");
            }

            cts.Cancel();

            task.Wait(configuration.StopMillisecondsTimeout);
        }
    }
}
