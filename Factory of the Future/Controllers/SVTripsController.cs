using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class SVTripsController : ApiController
    {
        // GET: api/SVTrips
        public JObject Get()
        {
            return JObject.Parse(JsonConvert.SerializeObject(AppParameters.RouteTripsList.Select(x => x.Value).ToList(), Formatting.None));
        }

        // GET: api/SVTrips/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SVTrips
        public IHttpActionResult Post([FromBody] JToken request_data)
        {
            //handle bad requests
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (request_data != null)
            {
                //Send data to be processed.
                Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(request_data, Formatting.None), "trips", "")).ConfigureAwait(false);
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        //// PUT: api/SVTrips/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/SVTrips/5
        //public void Delete(int id)
        //{
        //}
    }
}
