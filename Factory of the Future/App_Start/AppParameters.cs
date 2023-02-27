using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Factory_of_the_Future.Models;

namespace Factory_of_the_Future
{
    public class ThroughputValues
    {

        public ThroughputValues(string mpeType, int? max, int? averagePerHour, int? averagePerHour25)
        {
            MpeType = mpeType;
            MaxThroughput = max;
            AverageThroughputPerHour = averagePerHour;
            AverageThroughputPerHour25 = averagePerHour25;
        }
        public string MpeType { get; set; }
        public int? MaxThroughput { get; set; }
        public int? AverageThroughputPerHour { get; set; }
        public int? AverageThroughputPerHour25 { get; set; }
    }
    internal class AppParameters
    {
        public static TimeZone localZone = TimeZone.CurrentTimeZone;
        public static bool ActiveServer { get; set; } = false;
        public static string Appsetting { get; set; } = @"\AppSetting";
        public static string ConfigurationFloder { get; set; } = @"\Configuration";
        public static string LogFloder { get; set; } = @"\Log";
        public static string Images { get; set; } = @"\Images";
        public static string ORAQuery { get; set; } = @"\OracleQuery";
        public static JObject AppSettings { get; set; } = new JObject();
        public static string ApplicationEnvironment { get; set; } = string.Empty;
        public static string HWSerialNumber { get; set; } = string.Empty;
        public static string VersionInfo { get; set; } = string.Empty;
        public static string ServerHostname { get; set; } = string.Empty;
        public static string ServerIpAddress { get; set; } = string.Empty;
        public static DirectoryInfo CodeBase { get; set; } = null;
        public static DirectoryInfo Logdirpath { get; set; } = null;
        //public static readonly object darvisWSCameraLock = new object();
        private static readonly string PdHash = "P@@Sw0rd";
        private static readonly string SaltKey = "S@LT&KEY";
        private static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        public static string NoImage { get; set; } = "";
        public static List<ThroughputValues> MachineThroughputMax {get; set; } 

        //public static Dictionary<string, string> CameraMapping { get; set; }
        public static ConcurrentDictionary<string, MachData> MPEWatchData { get; set; } = new ConcurrentDictionary<string, MachData>();
       // public static ConcurrentDictionary<string, CoordinateSystem> CoordinateSystem { get; set; } = new ConcurrentDictionary<string, CoordinateSystem>();  
        public static ConcurrentDictionary<string, Cameras> CameraInfoList { get; set; } = new ConcurrentDictionary<string, Cameras>();
        //public static ConcurrentDictionary<string, Connection> ConnectionList { get; set; } = new ConcurrentDictionary<string, Connection>();
        public static ConcurrentDictionary<string, DoorTripAssociation> DoorTripAssociation { get; set; } = new ConcurrentDictionary<string, DoorTripAssociation>();
        //public static ConcurrentDictionary<string, GeoZone> ZoneList { get; set; } = new ConcurrentDictionary<string, GeoZone>();
        // public static ConcurrentDictionary<string, GeoMarker> TagsList { get; set; } = new ConcurrentDictionary<string, GeoMarker>();
        public static ConcurrentDictionary<string, ZoneInfo> ZoneInfo { get; set; } = new ConcurrentDictionary<string, ZoneInfo>();
        public static ConcurrentDictionary<string, string> DPSList { get; set; } = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, RunPerf> MPEPerformance { get; set; } = new ConcurrentDictionary<string, RunPerf>();
        public static ConcurrentDictionary<string, RPGPlan> MPEPRPGList = new ConcurrentDictionary<string, RPGPlan>();
        public static ConcurrentDictionary<string, string> DockdoorList { get; set; } = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<int, SV_Bullpen> SVZoneNameList { get; set; } = new ConcurrentDictionary<int, SV_Bullpen>();
        public static ConcurrentDictionary<string, Staff> StaffingSortplansList { get; set; } = new ConcurrentDictionary<string, Staff>();
        public static ConcurrentDictionary<string, RouteTrips> RouteTripsList { get; set; } = new ConcurrentDictionary<string, RouteTrips>();
        public static ConcurrentDictionary<string, Container> Containers { get; set; } = new ConcurrentDictionary<string, Container>();
        public static ConcurrentDictionary<string, Mission> MissionList { get; set; } = new ConcurrentDictionary<string, Mission>();
        public static ConcurrentDictionary<string, Notification> NotificationList { get; set; } = new ConcurrentDictionary<string, Notification>();
        public static ConcurrentDictionary<string, NotificationConditions> NotificationConditionsList { get; set; } = new ConcurrentDictionary<string, NotificationConditions>();
        //public static ConcurrentDictionary<string, ADUser> Users { get; set; } = new ConcurrentDictionary<string, ADUser>();
        public static string QuuppaBaseUrl { get; set; }
        public static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        public static ConnectionContainer RunningConnection { get; set; } = new ConnectionContainer();
        public static Dictionary<string, string> TimeZoneConvert { get; set; } = new Dictionary<string, string>()
        {
            { "America/Anchorage", "Alaskan Standard Time" },
            { "America/Chicago", "Central Standard Time" },
            { "America/Denver", "Mountain Standard Time" },
            { "America/Los_Angeles", "Pacific Standard Time" },
            { "America/New_York", "Eastern Standard Time" },
            { "Pacific/Honolulu", "Hawaiian Standard Time" }
        };
      
