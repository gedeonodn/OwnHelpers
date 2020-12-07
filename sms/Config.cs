using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sms
{
    public class Config
    {
        private IConfigurationRoot config;
        public Config(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddCommandLine(args)
                .AddEnvironmentVariables();

            config = builder.Build();
        }
        public string Hostname
        {
            get => config["modemHostName"]; 
        }
        public string MailSubject
        {
            get => config["mailSubject"];
        }
        public string MailCommand
        {
            get => config["mailScript"];
        }
        public string TelegramCommand
        {
            get => config["telegramScript"];
        }
    }
}
