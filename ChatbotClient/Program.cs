namespace ChatbotClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new AppConfiguration("appsettings.json");
            var client = new ChatClient(configuration);
            client.Run();
        }
    }
}
