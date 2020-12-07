using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sms
{
    public class Smses
    {
        private List<Sms> smses;
        public Smses()
        {
            smses = new List<Sms>();

        }
        public int Count 
        {
            get 
            {
                return smses.Count;
            }
        }
        public void Add(Sms sms)
        {
            smses.Add(sms);
        }
        public string TxtReport
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.Append($"Sms count: {Count}");
                sb.AppendLine();
                sb.Append('-', 25);
                sb.AppendLine();
                foreach(Sms sms in smses)
                {
                    string smsStatus = (sms.Smstat==0)?"***UNREAD!!!***":"";
                    sb.Append($"SmsId: {sms.Id} {smsStatus}");
                    sb.AppendLine();
                    sb.Append($"From: {sms.Phone}");
                    sb.AppendLine();
                    sb.Append($"Text: {sms.Message}");
                    sb.AppendLine();
                    sb.Append($"SmsDate: {sms.SmsDateTime}");
                    //sb.AppendLine();
                    //sb.Append($"Additional: Sca - {sms.Sca}, SaveType - {sms.SaveType}, Priority - {sms.Priority}, SmsType - {sms.SmsType}");
                    sb.AppendLine();
                    sb.Append('-', 25);
                    sb.AppendLine();
                }
                sb.AppendLine();

                return sb.ToString();

            }
        }
        public string HtmlReport
        {
            get
            {
                
                StringBuilder sb = new StringBuilder();
                sb.Append($"<html><h3>Total SMS: {smses.Count}</h3><br/>");
                sb.Append($"<table border=1 style='width:100%; border: 1px solid black; border-collapse: collapse;' cellpadding='5';>");
                //sb.Append($"");
                sb.Append($"<tr><th>Id</th><th>Date</th><th>Phone</th><th>Text</th></tr>");
                foreach(Sms sms in smses)
                {
                    string trStyle = sms.Smstat == 0 ? "style='color: blue; font-weight:bold;';" : "";
                    sb.Append($"<tr {trStyle}><td>{sms.Id}</td><td>{sms.SmsDateTime.ToString("dd.MM.yyyy HH:mm:ss").Replace(" ", "&nbsp;")}</td><td>{sms.Phone}</td><td>{sms.Message}</td></tr>");
                }
                sb.Append($"</table>");
                sb.Append("</html>");
                return sb.ToString();
            }

        }

        public void Print()
        {
            //ConsoleColor defaultColor = Console.ForegroundColor;
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Sms count: {Count}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("-------------------------");
            Console.ResetColor();
            foreach(Sms sms in smses)
            {
                string smsStatus = (sms.Smstat==0)?"***UNREAD!!!***":"";
                ConsoleColor unreadColor = sms.Smstat==0 ? ConsoleColor.Red : Console.ForegroundColor; 
                Console.Write($"SmsId: {sms.Id}");
                Console.ForegroundColor = unreadColor;
                Console.WriteLine($" {smsStatus}");
                Console.ResetColor();
                Console.WriteLine($"From: {sms.Phone}");
                Console.Write("Text: ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{sms.Message}");
                Console.ResetColor();
                Console.WriteLine($"SmsDate: {sms.SmsDateTime}");
                //Console.WriteLine($"Additional: Sca - {sms.Sca}, SaveType - {sms.SaveType}, Priority - {sms.Priority}, SmsType - {sms.SmsType}");
                
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("-------------------------");
                Console.ResetColor();
            }
            Console.WriteLine();

        }

        public void ClearRead()
        {
            smses = smses.Where(p => p.Smstat == 0).ToList();
        }
    }

}
