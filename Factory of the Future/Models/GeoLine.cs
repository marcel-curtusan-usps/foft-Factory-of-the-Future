using Newtonsoft.Json;

namespace Factory_of_the_Future
{
    public class GeoLine
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Feature";

        [JsonProperty("geometry")]
        public LineGeometry Geometry { get; set; } = new LineGeometry();

        [JsonProperty("properties")]
        public RoutePath Properties { get; set; } = new RoutePath();
    }
}