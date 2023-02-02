using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class DarvisCameraAlert
    {
        [JsonProperty("TYPE")]
        public string Type { get; set; }
        [JsonProperty("DWELL_TIME")]
        public float DwellTime { get; set; }
        [JsonProperty("TOP")]
        public int Top { get; set; }
        [JsonProperty("BOTTOM")]
        public int Bottom { get; set; }
        [JsonProperty("LEFT")]
        public int Left { get; set; }
        [JsonProperty("RIGHT")]
        public int Right { get; set; }
        [JsonProperty("OBJECT_ID")]
        public string object_id { get; set; }
        [JsonProperty("OBJECT_CLASS")]
        public string object_class { get; set; }
        [JsonProperty("ALERT_BASE64")]
        public string AlertBase64Image { get; set; }
    }
}