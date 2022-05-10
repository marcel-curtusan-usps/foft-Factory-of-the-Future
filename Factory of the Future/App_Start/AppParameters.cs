using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class AppParameters
    {
        public static TimeZone localZone = TimeZone.CurrentTimeZone;
        public static bool ActiveServer { get; set; } = false;
        public static string Appsetting { get; set; } = @"\AppSetting";
        public static string ConfigurationFloder { get; set; } = @"\Configuration";
        public static string LogFloder { get; set; } = @"\Log";
        public static string ORAQuery { get; set; } = @"\OracleQuery";
        public static JObject AppSettings { get; set; } = new JObject();
        public static string ApplicationEnvironment { get; set; } = string.Empty;
        public static string HWSerialNumber { get; set; } = string.Empty;
        public static string VersionInfo { get; set; } = string.Empty;
        public static string ServerHostname { get; set; } = string.Empty;
        public static string ServerIpAddress { get; set; } = string.Empty;
        public static DirectoryInfo CodeBase { get; set; } = null;
        public static DirectoryInfo Logdirpath { get; set; } = null;

        private static readonly string PdHash = "P@@Sw0rd";
        private static readonly string SaltKey = "S@LT&KEY";
        private static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        //Map
        public static ConcurrentDictionary<string, BackgroundImage> IndoorMap { get; set; } = new ConcurrentDictionary<string, BackgroundImage>();
        public static ConcurrentDictionary<string, Cameras> CameraInfoList { get; set; } = new ConcurrentDictionary<string, Cameras>();
        public static ConcurrentDictionary<string, Connection> ConnectionList { get; set; } = new ConcurrentDictionary<string, Connection>();

        public static ConcurrentDictionary<string, GeoZone> ZoneList { get; set; } = new ConcurrentDictionary<string, GeoZone>();
        public static ConcurrentDictionary<string, ZoneInfo> ZoneInfo { get; set; } = new ConcurrentDictionary<string, ZoneInfo>();
        public static ConcurrentDictionary<string, GeoMarker> TagsList { get; set; } = new ConcurrentDictionary<string, GeoMarker>();
        public static ConcurrentDictionary<string, string> DPSList { get; set; } = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, string> MPEPerformanceList { get; set; } = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, string> DockdoorList { get; set; } = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, string> StaffingSortplansList { get; set; } = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, RouteTrips> RouteTripsList { get; set; } = new ConcurrentDictionary<string, RouteTrips>();
        //public static ConcurrentDictionary<string, JObject> QSMList { get; set; } = new ConcurrentDictionary<string, JObject>();

        public static ConcurrentDictionary<string, Container> Containers { get; set; } = new ConcurrentDictionary<string, Container>();
        public static ConcurrentDictionary<string, Mission> MissionList { get; set; } = new ConcurrentDictionary<string, Mission>();
        public static ConcurrentDictionary<string, Notification> NotificationList { get; set; } = new ConcurrentDictionary<string, Notification>();
        public static ConcurrentDictionary<string, NotificationConditions> NotificationConditionsList { get; set; } = new ConcurrentDictionary<string, NotificationConditions>();
        public static ConcurrentDictionary<string, ADUser> Users { get; set; } = new ConcurrentDictionary<string, ADUser>();
        public static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        //public static ConcurrentDictionary<string, JObject> Tag { get; set; } = new ConcurrentDictionary<string, JObject>();
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
                ///load app settings
                GetAppSettings();

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
                ///load Default Notification settings
                GetNotificationDefault();
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }
        private static void GetAppSettings()
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
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
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
                            if (NotificationConditionsList.TryAdd(tempnotification[i].Id, tempnotification[i]))
                            {

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
                                    if (siteConfigDir.Exists)
                                    {
                                        new FileIO().Write(string.Concat(Logdirpath, ConfigurationFloder, @"\"), "AppSettings.json", JsonConvert.SerializeObject(AppSettings, Newtonsoft.Json.Formatting.Indented));

                                    }
                                    else
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
        internal static void LoadIndoorapData(string fileName)
        {
            try
            {
                if (AppSettings.ContainsKey("LOCAL_PROJECT_DATA") && (bool)AppSettings.Property("LOCAL_PROJECT_DATA").Value)
                {
                    string ProjectData = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), fileName);
                    if (!string.IsNullOrEmpty(ProjectData))
                    {
                        if (!string.IsNullOrEmpty(ProjectData))
                        {
                            Task.Run(() => new ProcessRecvdMsg().StartProcess(ProjectData, "getProjectInfo", ""));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        internal static void LoadData(string FileName)
        {
            try
            {
                if (Logdirpath != null)
                {
                    string file_content = new FileIO().Read(string.Concat(Logdirpath, ConfigurationFloder), FileName);

                    if (!string.IsNullOrEmpty(file_content))
                    {
                        if (FileName.StartsWith("Connection.json"))
                        {
                            List<Connection> tempcon = JsonConvert.DeserializeObject<List<Connection>>(file_content);
                            for (int i = 0; i < tempcon.Count; i++)
                            {
                                if (ConnectionList.TryAdd(tempcon[i].Id, tempcon[i]))
                                {
                                    RunningConnection.Add(tempcon[i]);
                                }
                            }
                        }
                        if (FileName.StartsWith("Zones.json"))
                        {
                            List<ZoneInfo> tempzone = JsonConvert.DeserializeObject<List<ZoneInfo>>(file_content);
                            for (int i = 0; i < tempzone.Count; i++)
                            {
                                if (!ZoneInfo.ContainsKey(tempzone[i].Id))
                                {
                                    if (ZoneInfo.TryAdd(tempzone[i].Id, tempzone[i]))
                                    {

                                    }
                                }
                            }
                        }
                        if (FileName.StartsWith("CustomZones.json"))
                        {
                            List<GeoZone> tempzone = JsonConvert.DeserializeObject<List<GeoZone>>(file_content);

                            for (int i = 0; i < tempzone.Count; i++)
                            {
                                if (!ZoneList.ContainsKey(tempzone[i].Properties.Id))
                                {
                                    tempzone[i].Properties.Source = "user";
                                    if (ZoneList.TryAdd(tempzone[i].Properties.Id, tempzone[i]))
                                    {

                                    }
                                }
                            }
                        }
                        if (FileName.StartsWith("Markers.json"))
                        {
                            List<GeoMarker> tempMarker = JsonConvert.DeserializeObject<List<GeoMarker>>(file_content);
                            for (int i = 0; i < tempMarker.Count; i++)
                            {
                                if (!TagsList.ContainsKey(tempMarker[i].Properties.Id))
                                {
                                    tempMarker[i].Properties.Source = "user";
                                    if (TagsList.TryAdd(tempMarker[i].Properties.Id, tempMarker[i]))
                                    {

                                    }
                                }
                            }
                        }
                        //Notification
                        if (FileName.StartsWith("Notification.json"))
                        {
                            List<NotificationConditions> tempnotification = JsonConvert.DeserializeObject<List<NotificationConditions>>(file_content);
                            for (int i = 0; i < tempnotification.Count; i++)
                            {
                                if (!NotificationConditionsList.ContainsKey(tempnotification[i].Id))
                                {
                                    if (NotificationConditionsList.TryAdd(tempnotification[i].Id, tempnotification[i]))
                                    {

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
    }
}