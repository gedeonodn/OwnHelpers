using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace sms
{
    class HuaweiSender
    {
        private string hostAddress;
        public HuaweiSender(string hostAddress)
        {
            this.hostAddress = hostAddress;
        }

        public bool Send(Sms sms)
        {
            Uri server = new Uri(hostAddress);

            HuaweiAuthorizer authorizer = new HuaweiAuthorizer(hostAddress);
            AuthParams authParams = authorizer.Authorize();

            // получим список смс
            HttpWebRequest sendRequest = WebRequest.Create(new Uri(server, "/api/sms/send-sms")) as HttpWebRequest;
            sendRequest.CookieContainer = authParams.Container;
            sendRequest.Headers.Add("__RequestVerificationToken", authParams.CsrfToken);

            sendRequest.Method = "POST";
            using (Stream dataStream = sendRequest.GetRequestStream())
            {
                string xmlRequest = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Index>-1</Index><Phones><Phone>{sms.Phone}</Phone></Phones><Sca></Sca><Content>{sms.Message}</Content><Length>{sms.Message.Length}</Length><Reserved>1</Reserved><Date>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}</Date></request>";
                byte[] byteArray = Encoding.UTF8.GetBytes(xmlRequest);
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            using (HttpWebResponse deleteResponse = sendRequest.GetResponse() as HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(deleteResponse.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    if (responseText.Contains("<response>OK</response>"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
