using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class IncomingDataController : ApiController
    {
        // GET: api/IncomingData
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/IncomingData/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/IncomingData
        public IHttpActionResult Post([FromBody] JObject request_data)
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
                    //create new Connection Object
                    JObject conn = new JObject(new JProperty("IP_ADDRESS", ""));
                    //Send data to be processed.
                   Global.ProcessRecvdMsg_callback.StartProcess(request_data, conn);
                }
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        // PUT: api/IncomingData/5
        public void Put(int id, [FromBody] string value)
        {
        }
    }
}