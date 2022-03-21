using System;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace Factory_of_the_Future
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                GlobalConfiguration.Configure(WebApiConfig.Register);
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            //if (authCookie != null)
            //{
            //    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            //    var principal = Context.Cache[authTicket.Name] as IPrincipal;
            //    if (!authTicket.Expired && principal != null)
            //    {
            //        Context.User = principal;
            //    }
            //}

            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["APIAuthorization"];
            string credentials = string.Empty;
            if (authHeader != null)
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                if (authHeader.StartsWith("/"))
                {
                    var identity = new GenericIdentity("APIUser");
                    SetPrincipal(new GenericPrincipal(identity, null));
                }
                else
                {
                    credentials = encoding.GetString(Convert.FromBase64String(authHeader));
                }
                int indexofbasic = credentials.IndexOf(' ');
                if (indexofbasic == 5 && credentials.Substring(0, indexofbasic) == "basic")
                {
                    var authHeaderVal = AuthenticationHeaderValue.Parse(credentials);
                    // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                    if (authHeaderVal.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && authHeaderVal.Parameter != null)
                    {
                        AuthenticateUser(authHeaderVal.Parameter);
                    }
                }
            }
        }

        private static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        private void AuthenticateUser(string credentials)
        {
            try
            {
                if (credentials == (AppParameters.AppSettings.ContainsKey("API_KEY") ? AppParameters.AppSettings.Property("API_KEY").Value.ToString() : ""))
                {
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    credentials = encoding.GetString(Convert.FromBase64String(credentials));

                    int separator = credentials.IndexOf(':');
                    string name = credentials.Substring(0, separator);
                    string password = credentials.Substring(separator + 1);
                    var identity = new GenericIdentity(name);
                    SetPrincipal(new GenericPrincipal(identity, null));
                }
                else
                {
                    // Invalid username or password.
                    HttpContext.Current.Response.StatusCode = 401;
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                HttpContext.Current.Response.StatusCode = 401;
            }
        }
    }
}
