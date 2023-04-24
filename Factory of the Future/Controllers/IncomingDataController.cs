using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Factory_of_the_Future.Controllers
{
    public class IncomingDataController : ApiController, IDisposable
    {
        // GET: api/IncomingData
        [AcceptVerbs("GET", "HEAD")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/IncomingData/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/IncomingData
        public IHttpActionResult Post([FromBody] JToken request_data)
        {
            //handle bad requests
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Connection connection = AppParameters.RunningConnection.Connection.Where(f => f.ConnectionInfo.ConnectionName.StartsWith("AGVM")).Select(y => y.ConnectionInfo).FirstOrDefault();
            if (connection != null)
            {
                connection.ApiConnected = true;
                connection.ActiveConnection = true;
                connection.Status = "Running";
                ToProcesser(request_data, connection.Id).ConfigureAwait(false);
                FOTFManager.Instance.BroadcastQSMUpdate(connection);
            }
            else
            {
                connection = CreateConnection();
                if (connection != null)
                {
                    connection.ApiConnected = true;
                    connection.ActiveConnection = true;
                    connection.Status = "Running";
                    ToProcesser(request_data, connection.Id).ConfigureAwait(false);
                    FOTFManager.Instance.BroadcastQSMUpdate(connection);
                }
                else
                {
                    return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
                }

            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        private Connection CreateConnection()
        {

            try
            {
                Connection con = new Connection()
                {
                    ActiveConnection = true,
                    ConnectionName = "AGVM",
                    ApiConnection = true,
                    Status = "Running",
                    IpAddress = "127.0.0.1",
                    CreatedDate = DateTime.Now,
                    DataRetrieve = 0,
                    Url = "http://" + AppParameters.ServerIpAddress + "/api/IncomingData",
                    Port = 80,
                    MessageType = "data_listener",
                    LasttimeApiConnected = DateTime.Now,
                };
                FOTFManager.Instance.AddAPI(JsonConvert.SerializeObject(con, Formatting.Indented));
                return con;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        private async Task ToProcesser(JToken request_data, string connectionID)
        {
            try
            {
                if (request_data != null)
                {
                    //start data process
                    if (request_data.HasValues)
                    {
                        //Send data to be processed.
                        var requestDataToString = JsonConvert.SerializeObject(request_data, Formatting.Indented);
                        await Task.Run(() => new ProcessRecvdMsg().StartProcess(requestDataToString, request_data["MESSAGE"].ToString(), connectionID)).ConfigureAwait(false);
                        //Task.Run(() => new ProcessRecvdMsg().StartProcess(request_data, request_data["MESSAGE"].ToString(), connectionID));
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                Dispose();
            }
        }

        // PUT: api/IncomingData/5
        public void Put(int id, [FromBody] string value)
        {
        }
    }
}