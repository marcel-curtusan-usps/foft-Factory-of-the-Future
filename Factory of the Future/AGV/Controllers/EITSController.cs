using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Factory_of_the_Future.AGV.Models;
using Newtonsoft.Json.Linq;

namespace Factory_of_the_Future.Controllers
{
    public class EITSController : ApiController
    {
        // GET: api/EITS
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/EITS/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/EITS
        public IHttpActionResult Post([FromBody] JToken request_data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            switch (request_data["ACTION"].ToString())
            {
                case "HEARTBEATSTATUS":
                  return CreatedAtRoute("DefaultApi", new { id = "0" }, new EITS_Messages.HeartbeatResponse());
                case "MOVEREQUEST":
                    return CreatedAtRoute("DefaultApi", new { id = "0" }, new EITS_Messages.MoveResponse());
                case "LOCATIONSTATUS":
                    return CreatedAtRoute("DefaultApi", new { id = "0" }, request_data);
                default:
                    break;
            }

            return CreatedAtRoute("DefaultApi", new { id = "0" }, new EITS_Messages.HeartbeatResponse());
        }

        //// PUT: api/EITS/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/EITS/5
        //public void Delete(int id)
        //{
        //}
    }
}
