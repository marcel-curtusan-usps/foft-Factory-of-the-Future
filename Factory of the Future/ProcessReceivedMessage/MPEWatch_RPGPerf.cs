using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class MPEWatch_RPGPerf : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public List<RunPerf> NewMPEData = null;
        public RunPerf currentMachine_Info = null;
        public JToken tempData = null;
        public JToken machineInfo = null;
        private bool saveToFile;
        private double intMinuteToCompletion = 0.0;

        internal async Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                string total_volume = "";
                string estCompletionTime = "";
                if (_data != null)
                {
                    tempData = JToken.Parse(_data);
                    machineInfo = tempData.SelectToken("data");
                    if (machineInfo != null && machineInfo.HasValues)
                    {
                        DateTime dtNow = DateTime.Now;
                        string windowsTimeZoneId = "";
                        if (!string.IsNullOrEmpty(AppParameters.AppSettings.FACILITY_TIMEZONE))
                        {
                            AppParameters.TimeZoneConvert.TryGetValue(AppParameters.AppSettings.FACILITY_TIMEZONE, out windowsTimeZoneId);
                        }

                        NewMPEData = GetMPEPerfList(machineInfo);
                        if (NewMPEData.Any())
                        {
                            foreach (RunPerf NewMachineInfo in NewMPEData)
                            {
                                //full bin check 
                                await Task.Run(() => new MPEFullBin().LoadAsync(NewMachineInfo.MpeType, NewMachineInfo.MpeNumber, NewMachineInfo.BinFullBins)).ConfigureAwait(false);

                                //if (NewMachineInfo.RpgEstVol > 0 && NewMachineInfo.CurThruputOphr > 0)
                                //{
                                //    if (!string.IsNullOrEmpty(windowsTimeZoneId))
                                //    {
                                //        dtNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                                //        intMinuteToCompletion = (NewMachineInfo.RpgEstVol - NewMachineInfo.TotSortplanVol) / (NewMachineInfo.CurThruputOphr / 60);
                                //        NewMachineInfo.RpgEstCompTime = dtNow.AddMinutes(intMinuteToCompletion);
                                //    }
                                //}
                                NewMachineInfo.ExpectedThroughput = parseExpectedThruput(NewMachineInfo.RpgExpectedThruput);
                                RPGPlan rpgPlan = Get_RPGPlan_Info(NewMachineInfo);
                                if (rpgPlan != null)
                                {
                                    //                item["rpg_start_dtm"] = results.ContainsKey("rpg_start_dtm") ? results["rpg_start_dtm"].ToString().Trim() : "";
                                    //                item["rpg_end_dtm"] = results.ContainsKey("rpg_end_dtm") ? results["rpg_end_dtm"].ToString().Trim() : "";
                                    //                item["expected_throughput"] = results.ContainsKey("expected_throughput") ? results["expected_throughput"].ToString().Trim() : "";
                                    NewMachineInfo.RPGStartDtm = rpgPlan.RpgStartDtm;
                                    NewMachineInfo.RPGEndDtm = rpgPlan.RpgEndDtm;
                                    NewMachineInfo.ExpectedThroughput = Convert.ToInt32(rpgPlan.RpgExpectedThruput);
                                }
                                if (AppParameters.MPEPerformance.ContainsKey(NewMachineInfo.MpeId) && AppParameters.MPEPerformance.TryGetValue(NewMachineInfo.MpeId, out currentMachine_Info))
                                {
                                    bool update = false;
                                    foreach (PropertyInfo prop in NewMachineInfo.GetType().GetProperties())
                                    {

                                        if (!new Regex("^(MpeNumber|MpeType|MpeId)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                        {
                                            if (prop.GetValue(NewMachineInfo, null).ToString() != prop.GetValue(currentMachine_Info, null).ToString())
                                            {
                                                update = true;
                                                prop.SetValue(currentMachine_Info, prop.GetValue(NewMachineInfo, null));

                                            }
                                        }
                                    }
                                    if (update)
                                    {
                                        await Task.Run(() => FOTFManager.Instance.UpdateMpeData(currentMachine_Info.MpeId)).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    if (AppParameters.MPEPerformance.TryAdd(NewMachineInfo.MpeId, NewMachineInfo))
                                    {
                                        await Task.Run(() => FOTFManager.Instance.UpdateMpeData(NewMachineInfo.MpeId)).ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                        //foreach (JObject item in machineInfo.Children())
                        //{

                        //    item["cur_sortplan"] = AppParameters.SortPlan_Name_Trimer(item["cur_sortplan"].ToString());
                        //    item["cur_operation_id"] = !string.IsNullOrEmpty(item["cur_operation_id"].ToString()) ? item["cur_operation_id"].ToString() : "0";
                        //    item["rpg_start_dtm"] = "";
                        //    item["rpg_end_dtm"] = "";
                        //    item["expected_throughput"] = "0";
                        //    MPEWatch_FullBins(item);

                        //    total_volume = item.ContainsKey("tot_sortplan_vol") ? item["tot_sortplan_vol"].ToString().Trim() : "0";
                        //    int.TryParse(item.ContainsKey("rpg_est_vol") ? item["rpg_est_vol"].ToString().Trim() : "0", out int rpg_volume);
                        //    double.TryParse(item.ContainsKey("rpg_est_vol") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out double throughput);
                        //    int.TryParse(item.ContainsKey("rpg_est_vol") ? item["tot_sortplan_vol"].ToString().Trim() : "0", out int piecesfed);

                        //    if (rpg_volume > 0 && throughput > 0)
                        //    {
                        //        if (!string.IsNullOrEmpty(windowsTimeZoneId))
                        //        {
                        //            dtNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                        //        }
                        //        double intMinuteToCompletion = (rpg_volume - piecesfed) / (throughput / 60);
                        //        DateTime dtEstCompletion = dtNow.AddMinutes(intMinuteToCompletion);
                        //        estCompletionTime = dtEstCompletion.ToString("yyyy-MM-dd HH:mm:ss");
                        //        item["rpg_est_comp_time"] = estCompletionTime;
                        //    }
                        //    else
                        //    {
                        //        item["rpg_est_comp_time"] = "";
                        //    }
                        //    if (item.ContainsKey("rpg_expected_thruput"))
                        //    {
                        //        item["expected_throughput"] = !string.IsNullOrEmpty(item["rpg_expected_thruput"].ToString()) ? item["rpg_expected_thruput"].ToString().Split(' ').FirstOrDefault() : "0";
                        //        if (!string.IsNullOrEmpty(item["expected_throughput"].ToString()) && item["expected_throughput"].ToString() != "0")
                        //        {
                        //            int.TryParse(item.ContainsKey("cur_thruput_ophr") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out int cur_thruput);
                        //            int.TryParse(item.ContainsKey("expected_throughput") ? item["expected_throughput"].ToString().Trim() : "0", out int expected_throughput);
                        //            double thrper = (double)cur_thruput / (double)expected_throughput * 100;
                        //            string throughputState = "1";
                        //            if (item["current_run_end"].ToString() != "" && item["current_run_end"].ToString() != "0")
                        //            {
                        //                throughputState = "0";
                        //            }
                        //            else if (thrper >= 100)
                        //            {
                        //                throughputState = "1";
                        //            }
                        //            else if (thrper >= 90)
                        //            {
                        //                throughputState = "2";
                        //            }
                        //            else if (thrper < 90)
                        //            {
                        //                throughputState = "3";
                        //            }
                        //            item["throughput_status"] = throughputState;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if ((item["current_run_end"].ToString() == "" || item["current_run_end"].ToString() == "0") && item["current_run_start"].ToString() != "")
                        //        {
                        //            // JObject results = new Oracle_DB_Calls().Get_RPG_Plan_Info(item);
                        //            JObject results = Get_RPG_Plan_Info(item);
                        //            if (results != null && results.HasValues)
                        //            {
                        //                item["rpg_start_dtm"] = results.ContainsKey("rpg_start_dtm") ? results["rpg_start_dtm"].ToString().Trim() : "";
                        //                item["rpg_end_dtm"] = results.ContainsKey("rpg_end_dtm") ? results["rpg_end_dtm"].ToString().Trim() : "";
                        //                item["expected_throughput"] = results.ContainsKey("expected_throughput") ? results["expected_throughput"].ToString().Trim() : "";
                        //                //item["throughput_status"] = "1";
                        //                if (!string.IsNullOrEmpty(item["expected_throughput"].ToString()) && item["expected_throughput"].ToString() != "0")
                        //                {
                        //                    int.TryParse(item.ContainsKey("cur_thruput_ophr") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out int cur_thruput);
                        //                    int.TryParse(item.ContainsKey("expected_throughput") ? item["expected_throughput"].ToString().Trim() : "0", out int expected_throughput);
                        //                    double thrper = (double)cur_thruput / (double)expected_throughput * 100;
                        //                    string throughputState = "1";
                        //                    if (thrper >= 100)
                        //                    {
                        //                        throughputState = "1";
                        //                    }
                        //                    else if (thrper >= 90)
                        //                    {
                        //                        throughputState = "2";
                        //                    }
                        //                    else if (thrper < 90)
                        //                    {
                        //                        throughputState = "3";
                        //                    }
                        //                    item["throughput_status"] = throughputState;
                        //                }
                        //            }
                        //        }
                        //    }

                        //    //CheckMachineNotifications(item);
                        //    string MpeName = string.Concat(item["mpe_type"].ToString().Trim(), "-", item["mpe_number"].ToString().PadLeft(3, '0'));
                        //    string newMPEPerf = JsonConvert.SerializeObject(item, Formatting.Indented);
                        //    //AppParameters.MPEPerformanceList.AddOrUpdate(MpeName, newMPEPerf,
                        //    //      (key, oldValue) =>
                        //    //      {
                        //    //          return newMPEPerf;
                        //    //      });

                        //    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                        //    {
                        //        cs.Zones.Where(f => f.Value.Properties.ZoneType == "Machine" &&
                        //        f.Value.Properties.Name == MpeName).Select(y => y.Value).ToList().ForEach(existingVal =>
                        //        {
                        //            string temp = JsonConvert.SerializeObject(existingVal.Properties.MPEWatchData, Formatting.None);
                        //            string tempItem = JsonConvert.SerializeObject(item, Formatting.None);
                        //            if (temp != tempItem)
                        //            {
                        //                existingVal.Properties.ZoneUpdate = true;
                        //            }
                        //        });
                        //    }
                        //    //AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Machine" && r.Value.Properties.Name == MpeName).Select(y => y.Key).ToList().ForEach(key =>
                        //    //{
                        //    //    if (AppParameters.ZoneList.TryGetValue(key, out GeoZone machineZone))
                        //    //    {
                        //    //        //convert to string
                        //    //        string temp = JsonConvert.SerializeObject(machineZone.Properties.MPEWatchData, Formatting.None);
                        //    //        string tempItem = JsonConvert.SerializeObject(item, Formatting.None);
                        //    //        if (temp != tempItem)
                        //    //        {
                        //    //            machineZone.Properties.ZoneUpdate = true;
                        //    //        }

                        //    //    }
                        //    //});

                        //}
                    }

                }
                return saveToFile;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return saveToFile;
            }
            finally
            {
                Dispose();
            }
        }

        private RPGPlan Get_RPGPlan_Info(RunPerf machine_Info)
        {
            RPGPlan tempRPG = new RPGPlan();
            try
            {
                if (AppParameters.MPEPRPGList.Any())
                {
                    tempRPG = AppParameters.MPEPRPGList.Where(x => x.Value.MpeType == machine_Info.MpeType &&
                              x.Value.MachineNum == machine_Info.MpeNumber &&
                              x.Value.SortProgramName == machine_Info.CurSortplan &&
                              Convert.ToInt32((x.Value.MailOperationNbr).ToString().PadRight(6, '0')) == Convert.ToInt32(machine_Info.CurOperationId.ToString().PadRight(6, '0')) &&
                              (DateTime.Now >= x.Value.RpgStartDtm.AddMinutes(-15) && DateTime.Now <= x.Value.RpgEndDtm.AddMinutes(+15))
                              ).Select(l => l.Value).FirstOrDefault();

                    if (tempRPG != null)
                    {
                        return tempRPG;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        private int parseExpectedThruput(string rpgExpectedThruput)
        {
            try
            {
                int ExpectedThruput = 0;
                if (!string.IsNullOrEmpty(rpgExpectedThruput))
                {
                    int.TryParse(rpgExpectedThruput.Split(' ').FirstOrDefault(), out ExpectedThruput);
                }
                return ExpectedThruput;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return 0;
            }
        }

        private List<RunPerf> GetMPEPerfList(JToken machineInfo)
        {
            List<RunPerf> MPErunPerfData = new List<RunPerf>();
            try
            {
                foreach (JObject item in machineInfo.Children())
                {
                    //if the MPE is not running then reset the time.
                    if (item["cur_sortplan"].ToString() == "0")
                    {
                        item["current_run_start"] = "0001-01-01 00:00:00";
                        item["current_run_end"] = "0001-01-01 00:00:00";
                    }
                    MPErunPerfData.Add(new RunPerf
                    {
                        MpeType = item["mpe_type"].ToString(),
                        MpeNumber = !string.IsNullOrEmpty(item["mpe_number"].ToString()) ? Convert.ToInt32(item["mpe_number"].ToString()) : 0,
                        Bins = !string.IsNullOrEmpty(item["bins"].ToString()) ? Convert.ToInt32(item["bins"].ToString()) : 0,
                        CurSortplan = new Utility().SortPlan_Name_Trimer(item["cur_sortplan"].ToString()),
                        CurThruputOphr = !string.IsNullOrEmpty(item["cur_thruput_ophr"].ToString()) ? Convert.ToInt32(item["cur_thruput_ophr"].ToString()) : 0,
                        TotSortplanVol = !string.IsNullOrEmpty(item["tot_sortplan_vol"].ToString()) ? Convert.ToInt32(item["tot_sortplan_vol"].ToString()) : 0,
                        RpgEstVol = !string.IsNullOrEmpty(item["rpg_est_vol"].ToString()) ? Convert.ToInt32(item["rpg_est_vol"].ToString()) : 0,
                        ActVolPlanVolNbr = !string.IsNullOrEmpty(item["act_vol_plan_vol_nbr"].ToString()) ? Convert.ToInt32(item["act_vol_plan_vol_nbr"].ToString()) : 0,
                        CurrentRunStart = item["current_run_start"].ToString(),
                        CurrentRunEnd = item["current_run_end"].ToString(),
                        CurOperationId = !string.IsNullOrEmpty(item["cur_operation_id"].ToString()) ? Convert.ToInt32(item["cur_operation_id"].ToString()) : 0,
                        BinFullStatus = !string.IsNullOrEmpty(item["bin_full_status"].ToString()) ? Convert.ToInt32(item["bin_full_status"].ToString()) : 0,
                        BinFullBins = item["bin_full_bins"].ToString().Trim(),
                        ThroughputStatus = !string.IsNullOrEmpty(item["throughput_status"].ToString()) ? Convert.ToInt32(item["throughput_status"].ToString()) : 0,
                        UnplanMaintSpStatus = !string.IsNullOrEmpty(item["unplan_maint_sp_status"].ToString()) ? Convert.ToInt32(item["unplan_maint_sp_status"].ToString()) : 0,
                        OpStartedLateStatus = !string.IsNullOrEmpty(item["op_started_late_status"].ToString()) ? Convert.ToInt32(item["op_started_late_status"].ToString()) : 0,
                        OpRunningLateStatus = !string.IsNullOrEmpty(item["op_running_late_status"].ToString()) ? Convert.ToInt32(item["op_running_late_status"].ToString()) : 0,
                        UnplanMaintSpTimer = !string.IsNullOrEmpty(item["unplan_maint_sp_timer"].ToString()) ? Convert.ToInt32(item["unplan_maint_sp_timer"].ToString()) : 0,
                        OpStartedLateTimer = !string.IsNullOrEmpty(item["op_started_late_timer"].ToString()) ? Convert.ToInt32(item["op_started_late_timer"].ToString()) : 0,
                        OpRunningLateTimer = !string.IsNullOrEmpty(item["op_running_late_timer"].ToString()) ? Convert.ToInt32(item["op_running_late_timer"].ToString()) : 0,
                        SortplanWrongStatus = !string.IsNullOrEmpty(item["sortplan_wrong_status"].ToString()) ? Convert.ToInt32(item["sortplan_wrong_status"].ToString()) : 0,
                        SortplanWrongTimer = !string.IsNullOrEmpty(item["sortplan_wrong_timer"].ToString()) ? Convert.ToInt32(item["sortplan_wrong_timer"].ToString()) : 0,
                        HourlyData = item["hourly_data"].ToObject<List<HourlyData>>(),
                        RpgExpectedThruput = item["rpg_expected_thruput"].ToString(),
                        ArsRecrej3 = !string.IsNullOrEmpty(item["ars_recrej3"].ToString()) ? Convert.ToInt32(item["ars_recrej3"].ToString()) : 0,
                        SweepRecrej3 = !string.IsNullOrEmpty(item["sweep_recrej3"].ToString()) ? Convert.ToInt32(item["sweep_recrej3"].ToString()) : 0,
                        MpeId = string.Concat(item["mpe_type"].ToString().Trim(), "-", Convert.ToInt32(item["mpe_number"].ToString()).ToString().PadLeft(3, '0'))
                    });

                }
                return MPErunPerfData;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        private JObject Get_RPG_Plan_Info(JObject item)
        {
            try
            {
                RPGPlan tempRPG = AppParameters.MPEPRPGList.Where(x => x.Value.MpeType == item["mpe_type"].ToString() &&
                             Convert.ToInt32(x.Value.MachineNum) == (int)item["mpe_number"] &&
                             x.Value.SortProgramName == item["cur_sortplan"].ToString() &&
                             Convert.ToInt32((x.Value.MailOperationNbr).ToString().PadRight(6, '0')) == Convert.ToInt32(((int)item["cur_operation_id"]).ToString().PadRight(6, '0')) &&
                             (DateTime.Now >= x.Value.RpgStartDtm.AddMinutes(-15) && DateTime.Now <= x.Value.RpgEndDtm.AddMinutes(+15))
                             ).Select(l => l.Value).FirstOrDefault();


                if (tempRPG == null)
                {
                    return null;
                }
                else
                {
                    return JObject.Parse(JsonConvert.SerializeObject(tempRPG, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
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
                _data = string.Empty;
                _Message_type = string.Empty;
                _connID = string.Empty;
                tempData = null;
                machineInfo = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MPEWatch_RPGPerf()
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