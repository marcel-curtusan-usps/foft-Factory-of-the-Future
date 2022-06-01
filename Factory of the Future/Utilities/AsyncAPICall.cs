using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Factory_of_the_Future
{
    public class AsyncAPICall 
    {

        public static string GetImageData(Uri url)
        {

            string statusDescription;
            int response_code;
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");
                    //add header
                    byte[] result = client.DownloadData(url);
                    response_code = GetStatusCode(client, out statusDescription);
                    //get the response from Vendor
                    if (response_code == 201 || response_code == 200)
                    {
                        string returnResult = "data:image/jpeg;base64," +
                        Convert.ToBase64String(result);
                        return returnResult;
                    }

                }


            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            return null;
        }
        public static string GetData(Uri url) {

            string statusDescription;
            int response_code;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    //add header
                    result = client.DownloadString(url);
                    response_code = GetStatusCode(client, out statusDescription);
                    //get the response from Vendor
                    if (response_code == 201 || response_code == 200)
                    {
                        return result;
                    }
                    
                }
                
                
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            return result;
        }


        public static string GetDataPost(Uri url, string Post, string requestBody)
        {
            string statusDescription;
            int response_code;
            string result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    result = client.UploadString(url, Post, requestBody);
                    //result = await client.UploadStringTaskAsync(url, Post, requestBody);
                    response_code = GetStatusCode(client, out statusDescription);
                    //get the response from Vendor
                    if (response_code == 201 || response_code == 200)
                    {
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            return result;
        }
      
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

    }
}