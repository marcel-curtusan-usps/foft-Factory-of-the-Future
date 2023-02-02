using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class ContainerHistory
    {

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("eventDtm")]
        public EventDtm EventDtm { get; set; } = new EventDtm();
        public DateTime EventDtmfmt
        {
            get
            {
                return new DateTime(EventDtm.Year, (EventDtm.Month + 1), EventDtm.DayOfMonth, EventDtm.HourOfDay, EventDtm.Minute, EventDtm.Second);
            }
            set { return; }
        }

        [JsonProperty("siteId")]
        public string SiteId { get; set; } = "";

        [JsonProperty("updtUserId")]
        public string UpdtUserId { get; set; } = "";

        [JsonProperty("siteName")]
        public string SiteName { get; set; } = "";

        [JsonProperty("siteType")]
        public string SiteType { get; set; } = "";

        [JsonProperty("route")]
        public string Route { get; set; } = "";

        [JsonProperty("trip")]
        public string Trip { get; set; } = "";

        [JsonProperty("trailer")]
        public string Trailer { get; set; } = "";

        [JsonProperty("source")]
        public string Source { get; set; } = "";

        [JsonProperty("redirectInd")]
        public string RedirectInd { get; set; } = "";

        [JsonProperty("location")]
        public string Location { get; set; } = "";

        [JsonProperty("binNumber")]
        public string BinNumber { get; set; } = "";

        [JsonProperty("binName")]
        public string BinName { get; set; } = "";

        public int sortind { get; set; }
    }
}