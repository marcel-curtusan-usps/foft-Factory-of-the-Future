using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class DockDoorStatus
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; }

        [JsonProperty("locationId")]
        public int LocationId { get; set; }

        [JsonProperty("locationBarcode")]
        public string LocationBarcode { get; set; }

        [JsonProperty("locationNumber")]
        public string LocationNumber { get; set; }

        [JsonProperty("siteIds")]
        public List<string> SiteIds { get; set; }

        [JsonProperty("tripReport")]
        public TripReport TripReport { get; set; }

        [JsonProperty("locationType")]
        public string LocationType { get; set; }
    }
}