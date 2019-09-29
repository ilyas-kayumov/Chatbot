using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatbotServer
{
    public class ChatServer
    {
        private readonly AppConfiguration configuration;
        private readonly IList<string> users = new List<string>();
        private readonly IList<KeyValuePair<string, string>> questions;
        public const string ByeCmd = "bye";
        public const string GetUsersCmd = "users";

        public ChatServer(AppConfiguration configuration)
        {
            this.configuration = configuration;
            questions = new List<KeyValuePair<string, string>>()
            {
                KeyValuePair.Create("1. How are you?", "I am fine"),
                KeyValuePair.Create("2. What are you doing?", "I am serving"),
                KeyValuePair.Create("3. What do you have?", "I have nothing")
            };
        }

        public async Task RunAsync(CancellationToken token)
        {
            var address = IPAddress.Parse(configuration.Hostname);

            var listener = new TcpListener(address, configuration.Port);
            listener.Start();

            Console.WriteLine("The sever started");
            Console.WriteLine(@"Enter ""stop"" to stop the server");

            while (!token.IsCancellationRequested)
            {
                if (listener.Pending())
                {
                    var client = await listener.AcceptTcpClientAsync();
                    ProcessClientAsync(client);
                }
            }

            Console.WriteLine("The sever stopped");
            listener.Stop();
        }

        private async void ProcessClientAsync(TcpClient client)
        {
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {
                await WriteLineAndFlushAsync(writer, "Please, enter your name");

                var username = await reader.ReadLineAsync();

                users.Add(username);

                var msg = GetGreetingsMessage(username);
                await WriteLineAndFlushAsync(writer, msg);

                var bye = false;
                while (!bye)
                {
                    msg = await reader.ReadLineAsync();
                    msg = msg.ToLower();
                    switch (msg)
                    {
                        case ByeCmd:
                            await WriteLineAndFlushAsync(writer, $"Bye, {username}!");
                            bye = true;
                            users.Remove(username);
                            break;
                        case GetUsersCmd:
                            await WriteLineAndFlushAsync(writer, GetAllUsersMessage());
                            break;
                        default:
                            int number;
                            if (int.TryParse(msg, out number) && (number > 0 && number <= questions.Count))
                            {
                                await WriteLineAndFlushAsync(writer, questions[number - 1].Value);
                            }
                            else
                            {
                                await WriteLineAndFlushAsync(writer, "Sorry, I can't understand you");
                            }
                            break;
                    }
                }
            }
        }

        private string GetGreetingsMessage(string username)
        {
            var msg = new MessageBuilder();

            msg.AppendLine($"Hi, {username}!")
               .AppendLine("Questions:");
            
            foreach (var q in questions.Select(q => q.Key))
            {
                msg.AppendLine(q);
            }

            msg.AppendLine("Commands:")
               .AppendLine($"{ByeCmd} - end the chat")
               .AppendLine($"{GetUsersCmd} - get all users in the chat");

            msg.AppendLine("Please enter a command or a number of a question");

            return msg.ToString();
        }

        private string GetAllUsersMessage()
        {
            var msg = new MessageBuilder();

            msg.AppendLine($"Users in the chat:");
            foreach (var u in users)
            {
                msg.AppendLine(u);
            }

            return msg.ToString();
        }

        private static async Task WriteLineAndFlushAsync(StreamWriter writer, string msg)
        {
            if (!msg.EndsWith(MessageBuilder.NewLine))
            {
                msg += MessageBuilder.NewLine;
            }

            await writer.WriteLineAsync($"server: {msg}");
            await writer.FlushAsync();
        }
    }
}