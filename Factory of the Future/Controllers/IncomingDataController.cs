using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class IncomingDataController : ApiController
    {
        // GET: api/IncomingData
        [AcceptVerbs("GET", "HEAD")]
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
                    Task.Run(() => new ProcessRecvdMsg().StartProcess(request_data,request_data["MESSAGE"].ToString(),""));
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