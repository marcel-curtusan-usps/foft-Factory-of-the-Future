using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace Factory_of_the_Future.Models
{
    public class TripReport
    {

        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; } = "";

        [JsonProperty("atDoor")]
        public bool AtDoor { get; set; }
        [JsonProperty("operDate")]
        public EventDtm OperDate { get; set; } = new EventDtm();

        [JsonProperty("routeTripId")]
        public int RouteTripId { get; set; } = 0;

        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId { get; set; } = 0;

        [JsonProperty("route")]
        public string Route { get; set; } = "";

        [JsonProperty("trip")]
        public string Trip { get; set; } = "";

        [JsonProperty("tripDirectionInd")]
        public string TripDirectionInd { get; set; } = "";

        [JsonProperty("legNumber")]
        public int LegNumber { get; set; } = 0;

        [JsonProperty("serviceTypeCode")]
        public string ServiceTypeCode { get; set; } = "";

        [JsonProperty("apptId")]
        public string ApptId { get; set; } = "";

        [JsonProperty("recurringApptId")]
        public string RecurringApptId { get; set; } = "";

        [JsonProperty("legSiteId")]
        public string LegSiteId { get; set; } = "";

        [JsonProperty("legSiteName")]
        public string LegSiteName { get; set; } = "";

        [JsonProperty("tripSiteId")]
        public string TripSiteId { get; set; } = "";

        [JsonProperty("tripSiteName")]
        public string TripSiteName { get; set; } = "";

        [JsonProperty("status")]
        public string Status { get; set; } = "";

        [JsonProperty("scheduledDtm")]
        public EventDtm ScheduledDtm { get; set; } = new EventDtm();

        [JsonProperty("actualDtm")]
        public EventDtm ActualDtm { get; set; } = new EventDtm();

        [JsonProperty("doorId")]
        public string DoorId { get; set; } = "";

        [JsonProperty("doorNumber")]
        public string DoorNumber { get; set; } = "";

        [JsonProperty("vanNumber")]
        public string VanNumber { get; set; } = "";

        [JsonProperty("trailerBarcode")]
        public string TrailerBarcode { get; set; } = "";

        [JsonProperty("trailerLengthCode")]
        public string TrailerLengthCode { get; set; } = "";

        [JsonProperty("loadPercent")]
        public int LoadPercent { get; set; } = 0;

        [JsonProperty("bedLoadPercent")]
        public int BedLoadPercent { get; set; } = 0;

        [JsonProperty("isEmpty")]
        public bool IsEmpty { get; set; }

        [JsonProperty("numNoBarcode")]
        public int NumNoBarcode { get; set; } = 0;

        [JsonProperty("numUnscannable")]
        public int NumUnscannable { get; set; } = 0;

        [JsonProperty("mailerName")]
        public string MailerName { get; set; } = "";

        [JsonProperty("schedulerId")]
        public string SchedulerId { get; set; } = "";

        [JsonProperty("crid")]
        public string Crid { get; set; } = "";

        [JsonProperty("nonStandardTripInd")]
        public string NonStandardTripInd { get; set; } = "";

        [JsonProperty("loadUnldStartDtm")]
        public EventDtm LoadUnldStartDtm { get; set; } = new EventDtm();

        [JsonProperty("loadUnldEndDtm")]
        public EventDtm LoadUnldEndDtm { get; set; } = new EventDtm();

        [JsonProperty("events")]
        public List<object> Events { get; set; }

        [JsonProperty("apptIrregs")]
        public List<object> ApptIrregs { get; set; }

        [JsonProperty("numExpected")]
        public int NumExpected { get; set; } = 0;

        [JsonProperty("originSiteName")]
        public string OriginSiteName { get; set; } = "";

        [JsonProperty("originSiteId")]
        public string OriginSiteId { get; set; } = "";

        [JsonProperty("destSiteName")]
        public string DestSiteName { get; set; } = "";

        [JsonProperty("destSiteId")]
        public string DestSiteId { get; set; } = "";

        [JsonProperty("ymsSiteInd")]
        public string YmsSiteInd { get; set; } = "";

        [JsonProperty("yardMgmtInd")]
        public string YardMgmtInd { get; set; } = "";

        [JsonProperty("completedDtm")]
        public EventDtm CompletedDtm { get; set; } = new EventDtm();

        [JsonProperty("trailerStatus")]
        public TrailerStatus TrailerStatus { get; set; } = new TrailerStatus();

        [JsonProperty("perishableInd")]
        public string PerishableInd { get; set; } = "";

        [JsonProperty("driverFirstName")]
        public string DriverFirstName { get; set; } = "";

        [JsonProperty("driverLastName")]
        public string DriverLastName { get; set; } = "";

        [JsonProperty("driverBarcode")]
        public string DriverBarcode { get; set; } = "";

        [JsonProperty("driverId")]
        public int? DriverId { get; set; } = 0;

        [JsonProperty("destSeal")]
        public string DestSeal { get; set; } = "";

        [JsonProperty("doorDtm")]
        public EventDtm DoorDtm { get; set; } = new EventDtm();

        [JsonProperty("supplier")]
        public string Supplier { get; set; } = "";

        [JsonProperty("driverPhoneNumber")]
        public string DriverPhoneNumber { get; set; } = "";

        [JsonProperty("delayCode")]
        public int? DelayCode { get; set; } = 0;

        [JsonProperty("delayReason")]
        public string DelayReason { get; set; } = "";

        [JsonProperty("originSeal")]
        public string OriginSeal { get; set; } = "";
        public string Id
        {
            get
            {
                if (!string.IsNullOrEmpty(TrailerStatus.ProcessStatus))
                {

                    if (TrailerStatus.ProcessStatus.StartsWith("O"))
                    {
                        TripDirectionInd = "O";
                        return string.Concat(RouteTripId.ToString(), RouteTripLegId.ToString(), "O");
                    }
                    else if (TrailerStatus.ProcessStatus.StartsWith("I"))
                    {
                        TripDirectionInd = "I";
                        return string.Concat(RouteTripId.ToString(), RouteTripLegId.ToString(), "I");
                    }
                }
                if (RouteTripId == 0 && !string.IsNullOrEmpty(TrailerBarcode))
                {
                    return string.Concat(TrailerBarcode, TripDirectionInd);
                }
                if (RouteTripId > 0)
                {
                    return string.Concat(RouteTripId.ToString(), RouteTripLegId.ToString(), TripDirectionInd);
                }
                return "";
            }
            set
            {
                return;
            }
        }
    }
}