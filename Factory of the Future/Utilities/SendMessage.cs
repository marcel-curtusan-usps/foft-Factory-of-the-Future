using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Factory_of_the_Future
{
    internal class SendMessage : IDisposable
    {
        private bool disposedValue;

        public string _url { get; protected set; }
        public string _statusDescription { get; protected set; }
        public string _responseDatastring { get; protected set; }
        public int _response_code { get; protected set; }
        private int GetWebExceptionStatusCode(WebException webError)
        {
            HttpWebResponse webResponse = (HttpWebResponse)webError.Response;
            if (webResponse != null)
            {
                return (int)webResponse.StatusCode;
            }
            return 0;
        }
        private int GetStatusCode(WebClient client)
        {
            FieldInfo responseField = client.GetType().GetField("m_WebResponse", BindingFlags.Instance | BindingFlags.NonPublic);

            if (responseField != null && responseField.GetValue(client) is HttpWebResponse response)
            {

                return (int)response.StatusCode;
            }
            return 0;
        }
        private string GetStatusMessage(WebClient client)
        {
            FieldInfo responseField = client.GetType().GetField("m_WebResponse", BindingFlags.Instance | BindingFlags.NonPublic);
            if (responseField != null && responseField.GetValue(client) is HttpWebResponse response)
            {
                return response.StatusDescription;
            }
            return "";
        }
        internal string Get(Uri url, JToken requestBody)
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
                using (WebClient client = new WebClient())
                {

                    //add header
                    if (!requestBody.HasValues)
                    {
                        _responseDatastring = client.DownloadString(url);
                    }
                    else
                    {
                        _responseDatastring = client.UploadString(url, JsonConvert.SerializeObject(requestBody, Formatting.None));
                    }

                    _response_code = GetStatusCode(client);
                    _statusDescription = GetStatusMessage(client);
                    //get the response from Vendor
                    if (_response_code == 201 || _response_code == 200)
                    {
                        return _responseDatastring;
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch (WebException webError)
            {
                //this will give us the response code.
                _response_code = GetWebExceptionStatusCode(webError);
                if (_response_code > 0)
                {
                    _responseDatastring = new StreamReader(webError.Response.GetResponseStream()).ReadToEnd();
                    if (!string.IsNullOrEmpty(_responseDatastring))
                    {
                        new ErrorLogger().CustomLog(_responseDatastring, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "UDP_InVaild_Message"));
                        return "";
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
            finally
            { 
             Dispose();
            }
        }
        private bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            return false;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
                _url = string.Empty;
                _statusDescription = string.Empty;
                _responseDatastring = string.Empty;
                _response_code = -0;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SendMessage()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}