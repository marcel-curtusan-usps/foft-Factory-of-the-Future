﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Factory_of_the_Future.Models
{
    public class QuuppaTag
    {

        [JsonProperty("code")]
        public string Code { get; set; } = "";

        [JsonProperty("status")]
        public string Status { get; set; } = "";

        [JsonProperty("command")]
        public string Command { get; set; } = "";

        [JsonProperty("message")]
        public string Message { get; set; } = "";

        [JsonProperty("responseTS")]
        public long ResponseTS { get; set; } = 0;

        [JsonProperty("version")]
        public string Version { get; set; } = "";

        [JsonProperty("formatId")]
        public string FormatId { get; set; } = "";

        [JsonProperty("formatName")]
        public string FormatName { get; set; } = "";

        [JsonProperty("tags")]
        public List<Tags> Tags { get; set; } = new List<Tags>();
    }
    public class Tags
    {
        [JsonProperty("deviceAddress")]
        public string TagId { get; set; } = "";
        [JsonProperty("deviceType")]
        public string DeviceType { get; set; } = "";

        [JsonProperty("tagName", NullValueHandling = NullValueHandling.Ignore)]
        public string TagName { get; set; } = "";

        [JsonProperty("tagState")]
        public string TagState { get; set; } = "";

        [JsonProperty("tagStateTS")]
        public long LagStateTS { get; set; } = 0;

        [JsonProperty("color")]
        public string Color { get; set; } = "";

        [JsonProperty("tagGroupName")]
        public string TagGroupName { get; set; } = "";

        [JsonProperty("locationType")]
        public string LocationType { get; set; } = "";

        [JsonProperty("lastSeenTS", NullValueHandling = NullValueHandling.Ignore)]
        public long LastSeenTS { get; set; } = 0;

        [JsonProperty("locationMovementStatus")]
        public string LocationMovementStatus { get; set; } = "";

        [JsonProperty("locationRadius")]
        public double? LocationRadius { get; set; } = 0.0;

        [JsonProperty("smoothedPosition")]
        public List<double> Location { get; set; } = new List<double>();

        [JsonProperty("locationTS")]
        public long LocationTS { get; set; } = 0;

        [JsonProperty("lastPacketTS")]
        public long LastPacketTS { get; set; } = 0;

        [JsonProperty("triggerCount")]
        public long TriggerCount { get; set; } = 0;

        [JsonProperty("triggerCountTS",NullValueHandling = NullValueHandling.Ignore)]
        public long TriggerCountTS { get; set; } = 0;

        [JsonProperty("coordinateSystemId")]
        public string LocationCoordSysId { get; set; } = "";

        [JsonProperty("coordinateSystemName")]
        public string LocationCoordSysName { get; set; } = "";

        [JsonProperty("locationZoneIds")]
        public List<string> LocationZoneIds { get; set; } = new List<string>();

        [JsonProperty("locationZoneNames")]
        public List<string> LocationZoneNames { get; set; } = new List<string>();
    }
}
/* data from quuppa
 {
	"acceleration": null,
	"accelerationTS": 1691158328866,
	"batteryAlarm": "ok",
	"batteryAlarmTS": 1691158328449,
	"batteryVoltage": 3,
	"batteryVoltageTS": 1691158327515,
	"button2State": "notPushed",
	"button2StateTS": 1691158328449,
	"button1State": "notPushed",
	"button1StateTS": 1691158328449,
	"color": "#FF0000",
	"configStatus": "notStarted",
	"configStatusTS": 1691158328866,
	"coordinateSystemId": "7a23124a-7bf5-48e6-afd1-24acec977410",
	"coordinateSystemName": "Floor",
	"deviceAddress": "efba5c8e412a",
	"deviceType": "QT1",
	"tagGroupName": null,
	"tagGroupId": null,
	"locationCoordSysName": "Floor",
	"locationTS": 1691158327761,
	"button2LastPressTS": null,
	"button1LastPressTS": null,
	"lastPacketTS": 1691158328754,
	"tagName": null,
	"smoothedPosition": [275.09, 48.40, 1.20],
	"smoothedPositionAccuracy": 0.32,
	"tagState": "triggered",
	"tagStateTS": 1691158328451,
	"temperature": null,
	"temperatureTS": 1691158328867,
	"triggerCount": 26299,
	"triggerCountTS": 1691158328130,
	"locationZoneIds": ["69172ee1-d563-4467-bd28-d1f5739ad026", "d2808df4-ad9e-4eed-b98a-da1207d209ca", "658987b1-741e-453a-b392-1647a4312f95"],
	"locationZoneNames": ["APPS-068", "MMWH_Packages-E", "VP_Packages"]
}
 */