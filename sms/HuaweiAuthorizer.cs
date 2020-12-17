using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace sms
{
    class HuaweiAuthorizer
    {
        private string hostAddress;
        public HuaweiAuthorizer(string hostAddress)
        {
            this.hostAddress = hostAddress;
        }
        public AuthParams Authorize()
        {
            Uri server = new Uri(hostAddress);

            // получим куки и токен безопасности
            CookieContainer container = new CookieContainer();
            HttpWebRequest getInboxRequest = WebRequest.Create(new Uri(server, "html/smsinbox.html")) as HttpWebRequest;
            getInboxRequest.CookieContainer = container;
            MatchCollection tokens;
            using (HttpWebResponse getInboxResponse = getInboxRequest.GetResponse() as HttpWebResponse)
            {
                using (var reader = new System.IO.StreamReader(getInboxResponse.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    string pattern = "(?<=<meta name=\\\"csrf_token\\\" content=\\\").*?(?=\\\"\\/>\\r\\n)";
                    Regex tokenRegex = new Regex(pattern);
                    tokens = tokenRegex.Matches(responseText);
                }
            }

            return new AuthParams() { Container = container, CsrfToken = tokens.ElementAt(0).ToString() };
        }
    }
}
