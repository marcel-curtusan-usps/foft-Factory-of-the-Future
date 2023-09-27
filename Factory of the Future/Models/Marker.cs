using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Factory_of_the_Future
{
    public class Marker
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "";

        [JsonProperty("floorId")]
        public string FloorId { get; set; }

        [JsonProperty("rFId")]
        public string RFid { get; set; } = "";

        [JsonProperty("visible")]
        public bool Visible { get; set; } = false;

        [JsonProperty("zones")]
        public List<string> Zones { get; set; } = new List<string>();

        [JsonProperty("color")]
        public string Color { get; set; } = "";

        [JsonProperty("tagVisible")]
        public bool TagVisible { get; set; } = false;

        [JsonProperty("tagVisibleMils")]
        public long TagVisibleMils { get; set; } = 0;

        [JsonProperty("isWearingTag")]
        public bool IsWearingTag { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("craftName")]
        public string CraftName { get; set; } = "";

        [JsonProperty("positionTS")]
        public DateTime PositionTS { get; set; } = DateTime.MinValue;

        [JsonProperty("Tag_TS")]
        public DateTime TagTS { get; set; } = DateTime.MinValue;

        [JsonProperty("Tag_Type")]
        public string TagType { get; set; } = "";

        [JsonProperty("Tag_Update")]
        public bool TagUpdate { get; set; }
        [JsonProperty("serverTS")]
        public DateTime ServerTS { get; set; } = DateTime.MinValue;
        public bool isSch { get; set; }
        public bool isTacs { get; set; }
        public bool isePacs { get; set; }
        public bool isPosition { get; set; }
        [JsonProperty("empId")]
        public string EmpId { get; set; } = "";
        [JsonProperty("bdate")]
        public string Bdate { get; set; } = "";
        [JsonProperty("blunch")]
        public string Blunch { get; set; } = "";
        [JsonProperty("elunch")]
        public string Elunch { get; set; } = "";
        [JsonProperty("edate")]
        public string Edate { get; set; } = "";
        [JsonProperty("tourNumber")]
        public string TourNumber { get; set; } = "";
        [JsonProperty("reqDate")]
        public string ReqDate { get; set; } = "";
        [JsonProperty("daysOff")]
        public string DaysOff { get; set; } = "";

        [JsonProperty("emptype")]
        public string Emptype { get; set; } = "";

        [JsonProperty("badgeId")]
        public string BadgeId { get; set; } = "";

        [JsonProperty("empName")]
        public string EmpName { get; set; } = "";

        [JsonProperty("isLdcAlert")]
        public bool IsLdcAlert { get; set; }

        [JsonProperty("currentLDCs")]
        public string CurrentLDCs { get; set; } = "";

        [JsonProperty("tacs")]
        //public string Tacs { get; set; } = "";
        public Tacs Tacs { get; set; } = new Tacs();

        [JsonProperty("sels")]
        public string Sels { get; set; } = "";

        [JsonProperty("movementStatus")]
        public string MovementStatus { get; set; } = "noData";

        [JsonProperty("Raw_Data")]
        public string RawData { get; set; } = "";

        [JsonProperty("Camera_Data")]
        public Cameras CameraData { get; set; } = new Cameras();

        [JsonProperty("Vehicle_Status_Data")]
        public VehicleStatus Vehicle_Status_Data { get; set; } = new VehicleStatus();

        [JsonProperty("Mission")]
        public Mission Missison { get; set; } = new Mission();

        [JsonProperty("source")]
        public string Source { get; set; } = "";

        [JsonProperty("notificationId")]
        public string NotificationId { get; set; } = "";

        [JsonProperty("routePath")]
        public GeoLine RoutePath { get; set; } = new GeoLine();
        public List<DarvisCameraAlert> DarvisAlerts { get; internal set; } = new List<DarvisCameraAlert>();
        public string ZonesNames { get; internal set; } = "";
        [JsonProperty("locationMovementStatus")]
        public string LocationMovementStatus { get; internal set; } = "";

        [JsonProperty("locationType")]
        public string LocationType { get; set; } = "";
        [JsonProperty("lastSeenTS")]
        public DateTime LastSeenTS_txt { get; internal set; } = DateTime.MinValue;
        public long LastSeenTS { get; internal set; } = 0;
    }
}