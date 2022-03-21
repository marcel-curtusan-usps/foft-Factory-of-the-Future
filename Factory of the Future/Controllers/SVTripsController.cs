using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class SVTripsController : ApiController
    {
        // GET: api/SVTrips
        public IEnumerable<JObject> Get()
        {
            return AppParameters.RouteTripsList.Select(x => x.Value).ToList();
        }

        // GET: api/SVTrips/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SVTrips
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/SVTrips/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SVTrips/5
        public void Delete(int id)
        {
        }
    }
}
