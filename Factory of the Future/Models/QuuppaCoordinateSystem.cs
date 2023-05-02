using Newtonsoft.Json;
using System.Collections.Generic;

namespace Factory_of_the_Future.Models
{
    public class QuuppaCoordinateSystem
    {
        [JsonProperty("coordinateSystems")]
        public List<Quuppa_CoordinateSystem> CoordinateSystems { get; set; }

        [JsonProperty("gatewayFilters")]
        public List<object> GatewayFilters { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("responseTS")]
        public long ResponseTS { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
    public class Quuppa_CoordinateSystem
    {
        [JsonProperty("locators")]
        public List<Locator> Locators { get; set; }

        [JsonProperty("backgroundImages")]
        public List<BackgroundImage> BackgroundImages { get; set; }

        [JsonProperty("relativeZ")]
        public int RelativeZ { get; set; }

        [JsonProperty("polygons")]
        public List<Polygon> Polygons { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("zones")]
        public List<qcZone> qcZones { get; set; }

    
    }
    public class qcZone
    {
        [JsonProperty("locationLocked")]
        public bool LocationLocked { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("zoneGroupId")]
        public string ZoneGroupId { get; set; }

        [JsonProperty("openingIds")]
        public List<object> OpeningIds { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("polygonData")]
        public string PolygonData { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("polygonHoles")]
        public List<object> PolygonHoles { get; set; }
    }

    public class Locator
    {
        [JsonProperty("locationLocked")]
        public bool LocationLocked { get; set; }

        [JsonProperty("orientation")]
        public List<double> Orientation { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("focusingErrorDeg")]
        public double FocusingErrorDeg { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("isOrientationSetManually")]
        public bool IsOrientationSetManually { get; set; }

        [JsonProperty("associatedAreas")]
        public List<string> AssociatedAreas { get; set; }

        [JsonProperty("locationLockedX")]
        public bool LocationLockedX { get; set; }

        [JsonProperty("locatorChannel")]
        public string LocatorChannel { get; set; }

        [JsonProperty("locationLockedZ")]
        public bool LocationLockedZ { get; set; }

        [JsonProperty("locationLockedY")]
        public bool LocationLockedY { get; set; }

        [JsonProperty("locatorType")]
        public string LocatorType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public List<double> Location { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
    public class Polygon
    {
        [JsonProperty("locationLocked")]
        public bool LocationLocked { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("openingIds")]
        public List<object> OpeningIds { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("polygonData")]
        public string PolygonData { get; set; }


        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("polygonHoles")]
        public List<PolygonHole> PolygonHoles { get; set; }
    }
    public class PolygonHole
    {
        [JsonProperty("locationLocked")]
        public bool LocationLocked { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("polygonData")]
        public string PolygonData { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}