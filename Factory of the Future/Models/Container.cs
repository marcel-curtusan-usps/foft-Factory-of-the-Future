using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class Container
    {

        public DateTime EventDtm { get; set; } = DateTime.Now;

        [JsonProperty("placardBarcode")]
        public string PlacardBarcode { get; set; } = "";

        [JsonProperty("trailer")]
        public string Trailer { get; set; } = "";

        [JsonProperty("containerHistory")]
        public List<ContainerHistory> ContainerHistory { get; set; } = new List<ContainerHistory>();

        [JsonProperty("origin")]
        public string Origin { get; set; } = "";

        [JsonProperty("dest")]
        public string Dest { get; set; } = "";

        [JsonProperty("originName")]
        public string OriginName { get; set; } = "";

        [JsonProperty("destinationName")]
        public string DestinationName { get; set; } = "";

        [JsonProperty("location")]
        public string Location { get; set; } = "";

        [JsonProperty("binNumber")]
        public string BinNumber { get; set; } = "";

        [JsonProperty("binName")]
        public string BinName { get; set; } = "";

        [JsonProperty("mailClass")]
        public string MailClass { get; set; } = "";

        [JsonProperty("mailType")]
        public string MailType { get; set; } = "";
        public string MailClassDisplay { get; set; } = "";
        public string BinDisplay { get; set; } = "";
        public bool hasPrintScans { get; set; }
        public bool hasAssignScans { get; set; }
        public bool hasCloseScans { get; set; }
        public bool hasLoadScans { get; set; }
        public bool hasUnloadScans { get; set; }
        public bool containerTerminate { get; set; }
        public bool containerAtDest { get; set; }
        public bool containerRedirectedDest { get; set; }
    }
}