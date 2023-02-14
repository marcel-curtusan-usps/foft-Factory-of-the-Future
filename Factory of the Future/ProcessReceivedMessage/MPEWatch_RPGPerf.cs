﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Factory_of_the_Future
{
    internal class MPEWatch_RPGPerf : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public List<RunPerf> MPErunPerfData = null;
        public RunPerf currentMachine_Info = null;
        private bool saveToFile;

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
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    JToken machineInfo = tempData.SelectToken("data");
                    if (machineInfo != null && machineInfo.HasValues)
                    {
                        DateTime dtNow = DateTime.Now;
                        string windowsTimeZoneId = "";
                        if (!string.IsNullOrEmpty((string)AppParameters.AppSettings["FACILITY_TIMEZONE"]))
                        {
                            AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings["FACILITY_TIMEZONE"], out windowsTimeZoneId);
                        }

                        MPErunPerfData = GetMPEPerfList(machineInfo);
                        foreach (RunPerf machine_Info in MPErunPerfData)
                        {
                            machine_Info.CurSortplan = AppParameters.SortPlan_Name_Trimer(machine_Info.CurSortplan);
                            if (machine_Info.RpgEstVol > 0 && machine_Info.CurThruputOphr > 0)
                            {
                                if (!string.IsNullOrEmpty(windowsTimeZoneId))
                                {
                                    dtNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                                    double intMinuteToCompletion = (machine_Info.RpgEstVol - machine_Info.TotSortplanVol) / (machine_Info.CurThruputOphr / 60);
                                    DateTime dtEstCompletion = dtNow.AddMinutes(intMinuteToCompletion);
                                    machine_Info.RpgEstCompTime = dtNow.AddMinutes(intMinuteToCompletion);
                                }
                            }
                            machine_Info.ExpectedThroughput = parseExpectedThruput(machine_Info.RpgExpectedThruput);
                            RPGPlan rpgPlan = Get_RPGPlan_Info(machine_Info);
                            if (rpgPlan != null)
                            {
                                //                item["rpg_start_dtm"] = results.ContainsKey("rpg_start_dtm") ? results["rpg_start_dtm"].ToString().Trim() : "";
                                //                item["rpg_end_dtm"] = results.ContainsKey("rpg_end_dtm") ? results["rpg_end_dtm"].ToString().Trim() : "";
                                //                item["expected_throughput"] = results.ContainsKey("expected_throughput") ? results["expected_throughput"].ToString().Trim() : "";
                                machine_Info.RPGStartDtm = rpgPlan.rpg_start_dtm;
                                machine_Info.RPGEndDtm = rpgPlan.rpg_start_dtm;
                                machine_Info.ExpectedThroughput = Convert.ToInt32(rpgPlan.expected_throughput);
                            }
                            if (machine_Info.ExpectedThroughput != 0)
                            {
                                double thrper = machine_Info.CurThruputOphr / machine_Info.ExpectedThroughput * 100;
                                if (string.IsNullOrEmpty(machine_Info.CurrentRunEnd))
                                {
                                    machine_Info.ThroughputStatus = 0;
                                }
                                else if (thrper >= 100)
                                {
                                    machine_Info.ThroughputStatus = 1;
                                }
                                else if (thrper >= 90)
                                {
                                    machine_Info.ThroughputStatus = 2;
                                }
                                else if (thrper < 90)
                                {
                                    machine_Info.ThroughputStatus = 3;
                                }
                            }
                          

                            if (AppParameters.MPEPerformance.ContainsKey(machine_Info.MpeId) && AppParameters.MPEPerformance.TryGetValue(machine_Info.MpeId, out currentMachine_Info))
                            {
                                bool update = false;
                                foreach (PropertyInfo prop in machine_Info.GetType().GetProperties())
                                {

                                    if (!new Regex("^(RpgEstCompTime)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                    {
                                        if (prop.GetValue(machine_Info, null).ToString() != prop.GetValue(currentMachine_Info, null).ToString())
                                        {
                                            update = true;
                                            prop.SetValue(currentMachine_Info, prop.GetValue(machine_Info, null));

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
                                if (AppParameters.MPEPerformance.TryAdd(machine_Info.MpeId, machine_Info))
                                {
                                    await Task.Run(() => FOTFManager.Instance.UpdateMpeData(machine_Info.MpeId)).ConfigureAwait(false);
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
                tempRPG = AppParameters.MPEPRPGList.Where(x => x.Value.mpe_type == machine_Info.MpeType &&
                          Convert.ToInt32(x.Value.machine_num) == machine_Info.MpeNumber &&
                          x.Value.sort_program_name == machine_Info.CurSortplan &&
                          Convert.ToInt32((x.Value.mail_operation_nbr).ToString().PadRight(6, '0')) == Convert.ToInt32(machine_Info.CurOperationId.ToString().PadRight(6, '0')) &&
                          (DateTime.Now >= x.Value.rpg_start_dtm.AddMinutes(-15) && DateTime.Now <= x.Value.rpg_end_dtm.AddMinutes(+15))
                          ).Select(l => l.Value).FirstOrDefault();

                if (tempRPG == null)
                {
                    return null;
                }
                else
                {
                    return tempRPG;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return tempRPG;
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
                foreach (JObject item in machineInfo.Children()) {
                    MPErunPerfData.Add( new RunPerf
                    {
                        MpeType = item["mpe_type"].ToString(),
                        MpeNumber = !string.IsNullOrEmpty(item["mpe_number"].ToString()) ? Convert.ToInt32(item["mpe_number"].ToString()) : 0,
                        Bins = !string.IsNullOrEmpty(item["bins"].ToString()) ? Convert.ToInt32(item["bins"].ToString()) : 0,
                        CurSortplan = item["cur_sortplan"].ToString(),
                        CurThruputOphr = !string.IsNullOrEmpty(item["cur_thruput_ophr"].ToString()) ? Convert.ToInt32(item["cur_thruput_ophr"].ToString()) : 0,
                        TotSortplanVol = !string.IsNullOrEmpty(item["tot_sortplan_vol"].ToString()) ? Convert.ToInt32(item["tot_sortplan_vol"].ToString()) : 0,
                        RpgEstVol = !string.IsNullOrEmpty(item["rpg_est_vol"].ToString()) ? Convert.ToInt32(item["rpg_est_vol"].ToString()) : 0,
                        ActVolPlanVolNbr = !string.IsNullOrEmpty(item["act_vol_plan_vol_nbr"].ToString()) ? Convert.ToInt32(item["act_vol_plan_vol_nbr"].ToString()) : 0,
                        CurrentRunStart = item["current_run_start"].ToString(),
                        CurrentRunEnd = item["current_run_end"].ToString(),
                        CurOperationId = !string.IsNullOrEmpty(item["cur_operation_id"].ToString()) ? Convert.ToInt32(item["cur_operation_id"].ToString()) : 0,
                        BinFullStatus = !string.IsNullOrEmpty(item["bin_full_status"].ToString()) ? Convert.ToInt32(item["bin_full_status"].ToString()): 0 ,
                        BinFullBins = item["bin_full_bins"].ToString(),
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
                RPGPlan tempRPG = AppParameters.MPEPRPGList.Where(x => x.Value.mpe_type == item["mpe_type"].ToString() &&
                             Convert.ToInt32(x.Value.machine_num) == (int)item["mpe_number"] &&
                             x.Value.sort_program_name == item["cur_sortplan"].ToString() &&
                             Convert.ToInt32((x.Value.mail_operation_nbr).ToString().PadRight(6, '0')) == Convert.ToInt32(((int)item["cur_operation_id"]).ToString().PadRight(6, '0')) &&
                             (DateTime.Now >= x.Value.rpg_start_dtm.AddMinutes(-15) && DateTime.Now <= x.Value.rpg_end_dtm.AddMinutes(+15))
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
        private static void MPEWatch_FullBins(JObject data)
        {
            try
            {
                string MpeType = data["mpe_type"].ToString().Trim();
                string MpeNumber = data["mpe_number"].ToString().PadLeft(3, '0');
                List<string> FullBins = !string.IsNullOrEmpty(data["bin_full_bins"].ToString()) ? data["bin_full_bins"].ToString().Split(',').Select(p => p.Trim().TrimStart('0')).ToList() : new List<string>();

                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                {
                    foreach (string key in cs.Zones.Where(f => f.Value.Properties.ZoneType == "Bin" &&
                    f.Value.Properties.MPEType.Trim() == MpeType && f.Value.Properties.MPENumber.ToString().PadLeft(3, '0') == MpeNumber).Select(y => y.Key).ToList())
                    {
                        List<string> FullBinList = new List<string>();
                        if (cs.Zones.TryGetValue(key, out GeoZone binZone))
                        {
                            binZone.Properties.MPEBins = null;
                            for (int i = 0; i < FullBins.Count; i++)
                            {
                                if (binZone.Properties.Bins.Split(',').Select(p => p.Trim()).ToList().Contains(FullBins[i]))
                                {
                                    FullBinList.Add(FullBins[i]);
                                }
                            }
                            binZone.Properties.MPEBins = FullBinList;
                            binZone.Properties.ZoneUpdate = true;
                        }
                        else
                        {
                            if (binZone.Properties.MPEBins.Count() != FullBinList.Count())
                            {
                                binZone.Properties.MPEBins = FullBinList;
                                binZone.Properties.ZoneUpdate = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
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