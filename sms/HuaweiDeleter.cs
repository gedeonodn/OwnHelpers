using System;
using System.IO;
using System.Net;
using System.Text;

namespace sms
{
    class HuaweiDeleter
    {
        private string hostAddress;
        public HuaweiDeleter(string hostAddress)
        {
            this.hostAddress = hostAddress;
        }

        public bool Delete(int index)
        {
            Uri server = new Uri(hostAddress);

            HuaweiAuthorizer authorizer = new HuaweiAuthorizer(hostAddress);
            AuthParams authParams = authorizer.Authorize();

            // получим список смс
            HttpWebRequest deleteRequest = WebRequest.Create(new Uri(server, "/api/sms/delete-sms")) as HttpWebRequest;
            deleteRequest.CookieContainer = authParams.Container;
            deleteRequest.Headers.Add("__RequestVerificationToken", authParams.CsrfToken);

            deleteRequest.Method = "POST";
            using (Stream dataStream = deleteRequest.GetRequestStream())
            {
                string xmlRequest = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><request><Index>{index}</Index></request>";
                byte[] byteArray = Encoding.UTF8.GetBytes(xmlRequest);
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            using (HttpWebResponse deleteResponse = deleteRequest.GetResponse() as HttpWebResponse)
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
