using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class SVDockDoorController : ApiController
    {
        // GET: api/DockDoor
        public IEnumerable<RouteTrips> Get()
        {

            List<RouteTrips> doors = new List<RouteTrips>();
            try
            {
                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => f.Value.Properties.ZoneType == "DockDoor"
                    ).Select(y => y.Value).ToList().ForEach(DockDoor =>
                    {
                        doors.AddRange(DockDoor.Properties.DockDoorData);
                    });
                }
                if (doors.Any())
                {
                    return doors;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
            finally
            {
                doors = null;
            }

        }

        // GET: api/DockDoor/5
        public IEnumerable<RouteTrips> Get(string id)
        {
            IEnumerable<RouteTrips> doors = new List<RouteTrips>();
            foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
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
        public async Task<IHttpActionResult> PostAsync([FromBody] JToken request_data)
        {
            //handle bad requests
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (request_data != null)
            {
                //start data process
                if (request_data.HasValues)
                {
                    //Send data to be processed.
                    await Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(request_data, Formatting.None), "doors", "")).ConfigureAwait(false);
                }
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }
    }
}
