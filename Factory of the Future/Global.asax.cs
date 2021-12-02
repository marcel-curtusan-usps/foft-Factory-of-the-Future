using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace Factory_of_the_Future
{
    public class Global : System.Web.HttpApplication
    {
        public static readonly ProcessRecvdMsg ProcessRecvdMsg_callback = new ProcessRecvdMsg();
        public static JObject AppSettings = new JObject();
        public static bool SERVER_ACTIVE;
        public static DirectoryInfo Logdirpath;
        public static string Application_Environment = string.Empty;
        public static DirectoryInfo CodeBase = null;
        public static string ConfigurationFloder = "/Configuration";
        public static string LogFloder = "/Log";
        public static string ORAQuery = "/OracleQuery";
        public static string DataSample = "/DataSample";
        public static string Appsetting = "/AppSetting";
        public static string VersionInfo = string.Empty;
        private static readonly string PasswordHash = "P@@Sw0rd";
        private static readonly string SaltKey = "S@LT&KEY";
        private static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        public static string HWSerialNumber = string.Empty;
        public static string ServerHostname = string.Empty;
        public static string ipaddress = "";
        public static ConcurrentDictionary<int, JObject> API_List = new ConcurrentDictionary<int, JObject>();

        // Tag
        public static ConcurrentDictionary<string, JObject> Tag = new ConcurrentDictionary<string, JObject>();

        //Zone
        public static ConcurrentDictionary<string, JObject> Zones = new ConcurrentDictionary<string, JObject>();

        ////Viewports
        //public static ConcurrentDictionary<string, JObject> ViewPorts = new ConcurrentDictionary<string, JObject>();

        ////Dock
        //public static ConcurrentDictionary<string, JObject> DockDoorZones = new ConcurrentDictionary<string, JObject>();

        ////MachineZone
        //public static ConcurrentDictionary<string, JObject> MachineZones = new ConcurrentDictionary<string, JObject>();

        ////MachineZone
        //public static ConcurrentDictionary<string, JObject> MachineZonesPerf = new ConcurrentDictionary<string, JObject>();

        ////AGV LoctionZone
        //public static ConcurrentDictionary<string, JObject> AGVLocationZones = new ConcurrentDictionary<string, JObject>();

        //Trips
        public static ConcurrentDictionary<string, JObject> Trips = new ConcurrentDictionary<string, JObject>();

        //CTS DockDeparted
        public static ConcurrentDictionary<string, JObject> CTS_DockDeparted = new ConcurrentDictionary<string, JObject>();

        //CTS LocalDockDeparted
        public static ConcurrentDictionary<string, JObject> CTS_LocalDockDeparted = new ConcurrentDictionary<string, JObject>();

        //CTS Trips -> Inbound Trips Scheduled
        public static ConcurrentDictionary<string, JObject> CTS_Inbound_Schedualed = new ConcurrentDictionary<string, JObject>();

        //CTS Trips -> Outbound Trips Scheduled
        public static ConcurrentDictionary<string, JObject> CTS_Outbound_Schedualed = new ConcurrentDictionary<string, JObject>();

        //list of containers
       // public static ConcurrentDictionary<string, JObject> Containers = new ConcurrentDictionary<string, JObject>();
        //list of containers
        public static ConcurrentDictionary<string, Container> Containers = new ConcurrentDictionary<string, Container>();

        //Map
        public static ConcurrentDictionary<string, JObject> Map = new ConcurrentDictionary<string, JObject>();

        //Quick Status Menu
        public static ConcurrentDictionary<string, JObject> QSM_List = new ConcurrentDictionary<string, JObject>();

        // Tag from SELS
        public static ConcurrentDictionary<string, JObject> DataParseType = new ConcurrentDictionary<string, JObject>();

        //site info list
        public static ConcurrentDictionary<string, JObject> Site_Info_List = new ConcurrentDictionary<string, JObject>();

        ////tag dwell
        //public static ConcurrentDictionary<string, JObject> Tag_Dwell = new ConcurrentDictionary<string, JObject>();

        //Sort plans for machines
        public static ConcurrentDictionary<string, JObject> Sortplans = new ConcurrentDictionary<string, JObject>();

        // Vehicle History
        public static ConcurrentDictionary<DateTime, JObject> Vehicle_State_History = new ConcurrentDictionary<DateTime, JObject>();

        // Notification Conditions
        public static ConcurrentDictionary<int, JObject> Notification_Conditions = new ConcurrentDictionary<int, JObject>();

        // Notification list
        public static ConcurrentDictionary<string, JObject> Notification = new ConcurrentDictionary<string, JObject>();

        public static readonly ConcurrentDictionary<string, JObject> _users = new ConcurrentDictionary<string, JObject>();
        public static Dictionary<string, string> TimeZoneConvert = new Dictionary<string, string>()
    {
    
        { "America/Anchorage", "Alaskan Standard Time" },
        { "America/Argentina/San_Juan", "Argentina Standard Time" },
        { "America/Asuncion", "Paraguay Standard Time" },
        { "America/Bahia", "Bahia Standard Time" },
        { "America/Bogota", "SA Pacific Standard Time" },
        { "America/Buenos_Aires", "Argentina Standard Time" },
        { "America/Caracas", "Venezuela Standard Time" },
        { "America/Cayenne", "SA Eastern Standard Time" },
        { "America/Chicago", "Central Standard Time" },
        { "America/Chihuahua", "Mountain Standard Time (Mexico)" },
        { "America/Cuiaba", "Central Brazilian Standard Time" },
        { "America/Denver", "Mountain Standard Time" },
        { "America/Fortaleza", "SA Eastern Standard Time" },
        { "America/Godthab", "Greenland Standard Time" },
        { "America/Guatemala", "Central America Standard Time" },
        { "America/Halifax", "Atlantic Standard Time" },
        { "America/Indianapolis", "US Eastern Standard Time" },
        { "America/Indiana/Indianapolis", "US Eastern Standard Time" },
        { "America/La_Paz", "SA Western Standard Time" },
        { "America/Los_Angeles", "Pacific Standard Time" },
        { "America/Montevideo", "Montevideo Standard Time" },
        { "America/New_York", "Eastern Standard Time" },
        { "America/Noronha", "UTC-02" },
        { "America/Phoenix", "US Mountain Standard Time" },
        { "America/Regina", "Canada Central Standard Time" },
        { "America/Santa_Isabel", "Pacific Standard Time (Mexico)" },
        { "America/Santiago", "Pacific SA Standard Time" },
        { "America/Sao_Paulo", "E. South America Standard Time" },
        { "America/St_Johns", "Newfoundland Standard Time" },
        { "America/Tijuana", "Pacific Standard Time" },
        { "Pacific/Apia", "Samoa Standard Time" },
        { "Pacific/Guam", "West Pacific Standard Time" },
        { "Pacific/Honolulu", "Hawaiian Standard Time" }
    };

        public static bool Connected { get; set; }
        public static bool Disconnected { get; set; }
        public static bool Errors { get; set; }

        private static MulticastUdpClient udpclient = null;
        public static BackgroundWorker worker = new BackgroundWorker();

        public static bool stopWorker = false;

        //background worker for checking configuration files
        public static BackgroundWorker APIWorker = new BackgroundWorker();

        public static bool APIStopWorker = false;

        //background worker for checking configuration files
        public static BackgroundWorker TagAPIWorker = new BackgroundWorker();

        public static bool TagAPIStopWorker = false;

        //background worker for Retention of data on server
        public static BackgroundWorker DataRetentionWorker = new BackgroundWorker();

        public static bool DataRetentionStopWorker = false;
        protected void Application_Start()
        {
            //get version
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            VersionInfo = string.Concat(fvi.FileMajorPart.ToString() + ".", fvi.FileMinorPart.ToString() + ".", fvi.FileBuildPart.ToString() + ".", fvi.FilePrivatePart.ToString());
            CodeBase = new DirectoryInfo(new Uri(assembly.CodeBase).LocalPath).Parent;
            ///////////////////////////////////////////////
            //////////////////////////////////////////////
            ///Super Important///////////////////////////
            ////////////////////////////////////////////
            ///////////////////////////////////////////
            //read AppSetting file fist
            GetAppSettings();
            //get ip address and zip code from hostname
            ipaddress = GetLocalIpAddress();


            if (string.IsNullOrEmpty(Application_Environment))
            {
                if (AppSettings.ContainsKey("DEV_SVRP_IP"))
                {
                    if ((string)AppSettings.Property("DEV_SVRP_IP").Value == ipaddress)
                    {
                        Application_Environment = "DEV";
                    }
                }
                if (AppSettings.ContainsKey("DEV_SVRS_IP"))
                {
                    if ((string)AppSettings.Property("dev_SVRS_IP").Value == ipaddress)
                    {
                        Application_Environment = "DEV";
                    }
                }
                if (AppSettings.ContainsKey("SIT_SVRP_IP"))
                {
                    if ((string)AppSettings.Property("SIT_SVRP_IP").Value == ipaddress)
                    {
                        Application_Environment = "SIT";
                    }
                }
                if (AppSettings.ContainsKey("SIT_SVRS_IP"))
                {
                    if ((string)AppSettings.Property("SIT_SVRS_IP").Value == ipaddress)
                    {
                        Application_Environment = "SIT";
                    }
                }
                if (AppSettings.ContainsKey("CAT_SVRS_IP"))
                {
                    if ((string)AppSettings.Property("CAT_SVRS_IP").Value == ipaddress)
                    {
                        Application_Environment = "CAT";
                    }
                }
                if (AppSettings.ContainsKey("CAT_SVRP_IP"))
                {
                    if ((string)AppSettings.Property("CAT_SVRP_IP").Value == ipaddress)
                    {
                        Application_Environment = "CAT";
                    }
                }

                if (string.IsNullOrEmpty(Application_Environment))
                {
                    Application_Environment = "PROD";
                }
            }
            if (!QSM_List.ContainsKey("SERVER"))
            {
                JObject qsm = new JObject_List().QSM_Connection;
                qsm.Property("CONNECTION_NAME").Value = "SERVER";
                qsm.Property("NUMBER_OF_CONNECTION").Value = ServerHostname;
                qsm.Property("INDEX").Value = 1;
                if (!QSM_List.TryAdd("SERVER", qsm))
                {
                    new ErrorLogger().CustomLog(string.Concat("Error adding Server connection to QSM"), string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "Applogs"));
                }
            }

            if (!string.IsNullOrEmpty(AppSettings.ContainsKey("ORACONNSTRING") ? (string)AppSettings.Property("ORACONNSTRING").Value : ""))
            {
                JObject qsm = new JObject_List().QSM_Connection;
                qsm.Property("CONNECTION_NAME").Value = "DATABASE";
                qsm.Property("NUMBER_OF_CONNECTION").Value = ServerHostname;
                qsm.Property("INDEX").Value = 2;
                if (!QSM_List.TryAdd("DATABASE", qsm))
                {
                    new ErrorLogger().CustomLog(string.Concat("Error adding Databse connection to QSM"), string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "Applogs"));
                }
            }
            //configuration worker
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
            //API worker
            APIWorker.DoWork += new DoWorkEventHandler(APIDoWork);
            APIWorker.WorkerReportsProgress = true;
            APIWorker.WorkerSupportsCancellation = true;
            APIWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(APIWorkerCompleted);
            //TAG API worker
            TagAPIWorker.DoWork += new DoWorkEventHandler(TagAPIDoWork);
            TagAPIWorker.WorkerReportsProgress = true;
            TagAPIWorker.WorkerSupportsCancellation = true;
            TagAPIWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(TagAPIWorkerCompleted);
            //DataRetentionWorker
            DataRetentionWorker.DoWork += new DoWorkEventHandler(DataRetentionDoWork);
            DataRetentionWorker.WorkerReportsProgress = true;
            DataRetentionWorker.WorkerSupportsCancellation = true;
            DataRetentionWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DataRetentionWorkerCompleted);

            // Calling the DoWork Method Asynchronously
            worker.RunWorkerAsync();
            APIWorker.RunWorkerAsync();
            TagAPIWorker.RunWorkerAsync();
            DataRetentionWorker.RunWorkerAsync();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker)
                {
                    int hours = 0;
                    int min = 0;
                    int sec = 30;
                    double tm = new TimeSpan(hours, min, sec).TotalMilliseconds;
                    Thread.Sleep(Convert.ToInt32(tm));
                    if (!stopWorker)
                    {
                        worker.RunWorkerAsync();
                    }
                    else
                    {
                        while (stopWorker)
                        {
                            Thread.Sleep(new TimeSpan(hours, min, sec).Milliseconds);
                        }
                        worker.RunWorkerAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //this is used to check if this is the active server.
                if (SERVER_ACTIVE)
                {
                    //Load Tags
                    //if (Tag.Count == 0)
                    //{
                    //    Get_Tag_Data();
                    //}
                    //Load Tag Dwell
                    //if (Tag_Dwell.Count == 0)
                    //{
                    //    Get_Tag_Dwell();
                    //}
                    //Load server Configuration
                    if (API_List.Count == 0)
                    {
                        Get_API_List();
                    }
                    if (Zones.Count == 0)
                    {
                        Get_Zones_List();
                    }
                    //Load floor plan Configuration
                    if (Map.Count == 0)
                    {
                        Get_FloorPlan();
                    }
                    if (Sortplans.Count == 0)
                    {
                        Get_P2PData();
                    }
                    if (Notification_Conditions.Count == 0)
                    {
                        Get_Notification_Conditions();
                    }
                    if (udpclient == null)
                    {
                        API_List.Where(v => (bool)v.Value.Property("UDP_CONNECTION").Value == true &&
                        (bool)v.Value.Property("ACTIVE_CONNECTION").Value == true).Select(y => y.Value).ToList().ForEach(m =>
                        {
                            if (!string.IsNullOrEmpty(m.ContainsKey("PORT") ? (string)m.Property("PORT").Value : ""))
                            {
                                int port = !string.IsNullOrEmpty((string)m.Property("PORT").Value) ? Convert.ToInt32(m.Property("PORT").Value) : 0;
                                if (port > 0)
                                {
                                    try
                                    {
                                        // Create and connect multi cast client
                                        udpclient = new MulticastUdpClient("0.0.0.0", port);
                                        udpclient.SetupMulticast(true);
                                        udpclient.Connect();
                                        if (udpclient.IsConnected)
                                        {
                                            m.Property("API_CONNECTED").Value = true;
                                            m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                            m.Property("UPDATE_STATUS").Value = true;
                                        }
                                        while (!udpclient.IsConnected)
                                        {
                                            m.Property("API_CONNECTED").Value = false;
                                            m.Property("UPDATE_STATUS").Value = true;
                                            Thread.Yield();
                                        }
                                        // Wait for all data processed...
                                        if (udpclient != null)
                                        {
                                            while (udpclient.BytesReceived != 4)
                                            {
                                                m.Property("API_CONNECTED").Value = true;
                                                m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                                Thread.Yield();
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        m.Property("API_CONNECTED").Value = false;
                                        new ErrorLogger().ExceptionLog(ex);
                                    }
                                }
                            }
                        });
                    }
                }
                if (!SERVER_ACTIVE)
                {
                    //clear Server connection
                    if (API_List.Count != 0)
                    {
                        API_List = new ConcurrentDictionary<int, JObject>();
                    }
                    //clear Zone
                    if (Zones.Count != 0)
                    {
                        Zones = new ConcurrentDictionary<string, JObject>();
                    }

                    //clear Notification Conditions
                    if (Notification_Conditions.Count != 0)
                    {
                        Notification_Conditions = new ConcurrentDictionary<int, JObject>();
                    }

                    //clear tag data
                    if (Tag.Count != 0)
                    {
                        Tag = new ConcurrentDictionary<string, JObject>();
                    }
                    if (Logdirpath != null)
                    {
                        //if not null then check if the drive is available
                        if (new Directory_Check().DirPath(Logdirpath))
                        {
                            if (!Logdirpath.Exists)
                            {
                                Directory.CreateDirectory(Logdirpath.FullName);
                            }
                        }
                    }
                    //check if Log directory is null
                    if (Logdirpath == null)
                    {
                        if (AppSettings.ContainsKey("LOG_LOCATION"))
                        {
                            if (!string.IsNullOrEmpty(AppSettings.Property("LOG_LOCATION").Value.ToString()))
                            {
                                // set the Log dir path to web configure value.
                                Logdirpath = new DirectoryInfo(AppSettings.Property("LOG_LOCATION").Value.ToString());
                                //if directory does not exists then create
                                if (!Logdirpath.Exists)
                                {
                                    Directory.CreateDirectory(Logdirpath.FullName);
                                }
                            }
                        }
                    }
                    if (udpclient != null)
                    {
                        if (!udpclient.IsDisposed)
                        {
                            udpclient.Dispose();
                            udpclient = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private void Get_Zones_List()
        {
            try
            {
                string api_config = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), "Zones.json");
                if (!string.IsNullOrEmpty(api_config))
                {
                    if (IsValidJson(api_config))
                    {
                        JArray list = JArray.Parse(api_config);
                        if (list != null)
                        {
                            if (list.HasValues)
                            {
                                foreach (JObject item in list.Children())
                                {
                                    if (!Zones.ContainsKey(item["properties"]["id"].ToString()))
                                    {

                                        item["properties"]["MPEWatchData"] = "";
                                        if (item["properties"]["Zone_Type"].ToString().StartsWith("DockDoor"))
                                        {
                                            item["properties"]["svDoorData"] = "";
                                            item["properties"]["unloadedcount"] = 0;
                                            item["properties"]["timeToDepart"] = 60;
                                        }
                                        Zones.TryAdd(item["properties"]["id"].ToString(), item);
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        public static void StopUdpClient()
        {
            if (udpclient != null)
            {
                if (!udpclient.IsDisposed)
                {
                    udpclient.Dispose();
                    udpclient = null;
                }
            }
        }

        private void Get_Notification_Conditions()
        {
            try
            {
                string notificationlist = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), "Notification.json");
                if (!string.IsNullOrEmpty(notificationlist))
                {
                    if (IsValidJson(notificationlist))
                    {
                        JArray list = JArray.Parse(notificationlist);
                        if (list != null)
                        {
                            if (list.HasValues)
                            {
                                foreach (JObject item in list.Children())
                                {
                                    JObject notification = new JObject_List().Notification_Conditions;
                                    foreach (dynamic kv in item.Children())
                                    {
                                        if (notification.ContainsKey(kv.Name))
                                        {
                                            if (kv.Value != notification.Property(kv.Name).Value)
                                            {
                                                notification.Property(kv.Name).Value = kv.Value;
                                            }
                                        }
                                    }

                                    if (!Notification_Conditions.ContainsKey((int)notification.Property("ID").Value))
                                    {
                                        Notification_Conditions.TryAdd((int)notification.Property("ID").Value, notification);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void Get_P2PData()
        {
            try
            {
                string P2PData = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), "P2PData.json");
                if (!string.IsNullOrEmpty(P2PData))
                {
                    if (IsValidJson(P2PData))
                    {
                        JObject data = JObject.Parse(P2PData);
                        if (data != null)
                        {
                            if (data.HasValues)
                            {
                                data.Add(new JProperty("message", "P2PBySite"));
                                new ProcessRecvdMsg().StartProcess(data, new JObject());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void Get_FloorPlan()
        {
            try
            {
                string ProjectData = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), "ProjectData.json");
                if (!string.IsNullOrEmpty(ProjectData))
                {
                    if (IsValidJson(ProjectData))
                    {
                        JObject list = JObject.Parse(ProjectData);
                        if (list != null)
                        {
                            if (list.HasValues)
                            {
                                new ProcessRecvdMsg().StartProcess(list, new JObject());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void Get_API_List()
        {
            try
            {
                string api_config = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), "API_Connection.json");
                if (!string.IsNullOrEmpty(api_config))
                {
                    if (IsValidJson(api_config))
                    {
                        JArray list = JArray.Parse(api_config);
                        if (list != null)
                        {
                            if (list.HasValues)
                            {
                                foreach (JObject item in list.Children())
                                {
                                    JObject api = new JObject_List().API;
                                    item["API_CONNECTED"] = false;
                                    item["LASTTIME_API_CONNECTED"] = DateTime.Now.AddDays(-30);
                                    api.Merge(item, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                    if (!API_List.ContainsKey((int)api.Property("ID").Value))
                                    {
                                        API_List.TryAdd((int)api["ID"], api);                                      
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void GetAppSettings()
        {
            try
            {
                string file_content = "";
                //appsetting

                if (string.IsNullOrEmpty(file_content))
                {
                    file_content = new FileIO().Read(string.Concat(CodeBase.Parent.FullName.ToString(), Appsetting), "AppSettings.json");
                }

                if (!string.IsNullOrEmpty(file_content))
                {
                    if (IsValidJson(file_content))
                    {
                        JObject filejsonrray = JObject.Parse(file_content);
                        if (filejsonrray.HasValues)
                        {
                            AppSettings = filejsonrray;
                        }
                    }
                }
                else
                {
                    if (!AppSettings.HasValues)
                    {
                        if (!AppSettings.ContainsKey("APPLICATION_NAME")) { AppSettings.Add(new JProperty("APPLICATION_NAME", "Factory of the Future")); }
                        if (!AppSettings.ContainsKey("FACILITY_NAME")) { AppSettings.Add(new JProperty("FACILITY_NAME", "")); }
                        if (!AppSettings.ContainsKey("FACILITY_ZIP")) { AppSettings.Add(new JProperty("FACILITY_ZIP", "")); }
                        if (!AppSettings.ContainsKey("FACILITY_ID")) { AppSettings.Add(new JProperty("FACILITY_ID", "")); }
                        if (!AppSettings.ContainsKey("FACILITY_NASS_CODE")) { AppSettings.Add(new JProperty("FACILITY_NASS_CODE", "")); }
                        if (!AppSettings.ContainsKey("RETENTION_TIMER_SECONDS")) { AppSettings.Add(new JProperty("RETENTION_TIMER_SECONDS", "0")); }
                        if (!AppSettings.ContainsKey("RETENTION_TIMER_MINUTES")) { AppSettings.Add(new JProperty("RETENTION_TIMER_MINUTES", "10")); }
                        if (!AppSettings.ContainsKey("RETENTION_TIMER_HOURS")) { AppSettings.Add(new JProperty("RETENTION_TIMER_HOURS", "0")); }
                        if (!AppSettings.ContainsKey("RETENTION_DAYS")) { AppSettings.Add(new JProperty("RETENTION_DAYS", "60")); }
                        if (!AppSettings.ContainsKey("RETENTION_MAX_FILE_SIZE")) { AppSettings.Add(new JProperty("RETENTION_MAX_FILE_SIZE", "1073741824")); }
                        if (!AppSettings.ContainsKey("SERVER_ACTIVE")) { AppSettings.Add(new JProperty("SERVER_ACTIVE", "true")); }
                        if (!AppSettings.ContainsKey("SERVER_ACTIVE_HOSTNAME")) { AppSettings.Add(new JProperty("SERVER_ACTIVE_HOSTNAME", "")); }
                        if (!AppSettings.ContainsKey("Domain")) { AppSettings.Add(new JProperty("Domain", "USA.DCE.USPS.GOV")); }
                        if (!AppSettings.ContainsKey("ADUSAContainer")) { AppSettings.Add(new JProperty("ADUSAContainer", "DC=usa,DC=dce,DC=usps,DC=gov")); }
                        if (!AppSettings.ContainsKey("API_KEY")) { AppSettings.Add(new JProperty("API_KEY", "QUdWUE9SVEFMVXNlcjpBZ3ZQb3J0YWx1JGVyMDE=")); }
                        if (!AppSettings.ContainsKey("LOG_LOCATION")) { AppSettings.Add(new JProperty("LOG_LOCATION", "")); }
                        if (!AppSettings.ContainsKey("ADMINOVERRIDEUSER")) { AppSettings.Add(new JProperty("ADMINOVERRIDEUSER", "")); }
                        if (!AppSettings.ContainsKey("REMOTEDB")) { AppSettings.Add(new JProperty("REMOTEDB", true)); }
                        if (!AppSettings.ContainsKey("ROLES_ADMIN")) { AppSettings.Add(new JProperty("ROLES_ADMIN", "TC Admins")); }
                        if (!AppSettings.ContainsKey("ROLES_OPERATOR")) { AppSettings.Add(new JProperty("ROLES_OPERATOR", "TC Operator")); }
                        if (!AppSettings.ContainsKey("ROLES_MAINTENANCE")) { AppSettings.Add(new JProperty("ROLES_MAINTENANCE", "TC Maintenance")); }
                        if (!AppSettings.ContainsKey("ROLES_OIE")) { AppSettings.Add(new JProperty("ROLES_OIE", "Operations Industrial Engineers")); }
                        if (!AppSettings.ContainsKey("ORACONNSTRING")) { AppSettings.Add(new JProperty("ORACONNSTRING", "rCTak7p+l/f5zECB1hvqOunOVIYlx85vkDRiQUqmcm1Y9H8GlrRjGPJHYAvYhhLbuYSJJumZHV+gHcgKf7r8Fqu0XOMp87riVLLMk5WVIDm3nCb1jx+Z58H6cqYer74r4PpPOVFvSSq8uokfDpa2Bw==")); }
                        if (!AppSettings.ContainsKey("ORACONNASSTRING")) { AppSettings.Add(new JProperty("ORACONNASSTRING", "")); }
                    }
                }
                //this will check the attributes if any default are mission it will add it.
                if (AppSettings.HasValues)
                {
                    if (!AppSettings.ContainsKey("APPLICATION_NAME")) { AppSettings.Add(new JProperty("APPLICATION_NAME", "Factory of the Future")); }
                    if (!AppSettings.ContainsKey("FACILITY_NAME")) { AppSettings.Add(new JProperty("FACILITY_NAME", "")); }
                    if (!AppSettings.ContainsKey("FACILITY_ZIP")) { AppSettings.Add(new JProperty("FACILITY_ZIP", "")); }
                    if (!AppSettings.ContainsKey("FACILITY_ID")) { AppSettings.Add(new JProperty("FACILITY_ID", "")); }
                    if (!AppSettings.ContainsKey("FACILITY_NASS_CODE")) { AppSettings.Add(new JProperty("FACILITY_NASS_CODE", "")); }
                    if (!AppSettings.ContainsKey("RETENTION_TIMER_SECONDS")) { AppSettings.Add(new JProperty("RETENTION_TIMER_SECONDS", "0")); }
                    if (!AppSettings.ContainsKey("RETENTION_TIMER_MINUTES")) { AppSettings.Add(new JProperty("RETENTION_TIMER_MINUTES", "10")); }
                    if (!AppSettings.ContainsKey("RETENTION_TIMER_HOURS")) { AppSettings.Add(new JProperty("RETENTION_TIMER_HOURS", "0")); }
                    if (!AppSettings.ContainsKey("RETENTION_DAYS")) { AppSettings.Add(new JProperty("RETENTION_DAYS", "60")); }
                    if (!AppSettings.ContainsKey("RETENTION_MAX_FILE_SIZE")) { AppSettings.Add(new JProperty("RETENTION_MAX_FILE_SIZE", "1073741824")); }
                    if (!AppSettings.ContainsKey("SERVER_ACTIVE")) { AppSettings.Add(new JProperty("SERVER_ACTIVE", "true")); }
                    if (!AppSettings.ContainsKey("SERVER_ACTIVE_HOSTNAME")) { AppSettings.Add(new JProperty("SERVER_ACTIVE_HOSTNAME", "")); }
                    if (!AppSettings.ContainsKey("Domain")) { AppSettings.Add(new JProperty("Domain", "USA.DCE.USPS.GOV")); }
                    if (!AppSettings.ContainsKey("ADUSAContainer")) { AppSettings.Add(new JProperty("ADUSAContainer", "DC=usa,DC=dce,DC=usps,DC=gov")); }
                    if (!AppSettings.ContainsKey("API_KEY")) { AppSettings.Add(new JProperty("API_KEY", "QUdWUE9SVEFMVXNlcjpBZ3ZQb3J0YWx1JGVyMDE=")); }
                    if (!AppSettings.ContainsKey("LOG_LOCATION")) { AppSettings.Add(new JProperty("LOG_LOCATION", "")); }
                    if (!AppSettings.ContainsKey("ADMINOVERRIDEUSER")) { AppSettings.Add(new JProperty("ADMINOVERRIDEUSER", "")); }
                    if (!AppSettings.ContainsKey("REMOTEDB")) { AppSettings.Add(new JProperty("REMOTEDB", true)); }
                    if (!AppSettings.ContainsKey("ROLES_ADMIN")) { AppSettings.Add(new JProperty("ROLES_ADMIN", "TC Admins")); }
                    if (!AppSettings.ContainsKey("ROLES_OPERATOR")) { AppSettings.Add(new JProperty("ROLES_OPERATOR", "TC Operator")); }
                    if (!AppSettings.ContainsKey("ROLES_MAINTENANCE")) { AppSettings.Add(new JProperty("ROLES_MAINTENANCE", "TC Maintenance")); }
                    if (!AppSettings.ContainsKey("ROLES_OIE")) { AppSettings.Add(new JProperty("ROLES_OIE", "Operations Industrial Engineers")); }
                    if (!AppSettings.ContainsKey("ORACONNSTRING")) { AppSettings.Add(new JProperty("ORACONNSTRING", "rCTak7p+l/f5zECB1hvqOunOVIYlx85vkDRiQUqmcm1Y9H8GlrRjGPJHYAvYhhLbuYSJJumZHV+gHcgKf7r8Fqu0XOMp87riVLLMk5WVIDm3nCb1jx+Z58H6cqYer74r4PpPOVFvSSq8uokfDpa2Bw==")); }
                    if (!AppSettings.ContainsKey("ORACONNASSTRING")) { AppSettings.Add(new JProperty("ORACONNASSTRING", "")); }
                    //check if system has log drive
                    if (AppSettings.ContainsKey("LOG_LOCATION"))
                    {
                        if (!string.IsNullOrEmpty(AppSettings.Property("LOG_LOCATION").Value.ToString()))
                        {
                            // set the Log dir path to web configure value.
                            if (new Directory_Check().DirPath(new DirectoryInfo(AppSettings.Property("LOG_LOCATION").Value.ToString())))
                            {
                                file_content = new FileIO().Read(string.Concat(AppSettings.Property("LOG_LOCATION").Value.ToString(), ConfigurationFloder), "AppSettings.json");

                                if (!string.IsNullOrEmpty(file_content))
                                {
                                    if (IsValidJson(file_content))
                                    {
                                        JObject filejsonrray = JObject.Parse(file_content);
                                        if (filejsonrray.HasValues)
                                        {
                                            AppSettings = filejsonrray;
                                        }
                                    }
                                }

                                //if directory does not exists then create
                                if (!Logdirpath.Exists)
                                {
                                    Directory.CreateDirectory(Logdirpath.FullName);
                                }
                                else
                                {
                                    DirectoryInfo configLogdirpath = new DirectoryInfo(string.Concat(AppSettings.Property("LOG_LOCATION").Value.ToString(), ConfigurationFloder));
                                    if (!configLogdirpath.Exists)
                                    {
                                        FileInfo file = new FileInfo(string.Concat(AppSettings.Property("LOG_LOCATION").Value.ToString(), ConfigurationFloder, "\\", "AppSettings.json"));
                                        if (!file.Exists)
                                        {
                                            new FileIO().Write(string.Concat(AppSettings.Property("LOG_LOCATION").Value.ToString(), ConfigurationFloder, "\\"), "AppSettings.json", JsonConvert.SerializeObject(AppSettings, Newtonsoft.Json.Formatting.Indented));
                                        }
                                    }
                                    else
                                    {
                                        FileInfo file = new FileInfo(string.Concat(AppSettings.Property("LOG_LOCATION").Value.ToString(), ConfigurationFloder, "\\", "AppSettings.json"));
                                        if (!file.Exists)
                                        {
                                            new FileIO().Write(string.Concat(AppSettings.Property("LOG_LOCATION").Value.ToString(), ConfigurationFloder), "AppSettings.json", JsonConvert.SerializeObject(AppSettings, Newtonsoft.Json.Formatting.Indented));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void DataRetentionWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker)
                {
                    int hours = AppSettings.ContainsKey("RETENTION_TIMER_HOURS") ? (int)AppSettings.Property("RETENTION_TIMER_HOURS").Value : 0;
                    int min = AppSettings.ContainsKey("RETENTION_TIMER_MINUTES") ? (int)AppSettings.Property("RETENTION_TIMER_MINUTES").Value : 10;
                    int sec = AppSettings.ContainsKey("RETENTION_TIMER_SECONDS") ? (int)AppSettings.Property("RETENTION_TIMER_SECONDS").Value : 0;
                    double tm = new TimeSpan(hours, min, sec).TotalMilliseconds;
                    Thread.Sleep(Convert.ToInt32(tm));
                    if (!DataRetentionStopWorker)
                    {
                        worker.RunWorkerAsync();
                    }
                    else
                    {
                        while (DataRetentionStopWorker)
                        {
                            Thread.Sleep(new TimeSpan(hours, min, sec).Milliseconds);
                        }
                        worker.RunWorkerAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private void DataRetentionDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (SERVER_ACTIVE)
                {
                    ManualResetEvent[] manualEvents;
                    Data_Retention.StartProcess StartProcess;

                    manualEvents = new ManualResetEvent[1];

                    manualEvents[0] = new ManualResetEvent(false);
                    StartProcess = new Data_Retention.StartProcess(manualEvents[0]);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StartDataRetentionProcess.StartProcess), StartProcess);
                    WaitHandle.WaitAll(manualEvents);
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private void APIWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker)
                {
                    int hours = 0;
                    int min = 0;
                    int sec = 1;
                    double tm = new TimeSpan(hours, min, sec).TotalMilliseconds;
                    Thread.Sleep(Convert.ToInt32(tm));
                    if (!APIStopWorker)
                    {
                        worker.RunWorkerAsync();
                    }
                    else
                    {
                        while (APIStopWorker)
                        {
                            Thread.Sleep(new TimeSpan(hours, min, sec).Milliseconds);
                        }
                        worker.RunWorkerAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private void APIDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (Global.API_List.Count > 0)
                {
                    JArray svr_conn_list = new JArray();

                    foreach (JObject item in Global.API_List.Values)
                    {

                        if ((bool)item.Property("ACTIVE_CONNECTION").Value)
                        {
                            if ((string)item.Property("MESSAGE_TYPE").Value != "getTagPosition")
                            {
                                if (DateTime.Now.Subtract((DateTime)item.Property("LASTTIME_API_CONNECTED").Value).TotalMilliseconds >= (double)item.Property("DATA_RETRIEVE").Value)
                                {
                                    svr_conn_list.Add(item);
                                }
                            }
                        }
                    }
                    if (svr_conn_list.Count > 0)
                    {
                        //Parallel.ForEach(svr_conn_list, (requestUrl) => {
                        //    ResultList.Add(HttpRequestHandler<string>.Get(requestUrl));
                        //});
                        //foreach (JObject item in svr_conn_list.Children())
                        //{
                        //    new HandleAsyncProcess(ProcessType.Parallel, true, item);
                        //}
                        SourceData_Process SourceData_list;
                        ManualResetEvent[] manualEvents;
                        manualEvents = new ManualResetEvent[svr_conn_list.Count];
                        for (int i = 0; i < svr_conn_list.Count; i++)
                        {
                            manualEvents[i] = new ManualResetEvent(false);
                            SourceData_list = new SourceData_Process((JObject)svr_conn_list[i], manualEvents[i]);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(SourceDataProcess.Start_processor), SourceData_list);
                        }

                        WaitHandle.WaitAll(manualEvents, -1, false);
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private void TagAPIWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (sender is BackgroundWorker worker)
                {
                    //int hours = 0;
                    //int min = 0;
                    //int sec = 1;
                    //double tm = new TimeSpan(hours, min, sec).TotalMilliseconds;
                    //Thread.Sleep(Convert.ToInt32(tm));
                    if (!TagAPIStopWorker)
                    {
                        worker.RunWorkerAsync();
                    }
                    else
                    {
                        while (TagAPIStopWorker)
                        {
                            Thread.Sleep(250);
                        }
                        worker.RunWorkerAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private void TagAPIDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (Global.API_List.Count > 0)
                {
                    JArray svr_conn_list = new JArray();

                    foreach (JObject item in Global.API_List.Values)
                    {
                        if (item.ContainsKey("CONNECTION_NAME"))
                        {
                            if ((string)item.Property("MESSAGE_TYPE").Value == "getTagPosition" && (bool)item.Property("UDP_CONNECTION").Value == false)
                            {
                                if ((bool)item.Property("ACTIVE_CONNECTION").Value)
                                {
                                    if (DateTime.Now.Subtract((DateTime)item.Property("LASTTIME_API_CONNECTED").Value).TotalMilliseconds >= (int)item.Property("DATA_RETRIEVE").Value)
                                    {
                                        svr_conn_list.Add(item);
                                    }
                                }
                                if ((bool)item.Property("ACTIVE_CONNECTION").Value == false && (bool)item.Property("API_CONNECTED").Value == true)
                                {
                                    if (Global.API_List.ContainsKey((int)item.Property("ID").Value))
                                    {
                                        if (Global.API_List.TryGetValue((int)item.Property("ID").Value, out JObject Svr_con))
                                        {
                                            Svr_con.Property("API_CONNECTED").Value = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (svr_conn_list.Count > 0)
                    {
                        SourceData_Process SourceData_list;
                        ManualResetEvent[] manualEvents;
                        manualEvents = new ManualResetEvent[svr_conn_list.Count];
                        for (int i = 0; i < svr_conn_list.Count; i++)
                        {

                            manualEvents[i] = new ManualResetEvent(false);
                            SourceData_list = new SourceData_Process((JObject)svr_conn_list[i], manualEvents[i]);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(SourceDataProcess.Start_processor), SourceData_list);
                        }

                        WaitHandle.WaitAll(manualEvents, -1, false);
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
          
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
                if (credentials == (AppSettings.ContainsKey("API_KEY") ? AppSettings.Property("API_KEY").Value.ToString() : ""))
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

        public static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    new ErrorLogger().ExceptionLog(jex);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    new ErrorLogger().ExceptionLog(ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static string Encrypt(string plainText)
        {
            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
                var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
                var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

                byte[] cipherTextBytes;

                MemoryStream memoryStream = new MemoryStream();
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                }
                return Convert.ToBase64String(cipherTextBytes);
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return ";";
            }
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
                var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

                var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
                //cryptoStream1.Close();
                int decryptedByteCount;
                byte[] plainTextBytes = new byte[cipherTextBytes.Length]; ;
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                }
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return ";";
            }
        }

        private string GetLocalIpAddress()
        {
            try
            {
                List<string> config_IP = new List<string>();
                List<string> list = new List<string>();

                var host = Dns.GetHostEntry(Dns.GetHostName());
               // string resultString = string.Join(string.Empty, Regex.Matches(host.HostName, @"\d+").OfType<Match>().Select(m => m.Value));

                string hostname = host.HostName.Split('.').Select(x => x).FirstOrDefault();
                if (!string.IsNullOrEmpty(hostname))
                {
                    ServerHostname = hostname.Trim();
                }
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip) && ip.ToString().StartsWith("56."))
                    {
                        list.Add(ip.ToString());
                    }
                }
                var tempIP = (from item in list
                              from item2 in config_IP
                              where item == item2
                              select item2).ToList();
                if (tempIP.Count > 0)
                {
                    if (!string.IsNullOrEmpty(tempIP[0]))
                    {
                        return tempIP[0];
                    }
                }

                return list[0].ToString();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp).DateTime.ToLocalTime();
        }
    }
}
