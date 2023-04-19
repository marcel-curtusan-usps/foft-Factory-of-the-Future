using Factory_of_the_Future.Models;
using Newtonsoft.Json;
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
    public class MPEAlertsController : ApiController
    {
        // GET api/MPEAlerts
        //public IEnumerable<GPIOStatus> Get()
        //[Route("MPEAlerts")]
        public List<GPIOStatus> Get()
        {
            var MPEGpioValues = new List<GPIOStatus>();
            try
            {
                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => f.Value.Properties.ZoneType == "Machine" || f.Value.Properties.ZoneType == "MPEZone")
                        .Select(y => y.Value)
                        .ToList().ForEach(mpeList =>
                        {
                            MPEGpioValues.Add(new GPIOStatus
                            {
                                MachineId = mpeList.Properties.Name,
                                GpioStatus = mpeList.Properties.GpioValue
                            });
                        });
                }

                if (MPEGpioValues.Any())
                {
                    //return JsonConvert.SerializeObject(MPEGpioValues);
                    return MPEGpioValues;
                }
                else
                {
                    return MPEGpioValues;
                }

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return MPEGpioValues;
            }
            finally
            {
                MPEGpioValues = null;
            }
        }

        // GET: api/MPEalerts/uss-001
        [Route("MPEAlerts/{mpeName}")]
        public int Get(string mpeName)
        {
            var MPEGpioValues = new GPIOStatus();
            try
            {
                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                {
                     MPEGpioValues = cs.Zones.Where(f => (f.Value.Properties.ZoneType == "Machine" || f.Value.Properties.ZoneType == "MPEZone") && f.Value.Properties.Name == mpeName.ToUpper())
                        .Select(m => new GPIOStatus
                        {
                            MachineId = m.Value.Properties.Name,
                            GpioStatus = m.Value.Properties.GpioValue

                        }).FirstOrDefault();
                }

                return MPEGpioValues.GpioStatus;

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return MPEGpioValues.GpioStatus;
            }
            finally
            {
                MPEGpioValues = null;
            }
        }

        // POST api/<controller>
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
                    await Task.Run(() => new ProcessRecvdMsg().StartProcess(JsonConvert.SerializeObject(request_data, Formatting.None), "macro", "")).ConfigureAwait(false);
                }
            }
            else
            {
                return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}