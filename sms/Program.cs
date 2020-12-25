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
                Console.WriteLine("-h,\t\t\t--help\t\t\t\tShow this help");
                Console.WriteLine("-u,\t\t\t--unread\t\t\tShow (send) only unread sms");
                Console.WriteLine("-w,\t\t\t--html\t\t\t\tOutput in html format");
                Console.WriteLine("-s,\t\t\t--sendemail\t\t\tSend report to email");
                Console.WriteLine("-t,\t\t\t--telegram\t\t\tSend report to telegram");
                Console.WriteLine("-q,\t\t\t--queit\t\t\t\tQueit mode (no additional info in command line)");
                Console.WriteLine("-d=index,\t\t--delete=index\t\t\tDelete sms with index");
                Console.WriteLine("-c=\"phone some text\",\t--create=\"phone some text\"\tSend sms with some text to phone");
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

            if (config.SendSms != null)
            {
                HuaweiSender sender = new HuaweiSender(config.Hostname);
                if (sender.Send(config.SendSms))
                {
                    Console.WriteLine($"Send sms to number={config.SendSms.Phone} succesfull");
                }
                else
                {
                    Console.WriteLine($"Send sms to number={config.SendSms.Phone} failed");
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
