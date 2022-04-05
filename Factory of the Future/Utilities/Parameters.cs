using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    public class ADUser
    {
        public string UserId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityTimeZone { get; set; }
        public string Environment { get; set; }
        public string PageType { get; set; }
        public string Role { get; set; }
        public string ZipCode { get; set; }
        public string BrowserType { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string IpAddress { get; set; }
        public string ServerIpAddress { get; set; }
        public string MiddleName { get; set; }
        public string Error { get; set; }
        public string AppType { get; set; }
        public string SoftwareVersion { get; set; }
        public string Domain { get; set; }
        public string NASSCode { get; set; }
        public string FDBID { get; set; }
        public DateTime LoginDate { get; set; }
        public string SessionID { get; set; }
        public bool IsAuthenticated { get; set; }
        public string GroupNames { get; set; }
        public string ConnectionId { get; set; }
        public string VoiceTelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string EIN { get; set; }
        public string TAGID { get; set; }
    }
    public class Locator
    {
        [JsonProperty("locationLocked")]
        public bool LocationLocked { get; set; }

        [JsonProperty("orientation")]
        public List<double> Orientation { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("focusingErrorDeg")]
        public double FocusingErrorDeg { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("isOrientationSetManually")]
        public bool IsOrientationSetManually { get; set; }

        [JsonProperty("associatedAreas")]
        public List<string> AssociatedAreas { get; set; }

        [JsonProperty("locationLockedX")]
        public bool LocationLockedX { get; set; }

        [JsonProperty("locatorChannel")]
        public string LocatorChannel { get; set; }

        [JsonProperty("locationLockedZ")]
        public bool LocationLockedZ { get; set; }

        [JsonProperty("locationLockedY")]
        public bool LocationLockedY { get; set; }

        [JsonProperty("locatorType")]
        public string LocatorType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public List<double> Location { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("rawData")]
        public string RawData { get; set; }
        [JsonProperty("locatorUpdate")]
        public bool LocatorUpdate { get; set; }
    }

    public class BackgroundImage
    {
        [JsonProperty("widthMeter")]
        public double WidthMeter { get; set; }

        [JsonProperty("xMeter")]
        public double XMeter { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("otherCoordSys")]
        public string OtherCoordSys { get; set; }

        [JsonProperty("rotation")]
        public int Rotation { get; set; }

        [JsonProperty("base64")]
        public string Base64 { get; set; }

        [JsonProperty("origoY")]
        public double OrigoY { get; set; }

        [JsonProperty("origoX")]
        public double OrigoX { get; set; }

        [JsonProperty("heightMeter")]
        public double HeightMeter { get; set; }

        [JsonProperty("yMeter")]
        public double YMeter { get; set; }

        [JsonProperty("alpha")]
        public int Alpha { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("metersPerPixelY")]
        public double MetersPerPixelY { get; set; }

        [JsonProperty("metersPerPixelX")]
        public double MetersPerPixelX { get; set; }
        [JsonProperty("updateStatus")]
        public bool UpdateStatus { get; set; } = false;
        [JsonProperty("rawData")]
        public string RawData { get; set; }
    }

    public class TrackingArea
    {
        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("maxZ")]
        public int MaxZ { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("minZ")]
        public int MinZ { get; set; }

        [JsonProperty("track3d")]
        public bool Track3d { get; set; }

        [JsonProperty("trackingAreaGroup")]
        public string TrackingAreaGroup { get; set; }
    }

    public class PolygonHole
    {
        [JsonProperty("locationLocked")]
        public bool LocationLocked { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("polygonData")]
        public string PolygonData { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Polygon
    {
        [JsonProperty("locationLocked")]
        public bool LocationLocked { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("polygonData")]
        public string PolygonData { get; set; }

        [JsonProperty("trackingArea")]
        public TrackingArea TrackingArea { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("polygonHoles")]
        public List<PolygonHole> PolygonHoles { get; set; }
    }
    public class Geometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<List<List<double>>> Coordinates { get; set; }
    }

    public class MissionList
    {
        [JsonProperty("Request_Id")]
        public string RequestId { get; set; }

        [JsonProperty("Vehicle")]
        public string Vehicle { get; set; }

        [JsonProperty("Vehicle_Number")]
        public string VehicleNumber { get; set; }

        [JsonProperty("Pickup_Location")]
        public string PickupLocation { get; set; }

        [JsonProperty("Dropoff_Location")]
        public string DropoffLocation { get; set; }

        [JsonProperty("End_Location")]
        public string EndLocation { get; set; }

        [JsonProperty("Door")]
        public string Door { get; set; }

        [JsonProperty("ETA")]
        public string ETA { get; set; }

        [JsonProperty("Placard")]
        public string Placard { get; set; }

        [JsonProperty("QueuePosition")]
        public string QueuePosition { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("MissionType")]
        public string MissionType { get; set; }

        [JsonProperty("MissionRequestTime")]
        public DateTime MissionRequestTime { get; set; }

        [JsonProperty("MissionAssignedTime")]
        public DateTime? MissionAssignedTime { get; set; }

        [JsonProperty("MissionPickupTime")]
        public DateTime? MissionPickupTime { get; set; }
    }

    public class Properties
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Zone_TS")]
        public DateTime ZoneTS { get; set; }

        [JsonProperty("Zone_Update")]
        public bool ZoneUpdate { get; set; }

        [JsonProperty("Zone_Type")]
        public string ZoneType { get; set; }

        [JsonProperty("MPEWatchData")]
        public string MPEWatchData { get; set; }

        [JsonProperty("MPE_Type")]
        public string MPEType { get; set; }

        [JsonProperty("MPE_Number")]
        public string MPENumber { get; set; }

        [JsonProperty("DPSData")]
        public string DPSData { get; set; }

        [JsonProperty("CurrentStaff")]
        public int CurrentStaff { get; set; }

        [JsonProperty("rawData")]
        public string RawData { get; set; }

        [JsonProperty("MissionList")]
        public List<MissionList> MissionList { get; set; } = new List<MissionList>();
    }

    public class GeoZone
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }
    }
    public class GeoMarker
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("properties")]
        public Marker Properties { get; set; }
    }
    public class Marker
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("rFId")]
        public string RFid { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("zones")]
        public List<Zone> Zones { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("tagVisible")]
        public bool TagVisible { get; set; }

        [JsonProperty("tagVisibleMils")]
        public int TagVisibleMils { get; set; }

        [JsonProperty("isWearingTag")]
        public bool IsWearingTag { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("craftName")]
        public string CraftName { get; set; }

        [JsonProperty("positionTS")]
        public DateTime PositionTS { get; set; }

        [JsonProperty("Tag_TS")]
        public DateTime TagTS { get; set; }

        [JsonProperty("Tag_Type")]
        public string TagType { get; set; }

        [JsonProperty("Tag_Update")]
        public bool TagUpdate { get; set; }

        [JsonProperty("empId")]
        public string EmpId { get; set; }

        [JsonProperty("emptype")]
        public string Emptype { get; set; }

        [JsonProperty("empName")]
        public string EmpName { get; set; }

        [JsonProperty("isLdcAlert")]
        public bool IsLdcAlert { get; set; }

        [JsonProperty("currentLDCs")]
        public string CurrentLDCs { get; set; }

        [JsonProperty("tacs")]
        public string Tacs { get; set; }

        [JsonProperty("sels")]
        public string Sels { get; set; }

        [JsonProperty("Raw_Data")]
        public string RawData { get; set; }
    }


    public class Zone
    {
        [JsonProperty("locationLocked")]
        public bool LocationLocked { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("zoneGroupId")]
        public string ZoneGroupId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("polygonData")]
        public string PolygonData { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("polygonHoles")]
        public List<object> PolygonHoles { get; set; }
        [JsonIgnore]
        public bool UpdateStatus { get; set; } = false;
    }

    public class CoordinateSystem
    {
        [JsonProperty("locators")]
        public List<Locator> Locators { get; set; }

        [JsonProperty("backgroundImages")]
        public List<BackgroundImage> BackgroundImages { get; set; }

        [JsonProperty("relativeZ")]
        public int RelativeZ { get; set; }

        [JsonProperty("polygons")]
        public List<Polygon> Polygons { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("zones")]
        public List<Zone> Zones { get; set; }
    }

    public class ZoneGroup
    {
        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("smartZone")]
        public bool SmartZone { get; set; }

        [JsonProperty("hideLocation")]
        public bool HideLocation { get; set; }

        [JsonProperty("activationCommands")]
        public List<object> ActivationCommands { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("rfBlocking")]
        public bool RfBlocking { get; set; }
    }

    public class TrackTagGroups
    {
        [JsonProperty("tagGroups")]
        public object TagGroups { get; set; }

        [JsonProperty("allTags")]
        public bool AllTags { get; set; }
    }

    public class TrackingAreaGroup
    {
        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("trackTagGroups")]
        public TrackTagGroups TrackTagGroups { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("radiusThreshold")]
        public double RadiusThreshold { get; set; }

        [JsonProperty("snapToPolygonDistanceZ")]
        public int SnapToPolygonDistanceZ { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("snapToPolygonDistance")]
        public double SnapToPolygonDistance { get; set; }

        [JsonProperty("mode3d")]
        public string Mode3d { get; set; }
    }

    public class TagGroup
    {
        [JsonProperty("trackHeight")]
        public double TrackHeight { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("sensorOnly")]
        public bool SensorOnly { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("unknownTags")]
        public bool UnknownTags { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("track3d")]
        public bool Track3d { get; set; }

        [JsonProperty("track")]
        public bool Track { get; set; }
    }

    public class ProjectInfo
    {
        [JsonProperty("coordinateSystems")]
        public List<CoordinateSystem> CoordinateSystems { get; set; }

        [JsonProperty("gatewayFilters")]
        public List<object> GatewayFilters { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("responseTS")]
        public long ResponseTS { get; set; }

        [JsonProperty("zoneGroups")]
        public List<ZoneGroup> ZoneGroups { get; set; }

        [JsonProperty("trackingAreaGroups")]
        public List<TrackingAreaGroup> TrackingAreaGroups { get; set; }

        [JsonProperty("tagGroups")]
        public List<TagGroup> TagGroups { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("localdata")]
        public bool Localdata { get; set; }
    }


    public class JObject_List
    {
        public JObject _notification_conditions = new JObject();

        public JObject Notification_Conditions
        {
            get
            {
                _notification_conditions["id"] = Guid.NewGuid();
                _notification_conditions["ACTIVE_CONDITION"] = false;
                _notification_conditions["CREATED_DATE"] = DateTime.Now;
                _notification_conditions["CREATED_BY_USERNAME"] = "";
                _notification_conditions["LASTUPDATE_BY_USERNAME"] ="";
                _notification_conditions["LASTUPDATE_DATE"] = new DateTime(1, 1, 1, 0, 0, 0);
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

        
        public JObject _container = new JObject();
        public JObject Container
        {
            get
            {
                _container["placardBarcode"] = "";
                _container["mailClassDisplay"] = "";
                _container["mailClass"] = "";
                _container["mailTypeDisplay"] = "";
                _container["containerTypeDisplay"] = "";
                _container["origin"] = "";
                _container["originName"] = "";
                _container["dest"] = "";
                _container["destinationName"] = "";
                _container["opArea"] = "";
                _container["opAreaDesc"] = "";
                _container["eventSite"] = "";
                _container["eventSiteName"] = "";
                _container["eventSiteType"] = "";
                _container["eventDisplay"] = "";
                _container["eventDtm"] = "";
                _container["redirected"] = "";
                _container["redirectInd"] = "";
                _container["location"] = "";
                _container["binDisplay"] = "";
                _container["trailer"] = "";
                _container["route"] = "";
                _container["trip"] = "";
                _container["source"] = "";
                _container["userId"] = "";
                _container["hasMissedAssign"] = 0;
                _container["hasMissedClose"] = 0;
                _container["hasMissedLoad"] = 0;
                _container["hasMissedUnload"] = 0;
                _container["hasAssignScans"] = 0;
                _container["hasCloseScans"] = 0;
                _container["hasLoadScans"] = 0;
                _container["hasUnloadScans"] = 0;
                _container["assignLocation"] = "";
                _container["complianceDate"] = "";
                _container["containerRedirectedDest"] = "";

                return _container;
            }
            set { return; }
        }
        public JObject api = new JObject();
        public JObject API
        {
            get
            {
                api["id"] = Guid.NewGuid();
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
                api["LASTTIME_API_CONNECTED"] = new DateTime(1, 1, 1, 0, 0, 0);
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
                properties["empId"] = "";
                properties["emptype"] = "";
                properties["empName"] = "";
                properties["isLdcAlert"] = false;
                properties["currentLDCs"] = "";
                properties["tacs"] = new JObject();
                properties["sels"] = new JObject();
                properties["Raw_Data"] = "";
                return properties;
            }
            set { return; }
        }

        public JObject GeoJSON_Locators
        {
            get
            {
                return new JObject(
                    new JProperty("type", "Feature"),
              new JProperty("geometry",
              new JObject(new JProperty("type", ""),
               new JProperty("coordinates", new JArray()))),
              new JProperty("properties", Locater));
            }
            set { return; }
        }
        public JObject locater = new JObject();
        public JObject Locater
        {
            get
            {

                locater["state"] = "";
                locater["Tag_Type"] = "";
                locater["Tag_Update"] = false;
                return locater;
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
        [JsonProperty("ActiveConnection")]
        public bool ActiveConnection { get; set; } = false;
        [JsonProperty("AdminEmailRecepient")]
        public string AdminEmailRecepient { get; set; }
        [JsonProperty("ApiConnected")]
        public bool ApiConnected { get; set; } = false;
        [JsonProperty("ConnectionName")]
        public string ConnectionName { get; set; } = "";
        [JsonProperty("CreatedByUsername")]
        public string CreatedByUsername { get; set; } = "";
        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [JsonProperty("DataRetrieve")]
        public int DataRetrieve { get; set; } = 60000;
        [JsonProperty("DeactivatedByUsername")]
        public string DeactivatedByUsername { get; set; } = "";
        [JsonProperty("DeactivatedDate")]
        public DateTime DeactivatedDate { get; set; }
        [JsonProperty("Hostname")]
        public string Hostname { get; set; } = "";
        [JsonProperty("HoursBack")]
        public int HoursBack { get; set; } = 0;
        [JsonProperty("HoursForward")]
        public int HoursForward { get; set; } = 0;
        [JsonProperty("Https")]
        public bool Https { get; set; } = false;
        [JsonProperty("Id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("IpAddress")]
        public string IpAddress { get; set; } = "";
        [JsonProperty("LasttimeApiConnected")]
        public DateTime LasttimeApiConnected { get; set; }
        [JsonProperty("LastupDate")]
        public DateTime LastupDate { get; set; }
        [JsonProperty("LastupdateByUsername")]  
        public string LastupdateByUsername { get; set; } = "";
        [JsonProperty("MessageType")]
        public string MessageType { get; set; } = "";
        [JsonProperty("NassCode")]
        public string NassCode { get; set; } = "";
        [JsonProperty("OutgoingApikey")]
        public string OutgoingApikey { get; set; } = "";
        [JsonProperty("Port")]
        public Int32 Port { get; set; } = 0;
        [JsonProperty("UdpConnection")]
        public bool UdpConnection { get; set; } = false;
        [JsonIgnore]
        public bool UpdateStatus { get; set; } = false;
        [JsonProperty("Url")]
        public string Url { get; set; } = "";

    }
    public class Cameras
    {
        [JsonProperty("LOCALE_KEY")]
        public string LocaleKey { get; set; }

        [JsonProperty("MODEL_NUM")]
        public string ModelNum { get; set; }

        [JsonProperty("FACILITY_PHYS_ADDR_TXT")]
        public string FacilityPhysAddrTxt { get; set; }

        [JsonProperty("GEO_PROC_REGION_NM")]
        public string GeoProcRegionNm { get; set; }

        [JsonProperty("FACILITY_SUBTYPE_DESC")]
        public string FacilitySubtypeDesc { get; set; }

        [JsonProperty("GEO_PROC_DIVISION_NM")]
        public string GeoProcDivisionNm { get; set; }

        [JsonProperty("AUTH_KEY")]
        public string AuthKey { get; set; }

        [JsonProperty("FACILITY_LATITUDE_NUM")]
        public double FacilitiyLatitudeNum { get; set; }

        [JsonProperty("FACILITY_LONGITUDE_NUM")]
        public double FacilitiyLongitudeNum { get; set; }

        [JsonProperty("CAMERA_NAME")]
        public string CameraName { get; set; }

        [JsonProperty("DESCRIPTION")]
        public string Description { get; set; }

        [JsonProperty("REACHABLE")]
        public string Reachable { get; set; }

        [JsonProperty("FACILITY_DISPLAY_NME")]
        public string FacilityDisplayName { get; set; }

    }
    public class Event
    {
        [JsonProperty("eventName")]
        public string EventName { get; set; }

        [JsonProperty("eventDtm")]
        public EventDtm EventDtm { get; set; }

        [JsonProperty("siteId")]
        public string SiteId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }

    public class ScanCount
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; }

        [JsonProperty("containerTypeCode")]
        public string ContainerTypeCode { get; set; }

        [JsonProperty("loadCount")]
        public int LoadCount { get; set; }

        [JsonProperty("unloadCount")]
        public int UnloadCount { get; set; }

        [JsonProperty("emptyLoadCount")]
        public int EmptyLoadCount { get; set; }

        [JsonProperty("emptyUnloadCount")]
        public int EmptyUnloadCount { get; set; }

        [JsonProperty("unscanLdCount")]
        public int UnscanLdCount { get; set; }

        [JsonProperty("unscanUnldCount")]
        public int UnscanUnldCount { get; set; }

        [JsonProperty("destBundleCount")]
        public int DestBundleCount { get; set; }

        [JsonProperty("destPieceCount")]
        public int DestPieceCount { get; set; }

        [JsonProperty("destPackageCount")]
        public int DestPackageCount { get; set; }
    }

    public class Form5500
    {
        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId { get; set; }

        [JsonProperty("formTypeCode")]
        public string FormTypeCode { get; set; }

        [JsonProperty("tripDirection")]
        public string TripDirection { get; set; }

        [JsonProperty("excptnCode")]
        public string ExcptnCode { get; set; }

        [JsonProperty("preparerName")]
        public string PreparerName { get; set; }

        [JsonProperty("updtUserId")]
        public string UpdtUserId { get; set; }

        [JsonProperty("exceptionList")]
        public List<string> ExceptionList { get; set; }

        [JsonProperty("createdDtm")]
        public EventDtm CreatedDtm { get; set; }
    }

    public class Form5466
    {
        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId { get; set; }

        [JsonProperty("formTypeCode")]
        public string FormTypeCode { get; set; }

        [JsonProperty("tripDirection")]
        public string TripDirection { get; set; }

        [JsonProperty("excptnCode")]
        public string ExcptnCode { get; set; }

        [JsonProperty("preparerName")]
        public string PreparerName { get; set; }

        [JsonProperty("updtUserId")]
        public string UpdtUserId { get; set; }

        [JsonProperty("exceptionList")]
        public List<string> ExceptionList { get; set; }

        [JsonProperty("createdDtm")]
        public EventDtm CreatedDtm { get; set; }
    }

    public class Form5500L
    {
        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId { get; set; }

        [JsonProperty("formTypeCode")]
        public string FormTypeCode { get; set; }

        [JsonProperty("tripDirection")]
        public string TripDirection { get; set; }

        [JsonProperty("excptnCode")]
        public string ExcptnCode { get; set; }

        [JsonProperty("preparerName")]
        public string PreparerName { get; set; }

        [JsonProperty("updtUserId")]
        public string UpdtUserId { get; set; }

        [JsonProperty("exceptionList")]
        public List<string> ExceptionList { get; set; }

        [JsonProperty("createdDtm")]
        public EventDtm CreatedDtm { get; set; }
    }
    //Arrive/Depart is used to view and process the network trips scheduled at the current facility
    public class Trips
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; }

        [JsonProperty("routeTripId")]
        public int RouteTripId { get; set; }

        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("trip")]
        public string Trip { get; set; }

        [JsonProperty("tripDirectionInd")]
        public string TripDirectionInd { get; set; }

        [JsonProperty("destSite")]
        public string DestSite { get; set; }
        [JsonProperty("legNumber")]
        public int LegNumber { get; set; }

        [JsonProperty("serviceTypeCode")]
        public string ServiceTypeCode { get; set; }

        [JsonProperty("legSiteId")]
        public string LegSiteId { get; set; }

        [JsonProperty("legSiteName")]
        public string LegSiteName { get; set; }

        [JsonProperty("tripSiteId")]
        public string TripSiteId { get; set; }

        [JsonProperty("tripSiteName")]
        public string TripSiteName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("legStatus")]
        public string LegStatus { get; set; }

        [JsonProperty("scheduledDtm")]
        public EventDtm _scheduledDtm { get; set; }
        public DateTime ScheduledDtm
        {
            get
            {

                if (_scheduledDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_scheduledDtm)));
                }
                return new DateTime(1, 1, 1);

            }
            set { return; }
        }

        [JsonProperty("timeToDepart")]
        public double TimeToDepart { get; set; }

        [JsonProperty("timeToArrive")]
        public double TimeToArrive { get; set; }

        [JsonProperty("isTripLate")]
        public bool isTripLate { get; set; }

        [JsonProperty("actArrivalDtm")]
        public DateTime ActArrivalDtm { get; set; }

        [JsonProperty("trip_Update")]
        public bool Trip_Update { get; set; }

        [JsonProperty("actDepartureDtm")]
        public DateTime ActDepartureDtm { get; set; }

        [JsonProperty("legScheduledDtm")]
        public EventDtm _legScheduledDtm { get; set; }
        public DateTime LegScheduledDtm
        {
            get
            {

                if (_legScheduledDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_legScheduledDtm)));
                }
                return new DateTime(1, 1, 1);

            }
            set { return; }
        }

        [JsonProperty("isEmpty")]
        public bool IsEmpty { get; set; }

        [JsonProperty("nonStandardTripInd")]
        public string NonStandardTripInd { get; set; }

        [JsonProperty("createdUserId")]
        public string CreatedUserId { get; set; }

        [JsonProperty("transpFreqCode")]
        public string TranspFreqCode { get; set; }

        [JsonProperty("cancelUserId")]
        public string CancelUserId { get; set; }

        [JsonProperty("cancelReason")]
        public string CancelReason { get; set; }

        [JsonProperty("cancelDtm")]
        public EventDtm _cancelDtm { get; set; }
        public DateTime CancelDtm
        {
            get
            {

                if (_cancelDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_cancelDtm)));
                }
                return new DateTime(1, 1, 1);

            }
            set { return; }
        }

        [JsonProperty("containers")]
        public IEnumerable<Container> Containers { get; set; }

        [JsonProperty("unloadedcontainers")]
        public int UnloadedContainers { get; set; }

        [JsonProperty("handlingUnits")]
        public List<object> HandlingUnits { get; set; }

        [JsonProperty("events")]
        public List<Event> Events { get; set; }

        [JsonProperty("apptIrregs")]
        public List<object> ApptIrregs { get; set; }

        [JsonProperty("imageCount")]
        public int ImageCount { get; set; }

        [JsonProperty("scanCounts")]
        public List<ScanCount> ScanCounts { get; set; }

        [JsonProperty("form5397Ind")]
        public string Form5397Ind { get; set; }

        [JsonProperty("originAreaName")]
        public string OriginAreaName { get; set; }

        [JsonProperty("originDistrictName")]
        public string OriginDistrictName { get; set; }

        [JsonProperty("originSiteName")]
        public string OriginSiteName { get; set; }

        [JsonProperty("originSiteId")]
        public string OriginSiteId { get; set; }

        [JsonProperty("destAreaName")]
        public string DestAreaName { get; set; }

        [JsonProperty("destDistrictName")]
        public string DestDistrictName { get; set; }

        [JsonProperty("destSiteName")]
        public string DestSiteName { get; set; }

        [JsonProperty("destSiteId")]
        public string DestSiteId { get; set; }

        [JsonProperty("tourNumber")]
        public int TourNumber { get; set; }

        [JsonProperty("ymsSiteInd")]
        public string YmsSiteInd { get; set; }

        [JsonProperty("hasManualEdit")]
        public string HasManualEdit { get; set; }

        [JsonProperty("yardMgmtInd")]
        public string YardMgmtInd { get; set; }

        [JsonProperty("supplier")]
        public string Supplier { get; set; }

        [JsonProperty("updatedScheduleInd")]
        public string UpdatedScheduleInd { get; set; }

        [JsonProperty("notUnloadedInd")]
        public string NotUnloadedInd { get; set; }

        [JsonProperty("operDate")]
        public EventDtm _operDate { get; set; }
        public DateTime OperDate
        {
            get
            {

                if (_operDate != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_operDate)));
                }

                return new DateTime(1, 1, 1);

            }
            set { return; }
        }

        [JsonProperty("initialOriginSiteId")]
        public string InitialOriginSiteId { get; set; }

        [JsonProperty("initialOriginSiteName")]
        public string InitialOriginSiteName { get; set; }

        [JsonProperty("finalDestSiteId")]
        public string FinalDestSiteId { get; set; }

        [JsonProperty("finalDestSiteName")]
        public string FinalDestSiteName { get; set; }

        [JsonProperty("isAODU")]
        public string IsAODU { get; set; }

        [JsonProperty("redirected")]
        public string Redirected { get; set; }

        [JsonProperty("originRegionName")]
        public string OriginRegionName { get; set; }

        [JsonProperty("originDivisionName")]
        public string OriginDivisionName { get; set; }

        [JsonProperty("destRegionName")]
        public string DestRegionName { get; set; }

        [JsonProperty("destDivisionName")]
        public string DestDivisionName { get; set; }

        [JsonProperty("transpFreqNo")]
        public int? TranspFreqNo { get; set; }

        [JsonProperty("extraTripReasonCode")]
        public int? ExtraTripReasonCode { get; set; }

        [JsonProperty("form5500")]
        public Form5500 Form5500 { get; set; }

        [JsonProperty("legGpsId")]
        public string LegGpsId { get; set; }

        [JsonProperty("legGpsIdSource")]
        public string LegGpsIdSource { get; set; }

        [JsonProperty("actualDtm")]
        public EventDtm _actualDtm { get; set; }
        public DateTime ActualDtm
        {
            get
            {
                if (_actualDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_actualDtm)));
                }

                return new DateTime(1, 1, 1);

            }
            set { return; }
        }

        [JsonProperty("legActualDtm")]
        public EventDtm _legActualDtm { get; set; }
        public DateTime LegActualDtm
        {
            get
            {

                if (_legActualDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_legActualDtm)));
                }

                return new DateTime(1, 1, 1);

            }
            set { return; }
        }
        [JsonProperty("driverFirstName")]
        public string DriverFirstName { get; set; }

        [JsonProperty("driverLastName")]
        public string DriverLastName { get; set; }

        [JsonProperty("driverPhoneNumber")]
        public string DriverPhoneNumber { get; set; }

        [JsonProperty("driverBarcode")]
        public string DriverBarcode { get; set; }

        [JsonProperty("driverId")]
        public int? DriverId { get; set; }

        [JsonProperty("vanNumber")]
        public string VanNumber { get; set; }

        [JsonProperty("trailerBarcode")]
        public string TrailerBarcode { get; set; }

        [JsonProperty("trailerLengthCode")]
        public string TrailerLengthCode { get; set; }

        [JsonProperty("mspBarcode")]
        public string MspBarcode { get; set; }

        [JsonProperty("doorDtm")]
        public EventDtm _doorDtm { get; set; }
        public DateTime DoorDtm
        {
            get
            {

                if (_doorDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_doorDtm)));
                }

                return new DateTime(1, 1, 1);

            }
            set { return; }
        }

        [JsonProperty("legDoorDtm")]
        public EventDtm _legDoorDtm { get; set; }
        public DateTime LegDoorDtm
        {
            get
            {

                if (_legDoorDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_legDoorDtm)));
                }

                return new DateTime(1, 1, 1);

            }
            set { return; }
        }

        [JsonProperty("actualDtmSource")]
        public string ActualDtmSource { get; set; }

        [JsonProperty("gpsId")]
        public string GpsId { get; set; }

        [JsonProperty("gpsIdSource")]
        public string GpsIdSource { get; set; }

        [JsonProperty("gpsSiteDtm")]
        public EventDtm _gpsSiteDtm { get; set; }
        public DateTime GpsSiteDtm
        {
            get
            {

                if (_gpsSiteDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_gpsSiteDtm)));
                }

                return new DateTime(1, 1, 1);
            }
            set { return; }
        }

        [JsonProperty("loadPercent")]
        public int? LoadPercent { get; set; }

        [JsonProperty("loadUnldStartDtm")]
        public EventDtm _loadUnldStartDtm { get; set; }
        public DateTime LoadUnldStartDtm
        {
            get
            {

                if (_loadUnldStartDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_loadUnldStartDtm)));
                }
               
                    return new DateTime(1, 1, 1);
              
            }
            set { return; }
        }

        [JsonProperty("loadUnldEndDtm")]
        public EventDtm _loadUnldEndDtm { get; set; }
        public DateTime LoadUnldEndDtm
        {
            get
            {

                if (_loadUnldEndDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_loadUnldEndDtm)));
                }

                return new DateTime(1, 1, 1);
            }
            set { return; }
        }


        [JsonProperty("doorId")]
        public string DoorId { get; set; }

        [JsonProperty("doorNumber")]
        public string DoorNumber { get; set; }

        [JsonProperty("originSeal")]
        public string OriginSeal { get; set; }

        [JsonProperty("destSeal")]
        public string DestSeal { get; set; }

        [JsonProperty("destComments")]
        public string DestComments { get; set; }

        [JsonProperty("delayCode")]
        public int? DelayCode { get; set; }

        [JsonProperty("delayReason")]
        public string DelayReason { get; set; }

        [JsonProperty("form5466")]
        public Form5466 Form5466 { get; set; }

        [JsonProperty("legGateDtm")]
        public EventDtm _legGateDtm { get; set; }
        public DateTime LegGateDtm
        {
            get
            {

                if (_legGateDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_legGateDtm)));
                }

                return new DateTime(1, 1, 1); ;
            }
        
            set { return; }
        }

        [JsonProperty("form5500L")]
        public Form5500L Form5500L { get; set; }

        [JsonProperty("originComments")]
        public string OriginComments { get; set; }

        [JsonProperty("bedLoadPercent")]
        public int? BedLoadPercent { get; set; }

        [JsonProperty("legs")]
        public List<Leg> Legs;

        [JsonProperty("rawData")]
        public string RawData;

    }
    public class Leg
    {
        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId;

        [JsonProperty("routeTripId")]
        public int RouteTripId;

        [JsonProperty("legNumber")]
        public int LegNumber;

        [JsonProperty("legDestSiteID")]
        public string LegDestSiteID;

        [JsonProperty("legOriginSiteID")]
        public string LegOriginSiteID;

        [JsonProperty("scheduledArrDTM")]
        public EventDtm _scheduledArrDTM { get; set; }
        public DateTime ScheduledArrDTM
        {
            get
            {

                if (_scheduledArrDTM != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_scheduledArrDTM)));
                }

                return new DateTime(1, 1, 1);

            }
            set { return; }
        }

        [JsonProperty("scheduledDepDTM")]
        public EventDtm _scheduledDepDTM { get; set; }
        public DateTime ScheduledDepDTM
        {
            get
            {
                if (_scheduledDepDTM != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_scheduledDepDTM)));
                }

                return new DateTime(1, 1, 1);

            }
            set { return; }
        }
        [JsonProperty("actDepartureDtm")]
        private EventDtm _actDepartureDtm { get; set; }
        public DateTime ActDepartureDtm
        {
            get
            {
                if (_actDepartureDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_actDepartureDtm)));
                }

                return new DateTime(1, 1, 1);

            }
            set { return; }
        }

        [JsonProperty("actArrivalDtm")]
        private EventDtm _actArrivalDtm { get; set; }
        public DateTime ActArrivalDtm
        {
            get
            {
                if (_actArrivalDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_actArrivalDtm)));
                }
                return new DateTime(1, 1, 1);
            }
            set { return; }
        }

        [JsonProperty("createdDtm")]
        private EventDtm _createdDtm { get; set; }
        public DateTime CreatedDtm
        {
            get
            {
                if (_createdDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_createdDtm)));
                }
               return new DateTime(1, 1, 1);
                
            }
            set { return; }
        }

        [JsonProperty("lastUpdtDtm")]
        private EventDtm _lastUpdtDtm { get; set; }
        public DateTime LastUpdtDtm
        {
            get
            {
                if (_lastUpdtDtm != null)
                {
                    return AppParameters.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(_lastUpdtDtm)));
                }
                else
                {
                    return new DateTime(1, 1, 1);
                }
        
            }
            set { return; }
        }

        [JsonProperty("legDestSiteName")]
        public string LegDestSiteName;

        [JsonProperty("legOriginSiteName")]
        public string LegOriginSiteName;

        [JsonProperty("outboundProcessedInd")]
        public string OutboundProcessedInd;

        [JsonProperty("inboundProcessedInd")]
        public string InboundProcessedInd;

        [JsonProperty("legDestMSPBarcode")]
        public string LegDestMSPBarcode;

        [JsonProperty("legOriginMSPBarcode")]
        public string LegOriginMSPBarcode;
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
}