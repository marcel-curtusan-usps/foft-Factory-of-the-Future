using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Factory_of_the_Future
{
    internal class MPEWatch_DPS : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        private bool saveToFile;
        private int time_to_comp_actual = 0;
        private DateTime dtNow = DateTime.Now; 
        internal async Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    JToken dpsInfo = tempData.SelectToken("data");
                    if (dpsInfo != null && dpsInfo.HasValues)
                    {
                        if (!string.IsNullOrEmpty((string)AppParameters.AppSettings["FACILITY_TIMEZONE"]))
                        {
                            if (AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings["FACILITY_TIMEZONE"], out string windowsTimeZoneId))
                            {
                                dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                            }
                        }
                        foreach (JObject item in dpsInfo.Children())
                        {
                            string strSortPlan = item.ContainsKey("sortplan_name_perf") ? item["sortplan_name_perf"].ToString().Trim() : "";
                            string[] strSortPlanList = strSortPlan.Split(',').Select(x => x.Trim()).ToArray();

                            //int.TryParse(item.ContainsKey("time_to_comp_optimal") ? item["time_to_comp_optimal"].ToString().Trim() : "0", out time_to_comp_optimal);
                            //DateTime dtCompOptimal = dtNow.AddSeconds(time_to_comp_optimal);
                            //time_to_comp_optimal_DateTime = dtCompOptimal.ToString("yyyy-MM-dd HH:mm:ss");
                            //item["time_to_comp_optimal_DateTime"] = time_to_comp_optimal_DateTime;

                            //int.TryParse(item.ContainsKey("time_to_comp_actual") ? item["time_to_comp_actual"].ToString().Trim() : "0", out time_to_comp_actual);
                            //DateTime dtCompActual = dtNow.AddSeconds(time_to_comp_actual);
                            //time_to_comp_actual_DateTime = dtCompActual.ToString("MM/dd/yyyy HH:mm:ss");
                            //item["time_to_comp_actual_DateTime"] = time_to_comp_actual_DateTime;
                            //for (int i = 0; i < strSortPlanList.Length; i++)
                            //{
                            //    string newDPS = JsonConvert.SerializeObject(item, Formatting.Indented);
                            //    AppParameters.DPSList.AddOrUpdate(strSortPlanList[i].Substring(0, 7), newDPS,
                            //        (key, oldValue) =>
                            //        {
                            //            return newDPS;
                            //        });

                            //}

                        }
                    }
            
                }
                return saveToFile;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MPEWatch_DPS()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}