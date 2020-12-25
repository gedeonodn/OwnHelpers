using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace sms
{
    public class Config
    {
        private string[] args = Environment.GetCommandLineArgs();
        private IConfigurationRoot config;
        public Config()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
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
        public bool PrintHelp
        {
            get => new string[] { "-h", "--help" }.Intersect(args).Any();
        }
        public bool UnreadOnly
        {
            get => new string[] { "-u", "--unread" }.Intersect(args).Any();
        }
        public bool HtmlOutput
        {
            get => new string[] { "-w", "--html" }.Intersect(args).Any();
        }
        public bool SendEmail
        {
            get => new string[] { "-s", "--sendemail" }.Intersect(args).Any();
        }
        public bool IsQueit
        {
            get => new string[] { "-q", "--queit" }.Intersect(args).Any();
        }
        public bool SendTelegram
        {
            get => new string[] { "-t", "--telegram" }.Intersect(args).Any();
        }
        public int DeleteIndex
        {
            get
            {
                string delParam = args
                    .Where(p => new Regex("-d=").IsMatch(p) || new Regex("--delete=").IsMatch(p))
                    .FirstOrDefault()?
                    .Split('=')
                    .ElementAt(1)
                    .Trim();
                if (string.IsNullOrWhiteSpace(delParam))
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(delParam);
                }
            }
        }

        public Sms SendSms
        { 
            get
            {
                string sendParam = args
                    .Where(p => new Regex("-c=").IsMatch(p) || new Regex("--create=").IsMatch(p))
                    .FirstOrDefault()?
                    .Split('=')
                    .ElementAt(1)
                    .Trim();
                if (string.IsNullOrWhiteSpace(sendParam))
                {
                    return null;
                }
                else
                {
                    string phone = sendParam.Split(' ').ElementAt(0);
                    string text = sendParam.Remove(0, sendParam.IndexOf(' ') + 1);
                    return new Sms() { Phone = phone, Message = text };
                }
            }
        }
    }
}
