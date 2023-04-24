using Newtonsoft.Json;
using System;

namespace Factory_of_the_Future.Models
{
    public class Connection
    {

        [JsonProperty("ActiveConnection")]
        public bool ActiveConnection { get; set; } = false;
        [JsonProperty("AdminEmailRecepient")]
        public string AdminEmailRecepient { get; set; } = "";
        [JsonProperty("ApiConnected")]
        public bool ApiConnected { get; set; } = false;
        [JsonProperty("ConnectionName")]
        public string ConnectionName { get; set; } = "";
        [JsonProperty("CreatedByUsername")]
        public string CreatedByUsername { get; set; } = "";
        [JsonProperty("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [JsonProperty("DataRetrieve")]
        public int DataRetrieve { get; set; } = 60000;
        [JsonProperty("DeactivatedByUsername")]
        public string DeactivatedByUsername { get; set; } = "";
        [JsonProperty("DeactivatedDate")]
        public DateTime DeactivatedDate { get; set; }
        [JsonProperty("Hostname")]
        public string Hostname { get; set; } = "";
        [JsonProperty("HoursBack")]
        public int HoursBack { get; set; } = 0;
        [JsonProperty("HoursForward")]
        public int HoursForward { get; set; } = 0;
        [JsonProperty("Https")]
        public bool Https { get; set; } = false;
        [JsonProperty("Id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("IpAddress")]
        public string IpAddress { get; set; } = "";
        [JsonProperty("LasttimeApiConnected")]
        public DateTime LasttimeApiConnected { get; set; }
        [JsonProperty("LastupDate")]
        public DateTime LastupDate { get; set; }
        [JsonProperty("LastupdateByUsername")]
        public string LastupdateByUsername { get; set; } = "";
        [JsonProperty("MessageType")]
        public string MessageType { get; set; } = "";
        [JsonProperty("NassCode")]
        public string NassCode { get; set; } = "";
        [JsonProperty("OutgoingApikey")]
        public string OutgoingApikey { get; set; } = "";
        [JsonProperty("Port")]
        public Int32 Port { get; set; } = 0;
        [JsonProperty("UdpConnection")]
        public bool UdpConnection { get; set; } = false;
        [JsonProperty("TcpIpConnection")]
        public bool TcpIpConnection { get; set; } = false;
        [JsonProperty("WsConnection")]
        public bool WsConnection { get; set; } = false;
        [JsonProperty("ApiConnection")]
        public bool ApiConnection { get; set; } = false;
        [JsonIgnore]
        public bool UpdateStatus { get; set; } = false;
        [JsonProperty("Url")]
        public string Url { get; set; } = "";
        [JsonProperty("Status")]
        public string Status { get; set; } = "";
    }
}