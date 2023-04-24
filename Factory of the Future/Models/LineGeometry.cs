using Newtonsoft.Json;
using System.Collections.Generic;

namespace Factory_of_the_Future
{
    public class LineGeometry
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "LineString";

        [JsonProperty("coordinates")]
        public List<List<List<double>>> Coordinates { get; set; }
    }
}