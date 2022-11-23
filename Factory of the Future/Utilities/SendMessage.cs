using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class SendMessage
    {
        private static int GetWebExceptionStatusCode(WebException webError, out string statusDescription)
        {
            HttpWebResponse webResponse = (HttpWebResponse)webError.Response;
            if (webResponse != null)
            {
                statusDescription = webResponse.StatusDescription;
                return (int)webResponse.StatusCode;
            }
            statusDescription = null;
            return 0;
        }

        private static int GetStatusCode(WebClient client, out string statusDescription)
        {
            FieldInfo responseField = client.GetType().GetField("m_WebResponse", BindingFlags.Instance | BindingFlags.NonPublic);

            if (responseField != null)
            {
                if (responseField.GetValue(client) is HttpWebResponse response)
                {
                    statusDescription = response.StatusDescription;
                    return (int)response.StatusCode;
                }
            }

            statusDescription = null;
            return 0;
        }

        internal static string SV_Get(string url)
        {
            string statusDescription;
            string responseDatastring;
            int response_code;
            try
            {
                using (WebClient client = new WebClient())
                {
                    Uri parURL = new Uri(url);
                    //add header
                    responseDatastring = client.DownloadString(parURL);
                    response_code = GetStatusCode(client, out statusDescription);
                    //get the response from Vendor
                    if (response_code == 201 || response_code == 200)
                    {
                        return responseDatastring;
                    }
                    else
                    {
                        return "[]";
                    }
                }
            }
            catch (WebException webError)
            {
                //this will give us the response code.
                response_code = GetWebExceptionStatusCode(webError, out statusDescription);
                if (response_code > 0)
                {
                    responseDatastring = new StreamReader(webError.Response.GetResponseStream()).ReadToEnd();
                    if (!string.IsNullOrEmpty(responseDatastring))
                    {
                    }
                }
                return "[]";
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "[]";
            }
        }
    }
}