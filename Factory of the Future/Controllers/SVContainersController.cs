using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Factory_of_the_Future.Controllers
{
    public class SVContainersController : ApiController
    {
        // GET: api/SVContainers

        [ResponseType(typeof(Container))]
        public IEnumerable<Container> Get([FromBody] JObject trailer)
        {
            //if (trailer.HasValues)
            //{
            //    return AppParameters.Containers.Where(r => r.Value.Otrailer == (string)trailer["trailerBarcode"]

            //                                                   ).Select(y => y.Value).ToList();
            //}
            return null;

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
