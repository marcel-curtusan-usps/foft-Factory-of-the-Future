﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class CoordinateSystem
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "";
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("locators")]
        public ConcurrentDictionary<string, GeoMarker> Locators = new ConcurrentDictionary<string, GeoMarker>();

        [JsonProperty("backgroundImages")]
        public BackgroundImage BackgroundImage = new BackgroundImage();

        [JsonProperty("zones")]
        public ConcurrentDictionary<string, GeoZone> Zones = new ConcurrentDictionary<string, GeoZone>();
    }

}