        internal static void Start()
        {
            try
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                CodeBase = new DirectoryInfo(new Uri(assembly.CodeBase).LocalPath).Parent;
               
                

                //SetCameraMapping();
                //get version
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                VersionInfo = string.Concat(fvi.FileMajorPart.ToString() + ".", fvi.FileMinorPart.ToString() + ".", fvi.FileBuildPart.ToString() + ".", fvi.FilePrivatePart.ToString());

                ServerIpAddress = GetLocalIpAddress();
                if (string.IsNullOrEmpty(ApplicationEnvironment))
                {
                    if (AppSettings.ContainsKey("DEV_SVRP_IP"))
                    {
                        if ((string)AppSettings.Property("DEV_SVRP_IP").Value == ServerIpAddress)
                        {
                            ApplicationEnvironment = "DEV";
                        }
                    }
                    if (AppSettings.ContainsKey("DEV_SVRS_IP"))
                    {
                        if ((string)AppSettings.Property("dev_SVRS_IP").Value == ServerIpAddress)
                        {
                            ApplicationEnvironment = "DEV";
                        }
                    }
                    if (AppSettings.ContainsKey("SIT_SVRP_IP"))
                    {
                        if ((string)AppSettings.Property("SIT_SVRP_IP").Value == ServerIpAddress)
                        {
                            ApplicationEnvironment = "SIT";
                        }
                    }
                    if (AppSettings.ContainsKey("SIT_SVRS_IP"))
                    {
                        if ((string)AppSettings.Property("SIT_SVRS_IP").Value == ServerIpAddress)
                        {
                            ApplicationEnvironment = "SIT";
                        }
                    }
                    if (AppSettings.ContainsKey("CAT_SVRS_IP"))
                    {
                        if ((string)AppSettings.Property("CAT_SVRS_IP").Value == ServerIpAddress)
                        {
                            ApplicationEnvironment = "CAT";
                        }
                    }
                    if (AppSettings.ContainsKey("CAT_SVRP_IP"))
                    {
                        if ((string)AppSettings.Property("CAT_SVRP_IP").Value == ServerIpAddress)
                        {
                            ApplicationEnvironment = "CAT";
                        }
                    }
                    if (string.IsNullOrEmpty(ApplicationEnvironment))
                    {
                        ApplicationEnvironment = "PROD";
                    }
                }
                NoImage = "data:image/jpeg;base64," + ImageToByteArray("NoImageFeed.jpg");
                ///load app settings
                if (GetAppSettings())
                {
                    //load MpeWatch Site Info
                    GetMPEWatchSite();
                    ///load Default Notification settings
                    GetNotificationDefault();
                    //loadtemp
                    LoadTempIndoorapData("Project_Data.json");
                    // load max (y-axis) values for machines
                    GetMachineThroughputMax("MachineThroughputMax.csv");
                    //load the door and trip association 
                    _ = GetDoorTripAssociation();
                    ///load default connection setting.
                    _ = GetConnectionDefaultAsync();
                }
              

            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }
        public static async Task GetDoorTripAssociation()
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), "DoorTripAssociation.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                    JToken tempData = JToken.Parse(file_content);
       
