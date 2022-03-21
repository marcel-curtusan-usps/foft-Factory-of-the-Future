using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace Factory_of_the_Future.Controllers
{
    [RoutePrefix("api/tags")]
    public class TagsController : ApiController
    {
        //GET api/Tags
        [Route("")]
        public IEnumerable<JObject> GetTags()
        {
            return AppParameters.Tag.Select(x => x.Value).ToList();
        }

        //GET api/Tags/tag_id
        [Route("{id}")]
        [ResponseType(typeof(JObject))]
        public IEnumerable<JObject> GetTagbyID(string id)
        {
            return AppParameters.Tag.Where(r => (string)r.Value["properties"]["id"] == id).Select(x => x.Value).ToList();
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
        //// POST api/<controller>
        //public void Post([FromBody] string value)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(value))
        //        {
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogger().ExceptionLog(ex);
        //    }
        //}

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