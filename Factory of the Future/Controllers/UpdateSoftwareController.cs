using Factory_of_the_Future.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    [Authorize]
    public class UpdateSoftwareController : ApiController
    {
        // POST: api/UpdateSoftware
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
                if (request_data.HasValues)
                {
                    if (!request_data.Contains("Version") && string.IsNullOrEmpty(request_data["Version"].ToString()))
                    {
                        return BadRequest(ModelState);
                    }
                    if (!request_data.Contains("AppName") && string.IsNullOrEmpty(request_data["AppName"].ToString()))
                    {
                        return BadRequest(ModelState);
                    }
                    if (!request_data.Contains("DrivePath") && string.IsNullOrEmpty(request_data["DrivePath"].ToString()))
                    {
                        return BadRequest(ModelState);
                    }
                    if (!request_data.Contains("UserName") && string.IsNullOrEmpty(request_data["UserName"].ToString()))
                    {
                        return BadRequest(ModelState);
                    }
                    if (!request_data.Contains("Token") && string.IsNullOrEmpty(request_data["Token"].ToString()))
                    {
                        return BadRequest(ModelState);
                    }
                    UpdateParam Param = request_data.ToObject<UpdateParam>();

                    _ = Task.Run(() => new UpdateSoftware().StartProcess(Param)).ConfigureAwait(false);
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