                    if (tempData.HasValues)
                    {
                        List<DoorTripAssociation> tempdata = tempData.ToObject<List<DoorTripAssociation>>();
                        for (int i = 0; i < tempdata.Count; i++)
                        {
                            if (DoorTripAssociation.TryAdd(string.Concat(tempdata[i].Route, tempdata[i].Trip), tempdata[i]))
                            {
                                //
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
        private static void GetMPEWatchSite()
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(CodeBase.Parent.FullName.ToString(), Appsetting), "MPEWatch_Site_List.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                    JToken tempData = JToken.Parse(file_content);
                    JToken machineInfo = tempData.SelectToken("mach_data");
                    if (machineInfo.HasValues)
                    {
                        List<MachData> tempdata = machineInfo.ToObject<List<MachData>>();
                        for (int i = 0; i < tempdata.Count; i++)
                        {
                            Uri tempUrl = new Uri(tempdata[i].LocalLink);
                            tempdata[i].Port = tempUrl.Port;
                            tempdata[i].Host = tempUrl.Host;
                            tempdata[i].URL = string.Concat(tempdata[i].LocalLink, "mpemaster.api_page.get_data_by_time?id={0}&data_source={1}&start_time={2}&end_time={3}");
                            if (!string.IsNullOrEmpty(tempdata[i].Host) && !MPEWatchData.ContainsKey(tempdata[i].Host))
                            {
                                MPEWatchData.TryAdd(tempdata[i].Host, tempdata[i]);
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

        private static void GetMachineThroughputMax(string filename)
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), filename);
                string[] lines = file_content.Split('\r');
                bool first = true;
                List<ThroughputValues> throughputMaximums = new List<ThroughputValues>();
                foreach (string line in lines) {
                    if (!first)
                    {
                        string thisLine = line.Trim();
                        if (!String.IsNullOrEmpty(thisLine))
                        {

                            string[] vals = thisLine.Split(',');
                            string mpeType = vals[0];
                            int? max = null;
                            if (!String.IsNullOrEmpty(vals[1]))
                            {
                                max = Convert.ToInt32(vals[1]);
                            }
                            int? avg = null;
                            if (!String.IsNullOrEmpty(vals[2]))
                            {
                                avg = Convert.ToInt32(vals[2]);
                            }
                            int? avg25 = null;
                            if (!String.IsNullOrEmpty(vals[3]))
                            {
                                Convert.ToInt32(vals[3]);
                            }
                            ThroughputValues values = new ThroughputValues(mpeType, max, avg, avg25);
                            throughputMaximums.Add(values);
                        }
                    }
                    first = false;

                }
                MachineThroughputMax = throughputMaximums;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static bool GetAppSettings()
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(CodeBase.Parent.FullName.ToString(), Appsetting), "AppSettings.json");
                
                if (!string.IsNullOrEmpty(file_content))
                {
                    AppSettings = JObject.Parse(file_content);
                    if (AppSettings.HasValues && AppSettings.ContainsKey("FACILITY_TIMEZONE"))
                    {
                        if (string.IsNullOrEmpty(AppSettings["FACILITY_TIMEZONE"].ToString()))
                        {
                            AppSettings["FACILITY_TIMEZONE"] = TimeZoneConvert.Where(r => r.Value == localZone.StandardName).Select(y => y.Key).FirstOrDefault();
                        }
                    }
                    //this will check the attributes if any default are mission it will add it.
                    if (AppSettings.HasValues && AppSettings.ContainsKey("LOG_LOCATION"))
                    {
                        LoglocationSetup();
                       
                    }
                    return true;
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }
        private static void GetNotificationDefault()
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(CodeBase.Parent.FullName.ToString(), Appsetting), "Notification.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                    List<NotificationConditions> tempnotification = JsonConvert.DeserializeObject<List<NotificationConditions>>(file_content);
                    for (int i = 0; i < tempnotification.Count; i++)
                    {
                        if (!NotificationConditionsList.ContainsKey(tempnotification[i].Id))
                        {
                            NotificationConditionsList.TryAdd(tempnotification[i].Id, tempnotification[i]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        public static async Task GetConnectionDefaultAsync()
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(CodeBase.Parent.FullName.ToString(), Appsetting), "Connection.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                    List<Connection> tempcon = JsonConvert.DeserializeObject<List<Connection>>(file_content);

                    AppParameters.AppSettings["MPE_WATCH_ID"] = "";

                    for (int i = 0; i < tempcon.Count; i++)
                    {

                        await Task.Run(() => RunningConnection.Add(tempcon[i])).ConfigureAwait(false);

                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        internal static string ConnectionOutPutdata(List<Connection> connections)
        {
            try
            {
                var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
                jsonResolver.IgnoreProperty(typeof(Connection), "Status");
                jsonResolver.IgnoreProperty(typeof(Connection), "ApiConnected");
                jsonResolver.IgnoreProperty(typeof(Connection), "LasttimeApiConnected");

                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = jsonResolver;
                return JsonConvert.SerializeObject(connections, Formatting.Indented, serializerSettings);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }
        public static void LoglocationSetup()
        {
            try
            {
                if (AppSettings.ContainsKey("LOG_LOCATION") && AppSettings.ContainsKey("APPLICATION_NAME") && !string.IsNullOrEmpty((string)AppSettings["LOG_LOCATION"]))
                {

                    DirectoryInfo rootDir = new DirectoryInfo(string.Concat(AppSettings["LOG_LOCATION"].ToString(), @"\"));
                    if (rootDir.Exists)
                    {
                        ActiveServer = true;
                        DirectoryInfo appDir = new DirectoryInfo(string.Concat(rootDir.ToString(), (string)AppSettings["APPLICATION_NAME"]));
                        if (appDir.Exists)
                        {
                            if (AppSettings.ContainsKey("FACILITY_NASS_CODE") && !string.IsNullOrEmpty((string)AppSettings["FACILITY_NASS_CODE"]))
                            {

                                DirectoryInfo siteDir = new DirectoryInfo(string.Concat(appDir.ToString(), @"\", (string)AppSettings["FACILITY_NASS_CODE"]));
                                if (siteDir.Exists)
                                {
                                    Logdirpath = siteDir;
                                    DirectoryInfo siteConfigDir = new DirectoryInfo(string.Concat(siteDir.ToString(), ConfigurationFloder));
                                    if (!siteConfigDir.Exists)
                                    {
                                        Directory.CreateDirectory(siteConfigDir.FullName.ToString());
                                        LoglocationSetup();
                                    }
                                }
                                else
                                {
                                    Directory.CreateDirectory(siteDir.FullName.ToString());
                                    Logdirpath = null;
                                    LoglocationSetup();
                                }
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(appDir.FullName);
                            Logdirpath = null;
                            LoglocationSetup();
                        }
                    }
                    else
                    {
                        Logdirpath = null;
                        ActiveServer = false;
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        internal static void LoadTempIndoorapData(string fileName)
        {
            try
            {
                if (Logdirpath != null)
                {
                    string data = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), fileName);

                    if (!string.IsNullOrEmpty(data))
                    {
                        Task.Run(() => new ProcessRecvdMsg().StartProcess(data, "getProjectInfo", ""));
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static string GetLocalIpAddress()
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
        public static string Decrypt(string encryptedText)
        {
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = new Rfc2898DeriveBytes(PdHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
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
        public static string Encrypt(string plainText)
        {
            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                byte[] keyBytes = new Rfc2898DeriveBytes(PdHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
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
        public static int Get_TripMin(EventDtm scheduledDtm)
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                if (AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                {
                    dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                }
                DateTime tripDtm = new DateTime(scheduledDtm.Year,
                                                scheduledDtm.Month + 1,
                                                scheduledDtm.DayOfMonth,
                                                scheduledDtm.HourOfDay,
                                                scheduledDtm.Minute,
                                                scheduledDtm.Second);
                return (int)Math.Ceiling(tripDtm.Subtract(dtNow).TotalMinutes);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return 0;
            }
        }
        public static DateTime GetSvDate(EventDtm scheduledDtm)
        {
            DateTime tripDtm = new DateTime(scheduledDtm.Year,
                                             scheduledDtm.Month + 1,
                                             scheduledDtm.DayOfMonth,
                                             scheduledDtm.HourOfDay,
                                             scheduledDtm.Minute,
                                             scheduledDtm.Second);
            return tripDtm;
        }
        public static int Get_TagTTL(DateTime positionTS, DateTime tagTS)
        {
            try
            {
                //POSITION_MAX_AGE
                return (int)Math.Ceiling(tagTS.Subtract(positionTS).TotalMilliseconds);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return 0;
            }
        }
        public static int Get_NotificationTTL(DateTime start, DateTime end)
        {
            try
            {
                //POSITION_MAX_AGE
                return (int)Math.Ceiling(end.Subtract(start).TotalMinutes);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return 0;
            }
        }
        public static string SortPlan_Name_Trimer(string sortplan)
        {
            string sortplan_name = "";
            try
            {
                if (!string.IsNullOrEmpty(sortplan))
                {
                    int dotindex = sortplan.IndexOf(".", 1);
                    if ((dotindex == -1))
                    {
                        sortplan_name = sortplan;
                    }
                    else
                    {
                        sortplan_name = sortplan.Substring(0, dotindex);
                    }
                }
                return sortplan_name;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }

        internal static void ResetParameters()
        {
            try
            {
                StaffingSortplansList = new ConcurrentDictionary<string, Staff>();
                DockdoorList = new ConcurrentDictionary<string, string>();
                SVZoneNameList = new ConcurrentDictionary<int, SV_Bullpen>();
                DPSList = new ConcurrentDictionary<string, string>();
                MPEPerformance = new ConcurrentDictionary<string, RunPerf>();

                MPEPRPGList = new ConcurrentDictionary<string, RPGPlan>();
                RouteTripsList = new ConcurrentDictionary<string, RouteTrips>();
                Containers = new ConcurrentDictionary<string, Container>();
                MissionList = new ConcurrentDictionary<string, Mission>();
                NotificationList = new ConcurrentDictionary<string, Notification>();

               
                ZoneInfo = new ConcurrentDictionary<string, ZoneInfo>();
                //IndoorMap = new ConcurrentDictionary<string, BackgroundImage>();
                //TagsList = new ConcurrentDictionary<string, GeoMarker>();
                //ZoneList = new ConcurrentDictionary<string, GeoZone>();
                //CoordinateSystem = new ConcurrentDictionary<string, CoordinateSystem>();
                foreach (Api_Connection conn in RunningConnection.Connection)
                {
                    if (conn.ConnectionInfo.UdpConnection)
                    {
                        conn.UDPStop();
                    }
                    else if (conn.ConnectionInfo.TcpIpConnection)
                    {
                        conn.TCPStop();
                    }
                    else
                    {
                        conn.Stop();
                    }
                  
                }
                RunningConnection = new ConnectionContainer();
                //ConnectionList = new ConcurrentDictionary<string, Connection>();
                AppParameters.AppSettings["MPE_WATCH_ID"] = "";

                if (ActiveServer)
                {
                    Start();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        public static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
        public static string ZoneOutPutdata(List<CoordinateSystem> coordinateSystems)
        {
            try
            {
                var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
                jsonResolver.IgnoreProperty(typeof(Properties), "MPEWatchData");
                jsonResolver.IgnoreProperty(typeof(Properties), "DPSData");
                jsonResolver.IgnoreProperty(typeof(Properties), "staffingData");
                jsonResolver.IgnoreProperty(typeof(Properties), "dockdoorData");
                jsonResolver.IgnoreProperty(typeof(Properties), "Zone_Update");
                jsonResolver.IgnoreProperty(typeof(Properties), "MissionList");
                jsonResolver.IgnoreProperty(typeof(Properties), "rawData");
                //jsonResolver.IgnoreProperty(typeof(Marker), "CameraData");
                jsonResolver.IgnoreProperty(typeof(Marker), "zones");
                jsonResolver.IgnoreProperty(typeof(Marker), "isWearingTag");
                jsonResolver.IgnoreProperty(typeof(Marker), "tagVisibleMils");
                jsonResolver.IgnoreProperty(typeof(Marker), "isLdcAlert");
                jsonResolver.IgnoreProperty(typeof(Marker), "currentLDCs");
                jsonResolver.IgnoreProperty(typeof(Marker), "Mission");
                jsonResolver.IgnoreProperty(typeof(Marker), "Tag_TS");
                jsonResolver.IgnoreProperty(typeof(Marker), "Tag_Update");
                jsonResolver.IgnoreProperty(typeof(Marker), "Raw_Data");
                //jsonResolver.IgnoreProperty(typeof(Marker), "Camera_Data");
                jsonResolver.IgnoreProperty(typeof(Marker), "Vehicle_Status_Data");
                jsonResolver.IgnoreProperty(typeof(Marker), "positionTS");
                jsonResolver.IgnoreProperty(typeof(Marker), "tacs");
                jsonResolver.IgnoreProperty(typeof(Marker), "sels");
                jsonResolver.IgnoreProperty(typeof(Marker), "base64Image");
                jsonResolver.IgnoreProperty(typeof(BackgroundImage), "updateStatus");
                jsonResolver.IgnoreProperty(typeof(BackgroundImage), "rawData");
                
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = jsonResolver;
                return JsonConvert.SerializeObject(coordinateSystems,Formatting.Indented, serializerSettings);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }
        public static bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            return false;
        }
        public static string ImageToByteArray(string Image)
        {
            try
            {
                Bitmap noimage = (Bitmap)System.Drawing.Image.FromFile(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), "/Content/images/", Image));
                byte[] input = AppParameters.ImageToByteArray(noimage);
                return Convert.ToBase64String(input);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }

        }
    }
}