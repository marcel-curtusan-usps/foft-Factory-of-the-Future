using Newtonsoft.Json;

namespace Factory_of_the_Future
{
    public class MachineData
    {
        [JsonProperty("site_name_local")]
        public string SiteNameLocal { get; set; } = "";

        [JsonProperty("area_name")]
        public string AreaName { get; set; } = "";

        [JsonProperty("district")]
        public string District { get; set; } = "";

        [JsonProperty("local_link")]
        public string LocalLink { get; set; } = "";

        [JsonProperty("rnum")]
        public string Rnum { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; } = "";

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; } = "";
    }
}