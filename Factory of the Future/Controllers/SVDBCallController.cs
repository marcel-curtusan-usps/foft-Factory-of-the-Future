﻿using Newtonsoft.Json;
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
    public class SVDBCallController : ApiController
    {
        // GET: api/SVDBCall
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SVDBCall/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SVDBCall
        public JObject Post([FromBody] JObject request_data)
        {
            
            if (request_data != null)
            {
                //start data process
                if (request_data.HasValues)
                {
                    Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(request_data, Formatting.None), "dps_run_estm", "")).ConfigureAwait(false);
                    return new JObject();
                   // return SV_DB_Call.GetData(request_data);
                }
                else
                {
                    return new JObject();
                }
            }
            else
            {
                return new JObject();
            }
        }

        // PUT: api/SVDBCall/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SVDBCall/5
        public void Delete(int id)
        {
        }
    }
}
