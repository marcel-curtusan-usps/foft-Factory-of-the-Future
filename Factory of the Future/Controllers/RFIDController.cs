using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class RFIDController : ApiController
    {
        // GET: api/RFID
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/RFID/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/RFID
        public IHttpActionResult Post([FromBody] JToken request_data)
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
                    var requestDataToString = JsonConvert.SerializeObject(request_data, Formatting.Indented);
                    Task.Run(() => new ProcessRecvdMsg().StartProcess(requestDataToString, "RFID", ""));
                    //Task.Run(() => new ProcessRecvdMsg().StartProcess(request_data, "RFID", ""));
                }
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        // PUT: api/RFID/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/RFID/5
        public void Delete(int id)
        {
        }
    }
}
