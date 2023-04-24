using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class LocationTagsController : ApiController
    {
        // GET: api/LocationTags
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/LocationTags/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/LocationTags
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
                    await Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(request_data, Formatting.None), "getTagPosition", "")).ConfigureAwait(false);
                }
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        // PUT: api/LocationTags/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/LocationTags/5
        public void Delete(int id)
        {
        }
    }
}
