using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class BackgroundImage
    {

        [JsonProperty("widthMeter")]
        public double WidthMeter { get; set; }

        [JsonProperty("xMeter")]
        public double XMeter { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("otherCoordSys")]
        public string OtherCoordSys { get; set; } = "";

        [JsonProperty("rotation")]
        public int Rotation { get; set; } = 0;

        [JsonProperty("base64")]
        public string Base64 { get; set; } = "";

        [JsonProperty("origoY")]
        public double OrigoY { get; set; } = 0.0;

        [JsonProperty("origoX")]
        public double OrigoX { get; set; } = 0.0;

        [JsonProperty("heightMeter")]
        public double HeightMeter { get; set; } = 0.0;

        [JsonProperty("yMeter")]
        public double YMeter { get; set; } = 0.0;

        [JsonProperty("alpha")]
        public int Alpha { get; set; } = 0;

        [JsonProperty("id")]
        public string Id { get; set; } = "";

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("metersPerPixelY")]
        public double MetersPerPixelY { get; set; } = 0.0;

        [JsonProperty("metersPerPixelX")]
        public double MetersPerPixelX { get; set; } = 0.0;
        [JsonProperty("updateStatus")]
        public bool UpdateStatus { get; set; } = false;

        [JsonProperty("rawData")]
        public string RawData { get; set; } = "";

        [JsonProperty("coordinateSystemId")]
        public string CoordinateSystemId { get; set; } = "";
    }
}