using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class MPEPerfController : ApiController
    {
        // GET: api/MPE_Perf
        public IEnumerable<RunPerf> Get()
        {
            try
            {
                return AppParameters.MPEPerformance.Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        // GET: api/MPE_Perf/5
        public IEnumerable<RunPerf> Get(int id, string name)
        {

            try
            {
                return AppParameters.MPEPerformance.Where(f => f.Value.MpeNumber == id && f.Value.MpeId.ToString() == name).Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
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
                    _ = Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(request_data, Formatting.None), "rpg_run_perf", "")).ConfigureAwait(false);
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
