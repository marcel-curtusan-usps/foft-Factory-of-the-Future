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
        //public static ConcurrentDictionary<string, JObject> SortplansList { get; set; } = new ConcurrentDictionary<string, JObject>();
        //public static ConcurrentDictionary<string, JObject> ZonesList { get; set; } = new ConcurrentDictionary<string, JObject>();
        public static ConcurrentDictionary<string, GeoZone> ZoneList { get; set; } = new ConcurrentDictionary<string, GeoZone>();
        public static ConcurrentDictionary<string, JObject> ZoneInfo { get; set; } = new ConcurrentDictionary<string, JObject>();
        public static ConcurrentDictionary<string, GeoMarker> TagsList { get; set; } = new ConcurrentDictionary<string, GeoMarker>();
        //public static ConcurrentDictionary<string, JObject> RouteTripsList { get; set; } = new ConcurrentDictionary<string, JObject>();
        //public static ConcurrentDictionary<string, JObject> QSMList { get; set; } = new ConcurrentDictionary<string, JObject>();

        public static ConcurrentDictionary<string, Container> Containers { get; set; } = new ConcurrentDictionary<string, Container>();
        //public static ConcurrentDictionary<string, JObject> MissionList { get; set; } = new ConcurrentDictionary<string, JObject>();
        //public static ConcurrentDictionary<string, JObject> NotificationList { get; set; } = new ConcurrentDictionary<string, JObject>();
        //public static ConcurrentDictionary<string, JObject> NotificationConditionsList { get; set; } = new ConcurrentDictionary<string, JObject>();
        public static  ConcurrentDictionary<string, ADUser> Users { get; set; } = new ConcurrentDictionary<string, ADUser>();
        public static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        //public static ConcurrentDictionary<string, JObject> Tag { get; set; } = new ConcurrentDictionary<string, JObject>();
        public static ConnectionContainer RunningConnection { get; set; } = new ConnectionContainer();
        public static Dictionary<string, string> TimeZoneConvert { get; set; } = new Dictionary<string, string>()
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
                            Task.Run(() => new ProcessRecvdMsg().StartProcess(ProjectData, "getProjectInfo"));
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
        public static int Get_TripMin(JObject triptime)
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                if (TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                {
                    dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                }
                DateTime tripDtm = new DateTime((int)triptime["year"], ((int)triptime["month"] + 1), (int)triptime["dayOfMonth"], (int)triptime["hourOfDay"], (int)triptime["minute"], (int)triptime["second"]);
                return (int)Math.Ceiling(tripDtm.Subtract(dtNow).TotalMinutes);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return 0;
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
    }
}