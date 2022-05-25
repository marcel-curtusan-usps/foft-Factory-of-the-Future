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
        public string UserId { get; set; } = "";
        public string FacilityName { get; set; }
        public string FacilityTimeZone { get; set; }
        public string Environment { get; set; }
        public string PageType { get; set; }
        public string Role { get; set; } = "Operator".ToUpper();
        public string ZipCode { get; set; }
        public string BrowserType { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string FirstName { get; set; } = "";
        public string SurName { get; set; } = "";
        public string IpAddress { get; set; }
        public string ServerIpAddress { get; set; }
        public string MiddleName { get; set; } = "";
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
        public string EmailAddress { get; set; } = "";
        public string Phone { get; set; } = "";
        public string EIN { get; set; } = "";
        public string TAGID { get; set; } = "";
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
        public string Base64 { get; set; } = "";

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

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("metersPerPixelY")]
        public double MetersPerPixelY { get; set; }

        [JsonProperty("metersPerPixelX")]
        public double MetersPerPixelX { get; set; }
        [JsonProperty("updateStatus")]
        public bool UpdateStatus { get; set; } = false;

        [JsonProperty("rawData")]
        public string RawData { get; set; } = "";

        [JsonProperty("facilityName")]
        public string FacilityName { get; set; } = "Site Not Configured";

        [JsonProperty("softwareVersion")]
        public string SoftwareVersion { get; set; } = AppParameters.VersionInfo;

        [JsonProperty("applicationFullName")]
        public string ApplicationFullName { get; set; } = "";

        [JsonProperty("applicationAbbr")]
        public string ApplicationAbbr { get; set; } = "";
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
    public class ZoneGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Polygon";

        [JsonProperty("coordinates")]
        public List<List<List<double>>> Coordinates { get; set; }
    }
    public class MarkerGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Point";

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; } 
    }
  
    public class Properties
    {
        [JsonProperty("id")]
        public string Id { get; set; }

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

        [JsonProperty("Zone_Type")]
        public string ZoneType { get; set; } = "";

        [JsonProperty("MPEWatchData")]
        public RunPerf MPEWatchData { get; set; } = new RunPerf();
   
        [JsonProperty("MPE_Type")]
        public string MPEType { get; set; } = "";

        [JsonProperty("bins")]
        public string Bins { get; set; } = "";

        [JsonProperty("MPE_Bins")]
        public List<string> MPEBins { get; set; } = new List<string>();

        [JsonProperty("MPE_Number")]
        public int MPENumber { get; set; }

        [JsonProperty("DPSData")]
        public DPS DPSData { get; set; } = new DPS();

        [JsonProperty("staffingData")]
        public StaffingSortplan StaffingData { get; set; } = new StaffingSortplan();

        [JsonProperty("CurrentStaff")]
        public int CurrentStaff { get; set; }
        
        [JsonProperty("doorNumber")]
        public string DoorNumber { get; set; }

        [JsonProperty("dockdoorData")]
        public RouteTrips DockDoorData { get; set; } = new RouteTrips();

        [JsonProperty("rawData")]
        public string RawData { get; set; } = "";

        [JsonProperty("MissionList")]
        public List<Mission> MissionList { get; set; } = new List<Mission>();
        [JsonProperty("source")]
        public string Source { get; set; } = "";
    }

    public class GeoZone
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Feature";

        [JsonProperty("geometry")]
        public ZoneGeometry Geometry { get; set; } = new ZoneGeometry(); 

        [JsonProperty("properties")]
        public Properties Properties { get; set; } = new Properties();
    }
    public class GeoMarker
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Feature"; 

        [JsonProperty("geometry")]
        public MarkerGeometry Geometry { get; set; } = new MarkerGeometry();

        [JsonProperty("properties")]
        public Marker Properties { get; set; } = new Marker();
    }
    public class Marker
    {
        [JsonProperty("id")]
        public string Id { get; set; }  = "";
        [JsonProperty("rFId")]
        public string RFid { get; set; } = "";

        [JsonProperty("visible")]
        public bool Visible { get; set; } = false;

        [JsonProperty("zones")]
        public List<Zone> Zones { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; } = "";

        [JsonProperty("tagVisible")]
        public bool TagVisible { get; set; } = false;

        [JsonProperty("tagVisibleMils")]
        public int TagVisibleMils { get; set; }

        [JsonProperty("isWearingTag")]
        public bool IsWearingTag { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("craftName")]
        public string CraftName { get; set; } = "";

        [JsonProperty("positionTS")]
        public DateTime PositionTS { get; set; }

        [JsonProperty("Tag_TS")]
        public DateTime TagTS { get; set; }

        [JsonProperty("Tag_Type")]
        public string TagType { get; set; } = "";

        [JsonProperty("Tag_Update")]
        public bool TagUpdate { get; set; }

        [JsonProperty("empId")]
        public string EmpId { get; set; } = "";

        [JsonProperty("emptype")]
        public string Emptype { get; set; } = "";
        [JsonProperty("badgeId")]
        public string BadgeId { get; set; } = "";

        [JsonProperty("empName")]
        public string EmpName { get; set; } = "";

        [JsonProperty("isLdcAlert")]
        public bool IsLdcAlert { get; set; }

        [JsonProperty("currentLDCs")]
        public string CurrentLDCs { get; set; } = "";

        [JsonProperty("tacs")]
        public string Tacs { get; set; } = "";

        [JsonProperty("sels")]
        public string Sels { get; set; } = "";

        [JsonProperty("ovementStatus")]
        public string MovementStatus { get; set; } = "noData";

        [JsonProperty("Raw_Data")]
        public string RawData { get; set; } = "";
        [JsonProperty("Camera_Data")]
        public string CameraData { get; set; } = "";
        [JsonProperty("Vehicle_Status_Data")]
        public VehicleStatus Vehicle_Status_Data { get; set; }
        [JsonProperty("Mission")]
        public Mission Misison { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; } = "";
        [JsonProperty("notificationId")]
        public string NotificationId { get; set; } = "";
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
    public class TrackTagGroups
    {
        [JsonProperty("tagGroups")]
        public object TagGroups { get; set; }

        [JsonProperty("allTags")]
        public bool AllTags { get; set; }
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
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("eventDtm")]
        public EventDtm EventDtm { get; set; }
        public DateTime EventDtmfmt
        {
            get
            {
                return new DateTime(EventDtm.Year, (EventDtm.Month + 1), EventDtm.DayOfMonth, EventDtm.HourOfDay, EventDtm.Minute, EventDtm.Second);
            }
            set { return; }
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

        public int sortind { get; set; }
    }
    public class Container
    {
        public DateTime EventDtm { get; set; }

        [JsonProperty("placardBarcode")]
        public string PlacardBarcode { get; set; }

        [JsonProperty("trailer")]
        public string Trailer { get; set; }

        [JsonProperty("containerHistory")]
        public List<ContainerHistory> ContainerHistory { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("dest")]
        public string Dest { get; set; }

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
        public string MailClassDisplay { get; set; }
        public string binDisplay { get; set; }
        public bool hasPrintScans { get; set; }
        public bool hasAssignScans { get; set; }
        public bool hasCloseScans { get; set; }
        public bool hasLoadScans { get; set; }
        public bool hasUnloadScans { get; set; }
        public bool containerTerminate { get; set; }
        public bool containerAtDest { get; set; }
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
    public class StaffingSortplan
    {
        [JsonProperty("mach_type")]
        public string MachType { get; set; }

        [JsonProperty("machine_no")]
        public int MachineNo { get; set; }

        [JsonProperty("sortplan")]
        public string Sortplan { get; set; }

        [JsonProperty("clerk")]
        public double Clerk { get; set; }

        [JsonProperty("mh")]
        public double Mh { get; set; }
    }
    public class HourlyData
    {
        [JsonProperty("hour")]
        public string Hour { get; set; } = "";

        [JsonProperty("count")]
        public int Count { get; set; }
    }
    public class RunPerf
    {
        [JsonProperty("mpe_type")]
        public string MpeType { get; set; } = "";

        [JsonProperty("mpe_number")]
        public string MpeNumber { get; set; } = "";

        [JsonProperty("bins")]
        public string Bins { get; set; } = "";

        [JsonProperty("cur_sortplan")]
        public string CurSortplan { get; set; } = "";

        [JsonProperty("cur_thruput_ophr")]
        public string CurThruputOphr { get; set; } = "";

        [JsonProperty("tot_sortplan_vol")]
        public string TotSortplanVol { get; set; } = "";

        [JsonProperty("rpg_est_vol")]
        public string RpgEstVol { get; set; } = "";

        [JsonProperty("act_vol_plan_vol_nbr")]
        public string ActVolPlanVolNbr { get; set; } = "";

        [JsonProperty("current_run_start")]
        public string CurrentRunStart { get; set; } = "";

        [JsonProperty("current_run_end")]
        public string CurrentRunEnd { get; set; } = "";

        [JsonProperty("cur_operation_id")]
        public string CurOperationId { get; set; } = "";

        [JsonProperty("bin_full_status")]
        public string BinFullStatus { get; set; } = "";

        [JsonProperty("bin_full_bins")]
        public string BinFullBins { get; set; } = "";

        [JsonProperty("throughput_status")]
        public string ThroughputStatus { get; set; } = "";

        [JsonProperty("unplan_maint_sp_status")]
        public string UnplanMaintSpStatus { get; set; } = "";

        [JsonProperty("op_started_late_status")]
        public string OpStartedLateStatus { get; set; } = "";

        [JsonProperty("op_running_late_status")]
        public string OpRunningLateStatus { get; set; } = "";

        [JsonProperty("sortplan_wrong_status")]
        public string SortplanWrongStatus { get; set; } = "";

        [JsonProperty("unplan_maint_sp_timer")]
        public string UnplanMaintSpTimer { get; set; } = "";

        [JsonProperty("op_started_late_timer")]
        public string OpStartedLateTimer { get; set; } = "";

        [JsonProperty("rpg_start_dtm")]
        public string RPGStartDtm { get; set; } = "";

        [JsonProperty("rpg_end_dtm")]
        public string RPGEndDtm { get; set; } = "";

        [JsonProperty("expected_throughput")]
        public string ExpectedThroughput { get; set; } = "";

        [JsonProperty("op_running_late_timer")]
        public string OpRunningLateTimer { get; set; } = "";

        [JsonProperty("sortplan_wrong_timer")]
        public string SortplanWrongTimer { get; set; } = "";

        [JsonProperty("rpg_est_comp_time")]
        public string RpgEstCompTime { get; set; } = "";

        [JsonProperty("hourly_data")]
        public List<HourlyData> HourlyData { get; set; } = new List<HourlyData>();
    }
    public class DPS
    {
        [JsonProperty("run_start_modsday")]
        public string RunStartModsday { get; set; } = "";

        [JsonProperty("sortplan_name_perf")]
        public string SortplanNamePerf { get; set; } = "";

        [JsonProperty("current_operation_id")]
        public string CurrentOperationId { get; set; } = "";

        [JsonProperty("pieces_fed_1st_cnt")]
        public string PiecesFed1stCnt { get; set; } = "";

        [JsonProperty("pieces_rejected_1st_cnt")]
        public string PiecesRejected1stCnt { get; set; } = "";

        [JsonProperty("pieces_to_2nd_pass")]
        public string PiecesTo2ndPass { get; set; } = "";

        [JsonProperty("op_time_1st")]
        public string OpTime1st { get; set; } = "";

        [JsonProperty("thruput_1st_pass")]
        public string Thruput1stPass { get; set; } = "";

        [JsonProperty("pieces_fed_2nd_cnt")]
        public string PiecesFed2ndCnt { get; set; } = "";

        [JsonProperty("pieces_rejected_2nd_cnt")]
        public string PiecesRejected2ndCnt { get; set; } = "";

        [JsonProperty("op_time_2nd")]
        public string OpTime2nd { get; set; } = "";

        [JsonProperty("thruput_2nd_pass")]
        public string Thruput2ndPass { get; set; } = "";

        [JsonProperty("pieces_remaining")]
        public string PiecesRemaining { get; set; } = "";

        [JsonProperty("thruput_optimal_cfg")]
        public string ThruputOptimalCfg { get; set; } = "";

        [JsonProperty("time_to_comp_optimal")]
        public string TimeToCompOptimal { get; set; } = "";

        [JsonProperty("thruput_actual")]
        public string ThruputActual { get; set; } = "";

        [JsonProperty("time_to_comp_actual")]
        public string TimeToCompActual { get; set; } = "";

        [JsonProperty("rpg_2nd_pass_end")]
        public string Rpg2ndPassEnd { get; set; } = "";

        [JsonProperty("time_to_2nd_pass_optimal")]
        public string TimeTo2ndPassOptimal { get; set; } = "";

        [JsonProperty("rec_2nd_pass_start_optimal")]
        public string Rec2ndPassStartOptimal { get; set; } = "";

        [JsonProperty("time_to_2nd_pass_actual")]
        public string TimeTo2ndPassActual { get; set; } = "";

        [JsonProperty("rec_2nd_pass_start_actual")]
        public string Rec2ndPassStartActual { get; set; } = "";

        [JsonProperty("time_to_comp_optimal_DateTime")]
        public string TimeToCompOptimalDateTime { get; set; } = "";

        [JsonProperty("time_to_comp_actual_DateTime")]
        public string TimeToCompActualDateTime { get; set; } = "";
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
    public class Leg
    {
        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId { get; set; }

        [JsonProperty("routeTripId")]
        public int RouteTripId { get; set; }

        [JsonProperty("legNumber")]
        public int LegNumber { get; set; }

        [JsonProperty("legDestSiteID")]
        public string LegDestSiteID { get; set; }

        [JsonProperty("legOriginSiteID")]
        public string LegOriginSiteID { get; set; }

        [JsonProperty("scheduledArrDTM")]
        public EventDtm ScheduledArrDTM { get; set; }

        [JsonProperty("scheduledDepDTM")]
        public EventDtm ScheduledDepDTM { get; set; }

        [JsonProperty("actDepartureDtm")]
        public EventDtm ActDepartureDtm { get; set; }

        [JsonProperty("createdDtm")]
        public EventDtm CreatedDtm { get; set; }

        [JsonProperty("lastUpdtDtm")]
        public EventDtm LastUpdtDtm { get; set; }

        [JsonProperty("legDestSiteName")]
        public string LegDestSiteName { get; set; }

        [JsonProperty("legOriginSiteName")]
        public string LegOriginSiteName { get; set; }

        [JsonProperty("outboundProcessedInd")]
        public string OutboundProcessedInd { get; set; }

        [JsonProperty("inboundProcessedInd")]
        public string InboundProcessedInd { get; set; }

        [JsonProperty("legOriginMSPBarcode")]
        public string LegOriginMSPBarcode { get; set; }

        [JsonProperty("legDestMSPBarcode")]
        public string LegDestMSPBarcode { get; set; }
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
    //// trips
    public class RouteTrips
    {
        [JsonProperty("routeTripId")]
        public int RouteTripId { get; set; }

        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("trip")]
        public string Trip { get; set; }

        [JsonProperty("tripDirectionInd")]
        public string TripDirectionInd { get; set; } = "";

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

        [JsonProperty("scheduledDtm")]
        public EventDtm ScheduledDtm { get; set; }

        [JsonProperty("legScheduledDtm")]
        public EventDtm LegScheduledDtm { get; set; }

        [JsonProperty("containerScans")]
        public IEnumerable<Container> Containers { get; set; } = null;

        [JsonProperty("Notloadedcontainers")]
        public int NotloadedContainers { get; set; }

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

        [JsonProperty("supplier")]
        public string Supplier { get; set; }

        [JsonProperty("notUnloadedInd")]
        public string NotUnloadedInd { get; set; }

        [JsonProperty("operDate")]
        public EventDtm OperDate { get; set; }

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

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("tripMin")]
        public int? TripMin { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = "";

        [JsonProperty("state")]
        public string State { get; set; } = "";

        [JsonProperty("notificationId")]
        public string NotificationId { get; set; } = "";

        [JsonProperty("legStatus")]
        public string LegStatus { get; set; } = "";

        [JsonProperty("trailerBarcode")]
        public string TrailerBarcode { get; set; } = "";

        [JsonProperty("actualDtm")]
        public EventDtm ActualDtm { get; set; }

        [JsonProperty("legActualDtm")]
        public EventDtm LegActualDtm { get; set; }

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

        [JsonProperty("doorId")]
        public string DoorId { get; set; } = "";

        [JsonProperty("doorNumber")]
        public string DoorNumber { get; set; } = "";

        [JsonProperty("vanNumber")]
        public string VanNumber { get; set; } = "";

        [JsonProperty("trailerLengthCode")]
        public string TrailerLengthCode { get; set; } = "";

        [JsonProperty("loadPercent")]
        public int? LoadPercent { get; set; }

        [JsonProperty("loadUnldStartDtm")]
        public EventDtm LoadUnldStartDtm { get; set; }

        [JsonProperty("loadUnldEndDtm")]
        public EventDtm LoadUnldEndDtm { get; set; }

        [JsonProperty("doorDtm")]
        public EventDtm DoorDtm { get; set; }

        [JsonProperty("legDoorDtm")]
        public EventDtm LegDoorDtm { get; set; }

        [JsonProperty("mspBarcode")]
        public string MspBarcode { get; set; } = "";

        [JsonProperty("destSite")]
        public string DestSites { get; set; } = "";

        [JsonProperty("rawData")]
        public string RawData { get; set; } = "";
        [JsonProperty("Trip_Update")]
        public bool TripUpdate { get; set; } = false;
        public List<Leg> Legs { get; set; }
    }

    public class Tacs
    {
        [JsonProperty("ldc")]
        public string Ldc { get; set; }

        [JsonProperty("finance")]
        public string Finance { get; set; }

        [JsonProperty("fnAlert")]
        public string FnAlert { get; set; }

        [JsonProperty("totalTime")]
        public int TotalTime { get; set; }

        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        [JsonProperty("payLocation")]
        public string PayLocation { get; set; }

        [JsonProperty("isOvertimeAuth")]
        public bool IsOvertimeAuth { get; set; }

        [JsonProperty("overtimeHours")]
        public int OvertimeHours { get; set; }

        [JsonProperty("isOvertime")]
        public bool IsOvertime { get; set; }

        [JsonProperty("startTs")]
        public object StartTs { get; set; }

        [JsonProperty("startTxt")]
        public string StartTxt { get; set; }

        [JsonProperty("ts")]
        public object Ts { get; set; }

        [JsonProperty("openRingCode")]
        public string OpenRingCode { get; set; }
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
        public string ProcessedSince { get; set; }

        [JsonProperty("missedSelsCount")]
        public int MissedSelsCount { get; set; }

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
    public class RPGPlan
    {
        public DateTime mods_date { get; set; }
        public string machine_num { get; set; } = "";
        public string sort_program_name { get; set; } = "";
        public DateTime rpg_start_dtm { get; set; }
        public DateTime rpg_end_dtm { get; set; }
        public string rpg_pieces_fed { get; set; } = "";
        public int mail_operation_nbr { get; set; } = 0;
        public string line_4_text { get; set; } = "";
        public DateTime mpew_start_15min_dtm { get; set; }
        public DateTime mpew_end_15min_dtm { get; set; }
        public string mpe_type { get; set; } = "";
        public string mpe_name { get; set; } = "";
        public DateTime update_date_time { get; set; }
        public string nass_code { get; set; } = "";
        public string expected_throughput { get; set; } = "";
    }
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
}