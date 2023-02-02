using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class EventDtm
    {
        [JsonProperty("year")]
        public int Year { get; set; } = 1;

        [JsonProperty("month")]
        public int Month { get; set; } = 1;

        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; } = 1;

        [JsonProperty("hourOfDay")]
        public int HourOfDay { get; set; } = 1;

        [JsonProperty("minute")]
        public int Minute { get; set; } = 0;

        [JsonProperty("second")]
        public int Second { get; set; } = 0;
    }
}