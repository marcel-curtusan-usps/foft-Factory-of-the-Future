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

namespace Factory_of_the_Future
{
    //class MulticastUdpClient : UdpClient
    //{
    //    private bool _stop;
    //    public MulticastUdpClient(string address, int port) : base(address, port) { }
    //    public void DisconnectAndStop()
    //    {
    //        _stop = true;
    //        Disconnect();
    //        while (IsConnected)
    //            Thread.Yield();
    //    }

    //    protected override void OnConnected()
    //    {
    //        // Start receive datagrams
    //        ReceiveAsync();
    //    }

    //    protected override void OnDisconnected()
    //    {
    //        // Wait for a while...
    //        Thread.Sleep(1000);

    //        // Try to connect again
    //        if (!_stop)
    //            Connect();
    //    }

    //    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    //    {
    //        try
    //        {
    //            string incomingData = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
    //            if (!string.IsNullOrEmpty(incomingData))
    //            {
    //                if (Global.IsValidJson(incomingData))
    //                {
    //                    JObject incomingDataJobject = JObject.Parse(incomingData);
    //                    JObject temp1 = new JObject(
    //                        new JProperty("code", "0"),
    //                        new JProperty("command", "UDP_Client"),
    //                        new JProperty("outputFormatId", "DefFormat002"),
    //                        new JProperty("outputFormatName", "Location JSON"),
    //                        new JProperty("message", "TagPosition"),
    //                        new JProperty("responseTS", DateTimeOffset.Now.ToUnixTimeMilliseconds()),
    //                        new JProperty("status", "0"),
    //                        new JProperty("tags", new JArray(incomingDataJobject))

    //                        );
    //                    //create new Connection Object
    //                    JObject conn = new JObject(new JProperty("IP_ADDRESS", ((IPEndPoint)endpoint).Address.ToString()));
    //                    try
    //                    {
    //                        Global.ProcessRecvdMsg_callback.StartProcess(temp1, conn);
    //                    }
    //                    catch (Exception r)
    //                    {
    //                        new ErrorLogger().ExceptionLog(r);
    //                    }
    //                }
    //                else
    //                {
    //                    new ErrorLogger().CustomLog(incomingData, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "UDP_InVaild_Message"));
    //                }
    //            }
    //            ReceiveAsync();
    //        }
    //        catch (Exception e)
    //        {
    //            new ErrorLogger().ExceptionLog(e);
    //        }
    //    }

