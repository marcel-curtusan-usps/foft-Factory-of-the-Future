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
        public EventDtm ScheduledDtm { get; set; }

        [JsonProperty("legScheduledDtm")]
        public EventDtm LegScheduledDtm { get; set; }

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
        public EventDtm CancelDtm { get; set; }

        [JsonProperty("containers")]
        public List<object> Containers { get; set; }

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

        [JsonProperty("vanNumber")]
        public string VanNumber { get; set; }

        [JsonProperty("trailerBarcode")]
        public string TrailerBarcode { get; set; }

        [JsonProperty("trailerLengthCode")]
        public string TrailerLengthCode { get; set; }

        [JsonProperty("mspBarcode")]
        public string MspBarcode { get; set; }

        [JsonProperty("doorDtm")]
        public EventDtm DoorDtm { get; set; }

        [JsonProperty("legDoorDtm")]
        public EventDtm LegDoorDtm { get; set; }

        [JsonProperty("actualDtmSource")]
        public string ActualDtmSource { get; set; }

        [JsonProperty("gpsId")]
        public string GpsId { get; set; }

        [JsonProperty("gpsIdSource")]
        public string GpsIdSource { get; set; }

        [JsonProperty("gpsSiteDtm")]
        public EventDtm GpsSiteDtm { get; set; }
      
        [JsonProperty("loadPercent")]
        public int? LoadPercent { get; set; }

        [JsonProperty("loadUnldStartDtm")]
        public EventDtm LoadUnldStartDtm { get; set; }
    
        [JsonProperty("loadUnldEndDtm")]
        public EventDtm LoadUnldEndDtm { get; set; }
      
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
        public EventDtm LegGateDtm { get; set; }

        [JsonProperty("form5500L")]
        public Form5500L Form5500L { get; set; }

        [JsonProperty("originComments")]
        public string OriginComments { get; set; }

        [JsonProperty("bedLoadPercent")]
        public int? BedLoadPercent { get; set; }

        [JsonProperty("scheduledArrDTM", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime ScheduledArrDTM { get; set; }

        [JsonProperty("scheduledDepDTM", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime ScheduledDepDTM { get; set; }

        [JsonProperty("actDepartureDtm", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime ActDepartureDtm { get; set; }

        [JsonProperty("actArrivalDtm", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime ActArrivalDtm { get; set; }
    }

}