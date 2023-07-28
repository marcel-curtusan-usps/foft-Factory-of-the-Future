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
    [RoutePrefix("api/employeeschedule")]
    public class EmployeeScheduleController : ApiController
    {
        // GET: api/EmployeeSchedule
       
        [Route("")]
        [ResponseType(typeof(JObject))]
        public IEnumerable<JObject> Get()
        {
            IEnumerable<JObject> scheduleEmployee = new List<JObject>();
            foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
            {
                IEnumerable<JObject> scheduleEmployeetemp = new List<JObject>();
                var  query = cs.Locators.Where(sl => sl.Value.Properties.TagType == "Person").Select(r => r.Value.Properties).ToList();
                scheduleEmployeetemp = from se in query
                                   select new JObject { 
                                       ["tagId"] = se.Id,
                                       ["empId"] = se.EmpId,
                                       ["tagType"] = se.TagType,
                                       ["bdate"] = se.Bdate,
                                       ["blunch"] = se.Blunch,
                                       ["elunch"] = se.Elunch,
                                       ["edate"] = se.Edate,
                                       ["reqDate"] = se.ReqDate,
                                       ["emptype"] = se.Emptype,
                                       ["badgeId"] = se.BadgeId,
                                       ["empName"] = se.EmpName,
                                       ["craftName"] = se.CraftName,
                                       ["isSch"] = se.isSch,
                                       ["isTacs"] = se.isTacs,
                                       ["isePacs"] = se.isePacs,
                                       ["isPosition"] = se.isPosition
                                   };
                scheduleEmployee.Concat(scheduleEmployeetemp);
            }
            return scheduleEmployee;
        }

        // GET: api/EmployeeSchedule/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/EmployeeSchedule
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/EmployeeSchedule/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/EmployeeSchedule/5
        public void Delete(int id)
        {
        }
    }
}
