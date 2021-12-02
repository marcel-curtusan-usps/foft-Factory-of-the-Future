using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    public class SourceDataProcess : IDisposable
    {

        private static int siteworkItemCount = 0;

        internal static void Start_processor(object state)
        {
            int siteworkItemNumber = siteworkItemCount;
            Interlocked.Increment(ref siteworkItemCount);
            string result = string.Empty;
            JObject requestBody = null;
            SourceData_Process Connection = (SourceData_Process)state;
            try
            {
                if (!string.IsNullOrEmpty(Connection.SERVER["NASS_CODE"].ToString()))
                {
                    try
                    {
                        Uri parURL = null;
                        DateTime dtNow = DateTime.Now;
                        if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value))
                        {
                            if (Global.TimeZoneConvert.TryGetValue((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                            {
                                dtNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));

                            }
                        }
                        string formatUrl = string.Empty;
                        if (Connection.SERVER.Property("CONNECTION_NAME").Value.ToString().ToUpper().StartsWith("MPEWatch".ToUpper()))
                        {
                            if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("MPE_WATCH_ID").Value))
                            {
                                string MpeWatch_id = (string)Global.AppSettings.Property("MPE_WATCH_ID").Value;
                                string MpeWatch_data_source = (string)Connection.SERVER.Property("MESSAGE_TYPE").Value;

                                int currentHour = dtNow.Hour;
                                DateTime modsDate = dtNow;
                                if (currentHour >= 0 && currentHour < 7)
                                {
                                    modsDate = dtNow.Date.AddDays(-1);
                                }
                                else
                                {
                                    modsDate = dtNow.Date;
                                }
                                string start_time = "";
                                string end_time = "";
                                switch (Connection.SERVER.Property("MESSAGE_TYPE").Value.ToString().ToUpper())
                                {
                                    case "RPG_PLAN":
                                        DateTime dtEnd = modsDate.AddDays(5);
                                        start_time = modsDate.ToString("MM/dd/yyyy_HH:mm:ss");
                                        end_time = dtEnd.ToString("MM/dd/yyyy_HH:mm:ss");
                                        formatUrl = string.Format((string)Connection.SERVER.Property("URL").Value, MpeWatch_id, MpeWatch_data_source, start_time, end_time);
                                        break;

                                    case "DPS_RUN_ESTM":
                                        DateTime modStart = dtNow.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
                                        DateTime modEnd = dtNow.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                                        start_time = modStart.ToString("MM/dd/yyyy HH:mm:ss");
                                        end_time = modEnd.ToString("MM/dd/yyyy HH:mm:ss");
                                        formatUrl = string.Format((string)Connection.SERVER.Property("URL").Value, MpeWatch_id, MpeWatch_data_source, start_time, end_time);
                                        break;

                                    case "RPG_RUN_PERF":
                                    default:
                                        string strTimeDiff = "-" + Connection.SERVER.Property("DATA_RETRIEVE").Value.ToString();
                                        Double dblTimeDiff = 0;
                                        if (Double.TryParse(strTimeDiff, out Double dblDiff)) { dblTimeDiff = dblDiff; }
                                        else { dblTimeDiff = -300000; }
                                        if (dblTimeDiff == 0) { dblTimeDiff = -300000; }
                                        DateTime endDate = dtNow;
                                        DateTime startDate = endDate.AddMilliseconds(dblTimeDiff);
                                        start_time = startDate.ToString("MM/dd/yyyy_HH:mm:ss");
                                        end_time = endDate.ToString("MM/dd/yyyy_HH:mm:ss");
                                        formatUrl = string.Format((string)Connection.SERVER.Property("URL").Value, MpeWatch_id, MpeWatch_data_source, start_time, end_time);
                                        break;
                                }
                            }
                            else
                            {
                                int index = ((string)Connection.SERVER.Property("URL").Value).IndexOf("ge.");
                                formatUrl = string.Concat(((string)Connection.SERVER.Property("URL").Value).Substring(0, (index + 3)), "get_id?group_name=client");
                            }
                        }
                        else if (Connection.SERVER.Property("CONNECTION_NAME").Value.ToString().ToUpper().StartsWith("SV".ToUpper()))
                        {
                            string start_time = string.Concat(DateTime.Now.AddHours(-4).ToString("yyyy-MM-dd'T'HH:"), "00:00");
                            string end_time = DateTime.Now.AddHours(+4).ToString("yyyy-MM-dd'T'HH:mm:ss");
                            formatUrl = string.Format((string)Connection.SERVER.Property("URL").Value, Connection.SERVER["NASS_CODE"], start_time, end_time);
                        }
                        else if (Connection.SERVER.Property("CONNECTION_NAME").Value.ToString().ToUpper().StartsWith("SELS".ToUpper()))
                        {
                            string data_source = (string)Connection.SERVER.Property("MESSAGE_TYPE").Value;
                            if (!string.IsNullOrEmpty(data_source))
                            {
                                string p2p_siteid = (string)Global.AppSettings.Property("FACILITY_P2P_SITEID").Value;
                                if (!string.IsNullOrEmpty(p2p_siteid))
                                {
                                    if (data_source.ToUpper().StartsWith("P2PBySite".ToUpper()))
                                    {
                                        requestBody = new JObject(new JProperty("siteId", p2p_siteid), new JProperty("operation", "bySite"));
                                    }
                                    formatUrl = string.Format((string)Connection.SERVER.Property("URL").Value, p2p_siteid, data_source);
                                }
                            }
                        }
                        else if (Connection.SERVER.Property("CONNECTION_NAME").Value.ToString().ToUpper().StartsWith("Quuppa".ToUpper()))
                        {
                            string data_source = (string)Connection.SERVER.Property("MESSAGE_TYPE").Value;
                            if (!string.IsNullOrEmpty(data_source))
                            {
                                formatUrl = string.Format((string)Connection.SERVER.Property("URL").Value, data_source);
                            }
                        }
                        else if (Connection.SERVER.Property("CONNECTION_NAME").Value.ToString().ToUpper().StartsWith("CTS".ToUpper()))
                        {
                            if (!string.IsNullOrEmpty(Connection.SERVER["NASS_CODE"].ToString()))
                            {
                                string key = Connection.SERVER["OUTGOING_APIKEY"].ToString();
                                if (!string.IsNullOrEmpty(key))
                                {
                                    string data_source = Connection.SERVER["MESSAGE_TYPE"].ToString();
                                    if (!string.IsNullOrEmpty(data_source))
                                    {
                                        formatUrl = string.Format(Connection.SERVER["URL"].ToString(), data_source, key, Connection.SERVER["NASS_CODE"]);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(formatUrl))
                        {
                            parURL = new Uri(formatUrl);

                            if (requestBody != null)
                            {
                                result = AsyncAPICall.GetDataPost(parURL, "POST", JsonConvert.SerializeObject(requestBody, Formatting.Indented));
                                //await webClient.UploadStringTaskAsync(parURL, "POST", JsonConvert.SerializeObject(requestBody, Formatting.Indented)).ConfigureAwait(false);
                            }
                            else
                            {
                                result = AsyncAPICall.GetData(parURL);
                                //await webClient.DownloadStringTaskAsync(parURL).ConfigureAwait(false);
                            }
                            if (!string.IsNullOrEmpty(result))
                            {
                                Global.API_List.Where(x => (int)x.Value.Property("ID").Value == (int)Connection.SERVER.Property("ID").Value).Select(y => y.Value).ToList().ForEach(m =>
                                {


                                    if (Global.IsValidJson(result))
                                    {
                                        if (result.StartsWith("{"))
                                        {
                                            JObject temp1 = JObject.Parse(result);
                                            if (formatUrl.Contains("api_page.get_id"))
                                            {
                                                temp1.Add(new JProperty("message", "mpe_watch_id"));
                                            }

                                            if (temp1.HasValues)
                                            {
                                                Global.ProcessRecvdMsg_callback.StartProcess(temp1, Connection.SERVER);
                                                m.Property("API_CONNECTED").Value = true;
                                            }
                                            else
                                            {
                                                m.Property("API_CONNECTED").Value = false;
                                            }

                                            temp1 = null;
                                        }
                                        else if (result.StartsWith("["))
                                        {
                                            JArray tempdata = JArray.Parse(result);
                                            if (tempdata.HasValues)
                                            {
                                                JObject temp1 = new JObject(new JProperty((string)Connection.SERVER.Property("MESSAGE_TYPE").Value, tempdata));
                                                Global.ProcessRecvdMsg_callback.StartProcess(temp1, Connection.SERVER);
                                                tempdata = null;
                                                temp1 = null;


                                                m.Property("API_CONNECTED").Value = true;

                                            }
                                            else
                                            {
                                                m.Property("API_CONNECTED").Value = false;
                                            }
                                        }
                                        m.Property("UPDATE_STATUS").Value = true;
                                        m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                    }

                                });
                            }
                        }
                        else
                        {
                            Global.API_List.Where(x => (int)x.Value.Property("ID").Value == (int)Connection.SERVER.Property("ID").Value).Select(y => y.Value).ToList().ForEach(m =>
                            {
                                if ((bool)m.Property("API_CONNECTED").Value)
                                {
                                    m.Property("API_CONNECTED").Value = false;
                                }
                                m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                                m.Property("UPDATE_STATUS").Value = true;
                            });
                        }
                    }
                    catch (WebException webError)
                    {
                        Global.API_List.Where(x => (int)x.Value.Property("ID").Value == (int)Connection.SERVER.Property("ID").Value).Select(y => y.Value).ToList().ForEach(m =>
                        {
                            if ((bool)m.Property("API_CONNECTED").Value)
                            {
                                m.Property("API_CONNECTED").Value = false;
                            }
                            m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                            m.Property("UPDATE_STATUS").Value = true;
                        });
                        //write to file
                        new ErrorLogger().ExceptionLog(webError);
                    }
                    catch (Exception ex)
                    {
                        Global.API_List.Where(x => (int)x.Value.Property("ID").Value == (int)Connection.SERVER.Property("ID").Value).Select(y => y.Value).ToList().ForEach(m =>
                        {
                            if ((bool)m.Property("API_CONNECTED").Value)
                            {
                                m.Property("API_CONNECTED").Value = false;
                            }
                            m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                            m.Property("UPDATE_STATUS").Value = true;
                        });

                        new ErrorLogger().ExceptionLog(ex);
                    }

                }
                else
                {
                    Global.API_List.Where(x => (int)x.Value.Property("ID").Value == (int)Connection.SERVER.Property("ID").Value).Select(y => y.Value).ToList().ForEach(m =>
                    {
                        if ((bool)m.Property("API_CONNECTED").Value)
                        {
                            m.Property("API_CONNECTED").Value = false;
                        }
                        m.Property("LASTTIME_API_CONNECTED").Value = DateTime.Now;
                        m.Property("UPDATE_STATUS").Value = true;
                    });
                }
                
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                result = string.Empty;
                Connection.manualResetEvent.Set();
            }
           
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}