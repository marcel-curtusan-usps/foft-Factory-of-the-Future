using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
   
    class MulticastUdpServer : UdpServer
    {
        public MulticastUdpServer(IPAddress address, int port, string conid) : base(address, port, conid) { }

        protected override void OnStarted()
        {
            // Start receive datagrams
            ReceiveAsync();
        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            //Console.WriteLine("Incoming: " + Encoding.UTF8.GetString(buffer, (int)offset, (int)size));

            // Echo the message back to the sender
            //SendAsync(endpoint, buffer, 0, size);

            string incomingData = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            foreach (Connection m in AppParameters.ConnectionList.Where(x => x.Value.Id == this.conid).Select(y => y.Value))
            {
                try
                {
                    if (!string.IsNullOrEmpty(incomingData))
                    {
                        if (AppParameters.IsValidJson(incomingData))
                        {
                            JObject incomingDataJobject = JObject.Parse(incomingData);
                            JObject temp1 = new JObject(
                                    new JProperty("code", "0"),
                                    new JProperty("command", "UDP_Client"),
                                    new JProperty("outputFormatId", "DefFormat002"),
                                    new JProperty("outputFormatName", "Location JSON"),
                                    new JProperty("message", m.MessageType),
                                    new JProperty("responseTS", DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                                    new JProperty("status", "0"),
                                    new JProperty("tags", new JArray(incomingDataJobject))

                                    );
                            m.ApiConnected = true;
                            m.LasttimeApiConnected = DateTime.Now;
                            m.UpdateStatus = true;
                            Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(temp1, Formatting.None), m.MessageType,this.conid));

                        }
                        else
                        {
                            new ErrorLogger().CustomLog(incomingData, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "UDP_InVaild_Message"));
                        }

                    }
                    else
                    {
                        m.ApiConnected = false;
                        m.LasttimeApiConnected = DateTime.Now;
                        m.UpdateStatus = true;
                    }
                }
                catch (Exception e)
                {
                    new ErrorLogger().ExceptionLog(e);
                    new ErrorLogger().CustomLog(incomingData, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "UDP_InVaild_Message"));
                    m.ApiConnected = false;
                    m.LasttimeApiConnected = DateTime.Now;
                    m.UpdateStatus = true;
                }
            };
            ReceiveAsync();

        }

        protected override void OnSent(EndPoint endpoint, long sent)
        {
            // Continue receive datagrams
            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            foreach (Connection m in AppParameters.ConnectionList.Where(x => x.Value.Id == this.conid).Select(y => y.Value))
            {
                m.ApiConnected = false;
                m.LasttimeApiConnected = DateTime.Now;
                m.UpdateStatus = true;
                new ErrorLogger().CustomLog(string.Concat("UDP server caught an error with code", error), string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "UDP_InVaild_Message"));

            }
        }
    }
    public class Api_Connection
    {
        public string ID;
        internal string MessageType = string.Empty;
        public int Thread_ID;
        public int Status; // 0 = Idle, 1 = Running, 2 = Stopped, 3 = Dead, 4 = Stopping(Paused)
       // public int DATA_RETRIEVE = 120000;
        public bool ConstantRefresh = true;
        public bool Stopping = false;
        public DateTime DownloadDatetime;
        public bool Connected;
        public UdpClient client;
        public UdpServer server;
        internal Connection ConnectionInfo;
        public string StatusInfo = "";

        // public DateTime DateAdded;

        //UDP
        public void DownloadLoop()
        {
            do
            {
                // We're going to sleep in sections
                // if user cancels the thread checking,
                // we want to be able to end this thread
                int SleptTime = 0;
                do
                {
                    SleptTime += 1000;
                    Thread.Sleep(1000);
                    if (!ConnectionInfo.ActiveConnection)
                    {
                        this.Status = 4;
                        break;
                    }
                    if (this.ConstantRefresh == false)
                    {
                        // If user wanted to stop watching this thread
                        // while thread was resting, we will exit
                        break;
                    }
                    if (this.Status == 4)
                    {
                        return;
                    }

                } while (SleptTime < ConnectionInfo.DataRetrieve);

                if (this.ConstantRefresh == true)
                {
                    this.Download();
                }

            } while (this.Status == 0 || this.Status == 1);

        }
        public void _ThreadRefresh()
        {
            /*
             * This starts the automatic downloading on
             * the specified board. The board will 
             * automatically redownload upon reaching
             * the timer interval.
             */
            this.DownloadDatetime = DateTime.Now;
            Thread RefreshLoopThread = new Thread(new ThreadStart(DownloadLoop));
            RefreshLoopThread.IsBackground = true;
            RefreshLoopThread.Start();
        }
        public void _ThreadDownload()
        {
            Thread DownloadThread = new Thread(new ThreadStart(Download));
            DownloadThread.IsBackground = true;
            DownloadThread.Start();
        }
        public void _UDPThreadListener()
        {
            Thread ListenerThread = new Thread(new ThreadStart(UDPInit));
            ListenerThread.IsBackground = true;
            ListenerThread.Start();
        }
        public void _StopUDPListener()
        {
            Thread StopListenerThread = new Thread(new ThreadStart(UDPStop));
            StopListenerThread.IsBackground = true;
            StopListenerThread.Start();
        }
        public void _StartUDPListener()
        {
            Thread StartListenerThread = new Thread(new ThreadStart(UDPStart));
            StartListenerThread.IsBackground = true;
            StartListenerThread.Start();
        }
        private void UDPStart()
        {
            //Start UDP server
            if (this.server != null)
            {
                this.server.Start();
            }
            else
            {
                this._UDPThreadListener();
            }
            
            this.Stopping = false;
            this.Status = 1;
           
        }
        private void UDPStop()
        {
            //stop UDP server
            this.server.Stop();
            this.Stopping = true;
            this.Status = 2;
        }
        private void UDPInit()
        {
            // start UDP Server
            if (!string.IsNullOrEmpty(ConnectionInfo.Port.ToString()))
            {
                if (ConnectionInfo.Port > 0)
                {
                    this.server = new MulticastUdpServer(IPAddress.Any, (int)ConnectionInfo.Port, ID);
                    this.server.Start();
                    this.Status = 1;
                }
            }
            
        }
        public void UDPDelete()
        {
            Thread DeleteThread = new Thread(new ThreadStart(UDPStop));
            DeleteThread.IsBackground = true;
            DeleteThread.Start();

        }
        public void Download()
        {

            /*
             * If thread is running, I don't 
             * want to start it again
             */

            if (this.Status == 1)
            {
                return;
            }

            /*
             * Set status to 1
             * Prevents the same board from running
             * on multiple threads. We want to update
             * the last runtime on this thread
             */
            this.Status = 1;
            string NASS_CODE = AppParameters.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString();

            JObject requestBody = null;
            DateTime dtNow = DateTime.Now;
            string fdb = string.Empty;
            string lkey = string.Empty;
            MessageType = ConnectionInfo.MessageType;
            if (!string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("FACILITY_TIMEZONE").Value))
            {
                if (AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                {
                    dtNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));

                }
            }
            if (!string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("FACILITY_ID").Value))
            {
                fdb = (string)AppParameters.AppSettings.Property("FACILITY_ID").Value;
            }
            if (!string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("FACILITY_LKEY").Value))
            {
                lkey = (string)AppParameters.AppSettings.Property("FACILITY_LKEY").Value;
            }
            string formatUrl = string.Empty;
            if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("MPEWatch".ToUpper()))
            {
                if (!string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("MPE_WATCH_ID").Value))
                {
                    string MpeWatch_id = (string)AppParameters.AppSettings.Property("MPE_WATCH_ID").Value;
                    string MpeWatch_data_source = ConnectionInfo.MessageType;

                    int currentHour = dtNow.Hour;
                    DateTime modsDate = dtNow;
                    if (currentHour >= 0 && currentHour < 7)
                    {
                        modsDate = dtNow.Date.AddDays(-1);
                    }
                    else
                    {
                        modsDate = dtNow.Date;
                    }
                    string start_time = "";
                    string end_time = "";
                    switch (ConnectionInfo.MessageType.ToUpper())
                    {
                        case "RPG_PLAN":
                            DateTime dtEnd = modsDate.AddDays(5);
                            start_time = modsDate.ToString("MM/dd/yyyy_HH:mm:ss");
                            end_time = dtEnd.ToString("MM/dd/yyyy_HH:mm:ss");
                            formatUrl = string.Format(ConnectionInfo.Url, MpeWatch_id, MpeWatch_data_source, start_time, end_time);
                            break;
                        case "DPS_RUN_ESTM":
                            DateTime modStart = dtNow.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
                            DateTime modEnd = dtNow.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                            start_time = modStart.ToString("MM/dd/yyyy HH:mm:ss");
                            end_time = modEnd.ToString("MM/dd/yyyy HH:mm:ss");
                            formatUrl = string.Format(ConnectionInfo.Url, MpeWatch_id, MpeWatch_data_source, start_time, end_time);
                            break;
                        case "RPG_RUN_PERF":
                            string strTimeDiff = "-" + ConnectionInfo.DataRetrieve;
                            Double dblTimeDiff = 0;
                            if (Double.TryParse(strTimeDiff, out Double dblDiff)) { dblTimeDiff = dblDiff; }
                            else { dblTimeDiff = -300000; }
                            if (dblTimeDiff == 0) { dblTimeDiff = -300000; }
                            DateTime endDate = dtNow;
                            DateTime startDate = endDate.AddMilliseconds(dblTimeDiff);
                            start_time = startDate.ToString("MM/dd/yyyy_HH:mm:ss");
                            end_time = endDate.ToString("MM/dd/yyyy_HH:mm:ss");
                            formatUrl = string.Format(ConnectionInfo.Url, MpeWatch_id, MpeWatch_data_source, start_time, end_time);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    MessageType = "mpe_watch_id";
                    int index = ConnectionInfo.Url.IndexOf("ge.");
                    formatUrl = string.Concat(ConnectionInfo.Url.Substring(0, (index + 3)), "get_id?group_name=client");
                }
            }
            else if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("SV".ToUpper()))
            {

                if (ConnectionInfo.HoursBack > 0 && ConnectionInfo.HoursForward >= 0)
                {
                    string start_time = string.Concat(DateTime.Now.AddHours(-ConnectionInfo.HoursBack).ToString("yyyy-MM-dd'T'HH:"), "00:00");
                    string end_time = DateTime.Now.AddHours(+ConnectionInfo.HoursForward).ToString("yyyy-MM-dd'T'HH:mm:ss");
                    formatUrl = string.Format(ConnectionInfo.Url, NASS_CODE, start_time, end_time);
                }
                else
                {
                    string start_time = string.Concat(DateTime.Now.AddHours(-10).ToString("yyyy-MM-dd'T'HH:"), "00:00");
                    string end_time = DateTime.Now.AddHours(+2).ToString("yyyy-MM-dd'T'HH:mm:ss");
                    formatUrl = string.Format(ConnectionInfo.Url, NASS_CODE, start_time, end_time);
                }
            }
            else if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("Web_Camera".ToUpper()))
            {
                if (!string.IsNullOrEmpty(fdb))
                {
                    formatUrl = string.Format(ConnectionInfo.Url, fdb);
                }
            }
            else if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("IV".ToUpper()))
            {
                if (!string.IsNullOrEmpty(lkey))
                {
                    requestBody = new JObject(new JProperty("lkey", lkey));
                    formatUrl = string.Format(ConnectionInfo.Url, ConnectionInfo.MessageType);
                }
            }
            else if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("SELS".ToUpper()))
            {
                string selsRT_siteid = (string)AppParameters.AppSettings.Property("FACILITY_P2P_SITEID").Value;
                if (!string.IsNullOrEmpty(selsRT_siteid))
                {
                    if (!string.IsNullOrEmpty(ConnectionInfo.MessageType))
                    {
                        formatUrl = string.Format(ConnectionInfo.Url, selsRT_siteid, ConnectionInfo.MessageType);
                    }
                }
            }
            else if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("Quuppa".ToUpper()))
            {
                formatUrl = string.Format(ConnectionInfo.Url, ConnectionInfo.MessageType);
            }
            if (!string.IsNullOrEmpty(formatUrl))
            {
                try
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(formatUrl);
                    if (requestBody != null)
                    {
                        request.ContentType = "application/json";
                        request.Method = "POST";
                        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                        {
                            streamWriter.Write(JsonConvert.SerializeObject(requestBody, Formatting.Indented));
                        }
                    }
                   
                    using (HttpWebResponse Response = (HttpWebResponse)request.GetResponse())
                    {
                        if (Response.StatusCode == HttpStatusCode.OK)
                        {
                            using (StreamReader reader = new System.IO.StreamReader(Response.GetResponseStream(), ASCIIEncoding.ASCII))
                            {
                                // Thread is complete. Return to idle
                                DownloadDatetime = dtNow;
                                Connected = true;
                                string responseData = reader.ReadToEnd();
                                if (!string.IsNullOrEmpty(responseData))
                                {
                                    Task.Run(() => new ProcessRecvdMsg().StartProcess(responseData, MessageType, ConnectionInfo.Id));
                                }
                                
                               
                                //result = JToken.Parse(reader.ReadToEnd());
                                //// process date
                                //if (result is JArray || result is JObject)
                                //{
                                //    if (formatUrl.Contains("api_page.get_id"))
                                //    {
                                //        new ProcessRecvdMsg().StartProcess(result, "mpe_watch_id");
                                //    }
                                //    else
                                //    {
                                //        new ProcessRecvdMsg().StartProcess(result, ConnectionInfo.MessageType);
                                //    }
                                //}
                                Thread.Sleep(100);
                                Task.Run(() => updateConnection(this));
                            }
                        }
                        else
                        {
                            Connected = false;
                            Thread.Sleep(100);
                            Task.Run(() => updateConnection(this));
                        }
                    }
                }
                catch (WebException ex)
                {
                    new ErrorLogger().ExceptionLog(ex);
                    // Check if Board is 404
                    if (ex.Status == WebExceptionStatus.ProtocolError & ex.Response != null)
                    {
                        // Page not found, thread has 404'd
                        //HttpWebResponse Resp = (HttpWebResponse)ex.Response;
                       
                            this.Status = 3;
                            this.ConstantRefresh = false;
                            this.Connected = false;
                            Task.Run(() => updateConnection(this));
                            return;
                        
                    }
                }
                catch (Exception e)
                {
                    new ErrorLogger().ExceptionLog(e);
                    
                    this.Connected = false;
                    Task.Run(() => updateConnection(this));
                }
            }
            this.Status = 0;
        }

        private void updateConnection(Api_Connection api_Connection)
        {
            try
            {
                foreach (Connection m in AppParameters.ConnectionList.Where(x => x.Value.Id == api_Connection.ID).Select(y => y.Value))
                {
                  
                    m.ApiConnected = api_Connection.Connected;
                    m.LasttimeApiConnected= api_Connection.DownloadDatetime;
                    m.UpdateStatus = true;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
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
        private string GetDataFromStream(WebResponse response)
        {
            var result = string.Empty;
            try
            {
                if (response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return result;
            }
            
            
        }
        private void _ThreadStop()
        {
            this.Stopping = true;
            this.ConstantRefresh = false;
            do
            {
                Thread.Sleep(100);
            } while (this.Status == 1);

            this.Status = 2;
        }
        private void _ThreadDelete()
        {
            this.Stopping = true;

            do
            {
                Thread.Sleep(100);
            } while (this.Status == 2);
        }
        public void Stop()
        {
            Thread StopThread = new Thread(new ThreadStart(_ThreadStop));
            StopThread.IsBackground = true;
            StopThread.Start();
        }
        public void Stop_Delete()
        {
            Thread DeleteThread = new Thread(new ThreadStart(_ThreadDelete));
            DeleteThread.IsBackground = true;
            DeleteThread.Start();

        }
      
    }
}