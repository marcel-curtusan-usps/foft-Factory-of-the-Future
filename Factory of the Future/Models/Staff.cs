using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Factory_of_the_Future.Models
{
    public class Staff
    {
        [JsonProperty("mach_type")]
        public string MachType { get; set; } = "";

        [JsonProperty("machine_no")]
        public int MachineNo { get; set; } = 0;

        [JsonProperty("sortplan")]
        public string Sortplan { get; set; } = "";
        
        [JsonProperty("clerk")]
        public double Clerk { get; set; } = 0.0;

        [JsonProperty("mh")]
        public double Mh { get; set; } = 0.0;
        [JsonProperty("id")]
        public string Id
        {
            get
            {
                return string.Concat(MachType.ToString(), MachineNo.ToString(), Sortplan);
            }
            set
            {
                return;
            }
        }
    }
}
