using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class DockDoorController : ApiController
    {
        // GET: api/DockDoor
        public IEnumerable<RouteTrips> Get()
        {

            IEnumerable<RouteTrips> doors = new List<RouteTrips>();
            foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
            {
                cs.Zones.Where(f => f.Value.Properties.ZoneType == "DockDoor"
                ).Select(y => y.Value).ToList().ForEach(DockDoor =>
                {
                    doors= DockDoor.Properties.DockDoorData;
                });
            }
            if (doors.Count() > 0)
            {
                return doors;
            }
            else
            {
                return null;
            }
        }

        // GET: api/DockDoor/5
        public IEnumerable<RouteTrips> Get(string id)
        {
            IEnumerable<RouteTrips> doors = new List<RouteTrips>();
            foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
            {
                cs.Zones.Where(f => f.Value.Properties.ZoneType == "DockDoor" && f.Value.Properties.DoorNumber.ToLower() == id.ToLower()
                ).Select(y => y.Value).ToList().ForEach(DockDoor =>
                {
                    doors = DockDoor.Properties.DockDoorData;
                });
            }
            if (doors.Count() > 0)
            {
                return doors;
            }
            else
            {
                return null;
            }
        }

        // POST: api/DockDoor
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/DockDoor/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DockDoor/5
        public void Delete(int id)
        {
        }
    }
}
