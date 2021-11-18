using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            if (trailer.HasValues)
            {
                return Global.Containers.Where(r => r.Value.Otrailer == (string)trailer.Property("trailerBarcode").Value
                                                             
                                                               ).Select(y => y.Value).ToList();
            }
            return null;
            
        }
        // POST: api/SVContainers
        public IHttpActionResult Post([FromBody] JArray request_data)
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
           
                    JObject temp1 = new JObject(new JProperty("container", request_data));
                    //create new Connection Object
                    JObject conn = new JObject(new JProperty("MESSAGE_TYPE", "container"));
                    //Send data to be processed.
                    Global.ProcessRecvdMsg_callback.StartProcess(temp1, conn);
                    temp1 = null;
                    conn = null;
                }
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        // PUT: api/SVContainers/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SVContainers/5
        public void Delete(int id)
        {
        }
    }
}
