using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace sms
{
    public class HuaweiReader
    {
        private string hostAddress;
        public HuaweiReader(string hostAddress)
        {
            this.hostAddress = hostAddress;
        }

        public XmlDocument Read()
        {
            //string hostName = 
            Uri server = new Uri(hostAddress);

            // получим куки и токен безопасности
            CookieContainer container = new CookieContainer();
            HttpWebRequest getInboxRequest = WebRequest.Create(new Uri(server, "html/smsinbox.html")) as HttpWebRequest;
            getInboxRequest.CookieContainer = container;
            MatchCollection tokens;
            using(HttpWebResponse getInboxResponse = getInboxRequest.GetResponse() as HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(getInboxResponse.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    string pattern = "(?<=<meta name=\\\"csrf_token\\\" content=\\\").*?(?=\\\"\\/>\\r\\n)";
                    Regex tokenRegex = new Regex(pattern);
                    tokens = tokenRegex.Matches(responseText);
                }
            }

            // получим список смс
            HttpWebRequest smsRequest = WebRequest.Create(new Uri(server, "api/sms/sms-list")) as HttpWebRequest;
            smsRequest.CookieContainer = container;
            foreach(var token in tokens)
            {
                smsRequest.Headers.Add("__RequestVerificationToken", token.ToString());
            }
            smsRequest.Method = "POST";
            using(Stream dataStream = smsRequest.GetRequestStream())
            {
                string xmlRequest = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><PageIndex>1</PageIndex><ReadCount>50</ReadCount><BoxType>1</BoxType><SortType>0</SortType><Ascending>0</Ascending><UnreadPreferred>0</UnreadPreferred></request>";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(xmlRequest);
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            using(HttpWebResponse smsResponse = smsRequest.GetResponse() as HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(smsResponse.GetResponseStream()))
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(reader))
                    {
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(xmlReader);
                        return xmldoc;
                    }
                }
            }
        }
    }
}