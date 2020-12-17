using System.Net;

namespace sms
{
    class AuthParams
    {
        public CookieContainer Container { get; set; }
        public string CsrfToken { get; set; }
    }
}
