using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class GeoZone
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Feature";

        [JsonProperty("geometry")]
        public ZoneGeometry Geometry { get; set; } = new ZoneGeometry();

        [JsonProperty("properties")]
        public Properties Properties { get; set; } = new Properties();
    }
}