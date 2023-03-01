using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;

namespace Factory_of_the_Future
{
    public static class BoolFlag
    {
        public const string Yes = "Y";
        public const string No = "N";
    }
    public static class ADProperties
    {
        public const string ContainerName = "cn";
        public const string Department = "department";
        public const string DisplayName = "displayName";
        public const string FirstName = "givenName";
        public const string SurName = "sn";
        public const string LoginName = "SAMAccountName";
        public const string MemberOf = "memberOf";
        public const string MiddleName = "initials";
        public const string Name = "name";
        public const string PostalCode = "postalCode";
        public const string Email = "mail";
    }
    public class ADUser
    {
        public string UserId { get; set; } = "";
        public string FacilityName { get; set; }
        public string FacilityTimeZone { get; set; }
        public string Environment { get; set; }
        public string PageType { get; set; }
        public string Role { get; set; } = "Operator".ToUpper();
        public string ZipCode { get; set; } = "";
        public string Browser_Type { get; set; } = "";
        public string Browser_Name { get; set; } = "";
        public string Browser_Version { get; set; }
        public string FirstName { get; set; } = "";
        public string SurName { get; set; } = "";
        public string Domain { get; set; } = "";
        public string IpAddress { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string Error { get; set; } = "";
        public string App_Type { get; set; } = "";
        public string NASSCode { get; set; }
        public string FDBID { get; set; }
        public DateTime LoginDate { get; set; }
        public bool IsAuthenticated { get; set; }
        public string ConnectionId { get; set; } = "";
        public string VoiceTelephoneNumber { get; set; }
        public string EmailAddress { get; set; } = "";
        public string Phone { get; set; } = "";
        public string EIN { get; set; } = "";
        public object Login_Date { get; internal set; } = "";
        public string Session_ID { get; internal set; } = "";
        public string Server_IpAddress { get; internal set; } = "";
        public string Software_Version { get; internal set; } = "";
    }
    public class Properties
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("floorId")]
        public string FloorId { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; } = "";

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("Zone_TS")]
        public DateTime ZoneTS { get; set; }

        [JsonProperty("Zone_Update")]
        public bool ZoneUpdate { get; set; }

        [JsonProperty("Quuppa_Override")]
        public bool QuuppaOverride { get; set; }

        [JsonProperty("Zone_Type")]
        public string ZoneType { get; set; } = "";

        [JsonProperty("MPEWatchData")]
        public RunPerf MPEWatchData { get; set; } = new RunPerf();
   
        [JsonProperty("MPE_Type")]
        public string MPEType { get; set; } = "";

        [JsonProperty("bins")]
        public string Bins { get; set; } = "";

        [JsonProperty("MPE_Bins")]
        public IEnumerable<string> MPEBins { get; set; } = new List<string>();
        //[JsonProperty("SV_Zone_Data")]
        //public SVZoneData SVZoneData { get; set; } = new SVZoneData();

        [JsonProperty("MPE_Number")]
        public int MPENumber { get; set; }

        [JsonProperty("DPSData")]
        public DeliveryPointSequence DPSData { get; set; } = new DeliveryPointSequence();

        [JsonProperty("staffingData")]
        public Staff StaffingData { get; set; } = new Staff();

        [JsonProperty("CurrentStaff")]
        public int CurrentStaff { get; set; }
        
        [JsonProperty("doorNumber")]
        public string DoorNumber { get; set; }  = "";

        [JsonProperty("GpioNumber")]
        public int GpioNumber { get; set; } = -1;

        [JsonProperty("GpioValue")]
        public int GpioValue { get; set; } = -1;

        [JsonProperty("dockdoorData")]
        public IEnumerable<RouteTrips> DockDoorData { get; set; } = new List<RouteTrips>();

        [JsonProperty("rawData")]
        public string RawData { get; set; } = "";

        [JsonProperty("MissionList")]
        public IEnumerable<Mission> MissionList { get; set; } = new List<Mission>();
        [JsonProperty("source")]
        public string Source { get; set; } = "";
    }
    public class RoutePath
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("to")]
        public string To { get; set; } = "";

        [JsonProperty("from")]
        public string From { get; set; } = "";

        [JsonProperty("average_duration")]
        public string Average_Duration { get; set; } = "";
    }
    public class ZoneInfo
    {
        [JsonProperty("MPE_Type")]
        public string MPEType { get; set; }

        [JsonProperty("MPE_Number")]
        public string MPENumber { get; set; }

        [JsonProperty("Zone_LDC")]
        public string ZoneLDC { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("Zone_Update")]
        public bool ZoneUpdate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public class NotificationConditions
    {
        [JsonProperty("Id")]
        public string Id { get; set; } = "";

        [JsonProperty("ActiveCondition")]
        public bool ActiveCondition { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate{ get; set; }

        [JsonProperty("CreatedByUsername")]
        public string CreatedByUsername { get; set; } = "";

        [JsonProperty("LastupdateByUsername")]
        public string LastupdateByUsername { get; set; } = "";

        [JsonProperty("LastupdateDate")]
        public DateTime LastupdateDate { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; } = "";

        [JsonProperty("Type")]
        public string Type { get; set; } = "";

        [JsonProperty("Conditions")]
        public string Conditions { get; set; } = "";

        [JsonProperty("Warning")]
        public int Warning { get; set; } 

        [JsonProperty("WarningAction")]
        public string WarningAction { get; set; } = "";

        [JsonProperty("WarningColor")]
        public string WarningColor { get; set; } = "";

        [JsonProperty("Critical")]
        public int Critical { get; set; } 

        [JsonProperty("CriticalAction")]
        public string CriticalAction { get; set; } = "";

        [JsonProperty("CriticalColor")]
        public string CriticalColor { get; set; } = "";
    }
    public class TrackTagGroups
    {
        [JsonProperty("tagGroups")]
        public object TagGroups { get; set; }

        [JsonProperty("allTags")]
        public bool AllTags { get; set; }
    }
    public class DoorTrip {
        public string DoorNumber { get; set; }
        public string Route { get; set; }
        public string Trip { get; set; }
    }
    public class HourlyData
    {
        [JsonProperty("hour")]
        public string Hour { get; set; } = "";

        [JsonProperty("count")]
        public int Count { get; set; } = 0;
    }

    public class NullToEmptyStringResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return type.GetProperties()
                    .Select(p =>
                    {
                        var jp = base.CreateProperty(p, memberSerialization);
                        jp.ValueProvider = new NullToEmptyStringValueProvider(p);
                        return jp;
                    }).ToList();
        }
    }
    public class NullToEmptyStringValueProvider : IValueProvider
    {
        readonly PropertyInfo _MemberInfo;
        public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
        {
            _MemberInfo = memberInfo;
        }

        public object GetValue(object target)
        {
            object result = _MemberInfo.GetValue(target);
            if (_MemberInfo.PropertyType == typeof(string) && result == null)
            {
                result = "";
            }

            return result;

        }

        public void SetValue(object target, object value)
        {
            _MemberInfo.SetValue(target, value);
        }
    }
    public class MissedSel
    {
        [JsonProperty("empId")]
        public string EmpId { get; set; } = "";

        [JsonProperty("tagId")]
        public string TagId { get; set; } = "";

        [JsonProperty("tagName")]
        public string TagName { get; set; } = "";

        [JsonProperty("type")]
        public string Type { get; set; } = "";

        [JsonProperty("processedTs")]
        public object ProcessedTs { get; set; }

        [JsonProperty("processedTxt")]
        public string ProcessedTxt { get; set; } = "";

        [JsonProperty("tacs")]
        public Tacs Tacs { get; set; }
    }
    public class TacsTags
    {
        [JsonProperty("processedSince")]
        public string ProcessedSince { get; set; } = "";

        [JsonProperty("missedSelsCount")]
        public int MissedSelsCount { get; set; } = 0;

        [JsonProperty("missedSels")]
        public List<MissedSel> MissedSels { get; set; }
    }
    public class VehicleStatus
    {
        [JsonProperty("OBJECT_TYPE")]
        public string OBJECT_TYPE { get; set; }

        [JsonProperty("MESSAGE")]
        public string MESSAGE { get; set; } = "";

        [JsonProperty("NASS_CODE")]
        public string NASS_CODE { get; set; } = "";

        [JsonProperty("VEHICLE")]
        public string VEHICLE { get; set; } = "";

        [JsonProperty("VEHICLE_MAC_ADDRESS")]
        public string VEHICLE_MAC_ADDRESS { get; set; } = "";

        [JsonProperty("VEHICLE_NUMBER")]
        public int VEHICLE_NUMBER { get; set; }

        [JsonProperty("STATE")]
        public string STATE { get; set; } = "";

        [JsonProperty("ETA")]
        public string ETA { get; set; } = "";

        [JsonProperty("BATTERYPERCENT")]
        public string BATTERYPERCENT { get; set; } = "";

        [JsonProperty("CATEGORY")]
        public int CATEGORY { get; set; }

        [JsonProperty("X_LOCATION")]
        public string X_LOCATION { get; set; } = "";

        [JsonProperty("Y_LOCATION")]
        public string Y_LOCATION { get; set; } = "";
        [JsonProperty("ERRORCODE")]
        public string ERRORCODE { get; set; } = "";

        [JsonProperty("ERRORCODE_DISCRIPTION")]
        public string ERRORCODE_DISCRIPTION { get; set; }

        [JsonProperty("TIME")]
        public DateTime TIME { get; set; }
    }
    public class Mission
    {
        [JsonProperty("RequestId")]
        public string REQUEST_ID { get; set; } = "";

        [JsonProperty("Vehicle")]
        public string VEHICLE { get; set; } = "";

        [JsonProperty("Vehicle_Number")]
        public int VEHICLE_NUMBER { get; set; } = 0;

        [JsonProperty("NASS_Code")]
        public string NASS_CODE { get; set; } = "";

        [JsonProperty("Pickup_Location")]
        public string PICKUP_LOCATION { get; set; } = "";

        [JsonProperty("Dropoff_Location")]
        public string DROPOFF_LOCATION { get; set; } = "";

        [JsonProperty("End_Location")]
        public string END_LOCATION { get; set; } = "";

        [JsonProperty("Door")]
        public string DOOR { get; set; } = "";

        [JsonProperty("ETA")]
        public string ETA { get; set; } = "";

        [JsonProperty("Placard")]
        public string PLACARD { get; set; } = "";

        [JsonProperty("QueuePosition")]
        public string QUEUEPOSITION { get; set; }

        [JsonProperty("State")]
        public string STATE { get; set; } = "";

        [JsonProperty("MissionType")]
        public string MISSIONTYPE { get; set; } = "";

        [JsonProperty("MissionRequestTime")]
        public DateTime MISSIONREQUESTTIME { get; set; }

        [JsonProperty("MissionAssignedTime")]
        public DateTime MISSIONASSIGNEDTIME { get; set; }

        [JsonProperty("MissionPickupTime")]
        public DateTime MISSIONPICKUPTIME { get; set; }

        [JsonProperty("MissionDropOffTime")]
        public DateTime MISSIONDROPOFFTIME { get; set; }

        [JsonProperty("Error_Discription")]
        public string ERROR_DISCRIPTION { get; set; }

        [JsonProperty("MissionErrorTime")]
        public DateTime MISSIONERRORTIME { get; set; }
    }
    public class Notification
    {
        [JsonProperty("Delete")]
        public bool Delete { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; } = "";

        [JsonProperty("ActiveCondition")]
        public bool ActiveCondition { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("CreatedByUsername")]
        public string CreatedByUsername { get; set; } = "";

        [JsonProperty("LastupdateByUsername")]
        public string LastupdateByUsername { get; set; } = "";

        [JsonProperty("LastupdateDate")]
        public DateTime LastupdateDate { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; } = "";

        [JsonProperty("Type")]
        public string Type { get; set; } = "";

        [JsonProperty("Conditions")]
        public string Conditions { get; set; } = "";

        [JsonProperty("Warning")]
        public int Warning { get; set; }

        [JsonProperty("WarningAction")]
        public string WarningAction { get; set; } = "";

        [JsonProperty("WarningColor")]
        public string WarningColor { get; set; } = "";

        [JsonProperty("Critical")]
        public int Critical { get; set; }

        [JsonProperty("CriticalAction")]
        public string CriticalAction { get; set; } = "";

        [JsonProperty("CriticalColor")]
        public string CriticalColor { get; set; } = "";

        [JsonProperty("TypeID")]
        public string Type_ID { get; set; } = "";

        [JsonProperty("TypeName")]
        public string Type_Name { get; set; } = "";

        [JsonProperty("TypeStatus")]
        public string Type_Status { get; set; } = "";

        [JsonProperty("TypeDuration")]
        public int? Type_Duration { get; set; }

        [JsonProperty("NotificationID")]
        public string Notification_ID { get; set; } = "";

        [JsonProperty("NotificationUpdate")]
        public bool Notification_Update { get; set; }

        [JsonProperty("TypeTime")]
        public DateTime Type_Time { get; set; }

    }  
    //public class RPGPlan
    //{
    //    public DateTime mods_date { get; set; }
    //    public string machine_num { get; set; } = "";
    //    public string sort_program_name { get; set; } = "";
    //    public DateTime rpg_start_dtm { get; set; }
    //    public DateTime rpg_end_dtm { get; set; }
    //    public string rpg_pieces_fed { get; set; } = "";
    //    public int mail_operation_nbr { get; set; } = 0;
    //    public string line_4_text { get; set; } = "";
    //    public string rpg_expected_thruput { get; set; } = "";
    //    public DateTime mpew_start_15min_dtm { get; set; }
    //    public DateTime mpew_end_15min_dtm { get; set; }
    //    public string mpe_type { get; set; } = "";
    //    public string mpe_name { get; set; } = "";
    //    public DateTime update_date_time { get; set; }
    //    public string nass_code { get; set; } = "";
    //    public string expected_throughput { get; set; } = "";
    //}
    public class MachData
    {
        [JsonProperty("site_name_local")]
        public string SiteNameLocal { get; set; } = "";

        [JsonProperty("area_name")]
        public string AreaName { get; set; } = "";

        [JsonProperty("district")]
        public string District { get; set; } = "";

        [JsonProperty("local_link")]
        public string LocalLink { get; set; } = "";

        [JsonProperty("rnum")]
        public string Rnum { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; } = "";

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; } = "";
    }
    public class SV_Site_Info
    {
        [JsonProperty("siteId")]
        public string SiteId;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("displayName")]
        public string DisplayName;

        [JsonProperty("timeZoneAbbr")]
        public string TimeZoneAbbr;

        [JsonProperty("financeNumber")]
        public string FinanceNumber;

        [JsonProperty("zipCode")]
        public string ZipCode;

        [JsonProperty("areaCode")]
        public string AreaCode;

        [JsonProperty("areaName")]
        public string AreaName;

        [JsonProperty("districtCode")]
        public string DistrictCode;

        [JsonProperty("districtName")]
        public string DistrictName;

        [JsonProperty("localeKey")]
        public string LocaleKey;

        [JsonProperty("fdbId")]
        public string FdbId;

        [JsonProperty("updtUserId")]
        public string UpdtUserId;

        [JsonProperty("tours")]
        public SV_Tours Tours;

        [JsonProperty("facilityId")]
        public string FacilityId;

        [JsonProperty("agvInd")]
        public string AgvInd;
    }
    public class SV_Tours
    {
        [JsonProperty("siteId")]
        public string SiteId;

        [JsonProperty("tour1Start")]
        public string Tour1Start;

        [JsonProperty("tour1End")]
        public string Tour1End;

        [JsonProperty("tour2Start")]
        public string Tour2Start;

        [JsonProperty("tour2End")]
        public string Tour2End;

        [JsonProperty("tour3Start")]
        public string Tour3Start;

        [JsonProperty("tour3End")]
        public string Tour3End;

        [JsonProperty("startingTour")]
        public int StartingTour;

        [JsonProperty("updtUserId")]
        public string UpdtUserId;
    }

    public class PropertyRenameAndIgnoreSerializerContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignores;
        private readonly Dictionary<Type, Dictionary<string, string>> _renames;

        public PropertyRenameAndIgnoreSerializerContractResolver()
        {
            _ignores = new Dictionary<Type, HashSet<string>>();
            _renames = new Dictionary<Type, Dictionary<string, string>>();
        }

        public void IgnoreProperty(Type type, params string[] jsonPropertyNames)
        {
            if (!_ignores.ContainsKey(type))
            {
                _ignores[type] = new HashSet<string>();
            }

            foreach (var prop in jsonPropertyNames)
            {
                _ignores[type].Add(prop);
            }
        }

        public void RenameProperty(Type type, string propertyName, string newJsonPropertyName)
        {
            if (!_renames.ContainsKey(type))
            {
                _renames[type] = new Dictionary<string, string>();
            }

            _renames[type][propertyName] = newJsonPropertyName;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = i => false;
                property.Ignored = true;
            }

            if (IsRenamed(property.DeclaringType, property.PropertyName, out var newJsonPropertyName))
            {
                property.PropertyName = newJsonPropertyName;
            }

            return property;
        }

        private bool IsIgnored(Type type, string jsonPropertyName)
        {
            if (!_ignores.ContainsKey(type))
            {
                return false;
            }

            return _ignores[type].Contains(jsonPropertyName);
        }

        private bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName)
        {
            Dictionary<string, string> renames;

            if (!_renames.TryGetValue(type, out renames) || !renames.TryGetValue(jsonPropertyName, out newJsonPropertyName))
            {
                newJsonPropertyName = null;
                return false;
            }

            return true;
        }
    }
    public class SessionKey
    {
        public const string ADUser = "ADUser";
        public const string Session_ID = "Session_ID";
        public const string IsAuthenticated = "IsAuthenticated";
        public const string AceId = "ACEID";
        public const string UserRole = "UserRole";
        public const string UserFirstName = "UserFirstName";
        public const string UserLastName = "UserLastName";
        public const string Facility_NASS_CODE = "Facility_NASS_CODE";
        public const string Facility_Id = "Facility_Id";
        public const string Facility_Name = "Facility_Name";
        public const string Facility_FDBID = "Facility_FDBID";
        public const string Environment = "Environment";
        public const string FacilityTimeZone = "FacilityTimeZone";
        public const string SoftwareVersion = "SoftwareVersion";
        public const string ApplicationFullName = "ApplicationFullName";
        public const string ApplicationAbbr = "ApplicationAbbr";
        public const string Server_IpAddress = "Server_IpAddress";
        public const string IpAddress = "IpAddress";
        public const string Domain = "Domain";
        public const string Browser_Type = "Browser_Type";
        public const string Browser_Name = "Browser_Name";
        public const string Browser_Version = "Browser_Version";
    }
    public class AuthenticationCookie
    {
        private static readonly string Name = FormsAuthentication.FormsCookieName;

        public static HttpCookie Create(string userId, string userData, bool isCookiePersistent)
        {
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                1, // Version
                userId, // ACE User
                DateTime.Now, // Created
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.Minutes), // Expires ( can be defined in web.config )
                isCookiePersistent, // Persistent
                userData); // User Role

            return CreateCookie(authTicket);
        }

        public static string GetData(HttpRequest request)
        {
            var authTicket = GetAuthTicket(request);
            return authTicket == null ? string.Empty : authTicket.UserData;
        }

        public static string GetAuthTicketName(HttpRequest request)
        {
            var authTicket = GetAuthTicket(request);
            return authTicket == null ? string.Empty : authTicket.Name;
        }

        public static HttpCookie OverrideUserData(string userData, HttpRequest request)
        {
            var authTicket = GetAuthTicket(request);

            var encryptedTicket = new FormsAuthenticationTicket(
                authTicket.Version,
                authTicket.Name,
                authTicket.IssueDate,
                authTicket.Expiration,
                authTicket.IsPersistent,
                userData);

            return CreateCookie(encryptedTicket);
        }

        private static HttpCookie CreateCookie(FormsAuthenticationTicket authTicket)
        {
            var encryptedAuthTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie authCookie = new HttpCookie(Name, encryptedAuthTicket)
            {
                Secure = true,
                HttpOnly = true
            };
            if (authTicket.IsPersistent)
            {
                authCookie.Expires = authTicket.Expiration;
            }

            return authCookie;
        }

        private static FormsAuthenticationTicket GetAuthTicket(HttpRequest request)
        {
            HttpCookie authCookie = request.Cookies[Name];

            if (authCookie != null)
            {
                return FormsAuthentication.Decrypt(authCookie.Value);
            }

            return null;
        }
    }
    public class Converter
    {
        public static string ObjectToString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T StringToObject<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public static Nullable<T> GetValueOrNull<T>(string value)
            where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static T ToEnum<T>(string value, bool ignoreCase = false)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static bool GetBoolean(string boolFlag)
        {
            if (boolFlag == null)
            {
                throw new ArgumentNullException("boolFlag");
            }

            if (boolFlag == BoolFlag.Yes || boolFlag.ToLower() == "yes")
            {
                return true;
            }
            else if (boolFlag == BoolFlag.No || boolFlag.ToLower() == "no")
            {
                return false;
            }
            else
            {
                throw new FormatException("Input string was not in a BoolFlag Format");
            }
        }

        public static string GetBoolFlag(bool value)
        {
            return value ? BoolFlag.Yes : BoolFlag.No;
        }
    }
}