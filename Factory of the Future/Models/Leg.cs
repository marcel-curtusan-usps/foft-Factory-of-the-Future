using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class Leg
    {

        [JsonProperty("routeTripLegId")]
        public int RouteTripLegId { get; set; } = 0;

        [JsonProperty("routeTripId")]
        public int RouteTripId { get; set; } = 0;

        [JsonProperty("legNumber")]
        public int LegNumber { get; set; } = 0;

        [JsonProperty("legDestSiteID")]
        public string LegDestSiteID { get; set; } = "";

        [JsonProperty("legOriginSiteID")]
        public string LegOriginSiteID { get; set; } = "";

        [JsonProperty("scheduledArrDTM")]
        public EventDtm ScheduledArrDTM { get; set; } = new EventDtm();

        [JsonProperty("scheduledDepDTM")]
        public EventDtm ScheduledDepDTM { get; set; } = new EventDtm();

        [JsonProperty("actDepartureDtm")]
        public EventDtm ActDepartureDtm { get; set; } = new EventDtm();

        [JsonProperty("createdDtm")]
        public EventDtm CreatedDtm { get; set; } = new EventDtm();

        [JsonProperty("lastUpdtDtm")]
        public EventDtm LastUpdtDtm { get; set; } = new EventDtm();

        [JsonProperty("legDestSiteName")]
        public string LegDestSiteName { get; set; } = "";

        [JsonProperty("legOriginSiteName")]
        public string LegOriginSiteName { get; set; } = "";

        [JsonProperty("outboundProcessedInd")]
        public string OutboundProcessedInd { get; set; } = "";

        [JsonProperty("inboundProcessedInd")]
        public string InboundProcessedInd { get; set; } = "";

        [JsonProperty("legOriginMSPBarcode")]
        public string LegOriginMSPBarcode { get; set; } = "";

        [JsonProperty("legDestMSPBarcode")]
        public string LegDestMSPBarcode { get; set; } = "";
    }
}