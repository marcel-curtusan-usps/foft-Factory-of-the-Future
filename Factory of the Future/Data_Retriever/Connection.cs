using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    public delegate void OnWsMessage(string msg);
    public delegate void OnWsEvent();
    public delegate void ThreadListenerCall();
    class MulticastUdpServer : UdpServer
    {
        public MulticastUdpServer(IPAddress address, int port, Connection conn) : base(address, port, conn) { }

        protected override void OnStarted()
        {
            // Start receive datagrams
            Conn.Status = "Running";
            Conn.ActiveConnection = true;
            Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(Conn)).ConfigureAwait(false);
            ReceiveAsync();
        }

        protected override async void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {

            string incomingData = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            try
            {
                if (!string.IsNullOrEmpty(incomingData))
                {

                    JToken incomingDataJobject = JToken.Parse(incomingData);
                    if (incomingDataJobject.HasValues && incomingDataJobject != null)
                    {
                        JToken temp1 = new JObject
                        {
                            ["code"] = "0",
                            ["command"] = "UDP_Client",
                            ["outputFormatId"] = "DefFormat002",
                            ["outputFormatName"] = "Location JSON",
                            ["message"] = Conn.MessageType,
                            ["responseTS"] = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                            ["status"] = "0",
                            ["tags"] = new JArray(incomingDataJobject)
                        };
                      await  Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(temp1, Formatting.None), Conn.MessageType, Conn.Id)).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                await Task.Run(() => new ErrorLogger().CustomLog(incomingData, string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "UDP_InVaild_Message"))).ConfigureAwait(false);
            }

            ReceiveAsync();

        }

        protected override void OnSent(EndPoint endpoint, long sent)
        {
            // Continue receive datagrams
            ReceiveAsync();
        }

        protected override void OnError(SocketError error)
        {
            Conn.Status = "No data";
            Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(Conn)).ConfigureAwait(false);
        }
    }

    public class Api_Connection : IDisposable
    {
        public string ID;
        internal string MessageType = string.Empty;
        public int Thread_ID;
        public int Status;  // 0 = Idle/Active, 1 = Running, 2 = Stopped/Deactivated, 3 = Invalid URL, 4 = No data, 5 = Stopping(Paused)
        public bool ConstantRefresh = false;
        public bool Stopping = false;
        public DateTime DownloadDatetime;
        //public bool Connected;
        public UdpClient client;
        public UdpServer udpserver;
        public TcpServer tcpServer;
        public WebSocketInstanceHandler webSocketIntanceHandler;
        internal Connection ConnectionInfo;
        private bool disposedValue;
        public JObject requestBody { get; protected set; }
        public string NASS_CODE { get; protected set; }
        public string fdb { get; protected set; }
        public string lkey { get; protected set; }
        public string formatUrl { get; protected set; }
        public string responseData { get; protected set; }

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


                    if (!ConstantRefresh)
                    {
                        // If user wanted to stop watching this thread
                        // while thread was resting, we will exit

                        break;
                    }
                    if (Status == 5)
                    {
                        return;
                    }

                } while (SleptTime < ConnectionInfo.DataRetrieve);

                if (ConstantRefresh)
                {
                    Download();
                }

            } while (Status == 0 || Status == 1 || Status == 3 || Status == 4);
            if (Status == 2 || Status == 5)
            {
                ConnectionInfo.Status = "Stopped/Deactivated";
                Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(ConnectionInfo)).ConfigureAwait(false);
            }

        }
        public void _ThreadRefresh()
        {
            /*
             * This starts the automatic downloading on
             * the specified board. The board will 
             * automatically redownload upon reaching
             * the timer interval.
             */
            Status = 0;
            DownloadDatetime = DateTime.Now;
            Thread RefreshLoopThread = new Thread(new ThreadStart(DownloadLoop));
            RefreshLoopThread.IsBackground = true;
            RefreshLoopThread.Start();
        }
        public void _ThreadDownload()
        {
            Stopping = false;
            DownloadDatetime = DateTime.Now;
            Thread DownloadThread = new Thread(new ThreadStart(Download));
            DownloadThread.IsBackground = true;
            DownloadThread.Start();
        }
        internal void _TCPThreadListener()
        {
            Thread ListenerThread = new Thread(new ThreadStart(TCPInit));
            ListenerThread.IsBackground = true;
            ListenerThread.Start();
        }
        public void _UDPThreadListener()
        {
            Thread ListenerThread = new Thread(new ThreadStart(UDPInit));
            ListenerThread.IsBackground = true;
            ListenerThread.Start();
        }
        public void _WSThreadListener()
        {
            Thread ListenerThread = new Thread(new ThreadStart(WSInit));
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
        public void _StopTCPListener()
        {
            Thread StopListenerThread = new Thread(new ThreadStart(TCPStop));
            StopListenerThread.IsBackground = true;
            StopListenerThread.Start();
        }
        public void _StartTCPListener()
        {
            Thread StartListenerThread = new Thread(new ThreadStart(TCPStart));
            StartListenerThread.IsBackground = true;
            StartListenerThread.Start();
        }
        public void _StopWSListener()
        {
            Thread DeleteThread = new Thread(new ThreadStart(WSStop));
            DeleteThread.IsBackground = true;
            DeleteThread.Start();

        }
        private void UDPStart()
        {
            //Start UDP server
            if (udpserver != null)
            {
                udpserver.Start();
            }
            else
            {
                _UDPThreadListener();
            }

            Stopping = false;
            Status = 1;

        }
        private void TCPStart()
        {
            //Start TCP server
            if (tcpServer != null)
            {
                tcpServer.Start();
            }
            else
            {
                _TCPThreadListener();
            }

            Stopping = false;
            Status = 1;

        }
        public void DarvisWSMessage(string message)
        {
            // temporary workaround for socket io without a socket io library

            if (message.StartsWith("42"))
            {
                Task.Run(() => new ProcessRecvdMsg().ProcessDarvisAlert42(message.Substring(2)));
            }

        }
        public void DarvisClose()
        {
            ConnectionInfo.ApiConnected = false;
            ConnectionInfo.Status = "Deactivated";
            Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(ConnectionInfo)).ConfigureAwait(false);
        }
        public void DarvisOpen()
        {
            ConnectionInfo.ApiConnected = true;
            ConnectionInfo.Status = "Running";
            Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(ConnectionInfo)).ConfigureAwait(false);
        }
    
        public void UDPStop()
        {
            //stop UDP server
            if (udpserver != null)
            {
                udpserver.Stop();
                Stopping = true;
                Status = 2;
            }
        }
        public void TCPStop()
        {
            //stop UDP server
            if (tcpServer != null)
            {
                tcpServer.Stop();
                Stopping = true;
                Status = 2;
            }
        }
        public void WSStop()
        {
            //stop WS instance
            try
            {
                if (!String.IsNullOrEmpty(ConnectionInfo.Url))
                {

                    webSocketIntanceHandler.Close(ConnectionInfo.ConnectionName);
                    Status = 2;

                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }
        private void WSInit()
        {
            webSocketIntanceHandler = new WebSocketInstanceHandler();
            OnWsMessage messageEvent = null;
            OnWsEvent closeEvent = null;
            OnWsEvent openEvent = null;

            switch (ConnectionInfo.MessageType.ToUpper())
            {
                case "WSDARVIS":
                    messageEvent = DarvisWSMessage;
                    closeEvent = DarvisClose;
                    openEvent = DarvisOpen;
                    break;
            }
            try
            {
                if (!String.IsNullOrEmpty(ConnectionInfo.Url) && messageEvent != null)
                {
                    webSocketIntanceHandler.CreateWSInstance(ConnectionInfo.ConnectionName, ConnectionInfo.Url, messageEvent, closeEvent, openEvent);
                    webSocketIntanceHandler.Connect(ConnectionInfo.ConnectionName);
                    Stopping = false;
                    if (webSocketIntanceHandler.Connected(ConnectionInfo.ConnectionName))
                    {
                        ConnectionInfo.ApiConnected = true;
                        ConnectionInfo.Status = "Running";
                        Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(ConnectionInfo)).ConfigureAwait(false);
                        Status = 1;
                    }
                }

            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }

        }
        private void UDPInit()
        {
            // start UDP Server
            if (!string.IsNullOrEmpty(ConnectionInfo.Port.ToString()))
            {
                if (ConnectionInfo.Port > 0)
                {
                    ConnectionInfo.IpAddress = AppParameters.ServerIpAddress;
                    //udpserver = new UdpServer(IPAddress.Any, ConnectionInfo.Port, ConnectionInfo);
                    udpserver = new MulticastUdpServer(IPAddress.Any, ConnectionInfo.Port, ConnectionInfo);
                    udpserver.Start();
                    Status = 1;
                }
            }

        }
        private void TCPInit()
        {
            // start UDP Server
            if (!string.IsNullOrEmpty(ConnectionInfo.Port.ToString()))
            {
                if (ConnectionInfo.Port > 0)
                {
                    ConnectionInfo.IpAddress = AppParameters.ServerIpAddress;
                    tcpServer = new TcpServer(ConnectionInfo.IpAddress, ConnectionInfo.Port, ConnectionInfo);
                    tcpServer.Start();
                    Status = 1;
                }
            }

        }
        public void UDPDelete()
        {
            Thread DeleteThread = new Thread(new ThreadStart(UDPStop));
            DeleteThread.IsBackground = true;
            DeleteThread.Start();

        }
        public void TCPDelete()
        {
            Thread DeleteThread = new Thread(new ThreadStart(TCPStop));
            DeleteThread.IsBackground = true;
            DeleteThread.Start();

        }
        public void WSDelete()
        {
            Thread DeleteThread = new Thread(new ThreadStart(WSStop));
            DeleteThread.IsBackground = true;
            DeleteThread.Start();

        }
        public void Download()
        {

            /*
             * If thread is running, I don't 
             * want to start it again
             */

            if (Status == 1)
            {
                return;
            }

            /*
             * Set status to 1
             * Prevents the same board from running
             * on multiple threads. We want to update
             * the last runtime on this thread
             */
            Status = 1;
            ConnectionInfo.ApiConnected = true;
            ConnectionInfo.ActiveConnection = true;

            if (ConnectionInfo.Status != "Running")
            {
                ConnectionInfo.Status = "Running";
                Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(ConnectionInfo)).ConfigureAwait(false);
               
            }
        
            try
            {
                NASS_CODE = AppParameters.AppSettings.FACILITY_NASS_CODE;

                requestBody = new JObject();
                DateTime dtNow = DateTime.Now;
                fdb = string.Empty;
                lkey = string.Empty;
                formatUrl = string.Empty;
                MessageType = ConnectionInfo.MessageType;
                //start login connection status
                string connStatus = string.Concat(DateTime.Now," Downloading Data from: ", ConnectionInfo.ConnectionName," message type: ", ConnectionInfo.MessageType);
                //end login
                Task.Run(() => new ErrorLogger().CustomLog(connStatus, string.Concat( "API_ConnectionStatus"))).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(AppParameters.AppSettings.FACILITY_TIMEZONE))
                {
                    if (AppParameters.TimeZoneConvert.TryGetValue(AppParameters.AppSettings.FACILITY_TIMEZONE, out string windowsTimeZoneId))
                    {
                        dtNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));

                    }
                }
                if (!string.IsNullOrEmpty(AppParameters.AppSettings.FACILITY_ID))
                {
                    fdb = AppParameters.AppSettings.FACILITY_ID;
                }
                if (!string.IsNullOrEmpty(AppParameters.AppSettings.FACILITY_LKEY))
                {
                    lkey = AppParameters.AppSettings.FACILITY_LKEY;
                }
                if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("MPEWatch".ToUpper()))
                {

                    if (!string.IsNullOrEmpty(AppParameters.AppSettings.MPE_WATCH_ID))
                    {
                        if (ConnectionInfo.Url.Length > 25)
                        {
                            string MpeWatch_id = AppParameters.AppSettings.MPE_WATCH_ID;
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
                    }
                    else
                    {
                        if (ConnectionInfo.Url.Length > 25)
                        {
                            if (ConnectionInfo.MessageType == "rpg_run_perf")
                            {
                                MessageType = "mpe_watch_id";
                                int index = ConnectionInfo.Url.IndexOf("ge.");
                                formatUrl = string.Concat(ConnectionInfo.Url.Substring(0, (index + 3)), "get_id?group_name=client");
                            }
                        }
                    }

                }
                else if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("SV".ToUpper()))
                {
                    //if (AppParameters.SVcontainerTypeCode.Any())
                    //{
                    //    Uri rurl = new Uri(AppParameters.AppSettings["SV_CODETYPE"].ToString());
                    //    Uri ruriResult;
                    //    bool rURLValid = Uri.TryCreate(formatUrl, UriKind.Absolute, out ruriResult) && (rurl.Scheme == Uri.UriSchemeHttp || rurl.Scheme == Uri.UriSchemeHttps);
                    //    if (rURLValid)
                    //    {
                    //        Task.Run(() => new ProcessRecvdMsg().StartProcess(new SendMessage().Get(ruriResult, requestBody), "SVcontainerTypeCode", ConnectionInfo.Id)).ConfigureAwait(false);
                    //    }
                    //}

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
                    if (ConnectionInfo.MessageType.ToUpper().EndsWith("Stills".ToUpper()))
                    {
                        foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                        {
                            cs.Locators.Where(f => f.Value.Properties.TagType != null &&
                                f.Value.Properties.TagType.StartsWith("Camera")).Select(y => y.Value).ToList().ForEach(Camera =>
                                {
                                    try
                                    {
                                        //Update when change to FQN
                                        formatUrl = string.Format(ConnectionInfo.Url, Camera.Properties.Name.Trim());
                                        string imageBase64 = AppParameters.NoImage;
                                        if (!string.IsNullOrEmpty(formatUrl))
                                        {
                                            if (ConnectionInfo.ActiveConnection)
                                            {
                                                using (WebClient client = new WebClient())
                                                {
                                                    client.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");
                                                    byte[] result = client.DownloadData(formatUrl);
                                                    imageBase64 = "data:image/jpeg;base64," + Convert.ToBase64String(result);


                                                }
                                            }
                                        }
                                        if (Camera.Properties.CameraData.Base64Image != imageBase64)
                                        {
                                            Camera.Properties.CameraData.Base64Image = imageBase64;
                                            Task.Run(() => FOTFManager.Instance.BroadcastCameraStatus(Camera, cs.Id)).ConfigureAwait(false);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        new ErrorLogger().ExceptionLog(e);
                                    }
                                });
                        }
                        DownloadDatetime = dtNow;
                        //Task.Run(() => new ProcessRecvdMsg().StartProcess(cameraImages, MessageType, ConnectionInfo.Id));
                        Status = 0;
                        return;
                    }
                    else if (ConnectionInfo.MessageType.ToUpper().EndsWith("Cameras".ToUpper()))
                    {
                        if (!string.IsNullOrEmpty(fdb))
                        {
                            formatUrl = string.Format(ConnectionInfo.Url, fdb);

                        }
                    }
                }
                else if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("MPE_Alerts".ToUpper()))
                {
                    formatUrl = string.Format(ConnectionInfo.Url);
                }
                else if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("IV".ToUpper()))
                {
                    if (!string.IsNullOrEmpty(lkey))
                    {
                        requestBody = new JObject { ["lkey"] = lkey };
                        formatUrl = string.Format(ConnectionInfo.Url, ConnectionInfo.MessageType);
                    }
                }
                else if (ConnectionInfo.ConnectionName.ToUpper().StartsWith("SELS".ToUpper()))
                {
                    switch (ConnectionInfo.MessageType.ToUpper())
                    {
                        case "GETTYPES":
                            formatUrl = string.Format(ConnectionInfo.Url, ConnectionInfo.MessageType);
                            break;
                        case "TAGIDTOEMPID":
                            formatUrl = string.Format(ConnectionInfo.Url, ConnectionInfo.MessageType);
                            break;
                        case "GETIVEMPDATA":
                            DateTime Date = dtNow;
                            formatUrl = string.Format(ConnectionInfo.Url, ConnectionInfo.MessageType , Date.ToString("yyyy-MM-dd"));
                            break;
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
                        Uri url = new Uri(formatUrl);
                        Uri uriResult;
                        bool URLValid = Uri.TryCreate(formatUrl, UriKind.Absolute, out uriResult) && (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps);
                        if (URLValid)
                        {
                            Task.Run(() => new ProcessRecvdMsg().StartProcess( new SendMessage().Get(uriResult, requestBody), MessageType, ConnectionInfo.Id)).ConfigureAwait(false);
                        }
                        else
                        {
                            Status = 3;
                            ConnectionInfo.ApiConnected = false;
                            if (ConnectionInfo.Status != "Invalid URL")
                            {
                                ConnectionInfo.Status = "Invalid URL";
                                Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(ConnectionInfo)).ConfigureAwait(false);
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

                            Status = 0;
                            ConnectionInfo.ApiConnected = false;
                            if (ConnectionInfo.Status != "No data")
                            {
                                ConnectionInfo.Status = "No data";
                                Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(ConnectionInfo)).ConfigureAwait(false);
                            }
                            //Task.Run(() => updateConnection(this));
                            return;

                        }
                    }
                    catch (Exception e)
                    {
                        new ErrorLogger().ExceptionLog(e);
                        Status = 0;
                        ConnectionInfo.ApiConnected = false;
                        if (ConnectionInfo.Status != "No data")
                        {
                            ConnectionInfo.Status = "No data";
                            Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(ConnectionInfo)).ConfigureAwait(false); ;
                        }
                    }
                }
                else
                {
                    Status = 0;
                    ConnectionInfo.ApiConnected = false;
                    if (ConnectionInfo.Status != "Invaild URL")
                    {
                        ConnectionInfo.Status = "Invaild URL";
                        Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(ConnectionInfo)).ConfigureAwait(false);
                    }
                }

                if (Stopping)
                {
                    Status = 5;
                    return;
                }
                DownloadDatetime = DateTime.Now;
                Status = 0;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                Dispose();
            }
        }

        private void _ThreadStop()
        {
            Stopping = true;
            do
            {
                Thread.Sleep(100);
            } while (Status == 1);
            Status = 2;
        }
        private void _ThreadDelete()
        {
            Stopping = true;

            do
            {
                Thread.Sleep(100);
            } while (Status == 2);
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
                NASS_CODE = string.Empty;
                fdb = string.Empty;
                lkey = string.Empty;
                formatUrl = string.Empty;
                responseData = string.Empty;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Api_Connection()
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