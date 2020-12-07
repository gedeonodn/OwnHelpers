using System;
using System.Linq;

namespace sms
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Config config = new Config(args);

            string hostAddress = config.Hostname;
            HuaweiParser parser = new HuaweiParser(new HuaweiReader(hostAddress).Read());
            Smses smses = parser.Parse();

            if(new string[] {"-h", "--help"}.Intersect(args).Any())
            {
                Console.WriteLine("Utility for read sms from remote huawei modem.");
                Console.WriteLine("Usage:");
                Console.WriteLine();
                Console.WriteLine("-h,\t--help\t\tShow this help");
                Console.WriteLine("-u,\t--unread\tShow (send) only unread sms");
                Console.WriteLine("-w,\t--html\t\tOutput in html format");
                Console.WriteLine("-s,\t--sendemail\tSend report to email");
                Console.WriteLine("-t,\t--telegram\tSend report to telegram");
                Console.WriteLine("-q,\t--queit\t\tQueit mode (no additional info in command line)");
                return;
            }

            if(new string[] {"-u", "--unread"}.Intersect(args).Any())
            {
                smses.ClearRead();
            }

            string result;

            if(new string[] {"-w", "--html"}.Intersect(args).Any())
            {
                result = smses.HtmlReport;
            }
            else
            {
                result = smses.TxtReport;
            }

            if(new string[] {"-s", "--sendemail"}.Intersect(args).Any())
            {
                if (smses.Count == 0) 
                {
                    if(!new string[] {"-q", "--queit"}.Intersect(args).Any())
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
            else if(new string[] {"-t", "--telegram"}.Intersect(args).Any())
            {
                if (smses.Count == 0) 
                {
                    if(!new string[] {"-q", "--queit"}.Intersect(args).Any())
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
