﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace Factory_of_the_Future.Models
{
    public class TripReport
    {

        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; } = "";

        [JsonProperty("routeTripId")]
        public int RouteTripId { get; set; }

        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; } = "";

        [JsonProperty("trip")]
        public string Trip { get; set; } = "";

        [JsonProperty("tripDirectionInd")]
        public string TripDirectionInd { get; set; } = "";

        [JsonProperty("legNumber")]
        public int LegNumber { get; set; }

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
        public EventDtm ScheduledDtm { get; set; }

        [JsonProperty("actualDtm")]
        public EventDtm ActualDtm { get; set; }

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
        public int LoadPercent { get; set; }

        [JsonProperty("bedLoadPercent")]
        public int BedLoadPercent { get; set; }

        [JsonProperty("isEmpty")]
        public bool IsEmpty { get; set; }

        [JsonProperty("numNoBarcode")]
        public int NumNoBarcode { get; set; }

        [JsonProperty("numUnscannable")]
        public int NumUnscannable { get; set; }

        [JsonProperty("mailerName")]
        public string MailerName { get; set; } = "";

        [JsonProperty("schedulerId")]
        public string SchedulerId { get; set; } = "";

        [JsonProperty("crid")]
        public string Crid { get; set; } = "";

        [JsonProperty("nonStandardTripInd")]
        public string NonStandardTripInd { get; set; } = "";

        [JsonProperty("loadUnldStartDtm")]
        public EventDtm LoadUnldStartDtm { get; set; }

        [JsonProperty("loadUnldEndDtm")]
        public EventDtm LoadUnldEndDtm { get; set; }

        [JsonProperty("events")]
        public List<object> Events { get; set; }

        [JsonProperty("apptIrregs")]
        public List<object> ApptIrregs { get; set; }

        [JsonProperty("numExpected")]
        public int NumExpected { get; set; } 

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
        public EventDtm CompletedDtm { get; set; }

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
        public int? DriverId { get; set; }

        [JsonProperty("destSeal")]
        public string DestSeal { get; set; } = "";

        [JsonProperty("doorDtm")]
        public EventDtm DoorDtm { get; set; }

        [JsonProperty("supplier")]
        public string Supplier { get; set; } = "";

        [JsonProperty("driverPhoneNumber")]
        public string DriverPhoneNumber { get; set; } = "";

        [JsonProperty("delayCode")]
        public int? DelayCode { get; set; }

        [JsonProperty("delayReason")]
        public string DelayReason { get; set; } = "";

        [JsonProperty("originSeal")]
        public string OriginSeal { get; set; } = "";
        public string Id
        {
            get
            {
                return string.Concat(RouteTripId.ToString(), RouteTripLegId.ToString(), TripDirectionInd);
            }
            set
            {
                return;
            }
        }
    }
}