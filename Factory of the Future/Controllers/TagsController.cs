using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Factory_of_the_Future.Controllers
{
    [RoutePrefix("api/tags")]
    public class TagsController : ApiController
    {
        //GET api/Tags
        [Route("")]
        public IEnumerable<Marker> GetTags()
        {
            IEnumerable<Marker> query = new List<Marker>();
            foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
            {
                query = cs.Locators.Where(sl => sl.Value.Properties.TagType == "Person").Select(r => r.Value.Properties).ToList();
            }
            return query;
        }

        //GET api/Tags/tag_id
        [Route("{id}")]
        [ResponseType(typeof(JObject))]
        public IEnumerable<JObject> GetTagbyID(string id)
        {
            return null; //AppParameters.Tag.Where(r => (string)r.Value["properties"]["id"] == id).Select(x => x.Value).ToList();
        }

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}
        // POST api/<controller>
        [ResponseType(typeof(JObject))]
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
                    await Task.Run(() => new ProcessRecvdMsg().StartProcess(request_data, "getTagPosition", "")).ConfigureAwait(false);
                }
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}
    }
}