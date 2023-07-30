using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future.Models
{
    public class Door_Association_Trips
    {
        [JsonProperty("SQLTypeName")]
        public string SQLTypeName = "";

        [JsonProperty("doorNum")]
        public string DoorNum = "";

        [JsonProperty("doorNbr")]
        public string DoorNbr = "";

        [JsonProperty("siteId")]
        public string SiteId = "";

        [JsonProperty("doorBarcode")]
        public string DoorBarcode = "";

        [JsonProperty("outOfSvcInd")]
        public string OutOfSvcInd = "";

        [JsonProperty("occupiedStatusInd")]
        public string OccupiedStatusInd = "";

        [JsonProperty("doorRefId")]
        public int DoorRefId;

        [JsonProperty("pendingMoveRequestInd")]
        public string PendingMoveRequestInd = "";

        [JsonProperty("associatedTrips")]
        public string AssociatedTrips = "";
        public string Route = "";
        public string Trip = "";
    }
}