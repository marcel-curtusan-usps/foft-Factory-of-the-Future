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
        public static ConnectionContainer RunningConnection = new ConnectionContainer();

        public static ConcurrentDictionary<string, JObject> API_List = new ConcurrentDictionary<string, JObject>();

        // Tag
        public static ConcurrentDictionary<string, JObject> Tag = new ConcurrentDictionary<string, JObject>();

        //Zone
        public static ConcurrentDictionary<string, JObject> Zones = new ConcurrentDictionary<string, JObject>();
        //Zone info
        public static ConcurrentDictionary<string, JObject> Zone_Info = new ConcurrentDictionary<string, JObject>();

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
        public static ConcurrentDictionary<string, Trips> Trips = new ConcurrentDictionary<string, Trips>();

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

       
        public static BackgroundWorker worker = new BackgroundWorker();

        public static bool stopWorker = false;

        //background worker for Retention of data on server
        public static BackgroundWorker DataRetentionWorker = new BackgroundWorker();

        public static bool DataRetentionStopWorker = false;
        protected void Application_Start()
        {
            try
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
                //DataRetentionWorker
                DataRetentionWorker.DoWork += new DoWorkEventHandler(DataRetentionDoWork);
                DataRetentionWorker.WorkerReportsProgress = true;
                DataRetentionWorker.WorkerSupportsCancellation = true;
                DataRetentionWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DataRetentionWorkerCompleted);

                // Calling the DoWork Method Asynchronously
                worker.RunWorkerAsync();
                DataRetentionWorker.RunWorkerAsync();
                GlobalConfiguration.Configure(WebApiConfig.Register);
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
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
                    
                    //Load server Configuration
                    if (API_List.Count == 0)
                    {
                        Get_API_List();
                    }
                    //Zone info
                    if (Zone_Info.Count == 0)
                    {
                        Get_Zone_Info_List();
                    }
                    //Load floor plan Configuration
                    if (Map.Count == 0)
                    {
                        Get_FloorPlan();
                    }
                    //Load P2P Data
                    if (Sortplans.Count == 0)
                    {
                        Get_P2PData();
                    }
                    if (Notification_Conditions.Count == 0)
                    {
                        Get_Notification_Conditions();
                    }
                    if (RunningConnection.Connection.Count == 0)
                    {
                        Thread ConectionSetupThread = new Thread(new ThreadStart(ConectionSetup));
                        ConectionSetupThread.IsBackground = true;
                        ConectionSetupThread.Start();
                    }
                }
                if (!SERVER_ACTIVE)
                {
                    //clear Server connection
                    if (API_List.Count != 0)
                    {
                        API_List = new ConcurrentDictionary<string, JObject>();
                    }
                    //clear Zone
                    if (Zone_Info.Count != 0)
                    {
                        Zone_Info = new ConcurrentDictionary<string, JObject>();
                    }

                    //clear Notification Conditions
                    if (Notification_Conditions.Count != 0)
                    {
                        Notification_Conditions = new ConcurrentDictionary<int, JObject>();
                    }
                    //clear floor plan Configuration
                    if (Map.Count != 0)
                    {
                        Map = new ConcurrentDictionary<string, JObject>();
                    }
                    //clear P2P Data
                    if (Sortplans.Count != 0)
                    {
                        Sortplans = new ConcurrentDictionary<string, JObject>();
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
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private void Get_Zone_Info_List()
        {
            try
            {
                string zone_info = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), "Zones.json");
                if (!string.IsNullOrEmpty(zone_info))
                {
                    if (IsValidJson(zone_info))
                    {
                        JArray list = JArray.Parse(zone_info);
                        if (list != null)
                        {
                            if (list.HasValues)
                            {
                                foreach (JObject item in list.Children())
                                {
                                    if (!Zone_Info.ContainsKey(item["id"].ToString()))
                                    {
                                        Zone_Info.TryAdd(item["id"].ToString(), item);
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

        private void ConectionSetup()
        {
            try
            {

                foreach (JObject item in Global.API_List.Values)
                {
                    RunningConnection.Add(JsonConvert.DeserializeObject<Connection>(JsonConvert.SerializeObject(item)));
                }

            }
            catch (Exception e)
            {

                new ErrorLogger().ExceptionLog(e);
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
                if (AppSettings.ContainsKey("LOCAL_PROJECT_DATA"))
                {
                    if ((bool)AppSettings.Property("LOCAL_PROJECT_DATA").Value)
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
                                    //JObject api = new JObject_List().API;
                                    //item["API_CONNECTED"] = false;
                                    //api["ID"] = item["ID"];
                                    //item["LASTTIME_API_CONNECTED"] = DateTime.Now.AddDays(-30);
                                    //api.Merge(item, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                    if (!API_List.ContainsKey((string)item.Property("ID").Value))
                                    {
                                        API_List.TryAdd((string)item["ID"], item);                                      
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
                        if (!AppSettings.ContainsKey("LOCAL_PROJECT_DATA")) { AppSettings.Add(new JProperty("LOCAL_PROJECT_DATA", false)); }
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
                    if (!AppSettings.ContainsKey("LOCAL_PROJECT_DATA")) { AppSettings.Add(new JProperty("LOCAL_PROJECT_DATA", false)); }
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
        public static DateTime SVdatetimeformat(JObject eventDtm)
        {

            return new DateTime(eventDtm.ContainsKey("year") ? (int)eventDtm.Property("year").Value : 0,
                                            eventDtm.ContainsKey("month") ? ((int)eventDtm.Property("month").Value + 1) : 0,
                                            eventDtm.ContainsKey("dayOfMonth") ? (int)eventDtm.Property("dayOfMonth").Value : 0,
                                            eventDtm.ContainsKey("hourOfDay") ? (int)eventDtm.Property("hourOfDay").Value : 0,
                                            eventDtm.ContainsKey("minute") ? (int)eventDtm.Property("minute").Value : 0,
                                                        eventDtm.ContainsKey("second") ? (int)eventDtm.Property("second").Value : 0);

        }
    }
}
