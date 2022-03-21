using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace Factory_of_the_Future.Controllers
{
    public class TimeZoneController : ApiController
    {
        // GET: api/TimeZone
        public IEnumerable<string> Get()
        {
            return AppParameters.TimeZoneConvert.Keys.ToList();
        }
    }
}
