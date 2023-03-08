using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Factory_of_the_Future.Controllers
{
    public class MPE_PerfController : ApiController
    {
        // GET: api/MPE_Perf
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MPE_Perf/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MPE_Perf
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
                    Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(request_data, Formatting.None), "rpg_run_perf", ""));
                }
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        // PUT: api/MPE_Perf/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MPE_Perf/5
        public void Delete(int id)
        {
        }
    }
}
