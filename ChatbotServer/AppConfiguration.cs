using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ChatbotServer
{
    public class AppConfiguration
    {
        private readonly string filename;
        private readonly Lazy<IConfiguration> configuration;

        public string Hostname
        {
            get { return configuration.Value["hostname"]; }
        }

        public int Port
        {
            get { return int.Parse(configuration.Value["port"]); }
        }

        public int StopMillisecondsTimeout
        {
            get { return int.Parse(configuration.Value["stopMillisecondsTimeout"]); }
        }

        public AppConfiguration(string filename)
        {
            this.filename = filename;
            configuration = new Lazy<IConfiguration>(CreateConfiguration);
        }

        private IConfiguration CreateConfiguration()
        {
            return CreateConfigurationBuilder().Build();
        }

        private IConfigurationBuilder CreateConfigurationBuilder()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(filename);
        }
    }
}