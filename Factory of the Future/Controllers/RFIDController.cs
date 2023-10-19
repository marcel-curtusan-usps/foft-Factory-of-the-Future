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
    public class RFIDController : ApiController, IDisposable
    {
        // GET: api/RFID
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/RFID/5
        public string Get(int id)
        {
            return "value";
        }

        public IHttpActionResult Post([FromBody] JToken request_data)
        {
            //handle bad requests
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Connection connection = AppParameters.RunningConnection.DataConnection.Where(f => f.ConnectionInfo.ConnectionName.StartsWith("RFID")).Select(y => y.ConnectionInfo).FirstOrDefault();
            if (connection != null)
            {
                connection.ApiConnected = true;
                connection.ActiveConnection = true;
                connection.Status = "Running";
                _ = ToProcesser(request_data, connection).ConfigureAwait(false);
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
                    _ = ToProcesser(request_data, connection).ConfigureAwait(false);
                    FOTFManager.Instance.BroadcastQSMUpdate(connection);
                }
                else
                {
                    return CreatedAtRoute("DefaultApi", new { message = "Invalid Data in the Request." }, 0);
                }

            }
            return CreatedAtRoute("DefaultApi", new { id = "0" }, 0);
        }

        private async Task ToProcesser(JToken request_data, Connection conn)
        {
            try
            {
                if (request_data != null)
                {
                    //start data process
                    if (request_data.HasValues)
                    {
                        //Send data to be processed.
                        await Task.Run(() => new ProcessRecvdMsg().StartProcess(request_data, conn.MessageType, conn.Id)).ConfigureAwait(false);
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

        private Connection CreateConnection()
        {

            try
            {
                Connection con = new Connection()
                {
                    ActiveConnection = true,
                    ConnectionName = "RFID",
                    ApiConnection = true,
                    Status = "Running",
                    IpAddress = "127.0.0.1",
                    CreatedDate = DateTime.Now,
                    DataRetrieve = 0,
                    Url = "http://" + AppParameters.ServerIpAddress + "/api/RFID",
                    Port = 80,
                    MessageType = "data_listener",
                    LasttimeApiConnected = DateTime.Now,
                };
                _ = FOTFManager.Instance.AddAPI(JsonConvert.SerializeObject(con, Formatting.Indented));
                return con;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
    }
}
