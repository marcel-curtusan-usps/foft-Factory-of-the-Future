using Newtonsoft.Json;

namespace Factory_of_the_Future.Models
{
    public class SVBullpen
    {
        [JsonProperty("locationId")]
        public int LocationId { get; set; } = 0;

        [JsonProperty("siteId")]
        public string SiteId { get; set; } = "";

        [JsonProperty("locationName")]
        public string LocationName { get; set; } = "";

        [JsonProperty("locationType")]
        public string LocationType { get; set; } = "";

        [JsonProperty("updtUserId")]
        public string UpdtUserId { get; set; } = "";

        [JsonProperty("bullpenType")]
        public string BullpenType { get; set; } = "";

        [JsonProperty("responseCode")]
        public int ResponseCode { get; set; } = 0;

        [JsonProperty("mpeName")]
        public string MpeName { get; set; } = "";

        [JsonProperty("binStart")]
        public int BinStart { get; set; } = 0;

        [JsonProperty("binEnd")]
        public int BinEnd { get; set; } = 0;
    }
}