    //    protected override void OnError(SocketError error)
    //    {
    //        Global.Errors = true;
    //        new ErrorLogger().CustomLog(error.ToString(), string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "UDP_Error_logs"));
    //    }
    //}
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
            Global.API_List.Where(x => (string)x.Value.Property("ID").Value == this.conid).Select(y => y.Value).ToList().ForEach(m =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(incomingData))
                    {
                        if (Global.IsValidJson(incomingData))
                        {
                            JObject incomingDataJobject = JObject.Parse(incomingData);
                            JObject temp1 = new JObject(
                                    new JProperty("code", "0"),
                                    new JProperty("command", "UDP_Client"),
                                    new JProperty("outputFormatId", "DefFormat002"),
                                    new JProperty("outputFormatName", "Location JSON"),
                                    new JProperty("message", "getTagPosition"),
                                    new JProperty("responseTS", DateTimeOffset.Now.ToUnixTimeMilliseconds()),
                                    new JProperty("status", "0"),
                                    new JProperty("tags", new JArray(incomingDataJobject))

                                    );
                            if (!(bool)m.Property("API_CONNECTED").Value)
                            {
                                m.Property("API_CONNECTED").Value = true;
                            }
                            m.Property("UPDATE_STATUS").Value = true;
                            m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                            Global.ProcessRecvdMsg_callback.StartProcess(temp1, m);

                        }
                        else
                        {
                            new ErrorLogger().CustomLog(incomingData, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "UDP_InVaild_Message"));
                        }

                    }
                    else
                    {
                        if ((bool)m.Property("API_CONNECTED").Value)
                        {
                            m.Property("API_CONNECTED").Value = false;
                        }
                        m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                        m.Property("UPDATE_STATUS").Value = true;
                    }
                }
                catch (Exception e)
                {
                    new ErrorLogger().ExceptionLog(e);
                    new ErrorLogger().CustomLog(incomingData, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "UDP_InVaild_Message"));
                    if ((bool)m.Property("API_CONNECTED").Value)
                    {
                        m.Property("API_CONNECTED").Value = false;
                    }
                    m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                    m.Property("UPDATE_STATUS").Value = true;
                }
            });
            ReceiveAsync();

        }

        protected override void OnSent(EndPoint endpoint, long sent)
        {
            // Continue receive datagrams
            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            Global.API_List.Where(x => (string)x.Value.Property("ID").Value == this.conid).Select(y => y.Value).ToList().ForEach(m =>
            {
                if ((bool)m.Property("API_CONNECTED").Value)
                {
                    m.Property("API_CONNECTED").Value = false;
                }
                m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                m.Property("UPDATE_STATUS").Value = true;
                new ErrorLogger().CustomLog(string.Concat("UDP server caught an error with code", error), string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "UDP_InVaild_Message"));
            });
        }
    }
    public class Api_Connection
    {
        public string ID;
        public int Thread_ID;
        public int Status; // 0 = Idle, 1 = Running, 2 = Stopped, 3 = Dead, 4 = Stopping(Paused)
       // public int DATA_RETRIEVE = 120000;
        public bool ConstantRefresh = true;
        public bool Stopping = false;
        public Connection API_Info = new Connection();
        public DateTime LastDownload;
        public UdpClient client;
        public UdpServer server;
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
                    if (!this.API_Info.ACTIVE_CONNECTION)
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

                } while (SleptTime < this.API_Info.DATA_RETRIEVE);

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
            this.LastDownload = DateTime.Now;
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
            if (!string.IsNullOrEmpty(this.API_Info.PORT.ToString()))
            {
                this.server = new MulticastUdpServer(IPAddress.Any, this.API_Info.PORT, this.ID);
                this.server.Start();
                this.Status = 1;
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
            string NASS_CODE = Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString();

            JObject requestBody = null;
            DateTime dtNow = DateTime.Now;
            string fdb = string.Empty;
            string lkey = string.Empty;
            if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value))
            {
                if (Global.TimeZoneConvert.TryGetValue((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                {
                    dtNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));

                }
            }
            if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_ID").Value))
            {
                fdb = (string)Global.AppSettings.Property("FACILITY_ID").Value;
            }
            if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_LKEY").Value))
            {
                lkey = (string)Global.AppSettings.Property("FACILITY_LKEY").Value;
            }
            string formatUrl = string.Empty;
            if (this.API_Info.CONNECTION_NAME.ToUpper().StartsWith("MPEWatch".ToUpper()))
            {
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("MPE_WATCH_ID").Value))
                {
                    string MpeWatch_id = (string)Global.AppSettings.Property("MPE_WATCH_ID").Value;
                    string MpeWatch_data_source = this.API_Info.MESSAGE_TYPE;

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
                    switch (this.API_Info.MESSAGE_TYPE.ToUpper())
                    {
                        case "RPG_PLAN":
                            DateTime dtEnd = modsDate.AddDays(5);
                            start_time = modsDate.ToString("MM/dd/yyyy_HH:mm:ss");
                            end_time = dtEnd.ToString("MM/dd/yyyy_HH:mm:ss");
                            formatUrl = string.Format(this.API_Info.URL, MpeWatch_id, MpeWatch_data_source, start_time, end_time);
                            break;

                        case "DPS_RUN_ESTM":
                            DateTime modStart = dtNow.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
                            DateTime modEnd = dtNow.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                            start_time = modStart.ToString("MM/dd/yyyy HH:mm:ss");
                            end_time = modEnd.ToString("MM/dd/yyyy HH:mm:ss");
                            formatUrl = string.Format(this.API_Info.URL, MpeWatch_id, MpeWatch_data_source, start_time, end_time);
                            break;

                        case "RPG_RUN_PERF":
                        default:
                            string strTimeDiff = "-" + this.API_Info.DATA_RETRIEVE;
                            Double dblTimeDiff = 0;
                            if (Double.TryParse(strTimeDiff, out Double dblDiff)) { dblTimeDiff = dblDiff; }
                            else { dblTimeDiff = -300000; }
                            if (dblTimeDiff == 0) { dblTimeDiff = -300000; }
                            DateTime endDate = dtNow;
                            DateTime startDate = endDate.AddMilliseconds(dblTimeDiff);
                            start_time = startDate.ToString("MM/dd/yyyy_HH:mm:ss");
                            end_time = endDate.ToString("MM/dd/yyyy_HH:mm:ss");
                            formatUrl = string.Format(this.API_Info.URL, MpeWatch_id, MpeWatch_data_source, start_time, end_time);
                            break;
                    }
                }
                else
                {
                    int index = (this.API_Info.URL).IndexOf("ge.");
                    formatUrl = string.Concat((this.API_Info.URL).Substring(0, (index + 3)), "get_id?group_name=client");
                }
            }
            else if (this.API_Info.CONNECTION_NAME.ToUpper().StartsWith("SV".ToUpper()))
            {
                int.TryParse(this.API_Info.HOURS_BACK, out int hours_back);
                int.TryParse(this.API_Info.HOURS_FORWARD, out int hours_forward);
                if (hours_back > 0 && hours_forward >= 0)
                {
                    string start_time = string.Concat(DateTime.Now.AddHours(-hours_back).ToString("yyyy-MM-dd'T'HH:"), "00:00");
                    string end_time = DateTime.Now.AddHours(+hours_forward).ToString("yyyy-MM-dd'T'HH:mm:ss");
                    formatUrl = string.Format(this.API_Info.URL, NASS_CODE, start_time, end_time);
                }
                else
                {
                    string start_time = string.Concat(DateTime.Now.AddHours(-10).ToString("yyyy-MM-dd'T'HH:"), "00:00");
                    string end_time = DateTime.Now.AddHours(+2).ToString("yyyy-MM-dd'T'HH:mm:ss");
                    formatUrl = string.Format(this.API_Info.URL, NASS_CODE, start_time, end_time);
                }
            }
            else if (this.API_Info.CONNECTION_NAME.ToUpper().StartsWith("Web_Camera".ToUpper()))
            {
                if (!string.IsNullOrEmpty(fdb))
                {
                    formatUrl = string.Format(this.API_Info.URL, fdb);
                }
            }
            else if (this.API_Info.CONNECTION_NAME.ToUpper().StartsWith("IV".ToUpper()))
            {
                if (!string.IsNullOrEmpty(lkey))
                {
                    string data_source = this.API_Info.MESSAGE_TYPE;
                    requestBody = new JObject(new JProperty("lkey", lkey));
                    formatUrl = string.Format(this.API_Info.URL, data_source);
                }
            }
            else if (this.API_Info.CONNECTION_NAME.ToUpper().StartsWith("SELS".ToUpper()))
            {
                string selsRT_siteid = (string)Global.AppSettings.Property("FACILITY_P2P_SITEID").Value;
                if (!string.IsNullOrEmpty(selsRT_siteid))
                {
                    string data_source = this.API_Info.MESSAGE_TYPE;
                    if (!string.IsNullOrEmpty(data_source))
                    {
                        formatUrl = string.Format(this.API_Info.URL, selsRT_siteid, data_source);
                    }
                }
            }
            else if (this.API_Info.CONNECTION_NAME.ToUpper().StartsWith("Quuppa".ToUpper()))
            {
                string data_source = this.API_Info.MESSAGE_TYPE;
                if (!string.IsNullOrEmpty(data_source))
                {
                    formatUrl = string.Format(this.API_Info.URL, data_source);
                }
            }
            else if (this.API_Info.CONNECTION_NAME.ToUpper().StartsWith("CTS".ToUpper()))
            {
                if (!string.IsNullOrEmpty(NASS_CODE))
                {
                    if (!string.IsNullOrEmpty(this.API_Info.OUTGOING_APIKEY))
                    {
                        if (!string.IsNullOrEmpty(this.API_Info.MESSAGE_TYPE))
                        {
                            formatUrl = string.Format(this.API_Info.URL, this.API_Info.MESSAGE_TYPE, this.API_Info.OUTGOING_APIKEY, NASS_CODE);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(formatUrl))
            {
                Uri tempURL = new Uri(formatUrl);
   
            }

            if (!string.IsNullOrEmpty(formatUrl) && requestBody == null)
            {

                /*
                 * Webrequest allows setting of proxy settings and Useragents
                 */
                try
                {
                   ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                   ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(formatUrl);
                    using (HttpWebResponse Response = (HttpWebResponse)request.GetResponse())
                    {
                        if (Response.StatusCode == HttpStatusCode.OK)
                        {
                            using (StreamReader reader = new System.IO.StreamReader(Response.GetResponseStream(), ASCIIEncoding.ASCII))
                            {
                                try
                                {
                                string result = reader.ReadToEnd();
                                // process date
                                if (!string.IsNullOrEmpty(result))
                                {
                                    Global.API_List.Where(x => (string)x.Value.Property("ID").Value == this.ID).Select(y => y.Value).ToList().ForEach(m =>
                                    {
                                        if (Global.IsValidJson(result))
                                        {
                                            if (result.StartsWith("{"))
                                            {
                                                JObject temp1 = JObject.Parse(result);
                                                if (formatUrl.Contains("api_page.get_id"))
                                                {
                                                    temp1.Add(new JProperty("message", "mpe_watch_id"));
                                                }

                                                if (temp1.HasValues)
                                                {
                                                    Global.ProcessRecvdMsg_callback.StartProcess(temp1, m);
                                                    m.Property("API_CONNECTED").Value = true;
                                                }
                                                else
                                                {
                                                    m.Property("API_CONNECTED").Value = false;
                                                }

                                                temp1 = null;
                                            }
                                            else if (result.StartsWith("["))
                                            {
                                                JArray tempdata = JArray.Parse(result);
                                                if (tempdata.HasValues)
                                                {
                                                    JObject temp1 = new JObject(new JProperty((string)m.Property("MESSAGE_TYPE").Value, tempdata));
                                                    Global.ProcessRecvdMsg_callback.StartProcess(temp1, m);
                                                    tempdata = null;
                                                    temp1 = null;


                                                    m.Property("API_CONNECTED").Value = true;

                                                }
                                                else
                                                {
                                                    m.Property("API_CONNECTED").Value = false;
                                                }
                                            }
                                            m.Property("UPDATE_STATUS").Value = true;
                                            m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                        }
                                        else
                                        {
                                            if ((bool)m.Property("API_CONNECTED").Value)
                                            {
                                                m.Property("API_CONNECTED").Value = false;
                                            }
                                            m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                            m.Property("UPDATE_STATUS").Value = true;
                                        }

                                    });
                                }
                                else
                                {
                                    Global.API_List.Where(x => (string)x.Value.Property("ID").Value == this.ID).Select(y => y.Value).ToList().ForEach(m =>
                                    {
                                        if ((bool)m.Property("API_CONNECTED").Value)
                                        {
                                            m.Property("API_CONNECTED").Value = false;
                                        }
                                        m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                        m.Property("UPDATE_STATUS").Value = true;
                                    });
                                }
                                }
                                catch (Exception e)
                                {
                                    new ErrorLogger().ExceptionLog(e);
                                    this.Status = 3;
                                }

                            }
                        }
                    }
                    /*
                     * LINQ To get rid of all scripts
                     * this speeds the opening of the HTM
                     * by ~500%
                     */


                }
                catch (WebException ex)
                {

                    Global.API_List.Where(x => (string)x.Value.Property("ID").Value == this.ID).Select(y => y.Value).ToList().ForEach(m =>
                    {
                        if ((bool)m.Property("API_CONNECTED").Value)
                        {
                            m.Property("API_CONNECTED").Value = false;
                        }
                        m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                        m.Property("UPDATE_STATUS").Value = true;
                    });
                    new ErrorLogger().ExceptionLog(ex);
                    // Check if Board is 404
                    if (ex.Status == WebExceptionStatus.ProtocolError & ex.Response != null)
                    {
                        // Page not found, thread has 404'd
                        HttpWebResponse Resp = (HttpWebResponse)ex.Response;
                        if (Resp.StatusCode == HttpStatusCode.NotFound)
                        {
                            this.Status = 3;
                            this.ConstantRefresh = false;

                            return;
                        }
                    }
                }
                // Thread is complete. Return to idle
                this.LastDownload = DateTime.Now;
                this.Status = 0;
            }
            if (!string.IsNullOrEmpty(formatUrl) && requestBody != null)
            {

                /*
                 * Webrequest allows setting of proxy settings and Useragents
                 */
                try
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(formatUrl);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(requestBody, Formatting.Indented));
                    }

                    using (HttpWebResponse Response = (HttpWebResponse)request.GetResponse())
                    {
                        if (Response.StatusCode == HttpStatusCode.OK)
                        {
                            using (StreamReader reader = new System.IO.StreamReader(Response.GetResponseStream(), ASCIIEncoding.ASCII))
                            {
                                string result = reader.ReadToEnd();
                                // process date
                                if (!string.IsNullOrEmpty(result))
                                {
                                    Global.API_List.Where(x => (string)x.Value.Property("ID").Value == this.ID).Select(y => y.Value).ToList().ForEach(m =>
                                    {
                                        if (Global.IsValidJson(result))
                                        {
                                            if (result.StartsWith("{"))
                                            {
                                                ////replace "dayOfMonth" to month and "hourOfDay" to day 
                                                //if (this.API_Info.CONNECTION_NAME.ToUpper().StartsWith("SV".ToUpper()))
                                                //{
                                                //    Regex.Replace(result, @"dayOfMonth", "month", RegexOptions.IgnorePatternWhitespace);
                                                //    Regex.Replace(result, @"hourOfDay", "hour", RegexOptions.IgnorePatternWhitespace);
                                                //}
                                                JObject temp1 = JObject.Parse(result);
                                                if (formatUrl.Contains("api_page.get_id"))
                                                {
                                                    temp1.Add(new JProperty("message", "mpe_watch_id"));
                                                }

                                                if (temp1.HasValues)
                                                {
                                                    Global.ProcessRecvdMsg_callback.StartProcess(temp1, m);
                                                    m.Property("API_CONNECTED").Value = true;
                                                }
                                                else
                                                {
                                                    m.Property("API_CONNECTED").Value = false;
                                                }

                                                temp1 = null;
                                            }
                                            else if (result.StartsWith("["))
                                            {
                                                JArray tempdata = JArray.Parse(result);
                                                if (tempdata.HasValues)
                                                {
                                                    JObject temp1 = new JObject(new JProperty((string)m.Property("MESSAGE_TYPE").Value, tempdata));
                                                    Global.ProcessRecvdMsg_callback.StartProcess(temp1, m);
                                                    tempdata = null;
                                                    temp1 = null;


                                                    m.Property("API_CONNECTED").Value = true;

                                                }
                                                else
                                                {
                                                    m.Property("API_CONNECTED").Value = false;
                                                }
                                            }
                                            m.Property("UPDATE_STATUS").Value = true;
                                            m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                        }
                                        else
                                        {
                                            if ((bool)m.Property("API_CONNECTED").Value)
                                            {
                                                m.Property("API_CONNECTED").Value = false;
                                            }
                                            m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                            m.Property("UPDATE_STATUS").Value = true;
                                        }

                                    });
                                }
                                else
                                {
                                    Global.API_List.Where(x => (string)x.Value.Property("ID").Value == this.ID).Select(y => y.Value).ToList().ForEach(m =>
                                    {
                                        if ((bool)m.Property("API_CONNECTED").Value)
                                        {
                                            m.Property("API_CONNECTED").Value = false;
                                        }
                                        m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                        m.Property("UPDATE_STATUS").Value = true;
                                    });
                                }

                            }
                        }
                    }
                    this.Status = 0;
                    /*
                     * LINQ To get rid of all scripts
                     * this speeds the opening of the HTM
                     * by ~500%
                     */


                }
                catch (WebException ex)
                {

                    Global.API_List.Where(x => (string)x.Value.Property("ID").Value == this.ID).Select(y => y.Value).ToList().ForEach(m =>
                    {
                        if ((bool)m.Property("API_CONNECTED").Value)
                        {
                            m.Property("API_CONNECTED").Value = false;
                        }
                        m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                        m.Property("UPDATE_STATUS").Value = true;
                    });
                    new ErrorLogger().ExceptionLog(ex);
                    // Check if Board is 404
                    if (ex.Status == WebExceptionStatus.ProtocolError & ex.Response != null)
                    {
                        // Page not found, thread has 404'd
                        HttpWebResponse Resp = (HttpWebResponse)ex.Response;
                        if (Resp.StatusCode == HttpStatusCode.NotFound)
                        {
                            this.Status = 3;
                            this.ConstantRefresh = false;

                            return;
                        }
                    }
                }
                // Thread is complete. Return to idle
                this.LastDownload = DateTime.Now;
                this.Status = 0;
            }
            if (string.IsNullOrEmpty(formatUrl) && requestBody == null)
            {
                // Thread is complete. Return to idle
                this.LastDownload = DateTime.Now;
                this.Status = 0;
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