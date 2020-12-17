using System;
using System.Linq;

namespace sms
{
    class Program
    {
        private static Config config = new Config();
        static void Main(string[] args)
        {
            if(config.PrintHelp)
            {
                Console.WriteLine("Utility for read sms from remote huawei modem.");
                Console.WriteLine("Usage:");
                Console.WriteLine();
                Console.WriteLine("-h,\t\t--help\t\tShow this help");
                Console.WriteLine("-u,\t\t--unread\tShow (send) only unread sms");
                Console.WriteLine("-w,\t\t--html\t\tOutput in html format");
                Console.WriteLine("-s,\t\t--sendemail\tSend report to email");
                Console.WriteLine("-t,\t\t--telegram\tSend report to telegram");
                Console.WriteLine("-q,\t\t--queit\t\tQueit mode (no additional info in command line)");
                Console.WriteLine("-d=index,\t--delete=index\tDelete sms with index");
                return;
            }

            if (config.DeleteIndex > 0)
            {
                HuaweiDeleter deleter = new HuaweiDeleter(config.Hostname);
                if (deleter.Delete(config.DeleteIndex))
                {
                    Console.WriteLine($"Delete sms with index={config.DeleteIndex} succesfull");
                }
                else
                {
                    Console.WriteLine($"Delete sms with index={config.DeleteIndex} failed");
                }
                return;
            }

            ReadSms();
        }

        private static void ReadSms()
        {
            HuaweiParser parser = new HuaweiParser(new HuaweiReader(config.Hostname).Read());
            Smses smses = parser.Parse();

            if (config.UnreadOnly)
            {
                smses.ClearRead();
            }

            string result;

            if (config.HtmlOutput)
            {
                result = smses.HtmlReport;
            }
            else
            {
                result = smses.TxtReport;
            }

            if (config.SendEmail)
            {
                if (smses.Count == 0)
                {
                    if (!config.IsQueit)
                    {
                        Console.WriteLine("There is no sms... exit");
                    }
                    return;
                }


                string mailScript = config.MailCommand;
                string mailSubj = "\"" + config.MailSubject + "\"";
                string mailText = "\"" + result + "\"";
                new Bash(mailScript, $"{mailSubj} {mailText}").Execute();
            }
            else if (config.SendTelegram)
            {
                if (smses.Count == 0)
                {
                    if (!config.IsQueit)
                    {
                        Console.WriteLine("There is no sms... exit");
                    }
                    return;
                }

                string telegramScript = config.TelegramCommand;
                new Bash(telegramScript, result).Execute();
            }
            else
            {
                smses.Print();
            }
        }

    }
}
