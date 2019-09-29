using System;
using System.IO;
using System.Net.Sockets;

namespace ChatbotClient
{
    public class ChatClient
    {
        private readonly AppConfiguration configuration;
        public const string NewLine = "$";

        public ChatClient(AppConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Run()
        {
            var client = new TcpClient(configuration.Hostname, configuration.Port);

            using (var stream = client.GetStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            {
                var msg = ReadLineFromServer(reader);
                Console.Write(msg);

                bool bye = false;
                while (!bye)
                {
                    msg = ReadLineFromConsole();
                    WriteLineAndFlush(writer, msg);

                    if (msg.ToLower() == "bye")
                    {
                        bye = true;
                    }

                    msg = ReadLineFromServer(reader);
                    Console.Write(msg);
                }
            }

            client.Close();
        }

        private static void WriteLineAndFlush(StreamWriter writer, string msg)
        {
            writer.WriteLine(msg);
            writer.Flush();
        }

        private static string ReadLineFromServer(StreamReader reader)
        {
            return reader.ReadLine().Replace(NewLine, Environment.NewLine);
        }

        private static string ReadLineFromConsole()
        {
            Console.Write("client: ");
            return Console.ReadLine();
        }
    }
}