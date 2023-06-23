using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Factory_of_the_Future.Controllers
{
    public class SVContainersController : ApiController
    {
        // GET: api/SVContainers

        [ResponseType(typeof(Container))]
        public async Task<IHttpActionResult>  Get(string dest,bool hasLoadScans, bool containerTerminat, bool containerAtDest, bool hasCloseScans )
        {
            //handle bad requests
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //start data process
            if (!string.IsNullOrEmpty(dest))
            {
                return Content(HttpStatusCode.OK, await Task.Run(() => {

                    return AppParameters.Containers.Where(r => Regex.IsMatch(r.Value.Dest, dest, RegexOptions.IgnoreCase)
                    && r.Value.hasLoadScans == hasLoadScans
                    && r.Value.containerTerminate == containerTerminat
                    && r.Value.containerAtDest == containerAtDest
                    && r.Value.hasCloseScans == hasCloseScans
                    ).Select(y => y.Value).ToList();
                }).ConfigureAwait(true));
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, dest);
            }

        }

        [ResponseType(typeof(Container))]
        public async Task<IHttpActionResult> GetTrip(string dest, int trip)
        {
            //handle bad requests
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //start data process
            if (!string.IsNullOrEmpty(dest))
            {
                return Content(HttpStatusCode.OK, await Task.Run(() => {

                    return AppParameters.Containers.Where(r => Regex.IsMatch(r.Value.Dest, dest, RegexOptions.IgnoreCase)

                    ).Select(y => y.Value).ToList();
                }).ConfigureAwait(true));
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, dest);
            }

        }


        // POST: api/SVContainers
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
                    Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(request_data, Formatting.None), "container", "")).ConfigureAwait(false);
                }
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        // PUT: api/SVContainers/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/SVContainers/5
        public void Delete(int id)
        {
        }
    }
}
