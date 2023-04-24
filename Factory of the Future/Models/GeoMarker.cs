using Newtonsoft.Json;

namespace Factory_of_the_Future
{
    public class GeoMarker
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "Feature";
        [JsonProperty("geometry")]
        public MarkerGeometry Geometry { get; set; } = new MarkerGeometry();
        [JsonProperty("properties")]
        public Marker Properties { get; set; } = new Marker();
    }
}