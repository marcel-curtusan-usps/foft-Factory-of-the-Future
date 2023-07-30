using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class DeviceHeartbeatController : ApiController
    {
        // POST: api/DeviceScan
        public async Task<IHttpActionResult> PostAsync([FromBody] JObject request_data)
        {
            //handle bad requests
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //start data process
            if (request_data.HasValues)
            {
                await Task.Run(() => { new ConnectionDeviceStatus().Log(request_data, Request); }).ConfigureAwait(false);
                return Content(HttpStatusCode.OK, FOTFManager.Instance.DeviceScan(request_data, ""));
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, request_data);
            }
        }
    }
}
