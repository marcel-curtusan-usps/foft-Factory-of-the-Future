using Newtonsoft.Json;

namespace Factory_of_the_Future
{
    public class Tacs
    {

        [JsonProperty("ldc")]
        public string Ldc { get; set; } = "";

        [JsonProperty("finance")]
        public string Finance { get; set; } = "";

        [JsonProperty("fnAlert")]
        public string FnAlert { get; set; } = "";

        [JsonProperty("totalTime")]
        public int TotalTime { get; set; } = 0;

        [JsonProperty("operationId")]
        public string OperationId { get; set; } = "";

        [JsonProperty("payLocation")]
        public string PayLocation { get; set; } = "";

        [JsonProperty("isOvertimeAuth")]
        public bool IsOvertimeAuth { get; set; }

        [JsonProperty("overtimeHours")]
        public int OvertimeHours { get; set; } = 0;

        [JsonProperty("isOvertime")]
        public bool IsOvertime { get; set; }

        [JsonProperty("startTs")]
        public object StartTs { get; set; }

        [JsonProperty("startTxt")]
        public string StartTxt { get; set; } = "";

        [JsonProperty("ts")]
        public object Ts { get; set; }

        [JsonProperty("openRingCode")]
        public string OpenRingCode { get; set; } = "";
    }
}