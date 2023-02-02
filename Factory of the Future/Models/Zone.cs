using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class Zone
    {
        public bool LocationLocked { get; set; }
        public string Notes { get; set; }
        public bool Visible { get; set; }
        public string Color { get; set; }
        public string ZoneGroupId { get; set; }
        public string Name { get; set; }
        public string PolygonData { get; set; }
        public string Id { get; set; }
        public List<object> PolygonHoles { get; set; }
        public bool UpdateStatus { get; set; } = false;
    }
}