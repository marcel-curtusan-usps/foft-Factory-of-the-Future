﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }
    public class Tag
    {
        [JsonProperty("tagId")]
        public string TagId { get; set; } = "";

        [JsonProperty("tagName")]
        public string TagName { get; set; } = "";

        [JsonProperty("color")]
        public string Color { get; set; } = "";

        [JsonProperty("tagGroupName")]
        public string TagGroupName { get; set; } = "";

        [JsonProperty("locationType")]
        public string LocationType { get; set; } = "";

        [JsonProperty("locationMovementStatus")]
        public string LocationMovementStatus { get; set; } = "";

        [JsonProperty("locationRadius")]
        public double? LocationRadius { get; set; } = 0.0;

        [JsonProperty("location")]
        public List<double> Location { get; set; } = new List<double>();

        [JsonProperty("locationTS")]
        public long LocationTS { get; set; } = 0;

        [JsonProperty("locationCoordSysId")]
        public string LocationCoordSysId { get; set; } = "";

        [JsonProperty("locationCoordSysName")]
        public string LocationCoordSysName { get; set; } = "";

        [JsonProperty("locationZoneIds")]
        public List<string> LocationZoneIds { get; set; } = new List<string>();

        [JsonProperty("locationZoneNames")]
        public List<string> LocationZoneNames { get; set; } = new List<string>();
    }
}