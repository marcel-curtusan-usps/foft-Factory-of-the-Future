using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public List<Zone> Zones { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; } = "";

        [JsonProperty("tagVisible")]
        public bool TagVisible { get; set; } = false;

        [JsonProperty("tagVisibleMils")]
        public int TagVisibleMils { get; set; }

        [JsonProperty("isWearingTag")]
        public bool IsWearingTag { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("craftName")]
        public string CraftName { get; set; } = "";

        [JsonProperty("positionTS")]
        public DateTime PositionTS { get; set; }

        [JsonProperty("Tag_TS")]
        public DateTime TagTS { get; set; }

        [JsonProperty("Tag_Type")]
        public string TagType { get; set; } = "";

        [JsonProperty("Tag_Update")]
        public bool TagUpdate { get; set; }

        [JsonProperty("empId")]
        public string EmpId { get; set; } = "";

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
        public Tacs Tacs { get; set; }

        [JsonProperty("sels")]
        public string Sels { get; set; } = "";

        [JsonProperty("movementStatus")]
        public string MovementStatus { get; set; } = "noData";

        [JsonProperty("Raw_Data")]
        public string RawData { get; set; } = "";

        [JsonProperty("Camera_Data")]
        public Cameras CameraData { get; set; }

        [JsonProperty("Vehicle_Status_Data")]
        public VehicleStatus Vehicle_Status_Data { get; set; }

        [JsonProperty("Mission")]
        public Mission Misison { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; } = "";

        [JsonProperty("notificationId")]
        public string NotificationId { get; set; } = "";

        [JsonProperty("routePath")]
        public GeoLine RoutePath { get; set; }
        public List<DarvisCameraAlert> DarvisAlerts { get; internal set; }
    }
}