using Newtonsoft.Json;

namespace Factory_of_the_Future.Models
{
    public class TrailerStatus
    {

        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; } = "";

        [JsonProperty("trailerStatusSeqId")]
        public int TrailerStatusSeqId { get; set; }

        [JsonProperty("trailerBarcode")]
        public string TrailerBarcode { get; set; } = "";

        [JsonProperty("vanNumber")]
        public string VanNumber { get; set; } = "";

        [JsonProperty("trailerLengthCode")]
        public string TrailerLengthCode { get; set; } = "";

        [JsonProperty("siteId")]
        public string SiteId { get; set; } = "";

        [JsonProperty("locationType")]
        public string LocationType { get; set; } = "";

        [JsonProperty("location")]
        public string Location { get; set; } = "";

        [JsonProperty("processStatus")]
        public string ProcessStatus { get; set; } = "";

        [JsonProperty("trailerOwner")]
        public string TrailerOwner { get; set; } = "";

        [JsonProperty("isEditable")]
        public string IsEditable { get; set; } = "";

        [JsonProperty("actualArrDtm")]
        public EventDtm ActualArrDtm { get; set; }

        [JsonProperty("forcedActiveInd")]
        public string ForcedActiveInd { get; set; } = "";

        [JsonProperty("pierMail")]
        public string PierMail { get; set; } = "";

        [JsonProperty("processStatusDtm")]
        public EventDtm ProcessStatusDtm { get; set; }

        [JsonProperty("loadStatusDtm")]
        public EventDtm LoadStatusDtm { get; set; }

        [JsonProperty("assignedDriver")]
        public EventDtm AssignedDriver { get; set; }

        [JsonProperty("loadStatus")]
        public string LoadStatus { get; set; } = "";

        [JsonProperty("nonRollingLoadPercent")]
        public int? NonRollingLoadPercent { get; set; }
    }
}