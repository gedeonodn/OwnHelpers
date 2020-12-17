using System;
using System.IO;
using System.Net;
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
            Uri server = new Uri(hostAddress);

            HuaweiAuthorizer authorizer = new HuaweiAuthorizer(hostAddress);
            AuthParams authParams = authorizer.Authorize();

            // получим список смс
            HttpWebRequest smsRequest = WebRequest.Create(new Uri(server, "api/sms/sms-list")) as HttpWebRequest;
            smsRequest.CookieContainer = authParams.Container;
            smsRequest.Headers.Add("__RequestVerificationToken", authParams.CsrfToken);

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