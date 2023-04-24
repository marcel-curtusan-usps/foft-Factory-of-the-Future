using Newtonsoft.Json;
using System.Collections.Generic;

namespace Factory_of_the_Future
{
    public class MarkerGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Point";

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }
    }
}