using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class Cameras
    {
        [JsonProperty("LOCALE_KEY")]
        public string LocaleKey { get; set; } = "";

        [JsonProperty("MODEL_NUM")]
        public string ModelNum { get; set; } = "";

        [JsonProperty("FACILITY_PHYS_ADDR_TXT")]
        public string FacilityPhysAddrTxt { get; set; } = "";

        [JsonProperty("GEO_PROC_REGION_NM")]
        public string GeoProcRegionNm { get; set; } = "";

        [JsonProperty("FACILITY_SUBTYPE_DESC")]
        public string FacilitySubtypeDesc { get; set; } = "";

        [JsonProperty("GEO_PROC_DIVISION_NM")]
        public string GeoProcDivisionNm { get; set; } = "";

        [JsonProperty("AUTH_KEY")]
        public string AuthKey { get; set; } = "";

        [JsonProperty("FACILITY_LATITUDE_NUM")]
        public double FacilitiyLatitudeNum { get; set; } = 0.0;

        [JsonProperty("FACILITY_LONGITUDE_NUM")]
        public double FacilitiyLongitudeNum { get; set; } = 0.0;

        [JsonProperty("CAMERA_NAME")]
        public string CameraName { get; set; } = "";

        [JsonProperty("DESCRIPTION")]
        public string Description { get; set; } = "";

        [JsonProperty("REACHABLE")]
        public string Reachable { get; set; } = "";

        [JsonProperty("FACILITY_DISPLAY_NME")]
        public string FacilityDisplayName { get; set; } = "";

        [JsonProperty("base64Image")]
        public string Base64Image { get; set; } = "";

        [JsonProperty("CAMERA_ALERTS")]
        public List<DarvisCameraAlert> Alerts { get; set; }

        [JsonProperty("LAST_ALERT_UPDATE")]
        public long LastAlertUpdate { get; set; } = 0;
    }
}