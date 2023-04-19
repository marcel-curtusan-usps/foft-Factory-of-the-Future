using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Factory_of_the_Future.Models;
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
        public JToken tempData = null;
        public JToken dpsInfo = null;
        private DateTime dtNow = DateTime.Now;
        public DeliveryPointSequence currentDPS_Info = null;
        public List<DeliveryPointSequence> NewMPEDPS = null;
        internal Task LoadAsync(dynamic data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (_data != null)
                {
                    tempData = JToken.Parse(_data);
                    dpsInfo = tempData.SelectToken("data");
                    if (dpsInfo != null && dpsInfo.HasValues)
                    {
                     
                        NewMPEDPS = GetMPEDPSList(dpsInfo);
                        if (NewMPEDPS != null)
                        {
                            foreach (DeliveryPointSequence dpsInfo in NewMPEDPS)
                            {
                                string[] strSortPlanList = dpsInfo.SortplanNamePerf.Split(',').Select(x => x.Trim()).ToArray();
                                for (int i = 0; i < strSortPlanList.Length; i++)
                                {
                                    if (!AppParameters.DPSList.ContainsKey(strSortPlanList[i]) && AppParameters.DPSList.TryAdd(strSortPlanList[i], dpsInfo))
                                    {
                                        //add
                                    }
                                    else
                                    {
                                        //update 
                                        if (AppParameters.DPSList.TryGetValue(strSortPlanList[i], out currentDPS_Info))
                                        {
                                            foreach (PropertyInfo prop in dpsInfo.GetType().GetProperties())
                                            {
                                                if (prop.GetValue(dpsInfo, null).ToString() != prop.GetValue(currentDPS_Info, null).ToString())
                                                {
                                                    prop.SetValue(currentDPS_Info, prop.GetValue(dpsInfo, null));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //foreach (JObject item in dpsInfo.Children())
                        //{
                        //    string strSortPlan = item.ContainsKey("sortplan_name_perf") ? item["sortplan_name_perf"].ToString().Trim() : "";
                        //    string[] strSortPlanList = strSortPlan.Split(',').Select(x => x.Trim()).ToArray();

                        //    //int.TryParse(item.ContainsKey("time_to_comp_optimal") ? item["time_to_comp_optimal"].ToString().Trim() : "0", out time_to_comp_optimal);
                        //    //DateTime dtCompOptimal = dtNow.AddSeconds(time_to_comp_optimal);
                        //    //time_to_comp_optimal_DateTime = dtCompOptimal.ToString("yyyy-MM-dd HH:mm:ss");
                        //    //item["time_to_comp_optimal_DateTime"] = time_to_comp_optimal_DateTime;

                        //    //int.TryParse(item.ContainsKey("time_to_comp_actual") ? item["time_to_comp_actual"].ToString().Trim() : "0", out time_to_comp_actual);
                        //    //DateTime dtCompActual = dtNow.AddSeconds(time_to_comp_actual);
                        //    //time_to_comp_actual_DateTime = dtCompActual.ToString("MM/dd/yyyy HH:mm:ss");
                        //    //item["time_to_comp_actual_DateTime"] = time_to_comp_actual_DateTime;
                        //    //for (int i = 0; i < strSortPlanList.Length; i++)
                        //    //{
                        //    //    string newDPS = JsonConvert.SerializeObject(item, Formatting.Indented);
                        //    //    AppParameters.DPSList.AddOrUpdate(strSortPlanList[i].Substring(0, 7), newDPS,
                        //    //        (key, oldValue) =>
                        //    //        {
                        //    //            return newDPS;
                        //    //        });

                        //    //}

                        //}
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

            return Task.CompletedTask;
        }

        private List<DeliveryPointSequence> GetMPEDPSList(JToken dpsInfo)
        {
            List<DeliveryPointSequence> MPEDPSData = new List<DeliveryPointSequence>();
            try
            {
                foreach (JObject item in dpsInfo.Children())
                {
                    MPEDPSData.Add(new DeliveryPointSequence
                    {
                        RunStartModsday = item["run_start_modsday"].ToString() == "0" ? DateTime.MinValue : DateTime.Parse(item["run_start_modsday"].ToString()),//(DateTime)item["run_start_modsday"],
                        SortplanNamePerf = item["sortplan_name_perf"].ToString(),
                        CurrentOperationId = Convert.ToInt32(item["current_operation_id"].ToString()),
                        PiecesFed1stCnt = Convert.ToInt32(item["pieces_fed_1st_cnt"].ToString()),
                        PiecesRejected1stCnt = Convert.ToInt32(item["pieces_rejected_1st_cnt"].ToString()),
                        PiecesTo2ndPass = Convert.ToInt32(item["pieces_to_2nd_pass"].ToString()),
                        OpTime1st = Convert.ToInt32(item["op_time_1st"].ToString()),
                        Thruput1stPass = Convert.ToInt32(item["thruput_1st_pass"].ToString()),
                        PiecesFed2ndCnt = Convert.ToInt32(item["pieces_fed_2nd_cnt"].ToString()),
                        PiecesRejected2ndCnt = Convert.ToInt32(item["pieces_rejected_2nd_cnt"].ToString()),
                        OpTime2nd = Convert.ToInt32(item["op_time_2nd"].ToString()),
                        Thruput2ndPass = Convert.ToInt32(item["thruput_2nd_pass"].ToString()),
                        PiecesRemaining = Convert.ToInt32(item["pieces_remaining"].ToString()),
                        ThruputOptimalCfg = Convert.ToInt32(item["thruput_optimal_cfg"].ToString()),
                        TimeToCompOptimal = Convert.ToInt32(item["time_to_comp_optimal"].ToString()),
                        ThruputActual = Convert.ToInt32(item["thruput_actual"].ToString()),
                        TimeToCompActual = Convert.ToInt32(item["time_to_comp_actual"].ToString()),
                        Rpg2ndPassEnd = item["rpg_2nd_pass_end"].ToString() == "0" ? DateTime.MinValue : DateTime.Parse(item["rpg_2nd_pass_end"].ToString()) ,// (DateTime)item["rpg_2nd_pass_end"],
                        TimeTo2ndPassOptimal = Convert.ToInt32(item["time_to_2nd_pass_optimal"].ToString()),
                        Rec2ndPassStartOptimal = item["rec_2nd_pass_start_optimal"].ToString() == "0" ? DateTime.MinValue : DateTime.Parse(item["rec_2nd_pass_start_optimal"].ToString()),// (DateTime)item["rec_2nd_pass_start_optimal"],
                        TimeTo2ndPassActual = Convert.ToInt32(item["time_to_2nd_pass_actual"].ToString()),
                        Rec2ndPassStartActual = item["rec_2nd_pass_start_actual"].ToString() == "0" ? DateTime.MinValue : DateTime.Parse(item["rec_2nd_pass_start_actual"].ToString()),//(DateTime)item["rec_2nd_pass_start_actual"]
                    });

                }
                return MPEDPSData;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
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
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
                tempData = null;
                NewMPEDPS = null;
                dpsInfo = null;
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