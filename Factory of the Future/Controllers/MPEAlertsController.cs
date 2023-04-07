using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class MPEAlertsController : ApiController
    {
        // GET: api/MPEAlerts
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MPEAlerts/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MPEAlerts
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/MPEAlerts/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MPEAlerts/5
        public void Delete(int id)
        {
        }
    }
}
