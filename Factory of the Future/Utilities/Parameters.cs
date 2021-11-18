using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Factory_of_the_Future
{
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

    public enum ProcessType
    {
        None = 0,
        WhenAll = 1,
        WaitAll = 2,
        Parallel = 3
    }

    public class JObject_List
    {
        public JObject _notification_conditions = new JObject();

        public JObject Notification_Conditions
        {
            get
            {
                _notification_conditions["ID"] = 0;
                _notification_conditions["ACTIVE_CONDITION"] = false;
                _notification_conditions["CREATED_DATE"] = DateTime.Now;
                _notification_conditions["CREATED_BY_USERNAME"] = "";
                _notification_conditions["LASTUPDATE_BY_USERNAME"] ="";
                _notification_conditions["LASTUPDATE_DATE"] = DateTime.Now;
                _notification_conditions["NAME"] ="";
                _notification_conditions["TYPE"] ="";
                _notification_conditions["CONDITIONS"] = "";
                _notification_conditions["WARNING"] = "";
                _notification_conditions["WARNING_ACTION"] = "";
                _notification_conditions["WARNING_COLOR"] = "";
                _notification_conditions["CRITICAL"] = "";
                _notification_conditions["CRITICAL_ACTION"] = "";
                _notification_conditions["CRITICAL_COLOR"] = "";
                return _notification_conditions;
            }
            set { return; }
        }

        public JObject _qsm_connection = new JObject();

        public JObject QSM_Connection
        {
            get
            {
                _qsm_connection["CONNECTION_NAME"] = "";
                _qsm_connection["NUMBER_OF_CONNECTION"] = "";
                _qsm_connection["INDEX"] = "";
                return _qsm_connection;
            }
            set { return; }
        }

        public JObject api = new JObject();

        public JObject API
        {
            get
            {
                api["ID"] = 0;
                api["CONNECTION_NAME"] = "";
                api["CREATED_DATE"] =  DateTime.Now;
                api["CREATED_BY_USERNAME"] = "";
                api["LASTUP_DATE"] = "";
                api["LASTUPDATE_BY_USERNAME"] = "";
                api["ACTIVE_CONNECTION"] = false;
                api["UDP_CONNECTION"] = false;
                api["DEACTIVATED_BY_USERNAME"] = "";
                api["DEACTIVATED_DATE"] = "";
                api["API_CONNECTED"] = false;
                api["UPDATE_STATUS"] = true;
                api["LASTTIME_API_CONNECTED"] = DateTime.Now.AddDays(-24);
                api["DATA_RETRIEVE"] = 60000;
                api["HTTPS"] = false;
                api["ADMIN_EMAIL_RECEPIENT"] = "";
                api["HOSTNAME"] = "";
                api["IP_ADDRESS"] = "";
                api["PORT"] = 0;
                api["URL"] = "";
                api["OUTGOING_APIKEY"] = "";
                api["MESSAGE_TYPE"] = "";
                api["NASS_CODE"] = "";
                return api;
            }
            set { return; }
        }

        public JObject GeoJSON_Zone
        {
            get
            {
                return new JObject(
                    new JProperty("type", "Feature"),
              new JProperty("geometry",
              new JObject(new JProperty("type", ""),
               new JProperty("coordinates", new JArray()))),
              new JProperty("properties", ZoneProperties));
            }
            set { return; }
        }

        public JObject zoneproperties = new JObject();

        public JObject ZoneProperties
        {
            get
            {
                zoneproperties["id"] = "";
                zoneproperties["visible"] = false;
                zoneproperties["color"] = "";
                zoneproperties["name"] = "";
                zoneproperties["Zone_TS"] = DateTime.Now;
                zoneproperties["Zone_Update"] = false;
                zoneproperties["Zone_Type"] = "";
                zoneproperties["MPEWatchData"] = "";
                zoneproperties["nameOverride"] = false;
                zoneproperties["MPE_Type"] = "";
                zoneproperties["MPE_Number"] = "";
                zoneproperties["DPSData"] = "";
                zoneproperties["CurrentStaff"] = 0;
                zoneproperties["Raw_Data"] = "";
                return zoneproperties;
            }
            set { return; }
        }

        public JObject GeoJSON_Tag
        {
            get
            {
                return new JObject(
                    new JProperty("type", "Feature"),
              new JProperty("geometry",
              new JObject(new JProperty("type", ""),
               new JProperty("coordinates", new JArray()))),
              new JProperty("properties", Properties));
            }
            set { return; }
        }

        public JObject properties = new JObject();

        public JObject Properties
        {
            get
            {
               
                properties["id"] = "";
                properties["visible"] = false;
                properties["zones"] = new JArray();
                properties["color"] = "";
                properties["tagVisible"] = false;
                properties["tagVisibleMils"] = 0;
                properties["isWearingTag"] = false;
                properties["name"] = "";
                properties["craftName"] = "";
                properties["badgeId"] = "";
                properties["positionTS"] = DateTime.Now;
                properties["Tag_TS"] = DateTime.Now;
                properties["Tag_Type"] = "";
                properties["Tag_Update"] = false;
                properties["Employee_EIN"] = "";
                properties["Employee_Group_type"] = "";
                properties["Employee_Name"] = "";
                properties["Employee_PL"] = "";
                properties["Employee_Role"] = "";
                properties["isLdcAlert"] =false;
                properties["currentLDCs"] = "";
                properties["Tacs"] = new JObject();
                properties["HasTablet"] = false;
                properties["TabletSN"] ="";
                properties["Raw_Data"] = "";
                return properties;
            }
            set { return; }
        }

        public JObject map = new JObject();

        public JObject Map
        {
            get
            {
                map["Id"] = "";
                map["Site_Id"] = "";
                map["OrigoY"] = "";
                map["OrigoX"] = "";
                map["MetersPerPixelY"] = "";
                map["MetersPerPixelX"] = ""; 
                map["WidthMeter"] = "";
                map["HeightMeter"] = "";
                map["YMeter"] = "";
                map["XMeter"] = "";
                map["Base64Img"] = "";
                map["Facility_Name"] = "";
                map["Facility_TimeZone"] = "";
                map["Environment"] = "";
                map["Software_Version"] = "";
                map["Map_Update"] = false;
                return map;
            }
            set { return; }
        }

        public JObject aduser = new JObject();

        public JObject ADuser
        {
            get
            {
                aduser["UserId"] = "";
                aduser["Facility_Name"] = "";
                aduser["Facility_TimeZone"] = "";
                aduser["Environment"] = "";
                aduser["PageType"] = "";
                aduser["BodyType"] = "";
                aduser["Role"] = "";
                aduser["ZipCode"] = "";
                aduser["Browser_Type"] = "";
                aduser["Browser_Name"] = "";
                aduser["Browser_Version"] = "";
                aduser["FirstName"] = "";
                aduser["SurName"] = "";
                aduser["IpAddress"] = "";
                aduser["Server_IpAddress"] = "";
                aduser["MiddleName"] ="";
                aduser["Error"] = "";
                aduser["App_Type"] = "";
                aduser["Software_Version"] = "";
                aduser["Domain"] = "";
                aduser["NASS_Code"] = "";
                aduser["FDB_ID"] = "";
                aduser["Login_Date"] = DateTime.Now;
                aduser["Session_ID"] = "";
                aduser["IsAuthenticated"] = false;
                aduser["GroupNames"] = "";
                aduser["ConnectionId"] = "";
                aduser["VoiceTelephoneNumber"] = "";
                aduser["EmailAddress"] = "";
                aduser["Phone"] = "";
                aduser["EIN"] = "";
                aduser["TAG_ID"] = "";
                return aduser;
            }
            set { return; }
        }
    
    }
    public class EventDtm
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [JsonProperty("hourOfDay")]
        public int HourOfDay { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("second")]
        public int Second { get; set; }
    }

    public class ContainerHistory
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("eventDtm")]
        public EventDtm EventDtm { get; set; }
  
        public DateTime EventDtmfmt { 
            get {                 
                return new DateTime(EventDtm.Year,(EventDtm.Month + 1),EventDtm.DayOfMonth,EventDtm.HourOfDay,EventDtm.Minute,EventDtm.Second);
            } set { return; } 
        }

        [JsonProperty("siteId")]
        public string SiteId { get; set; }

        [JsonProperty("updtUserId")]
        public string UpdtUserId { get; set; }

        [JsonProperty("siteName")]
        public string SiteName { get; set; }

        [JsonProperty("siteType")]
        public string SiteType { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("trip")]
        public string Trip { get; set; }

        [JsonProperty("trailer")]
        public string Trailer { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("redirectInd")]
        public string RedirectInd { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("binNumber")]
        public int? BinNumber { get; set; }

        [JsonProperty("binName")]
        public string BinName { get; set; }

        [JsonProperty("opAreaName")]
        public string OpAreaName { get; set; }

        [JsonProperty("opAreaDesc")]
        public string OpAreaDesc { get; set; }
        public int sortind { get; set; }
    }

    internal class SVdatetimeformat
    {
        private EventDtm eventDtm;

        public SVdatetimeformat(EventDtm eventDtm)
        {
            this.eventDtm = eventDtm;
        }
    }

    public class Container
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; }

        [JsonProperty("placardBarcode")]
        public string PlacardBarcode { get; set; }

        [JsonProperty("ctrTypeCode")]
        public string CtrTypeCode { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("dest")]
        public string Dest { get; set; }

        [JsonProperty("containerHistory")]
        public List<ContainerHistory> ContainerHistory { get; set; }

        [JsonProperty("originName")]
        public string OriginName { get; set; }

        [JsonProperty("destinationName")]
        public string DestinationName { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("binNumber")]
        public int? BinNumber { get; set; }

        [JsonProperty("binName")]
        public string BinName { get; set; }

        [JsonProperty("mailClass")]
        public string MailClass { get; set; }

        [JsonProperty("mailType")]
        public string MailType { get; set; }

        public DateTime EventDtm { get; set; }
        public string MailClassDisplay { get; set; }
        public string binDisplay { get; set; }
        public bool hasPrintScans { get; set; }
        public bool hasAssignScans { get; set; }
        public bool hasCloseScans { get; set; }
        public bool hasLoadScans { get; set; }
        public bool hasUnloadScans { get; set; }
        public bool containerTerminate { get; set; }
        public bool containerAtDest { get; set; }
        public string Oroute { get; set; }
        public string Otrip { get; set; }
        public string Otrailer { get; set; }
        public string Iroute { get; set; }
        public string Itrip { get; set; }
        public string Itrailer { get; set; }
        public bool containerRedirectedDest { get; set; }
    }
    public class Connection
    {
        [JsonProperty("ID")]
        public int ID;

        [JsonProperty("CONNECTION_NAME")]
        public string CONNECTION_NAME;

        [JsonProperty("CREATED_DATE")]
        public DateTime CREATED_DATE;

        [JsonProperty("CREATED_BY_USERNAME")]
        public string CREATED_BY_USERNAME;

        [JsonProperty("LASTUP_DATE")]
        public string LASTUP_DATE;

        [JsonProperty("LASTUPDATE_BY_USERNAME")]
        public string LASTUPDATE_BY_USERNAME;

        [JsonProperty("ACTIVE_CONNECTION")]
        public bool ACTIVE_CONNECTION;

        [JsonProperty("UDP_CONNECTION")]
        public bool UDP_CONNECTION;

        [JsonProperty("DEACTIVATED_BY_USERNAME")]
        public string DEACTIVATED_BY_USERNAME;

        [JsonProperty("DEACTIVATED_DATE")]
        public string DEACTIVATE_DDATE;

        [JsonProperty("CONECTION_CONNECTED")]
        public bool CONECTION_CONNECTED;

        [JsonProperty("UPDATE_STATUS")]
        public bool UPDATE_STATUS;

        [JsonProperty("LASTTIME_CONECTION_CONNECTED")]
        public DateTime LASTTIME_CONECTION_CONNECTED;

        [JsonProperty("DATA_RETRIEVE")]
        public int DATA_RETRIEVE;

        [JsonProperty("HTTPS")]
        public bool HTTPS;

        [JsonProperty("ADMIN_EMAIL_RECEPIENT")]
        public string ADMIN_EMAIL_RECEPIENT;

        [JsonProperty("HOSTNAME")]
        public string HOSTNAME;

        [JsonProperty("IP_ADDRESS")]
        public string IP_ADDRESS;

        [JsonProperty("PORT")]
        public int PORT;

        [JsonProperty("URL")]
        public string URL;

        [JsonProperty("OUTGOING_APIKEY")]
        public string OUTGOING_APIKEY;

        [JsonProperty("MESSAGE_TYPE")]
        public string MESSAGE_TYPE;

        [JsonProperty("NASS_CODE")]
        public string NASS_CODE;
    }
}