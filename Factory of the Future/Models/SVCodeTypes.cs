using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class CreatedDtm
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [JsonProperty("hourOfDay")]
        public int HourOfDay { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("second")]
        public int Second { get; set; }
    }

    public class LastUpdtDtm
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [JsonProperty("hourOfDay")]
        public int HourOfDay { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("second")]
        public int Second { get; set; }
    }

    public class SVCodeTypes
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName { get; set; }

        [JsonProperty("codeTypeName")]
        public string CodeTypeName { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("decodeDesc")]
        public string DecodeDesc { get; set; }

        [JsonProperty("attr1Text")]
        public string Attr1Text { get; set; }

        [JsonProperty("attr3Text")]
        public string Attr3Text { get; set; }

        [JsonProperty("attr4Text")]
        public string Attr4Text { get; set; }

        [JsonProperty("deviceInd")]
        public string DeviceInd { get; set; }

        [JsonProperty("createdDtm")]
        public CreatedDtm CreatedDtm { get; set; }

        [JsonProperty("lastUpdtDtm")]
        public LastUpdtDtm LastUpdtDtm { get; set; }

        [JsonProperty("updtUserId")]
        public string UpdtUserId { get; set; }

        [JsonProperty("attr2Text")]
        public string Attr2Text { get; set; }

        [JsonProperty("attr5Text")]
        public string Attr5Text { get; set; }
    }
}