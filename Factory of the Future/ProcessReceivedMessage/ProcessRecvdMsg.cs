using Factory_of_the_Future.ProcessReceivedMessage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    public class ProcessRecvdMsg : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public async Task StartProcess(dynamic data, string Message_type, string connID)
        {
            try
            {
                _data = data;
                _Message_type = Message_type;
                _connID = connID;
                if (!string.IsNullOrEmpty(_Message_type) && !string.IsNullOrEmpty(_data))
                {
                    switch (_Message_type)
                    {
                        ///*Web cameras*/
                        case "RFID":
                            await Task.Run(() => new RFIdData().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            break;
                        ///*Web cameras*/
                        case "Cameras":
                            await Task.Run(() => new CameraData().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            //  CameraData(_data, _connID);
                            break;
                        /*Quuppa Data Start*/
                        case "getTagData":
                            await Task.Run(() => new TagData().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            // TagPosition(_data, _connID);
                            break;
                        case "getProjectInfo":
                            await Task.Run(() => new ProjectData().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            // ProjectData(data, connID);
                            break;
                        ///*Quuppa Data End*/
                        ///*SVWeb Data Start*/
                        case "doors":
                            await Task.Run(() => new DoorData().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            //Doors(data, connID);
                            break;
                        case "getdoor_associated_trips":
                            await Task.Run(() => new Door_Associated_Trips().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            //Doors(data, connID);
                            break;
                        case "trips":
                            await Task.Run(() => new TripData().Load(_data, _Message_type, _connID)).ConfigureAwait(false);
                            // Trips(data, Message_type, connID);
                            break;
                        case "container":
                            await Task.Run(() => new ContainerData().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            //Container(_data, _connID);
                            break;
                        case "getTacsVsSels":
                            TacsVsSels(_data, _Message_type, _connID);
                            break;
                        case "getTacsVsSels_Summary":
                            TacsVsSels(_data, _Message_type, _connID);
                            break;
                        //case "TacsVsSelsAnomaly":
                        //    TacsVsSelsLDCAnomaly(data, Message_type);
                        //    break;
                        ///*SELS RT Data End*/
                        ///*IV Data Start*/
                        case "getStaffBySite":
                            await Task.Run(() => new StaffingData().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            //Staffing(_data, _connID);
                            break;
                        ///*IV Data End*/
                        ///*AGVM Data Start*/
                        case "FLEET_STATUS":
                            FLEET_STATUS(_data);
                            break;
                        case "MATCHEDWITHWORK":
                            MATCHEDWITHWORK(_data);
                            break;
                        case "SUCCESSFULPICKUP":
                            SUCCESSFULPICKUP(_data);
                            break;
                        case "SUCCESSFULDROP":
                            SUCCESSFULDROP(_data);
                            break;
                        case "ERRORWITHOUTWORK":
                            ERRORWITHOUTWORK(_data);
                            break;
                        case "ERRORWITHWORK":
                            ERRORWITHWORK(_data);
                            break;
                        case "MOVEREQUEST":
                            MOVEREQUEST(_data);
                            break;
                        case "MISSIONCANCELED":
                            if (!_data.ContainsKey("NASS_CODE"))
                            {
                                _data["NASS_CODE"] = AppParameters.AppSettings.FACILITY_NASS_CODE;
                            }
                            ERRORWITHWORK(_data);
                            break;
                        case "MISSIONFAILED":
                            ERRORWITHWORK(_data);
                            break;
                        ///*AGVM Data End*/
                        ///*MPEWatch Data Start*/
                        case "mpe_watch_id":
                            MPE_Watch_Id(_data);
                            break;
                        case "rpg_run_perf":
                            await Task.Run(() => new MPEWatch_RPGPerf().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            break;
                        case "rpg_plan":
                            await Task.Run(() => new MPEWatch_RPGPlan().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            break;
                        case "dps_run_estm":
                            await Task.Run(() => new MPEWatch_DPS().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            break;
                        ///*MPEWatch Data End*/
                        case "getSVZones":
                            await Task.Run(() => new SV_Zone().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            break;
                        ///*MPE call system Start*/
                        case "macro":
                            await Task.Run(() => new MPE_Alerts().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            break;
                        ///*SELS RT Start*/
                        case "getTypes":
                            await Task.Run(() => new Tag_Types().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            break;
                        case "tagIdToEmpId":
                            await Task.Run(() => new Tag_to_EMP().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            break;
                        case "getIvEmpData":
                            await Task.Run(() => new Emp_Schedule().LoadAsync(_data, _Message_type, _connID)).ConfigureAwait(false);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    await Task.Run(() => AppParameters.RunningConnection.ConnectionUpdate(connID, 4)).ConfigureAwait(false);
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
        //private void Staffing(dynamic data ,string conID)
        //{
        //    try
        //    {
        //        bool updatefile = false;
        //        if (data != null )
        //        {
        //            JToken tempData = JToken.Parse(data);
        //            IEnumerable<JToken> staff = tempData.SelectTokens("$..DATA[*]");
        //            JArray sortplanlist = new JArray();
        //            if (staff.Count() > 0)
        //            {
        //                foreach (JToken stafff_item in staff)
        //                {
        //                    sortplanlist.Add(new JObject(new JProperty("mach_type", (string)stafff_item[0]),
        //                        new JProperty("machine_no", (int)stafff_item[1]),
        //                        new JProperty("sortplan", (string)stafff_item[2]),
        //                        new JProperty("clerk", stafff_item[3]),
        //                        new JProperty("mh", stafff_item[4])));
        //                }

        //            }
        //            else
        //            {
        //                Task.Run(() => UpdateConnection(conID, "error"));
        //            }
        //            if (sortplanlist.HasValues)
        //            {
        //                foreach (JObject Dataitem in sortplanlist.Children())
        //                {
        //                    if (!string.IsNullOrEmpty((string)Dataitem["sortplan"]))
        //                    {
        //                        string mach_type = Dataitem.ContainsKey("mach_type") ? (string)Dataitem["mach_type"] : "";
        //                        string machine_no = Dataitem.ContainsKey("machine_no") ? (string)Dataitem["machine_no"] : "";
        //                        string sortplan = Dataitem.ContainsKey("sortplan") ? (string)Dataitem["sortplan"] : "";
        //                        if (mach_type == "APBS")
        //                        {
        //                            mach_type = "SPBSTS";
        //                        }

        //                        sortplan = AppParameters.SortPlan_Name_Trimer(sortplan);
        //                        string mch_sortplan_id = mach_type + "-" + machine_no + "-" + sortplan;
        //                        string newtempData = JsonConvert.SerializeObject(Dataitem, Formatting.None);
        //                        AppParameters.StaffingSortplansList.AddOrUpdate(mch_sortplan_id, newtempData,
        //                             (key, existingVal) =>
        //                             {
        //                                 updatefile = true;
        //                                 return newtempData;
        //                             });
        //                    }
        //                }
        //                Task.Run(() => UpdateConnection(conID, "good"));
        //            }
        //            else
        //            {
        //                Task.Run(() => UpdateConnection(conID, "error"));
        //            }
        //            if (updatefile)
        //            {
        //                new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Staffing.json", JsonConvert.SerializeObject(AppParameters.StaffingSortplansList.Select(x => x.Value).ToList(), Formatting.Indented));
        //            }

        //        }
        //        else
        //        {
        //            Task.Run(() => UpdateConnection(conID, "error"));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Task.Run(() => UpdateConnection(conID, "error"));
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}
        //private static void Trips(dynamic data, string message_type, string conID)
        //{

        //    try
        //    {
        //        if (data != null)
        //        {
        //            JToken jsonObject = JToken.Parse(data);
        //            if (jsonObject != null && jsonObject.HasValues)
        //            {
        //                foreach (JObject rt in jsonObject.Children())
        //                {
        //                    if (rt.ContainsKey("routeTripId") && rt.ContainsKey("routeTripLegId") && rt.ContainsKey("tripDirectionInd"))
        //                    {
        //                        string routetripid = string.Concat(rt["routeTripId"].ToString(), rt["routeTripLegId"].ToString(), rt["tripDirectionInd"].ToString());
        //                        rt["id"] = routetripid;

        //                        rt["rawData"] = JsonConvert.SerializeObject(rt, Formatting.None);
        //                        RouteTrips newRTData = rt.ToObject<RouteTrips>();
        //                        newRTData.TripMin = AppParameters.Get_TripMin(newRTData.ScheduledDtm);
        //                        // if trip does not exist and does not have CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE in LegStatus
        //                        // then add to list
        //                        //if (AppParameters.RouteTripsList.ContainsKey(routetripid))
        //                        //{
        //                        //    Task.Run(() => AddTriptoList(routetripid, newRTData));
        //                        //}
        //                        //else if (!AppParameters.RouteTripsList.ContainsKey(routetripid) &&
        //                        //    !(Regex.IsMatch(newRTData.LegStatus, "(CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE)", RegexOptions.IgnoreCase)
        //                        //    || Regex.IsMatch(newRTData.Status, "(CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE)", RegexOptions.IgnoreCase)))
        //                        //{
        //                            Task.Run(() => AddTriptoList(routetripid, newRTData));
        //                        //}


        //                        //if (AppParameters.RouteTripsList.ContainsKey(routetripid))
        //                        //{

        //                        //    if (AppParameters.RouteTripsList.AddOrUpdate(routetripid, out RouteTrips existingVal))
        //                        //    {
        //                        //        if (rt.ToString() != existingVal.RawData)
        //                        //        {
        //                        //            existingVal.TripUpdate = true;
        //                        //            existingVal.Merge(rt, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        //                        //            //Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt["route"].ToString(), rt["trip"].ToString(), AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), GetSvDate((JObject)rt["operDate"])), rt["tripDirectionInd"].ToString(), routetripid));
        //                        //        }
        //                        //    }
        //                        //}
        //                        //else
        //                        //{

        //                        //    if (AppParameters.RouteTripsList.TryAdd(routetripid, rt.ToObject<RouteTrips>()))
        //                        //    {
        //                        //        // Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt["route"].ToString(), rt["trip"].ToString(), AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), GetSvDate((JObject)rt["operDate"])), rt["tripDirectionInd"].ToString(), routetripid));
        //                        //    }
        //                        //}
        //                    }

        //                }
        //                Task.Run(() => UpdateConnection(conID, "good"));
        //            }
        //            else
        //            {
        //                Task.Run(() => UpdateConnection(conID, "error"));
        //            }
        //            jsonObject = null;
        //        }
        //        else
        //        {
        //            Task.Run(() => UpdateConnection(conID, "error"));
        //        }
        //        if (AppParameters.RouteTripsList.Count > 0)
        //        {
        //            foreach (string m in AppParameters.RouteTripsList.Where(r => r.Value.TripMin < -1440 ).Select(y => y.Key))
        //            {
        //                AppParameters.RouteTripsList.TryRemove(m, out RouteTrips value);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Task.Run(() => UpdateConnection(conID, "error"));
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}
        //private static void AddTriptoList(string routetripid, RouteTrips newRTData)
        //{
        //    try
        //    {
        //        if (getDefaultDockDoor(string.Concat(newRTData.Route, newRTData.Trip), out string RouteTritDefaultDoor))
        //        {
        //            newRTData.DoorNumber = RouteTritDefaultDoor;
        //            newRTData.DoorId = !string.IsNullOrEmpty(RouteTritDefaultDoor) ? string.Concat("99D", RouteTritDefaultDoor.PadLeft(4, '-')) : "";


        //            if (AppParameters.RouteTripsList.ContainsKey(routetripid) && AppParameters.RouteTripsList.TryGetValue(routetripid, out RouteTrips existingVal))
        //            {
        //                if (AppParameters.RouteTripsList.TryUpdate(routetripid, newRTData, existingVal))
        //                {
        //                    //update 

        //                }

        //            }
        //            else
        //            {

        //                if (AppParameters.RouteTripsList.TryAdd(routetripid, newRTData))
        //                {
        //                    //add
        //                }
        //            }

        //            if (newRTData.OperDate != null)
        //            {
        //                Task.Run(() => new ItineraryTrip_Update(GetItinerary(newRTData.Route, newRTData.Trip, AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), AppParameters.GetSvDate(newRTData.OperDate)), routetripid));
        //            }
        //        }
        //        else
        //        {
        //            if (AppParameters.RouteTripsList.TryAdd(routetripid, newRTData))
        //            {
        //                if (newRTData.OperDate != null)
        //                {
        //                    Task.Run(() => new ItineraryTrip_Update(GetItinerary(newRTData.Route, newRTData.Trip, AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), AppParameters.GetSvDate(newRTData.OperDate)), routetripid));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}


        //private static string GetItinerary(string route, string trip, string nasscode, DateTime start_time)
        //{
        //    string temp = "";
        //    try
        //    {
        //        //string start_time = string.Concat(DateTime.Now.ToString("yyyy-MM-dd'T'"), "00:00:00");

        //        Uri parURL = new Uri(string.Format((string)AppParameters.AppSettings["SV_ITINERARY"], route, trip, string.Concat(start_time.ToString("yyyy-MM-dd'T'"), "00:00:00")));
        //        string SV_Response = SendMessage.SV_Get(parURL.AbsoluteUri);
        //        if (!string.IsNullOrEmpty(SV_Response))
        //        {
        //            temp = SV_Response;
        //        }
        //        return temp;
        //    }
        //    catch (Exception e)
        //    {   
        //        new ErrorLogger().ExceptionLog(e);
        //        return temp;
        //    }
        //}
        private void TacsVsSels(dynamic data, string message_type, string conID)
        {
            // "processedSince": "21-08-12 09:08:42",
            //"missedSelsCount": 561,
            //"missedSels": [
            //    {
            //        "empId": "01055142",
            //        "tagId": "ca0800004400",
            //        "tagName": "n/a",
            //        "type": "n/a",
            //        "tacs": {
            //            "ldc": "36",
            //            "totalTime": 8393,
            //            "operationId": "750",
            //            "payLocation": "n/a",
            //            "isOvertimeAuth": false,
            //            "overtimeHours": 0,
            //            "isOvertime": false
            //        }
            //    }}
            try
            {
                if (data != null)
                {
                    if (message_type == "getTacsVsSels_Summary")
                    {
                        JToken tempData = JToken.Parse(data);
                        //JArray ja = JArray.Parse(data);
                        foreach (JObject item in tempData.Cast<JObject>())
                        {
                            string badgeID = !string.IsNullOrEmpty(item["tagId"].ToString()) ? item["tagId"].ToString() : "";

                            string ldc = !string.IsNullOrEmpty(item["tacs"]["ldc"].ToString()) ? item["tacs"]["ldc"].ToString() : "";
                            string finance = !string.IsNullOrEmpty(item["tacs"]["finance"].ToString()) ? item["tacs"]["finance"].ToString() : "";
                            string fnAlert = !string.IsNullOrEmpty(item["tacs"]["fnAlert"].ToString()) ? item["tacs"]["fnAlert"].ToString() : "";
                            int totalTime = !string.IsNullOrEmpty(item["tacs"]["totalTime"].ToString()) ? (Int32)item["tacs"]["totalTime"] : 0;
                            string operationId = !string.IsNullOrEmpty(item["tacs"]["operationId"].ToString()) ? item["tacs"]["operationId"].ToString() : "";
                            string payLocation = !string.IsNullOrEmpty(item["tacs"]["payLocation"].ToString()) ? item["tacs"]["payLocation"].ToString() : "";
                            int overtimeHours = !string.IsNullOrEmpty(item["tacs"]["overtimeHours"].ToString()) ? (Int32)item["tacs"]["overtimeHours"] : 0;
                            bool isOverTime = !string.IsNullOrEmpty(item["tacs"]["isOvertime"].ToString()) && (bool)item["tacs"]["isOvertime"];
                            bool isOverTimeAuth = !string.IsNullOrEmpty(item["tacs"]["isOvertimeAuth"].ToString()) && (bool)item["tacs"]["isOvertimeAuth"];
                            if (!string.IsNullOrEmpty(badgeID))
                            {
                                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                                {
                                    List<string> tagID = cs.Locators.Where(x => x.Value.Properties.TagType.EndsWith("Person")
                                    && x.Value.Properties.Id.Trim().ToUpper() == badgeID.Trim().ToUpper()).Select(y => y.Key).ToList();
                                    if (tagID.Count > 0)
                                    {
                                        for (int i = 0; i < tagID.Count; i++)
                                        {
                                            cs.Locators.Where(f => f.Key == tagID[i]).Select(y => y.Value).ToList().ForEach(existingValue =>
                                            {

                                                if (existingValue.Properties.Tacs != null)
                                                {
                                                    //Update if used for more that OT
                                                    if (existingValue.Properties.Tacs.IsOvertime != isOverTime || existingValue.Properties.Tacs.IsOvertimeAuth != isOverTimeAuth)
                                                    {
                                                        existingValue.Properties.TagUpdate = true;
                                                    }
                                                }
                                                Tacs tc = new Tacs
                                                {
                                                    Ldc = ldc,
                                                    Finance = finance,
                                                    FnAlert = fnAlert,
                                                    TotalTime = totalTime,
                                                    OperationId = operationId,
                                                    PayLocation = payLocation,
                                                    IsOvertimeAuth = isOverTimeAuth,
                                                    OvertimeHours = overtimeHours,
                                                    IsOvertime = isOverTime,
                                                    StartTs = null,
                                                    StartTxt = "",
                                                    Ts = null,
                                                    OpenRingCode = ""
                                                };
                                                existingValue.Properties.Tacs = tc;
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }



                }


            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                Task.Run(() => UpdateConnection(conID, "error"));
            }

        }
        private void MPE_Watch_Id(string data)
        {
            try
            {
                if (data != null)
                {
                    JToken jsonObject = JToken.Parse(data);
                    if (jsonObject.HasValues)
                    {
                       AppParameters.AppSettings.MPE_WATCH_ID = jsonObject["id"].ToString();
                    }
                }

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        //private static void MPEWatch_RPGPerf(dynamic data, string conID)
        //{
        //    try
        //    {
        //        string total_volume = "";
        //        string estCompletionTime = "";
        //        if (data != null)
        //        {
        //            JToken tempData = JToken.Parse(data);
        //            JToken machineInfo = tempData.SelectToken("data");
        //            if (machineInfo != null && machineInfo.HasValues)
        //            {
        //                DateTime dtNow = DateTime.Now;
        //                string windowsTimeZoneId = "";
        //                if (!string.IsNullOrEmpty((string)AppParameters.AppSettings["FACILITY_TIMEZONE"]))
        //                {
        //                    AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings["FACILITY_TIMEZONE"], out windowsTimeZoneId);
        //                }
        //                foreach (JObject item in machineInfo.Children())
        //                {
        //                    item["cur_sortplan"] = AppParameters.SortPlan_Name_Trimer(item["cur_sortplan"].ToString());
        //                    item["cur_operation_id"] = !string.IsNullOrEmpty(item["cur_operation_id"].ToString()) ? item["cur_operation_id"].ToString() : "0";
        //                    item["rpg_start_dtm"] = "";
        //                    item["rpg_end_dtm"] = "";
        //                    item["expected_throughput"] = "0";
        //                    MPEWatch_FullBins(item);

        //                    total_volume = item.ContainsKey("tot_sortplan_vol") ? item["tot_sortplan_vol"].ToString().Trim() : "0";
        //                    int.TryParse(item.ContainsKey("rpg_est_vol") ? item["rpg_est_vol"].ToString().Trim() : "0", out int rpg_volume);
        //                    double.TryParse(item.ContainsKey("rpg_est_vol") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out double throughput);
        //                    int.TryParse(item.ContainsKey("rpg_est_vol") ? item["tot_sortplan_vol"].ToString().Trim() : "0", out int piecesfed);

        //                    if (rpg_volume > 0 && throughput > 0)
        //                    {
        //                        if (!string.IsNullOrEmpty(windowsTimeZoneId))
        //                        {
        //                            dtNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
        //                        }
        //                        double intMinuteToCompletion = (rpg_volume - piecesfed) / (throughput / 60);
        //                        DateTime dtEstCompletion = dtNow.AddMinutes(intMinuteToCompletion);
        //                        estCompletionTime = dtEstCompletion.ToString("yyyy-MM-dd HH:mm:ss");
        //                        item["rpg_est_comp_time"] = estCompletionTime;
        //                    }
        //                    else
        //                    {
        //                        item["rpg_est_comp_time"] = "";
        //                    }
        //                    if (item.ContainsKey("rpg_expected_thruput"))
        //                    {
        //                        item["expected_throughput"] = !string.IsNullOrEmpty(item["rpg_expected_thruput"].ToString()) ? item["rpg_expected_thruput"].ToString().Split(' ').FirstOrDefault() : "0";
        //                        if (!string.IsNullOrEmpty(item["expected_throughput"].ToString()) && item["expected_throughput"].ToString() != "0")
        //                        {
        //                            int.TryParse(item.ContainsKey("cur_thruput_ophr") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out int cur_thruput);
        //                            int.TryParse(item.ContainsKey("expected_throughput") ? item["expected_throughput"].ToString().Trim() : "0", out int expected_throughput);
        //                            double thrper = (double)cur_thruput / (double)expected_throughput * 100;
        //                            string throughputState = "1";
        //                            if (item["current_run_end"].ToString() != "" && item["current_run_end"].ToString() != "0")
        //                            {
        //                                throughputState = "0";
        //                            }
        //                            else if (thrper >= 100)
        //                            {
        //                                throughputState = "1";
        //                            }
        //                            else if (thrper >= 90)
        //                            {
        //                                throughputState = "2";
        //                            }
        //                            else if (thrper < 90)
        //                            {
        //                                throughputState = "3";
        //                            }
        //                            item["throughput_status"] = throughputState;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if ((item["current_run_end"].ToString() == "" || item["current_run_end"].ToString() == "0") && item["current_run_start"].ToString() != "")
        //                        {
        //                            // JObject results = new Oracle_DB_Calls().Get_RPG_Plan_Info(item);
        //                            JObject results = Get_RPG_Plan_Info(item);
        //                            if (results != null && results.HasValues)
        //                            {
        //                                item["rpg_start_dtm"] = results.ContainsKey("rpg_start_dtm") ? results["rpg_start_dtm"].ToString().Trim() : "";
        //                                item["rpg_end_dtm"] = results.ContainsKey("rpg_end_dtm") ? results["rpg_end_dtm"].ToString().Trim() : "";
        //                                item["expected_throughput"] = results.ContainsKey("expected_throughput") ? results["expected_throughput"].ToString().Trim() : "";
        //                                //item["throughput_status"] = "1";
        //                                if (!string.IsNullOrEmpty(item["expected_throughput"].ToString()) && item["expected_throughput"].ToString() != "0")
        //                                {
        //                                    int.TryParse(item.ContainsKey("cur_thruput_ophr") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out int cur_thruput);
        //                                    int.TryParse(item.ContainsKey("expected_throughput") ? item["expected_throughput"].ToString().Trim() : "0", out int expected_throughput);
        //                                    double thrper = (double)cur_thruput / (double)expected_throughput * 100;
        //                                    string throughputState = "1";
        //                                    if (thrper >= 100)
        //                                    {
        //                                        throughputState = "1";
        //                                    }
        //                                    else if (thrper >= 90)
        //                                    {
        //                                        throughputState = "2";
        //                                    }
        //                                    else if (thrper < 90)
        //                                    {
        //                                        throughputState = "3";
        //                                    }
        //                                    item["throughput_status"] = throughputState;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    CheckMachineNotifications(item);
        //                    string MpeName = string.Concat(item["mpe_type"].ToString().Trim(), "-", item["mpe_number"].ToString().PadLeft(3, '0'));
        //                    string newMPEPerf = JsonConvert.SerializeObject(item, Formatting.Indented);
        //                    AppParameters.MPEPerformanceList.AddOrUpdate(MpeName, newMPEPerf,
        //                          (key, oldValue) =>
        //                          {
        //                              return newMPEPerf;
        //                          });

        //                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
        //                    {
        //                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "Machine" &&
        //                        f.Value.Properties.Name == MpeName).Select(y => y.Value).ToList().ForEach(existingVal =>
        //                        {
        //                            string temp = JsonConvert.SerializeObject(existingVal.Properties.MPEWatchData, Formatting.None);
        //                            string tempItem = JsonConvert.SerializeObject(item, Formatting.None);
        //                            if (temp != tempItem)
        //                            {
        //                                existingVal.Properties.ZoneUpdate = true;
        //                            }
        //                        });
        //                    }
        //                     //AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Machine" && r.Value.Properties.Name == MpeName).Select(y => y.Key).ToList().ForEach(key =>
        //                     //{
        //                     //    if (AppParameters.ZoneList.TryGetValue(key, out GeoZone machineZone))
        //                     //    {
        //                     //        //convert to string
        //                     //        string temp = JsonConvert.SerializeObject(machineZone.Properties.MPEWatchData, Formatting.None);
        //                     //        string tempItem = JsonConvert.SerializeObject(item, Formatting.None);
        //                     //        if (temp != tempItem)
        //                     //        {
        //                     //            machineZone.Properties.ZoneUpdate = true;
        //                     //        }

        //                     //    }
        //                     //});

        //                }
        //                Task.Run(() => UpdateConnection(conID, "good"));
        //            }
        //            else
        //            {
        //                Task.Run(() => UpdateConnection(conID, "error"));
        //            }
        //            machineInfo = null;
        //            data = null;

        //        }
        //        else
        //        {
        //            Task.Run(() => UpdateConnection(conID, "error"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Task.Run(() => UpdateConnection(conID, "error"));
        //        new ErrorLogger().ExceptionLog(ex);
        //    }

        //}
        //private static JObject Get_RPG_Plan_Info(JObject item)
        //{
        //    try
        //    {
        //        RPGPlan tempRPG = AppParameters.MPEPRPGList.Where(x => x.Value.mpe_type == item["mpe_type"].ToString() &&
        //                     Convert.ToInt32(x.Value.machine_num) == (int)item["mpe_number"] &&
        //                     x.Value.sort_program_name == item["cur_sortplan"].ToString() &&
        //                     Convert.ToInt32((x.Value.mail_operation_nbr).ToString().PadRight(6,'0')) == Convert.ToInt32(((int)item["cur_operation_id"]).ToString().PadRight(6,'0')) &&
        //                     (DateTime.Now >= x.Value.rpg_start_dtm.AddMinutes(-15) && DateTime.Now <= x.Value.rpg_end_dtm.AddMinutes(+15))
        //                     ).Select(l => l.Value).FirstOrDefault();


        //        if (tempRPG == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return JObject.Parse(JsonConvert.SerializeObject(tempRPG, Formatting.Indented));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogger().ExceptionLog(ex);
        //        return null;
        //    }
        //}
        //private static void MPEWatch_FullBins(JObject data)
        //{
        //    try
        //    {
        //        string MpeType = data["mpe_type"].ToString().Trim();
        //        string MpeNumber = data["mpe_number"].ToString().PadLeft(3, '0');
        //        List<string> FullBins = !string.IsNullOrEmpty(data["bin_full_bins"].ToString()) ? data["bin_full_bins"].ToString().Split(',').Select(p => p.Trim().TrimStart('0')).ToList() : new List<string>();

        //        foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
        //        {
        //            foreach (string key in cs.Zones.Where(f => f.Value.Properties.ZoneType == "Bin" &&
        //            f.Value.Properties.MPEType.Trim() == MpeType && f.Value.Properties.MPENumber.ToString().PadLeft(3, '0') == MpeNumber).Select(y => y.Key).ToList())
        //            {
        //                List<string> FullBinList = new List<string>();
        //                if(cs.Zones.TryGetValue(key, out GeoZone binZone))
        //                {
        //                    binZone.Properties.MPEBins = null;
        //                    for (int i = 0; i < FullBins.Count; i++)
        //                    {
        //                        if (binZone.Properties.Bins.Split(',').Select(p => p.Trim()).ToList().Contains(FullBins[i]))
        //                        {
        //                            FullBinList.Add(FullBins[i]);
        //                        }
        //                    }
        //                    binZone.Properties.MPEBins = FullBinList;
        //                    binZone.Properties.ZoneUpdate = true;
        //                }
        //                else
        //                {
        //                    if (binZone.Properties.MPEBins.Count() != FullBinList.Count())
        //                    {
        //                        binZone.Properties.MPEBins = FullBinList;
        //                        binZone.Properties.ZoneUpdate = true;
        //                    }
        //                }
        //            }
        //        }





        //        //    foreach (string key in AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Bin" && r.Value.Properties.MPEType.Trim() == MpeType && r.Value.Properties.MPENumber.ToString().PadLeft(3, '0') == MpeNumber).Select(y => y.Key).ToList())
        //        //{
        //        //    List<string> FullBinList = new List<string>();
        //        //    if (AppParameters.ZoneList.TryGetValue(key, out GeoZone binZone))
        //        //    {
        //        //        if (FullBins.Any())
        //        //        {
        //        //            binZone.Properties.MPEBins = null;
        //        //            for (int i = 0; i < FullBins.Count; i++)
        //        //            {
        //        //                if (binZone.Properties.Bins.Split(',').Select(p => p.Trim()).ToList().Contains(FullBins[i]))
        //        //                {
        //        //                    FullBinList.Add(FullBins[i]);
        //        //                }
        //        //            }
        //        //            binZone.Properties.MPEBins = FullBinList;
        //        //            binZone.Properties.ZoneUpdate = true;
        //        //        }
        //        //        else
        //        //        {
        //        //            if (binZone.Properties.MPEBins.Count() != FullBinList.Count())
        //        //            {
        //        //                binZone.Properties.MPEBins = FullBinList;
        //        //                binZone.Properties.ZoneUpdate = true;
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogger().ExceptionLog(ex);
        //    }
        //}
        //private static void MPEWatch_RPGPlan(dynamic data, string conID)
        //{
        //    try
        //    {
        //        if (data != null)
        //        {
        //            JToken tempData = JToken.Parse(data);
        //            JToken planInfo = tempData.SelectToken("data");
        //            if (planInfo != null && planInfo.HasValues)
        //            {
        //                List<RPGPlan> RPG_collection = planInfo.ToObject<List<RPGPlan>>();
        //                foreach (RPGPlan RPG_item in RPG_collection)
        //                {
        //                    if (!string.IsNullOrEmpty(RPG_item.line_4_text))
        //                    {
        //                        RPG_item.expected_throughput = !string.IsNullOrEmpty(RPG_item.line_4_text) ? RPG_item.line_4_text.Split(' ').FirstOrDefault() : "0";
        //                    }
        //                    else
        //                    {
        //                        RPG_item.expected_throughput = !string.IsNullOrEmpty(RPG_item.rpg_expected_thruput) ? RPG_item.rpg_expected_thruput.Split(' ').FirstOrDefault() : "0";
        //                    }

        //                    RPG_item.sort_program_name = AppParameters.SortPlan_Name_Trimer(RPG_item.sort_program_name);

        //                    string RPGKey = AppParameters.MPEPRPGList.Where(x => x.Value.mpe_type == RPG_item.mpe_type &&
        //                    x.Value.machine_num == RPG_item.machine_num &&
        //                    x.Value.sort_program_name == RPG_item.sort_program_name &&
        //                    x.Value.mail_operation_nbr == RPG_item.mail_operation_nbr &&
        //                    x.Value.rpg_start_dtm == RPG_item.rpg_start_dtm
        //                    )
        //                       .Select(l => l.Key).FirstOrDefault();

        //                    if (!string.IsNullOrEmpty(RPGKey))
        //                    {
        //                        if (AppParameters.MPEPRPGList.TryGetValue(RPGKey, out RPGPlan OldRPG_item))
        //                        {
        //                            if (!AppParameters.MPEPRPGList.TryUpdate(RPGKey, RPG_item, OldRPG_item))
        //                            {
        //                                new ErrorLogger().CustomLog("Unable to update RPG Data" + RPG_item.mpe_name + " " + RPG_item.mpe_type + " " + RPG_item.machine_num, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        string newKey = Guid.NewGuid().ToString();
        //                        if (!AppParameters.MPEPRPGList.TryAdd(newKey, RPG_item))
        //                        {
        //                            new ErrorLogger().CustomLog("Unable to update RPG Data" + RPG_item.mpe_name + " " + RPG_item.mpe_type + " " + RPG_item.machine_num, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
        //                        }

        //                    }
        //                }
        //                Task.Run(() => UpdateConnection(conID, "good"));
        //            }
        //            else
        //            {
        //                Task.Run(() => UpdateConnection(conID, "error"));
        //            }
        //            planInfo = null;
        //            data = null;
        //            tempData = null;

        //        }
        //        else
        //        {
        //            Task.Run(() => UpdateConnection(conID, "error"));
        //        }
        //        //remove old data
        //        if (AppParameters.MPEPRPGList.Keys.Count > 0)
        //            {
        //                foreach (string existingkey in AppParameters.MPEPRPGList.Where(f => f.Value.rpg_start_dtm.Date <= DateTime.Now.AddDays(-2).Date).Select(y => y.Key))
        //                {
        //                    if (!AppParameters.MPEPRPGList.TryRemove(existingkey, out RPGPlan existingValue))
        //                    {
        //                        new ErrorLogger().CustomLog("Unable to update RPG Data" + existingValue.mpe_name + " " + existingValue.mpe_type + " " + existingValue.machine_num, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
        //                    }
        //                }
        //            }

        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogger().ExceptionLog(ex);
        //    }
        //}
        //private static void MPEWatch_DPSEst(dynamic data, string conID)
        //{
        //    try
        //    {
        //        if (data != null)
        //        {
        //            JToken tempData = JToken.Parse(data);
        //            JToken dpsInfo = tempData.SelectToken("data");
        //            if (dpsInfo != null && dpsInfo.HasValues)
        //            {
        //                int time_to_comp_optimal = 0;
        //                int time_to_comp_actual = 0;
        //                string time_to_comp_optimal_DateTime = "";
        //                string time_to_comp_actual_DateTime = "";
        //                DateTime dtNow = DateTime.Now;
        //                if (!string.IsNullOrEmpty((string)AppParameters.AppSettings["FACILITY_TIMEZONE"]))
        //                {
        //                    if (AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings["FACILITY_TIMEZONE"], out string windowsTimeZoneId))
        //                    {
        //                        dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
        //                    }
        //                }
        //                foreach (JObject item in dpsInfo.Children())
        //                {
        //                    string strSortPlan = item.ContainsKey("sortplan_name_perf") ? item["sortplan_name_perf"].ToString().Trim() : "";
        //                    string[] strSortPlanList = strSortPlan.Split(',').Select(x => x.Trim()).ToArray();

        //                    int.TryParse(item.ContainsKey("time_to_comp_optimal") ? item["time_to_comp_optimal"].ToString().Trim() : "0", out time_to_comp_optimal);
        //                    DateTime dtCompOptimal = dtNow.AddSeconds(time_to_comp_optimal);
        //                    time_to_comp_optimal_DateTime = dtCompOptimal.ToString("yyyy-MM-dd HH:mm:ss");
        //                    item["time_to_comp_optimal_DateTime"] = time_to_comp_optimal_DateTime;

        //                    int.TryParse(item.ContainsKey("time_to_comp_actual") ? item["time_to_comp_actual"].ToString().Trim() : "0", out time_to_comp_actual);
        //                    DateTime dtCompActual = dtNow.AddSeconds(time_to_comp_actual);
        //                    time_to_comp_actual_DateTime = dtCompActual.ToString("MM/dd/yyyy HH:mm:ss");
        //                    item["time_to_comp_actual_DateTime"] = time_to_comp_actual_DateTime;
        //                    for (int i = 0; i < strSortPlanList.Length; i++)
        //                    {
        //                        string newDPS = JsonConvert.SerializeObject(item, Formatting.Indented);
        //                        AppParameters.DPSList.AddOrUpdate(strSortPlanList[i].Substring(0, 7), newDPS,
        //                            (key, oldValue) =>
        //                            {
        //                                return newDPS;
        //                            });

        //                    }

        //                }

        //                Task.Run(() => UpdateConnection(conID, "good"));
        //            }
        //            else
        //            {
        //                Task.Run(() => UpdateConnection(conID, "error"));
        //            }
        //            tempData = null;
        //            dpsInfo = null;
        //            data = null;
        //        }
        //        else
        //        {
        //            Task.Run(() => UpdateConnection(conID, "error"));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogger().ExceptionLog(ex);
        //        Task.Run(() => UpdateConnection(conID, "error"));
        //    }
        //}
        private void ERRORWITHWORK(JObject data)
        {
            try
            {

                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings.FACILITY_NASS_CODE)
                {

                    Mission tempMission = data.ToObject<Mission>(new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                    tempMission.MISSIONERRORTIME = (DateTime)data["time".ToUpper()];
                    tempMission.STATE = "Error";
                    //JObject mission = new JObject
                    //{
                    //    ["Request_Id"] = (string)data["requestId".ToUpper()],
                    //    ["Vehicle"] = (string)data["vehicle".ToUpper()],
                    //    ["Vehicle_Number"] = (string)data["vehicle_Number".ToUpper()],
                    //    ["Error_Discription"] = (string)data["Error_Discription".ToUpper()],
                    //    ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
                    //    ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
                    //    ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
                    //    ["State"] = "Error",
                    //    ["MissionType"] = (string)data["message".ToUpper()],
                    //    ["MissionErrorTime"] = (DateTime)data["time".ToUpper()]
                    //};


                    //match with vehicle
                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        //get the key for the tag.
                        List<string> tag_id = cs.Locators.Where(f => f.Value.Properties.Name == tempMission.VEHICLE
                        && f.Value.Properties.TagType.EndsWith("Vehicle")).Select(y => y.Key).ToList();
                        //if tag is found then update data if tag is not found then at tag/vehicle 
                        if (tag_id.Count > 0)
                        {
                            for (int i = 0; i < tag_id.Count; i++)
                            {
                                cs.Locators.Where(f => f.Key == tag_id[i]).Select(y => y.Value).ToList().ForEach(existingValue =>
                                {
                                    existingValue.Properties.Missison = null;
                                    existingValue.Properties.TagUpdate = true;
                                });
                            }
                        }
                    }

                    //remove request id
                    if (AppParameters.MissionList.Keys.Count > 0)
                    {
                        if (AppParameters.MissionList.ContainsKey(tempMission.REQUEST_ID) && !AppParameters.MissionList.TryRemove(tempMission.REQUEST_ID, out Mission mission))
                        {
                            new ErrorLogger().CustomLog("unable to remove Mission " + mission.REQUEST_ID + " to list", string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "Appslogs"));
                        }
                    }
                    //update AGV zone location
                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        //pickup location
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                        {
                            existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                                && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                            existingVal.Properties.ZoneUpdate = true;
                        });
                        //drop-off location
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                        {
                            existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                                && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                            existingVal.Properties.ZoneUpdate = true;
                        });
                    }

                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                data = null;
            }
        }
        private void ERRORWITHOUTWORK(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings.FACILITY_NASS_CODE)
                {
                    if (data.ContainsKey("VEHICLE"))
                    {
                        //match with vehicle
                        foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                        {
                            //get the key for the tag.
                            List<string> tag_id = cs.Locators.Where(f => f.Value.Properties.Name == (string)data["VEHICLE"]
                            && f.Value.Properties.TagType.EndsWith("Vehicle")).Select(y => y.Key).ToList();
                            //if tag is found then update data if tag is not found then at tag/vehicle 
                            if (tag_id.Count > 0)
                            {
                                for (int i = 0; i < tag_id.Count; i++)
                                {
                                    cs.Locators.Where(f => f.Key == tag_id[i]).Select(y => y.Value).ToList().ForEach(existingValue =>
                                    {
                                        existingValue.Properties.Missison = null;
                                        existingValue.Properties.Vehicle_Status_Data.ERRORCODE = (string)data["errorCode".ToUpper()];
                                        existingValue.Properties.Vehicle_Status_Data.ERRORCODE_DISCRIPTION = (string)data["error_Discription".ToUpper()];
                                        existingValue.Properties.Vehicle_Status_Data.TIME = (DateTime)data["time".ToUpper()];
                                        existingValue.Properties.TagUpdate = true;
                                    });
                                }
                            }

                        }

                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                data = null;
            }
        }
        private void SUCCESSFULDROP(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings.FACILITY_NASS_CODE)
                {
                    Mission tempMission = data.ToObject<Mission>(new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                    tempMission.MISSIONDROPOFFTIME = (DateTime)data["time".ToUpper()];
                    tempMission.PLACARD = (string)data["mtel".ToUpper()];
                    tempMission.STATE = "Complete";
                    //JObject mission = new JObject
                    //{
                    //    ["Request_Id"] = (string)data["requestId".ToUpper()],
                    //    ["Vehicle"] = (string)data["vehicle".ToUpper()],
                    //    ["Vehicle_Number"] = (string)data["vehicle_Number".ToUpper()],
                    //    ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
                    //    ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
                    //    ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
                    //    ["Placard"] = (string)data["mtel".ToUpper()],
                    //    ["State"] = "Complete",
                    //    ["MissionType"] = (string)data["message".ToUpper()],
                    //    ["MissionDropOffTime"] = (DateTime)data["time".ToUpper()]
                    //};


                    //match with vehicle
                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        //get the key for the tag.
                        List<string> tag_id = cs.Locators.Where(f => f.Value.Properties.Name == tempMission.VEHICLE
                        && f.Value.Properties.TagType.EndsWith("Vehicle")).Select(y => y.Key).ToList();
                        //if tag is found then update data if tag is not found then at tag/vehicle 
                        if (tag_id.Count > 0)
                        {
                            for (int i = 0; i < tag_id.Count; i++)
                            {
                                cs.Locators.Where(f => f.Key == tag_id[i]).Select(y => y.Value).ToList().ForEach(existingValue =>
                                {
                                    existingValue.Properties.Missison = null;
                                    existingValue.Properties.TagUpdate = true;
                                });
                            }
                        }

                    }

                    //remove request id
                    if (AppParameters.MissionList.Keys.Count > 0)
                    {
                        if (AppParameters.MissionList.ContainsKey(tempMission.REQUEST_ID) && !AppParameters.MissionList.TryRemove(tempMission.REQUEST_ID, out Mission mission))
                        {
                            new ErrorLogger().CustomLog("unable to remove Mission " + mission.REQUEST_ID + " to list", string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "Appslogs"));
                        }
                    }


                    //update AGV zone location
                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        //pickup location
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                        {
                            existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                                && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                            existingVal.Properties.ZoneUpdate = true;
                        });
                        //drop-off location
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                        {
                            existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                                && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                            existingVal.Properties.ZoneUpdate = true;
                        });
                    }

                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                data = null;
            }
        }
        private void SUCCESSFULPICKUP(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings.FACILITY_NASS_CODE)
                {
                    Mission tempMission = data.ToObject<Mission>(new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                    tempMission.MISSIONPICKUPTIME = (DateTime)data["time".ToUpper()];
                    tempMission.PLACARD = (string)data["mtel".ToUpper()];
                    tempMission.STATE = "PickedUp";
                    //JObject mission = new JObject
                    //{
                    //    ["Request_Id"] = (string)data["requestId".ToUpper()],
                    //    ["Vehicle"] = data["vehicle".ToUpper()].ToString().ToUpper(),
                    //    ["Vehicle_Number"] = (string)data["vehicle_Number".ToUpper()],
                    //    ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
                    //    ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
                    //    ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
                    //    ["Door"] = (string)data["door".ToUpper()],
                    //    ["Placard"] = (string)data["mtel".ToUpper()],
                    //    ["State"] = "Active",
                    //    ["MissionType"] = (string)data["message".ToUpper()],
                    //    ["MissionPickupTime"] = (DateTime)data["time".ToUpper()]
                    //};



                    //merge changes 
                    if (AppParameters.MissionList.TryGetValue(tempMission.REQUEST_ID, out Mission existinnMision))
                    {
                        existinnMision.MISSIONPICKUPTIME = tempMission.MISSIONPICKUPTIME;
                        existinnMision.VEHICLE = tempMission.VEHICLE;
                        existinnMision.VEHICLE_NUMBER = tempMission.VEHICLE_NUMBER;
                        existinnMision.PICKUP_LOCATION = tempMission.PICKUP_LOCATION;
                        existinnMision.DROPOFF_LOCATION = tempMission.DROPOFF_LOCATION;
                        existinnMision.END_LOCATION = tempMission.END_LOCATION;
                        existinnMision.DOOR = tempMission.DOOR;
                        existinnMision.PLACARD = tempMission.PLACARD;
                        existinnMision.STATE = tempMission.STATE;
                    }
                    //match with vehicle

                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        //get the key for the tag.
                        List<string> tag_id = cs.Locators.Where(f => f.Value.Properties.Name == tempMission.VEHICLE
                        && f.Value.Properties.TagType.EndsWith("Vehicle")).Select(y => y.Key).ToList();
                        //if tag is found then update data if tag is not found then at tag/vehicle 
                        if (tag_id.Count > 0)
                        {
                            for (int i = 0; i < tag_id.Count; i++)
                            {
                                cs.Locators.Where(f => f.Key == tag_id[i]).Select(y => y.Value).ToList().ForEach(existingValue =>
                                {

                                    existingValue.Properties.Missison = AppParameters.MissionList.Where(r => r.Value.VEHICLE == tempMission.VEHICLE
                                    && r.Value.STATE == tempMission.STATE
                                    && r.Value.REQUEST_ID == tempMission.REQUEST_ID).Select(y => y.Value).FirstOrDefault();
                                    existingValue.Properties.TagUpdate = true;

                                });
                            }
                        }
                    }

                    //update AGV zone location
                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        //pickup location
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                        {
                            existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                                && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                            existingVal.Properties.ZoneUpdate = true;
                        });
                        //drop-off location
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                        {
                            existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                                && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                            existingVal.Properties.ZoneUpdate = true;
                        });
                    }

                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                data = null;
            }
        }
        private void MATCHEDWITHWORK(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings.FACILITY_NASS_CODE)
                {
                    Mission tempMission = data.ToObject<Mission>(new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                    tempMission.MISSIONASSIGNEDTIME = (DateTime)data["time".ToUpper()];
                    tempMission.PLACARD = (string)data["mtel".ToUpper()];
                    tempMission.STATE = "Active";
                    //JObject mission = new JObject
                    //{
                    //    ["Request_Id"] = (string)data["requestId".ToUpper()],
                    //    ["Vehicle"] = data["vehicle".ToUpper()].ToString().ToUpper(),
                    //    ["Vehicle_Number"] = (string)data["vehicle_Number".ToUpper()],
                    //    ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
                    //    ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
                    //    ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
                    //    ["ETA"] = (string)data["eta".ToUpper()],
                    //    ["Door"] = (string)data["door".ToUpper()],
                    //    ["Placard"] = (string)data["mtel".ToUpper()],
                    //    ["State"] = "Active",
                    //    ["MissionType"] = (string)data["message".ToUpper()],
                    //    ["MissionAssignedTime"] = (DateTime)data["time".ToUpper()]
                    //};

                    if (AppParameters.MissionList.Keys.Count == 0)
                    {
                        AppParameters.MissionList.TryAdd(tempMission.REQUEST_ID, tempMission);
                    }
                    else if (!AppParameters.MissionList.ContainsKey(tempMission.REQUEST_ID))
                    {
                        if (!AppParameters.MissionList.TryAdd(tempMission.REQUEST_ID, tempMission))
                        {
                            new ErrorLogger().CustomLog("unable to add Mission " + tempMission.REQUEST_ID + " to list", string.Concat(AppParameters.AppSettings.FACILITY_TIMEZONE, "Appslogs"));
                        }
                    }
                    else
                    {
                        //merge changes 
                        if (AppParameters.MissionList.TryGetValue(tempMission.REQUEST_ID, out Mission existinnMision))
                        {
                            existinnMision.MISSIONASSIGNEDTIME = tempMission.MISSIONASSIGNEDTIME;
                            existinnMision.VEHICLE = tempMission.VEHICLE;
                            existinnMision.VEHICLE_NUMBER = tempMission.VEHICLE_NUMBER;
                            existinnMision.PICKUP_LOCATION = tempMission.PICKUP_LOCATION;
                            existinnMision.DROPOFF_LOCATION = tempMission.DROPOFF_LOCATION;
                            existinnMision.END_LOCATION = tempMission.END_LOCATION;
                            existinnMision.DOOR = tempMission.DOOR;
                            existinnMision.PLACARD = tempMission.PLACARD;
                            existinnMision.STATE = tempMission.STATE;
                        }
                    }
                    //match with vehicle
                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        //get the key for the tag.
                        List<string> tag_id = cs.Locators.Where(f => f.Value.Properties.Name == tempMission.VEHICLE
                        && f.Value.Properties.TagType.EndsWith("Vehicle")).Select(y => y.Key).ToList();
                        //if tag is found then update data if tag is not found then at tag/vehicle 
                        if (tag_id.Count > 0)
                        {
                            for (int i = 0; i < tag_id.Count; i++)
                            {
                                cs.Locators.Where(f => f.Key == tag_id[i]).Select(y => y.Value).ToList().ForEach(existingValue =>
                                {

                                    existingValue.Properties.Missison = AppParameters.MissionList.Where(r => r.Value.VEHICLE == tempMission.VEHICLE
                                    && r.Value.STATE == tempMission.STATE
                                    && r.Value.REQUEST_ID == tempMission.REQUEST_ID).Select(y => y.Value).FirstOrDefault();
                                    existingValue.Properties.TagUpdate = true;

                                });
                            }
                        }
                    }

                    //update AGV zone location
                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        //pickup location
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                        {
                            existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                                && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                            existingVal.Properties.ZoneUpdate = true;
                        });
                        //drop-off location
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                        {
                            existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                                && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                            existingVal.Properties.ZoneUpdate = true;
                        });
                    }


                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private void MOVEREQUEST(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings.FACILITY_NASS_CODE)
                {
                    if (data.HasValues && data.ContainsKey("requestId".ToUpper()))
                    {
                        //JObject mission = new JObject
                        //{
                        //    ["Request_Id"] = (string)data["requestId".ToUpper()],
                        //    ["Vehicle"] = "",
                        //    ["Vehicle_Number"] = "",
                        //    ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
                        //    ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
                        //    ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
                        //    ["Door"] = (string)data["door".ToUpper()],
                        //    ["ETA"] = "",
                        //    ["Placard"] = (string)data["mtel".ToUpper()],
                        //    ["QueuePosition"] = (string)data["QueuePosition".ToUpper()],
                        //    ["State"] = "Active",
                        //    ["MissionType"] = (string)data["message".ToUpper()],
                        //    ["MissionRequestTime"] = (DateTime)data["time".ToUpper()]
                        //};
                        Mission tempMission = data.ToObject<Mission>(new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                        tempMission.MISSIONREQUESTTIME = (DateTime)data["time".ToUpper()];
                        tempMission.PLACARD = (string)data["mtel".ToUpper()];
                        tempMission.STATE = "Active";
                        if (!AppParameters.MissionList.ContainsKey(tempMission.REQUEST_ID))
                        {
                            if (!AppParameters.MissionList.TryAdd(tempMission.REQUEST_ID, tempMission))
                            {
                                new ErrorLogger().CustomLog("unable to add Mission " + tempMission.REQUEST_ID + " to list", string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "Appslogs"));
                            }
                        }
                        else
                        {
                            if (AppParameters.MissionList.TryRemove(tempMission.REQUEST_ID, out Mission valueOut))
                            {
                                if (!AppParameters.MissionList.TryAdd(tempMission.REQUEST_ID, tempMission))
                                {
                                    new ErrorLogger().CustomLog("unable to add Mission " + tempMission.REQUEST_ID + " to list", string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "Appslogs"));
                                }
                            }
                        }
                        //update AGV zone location
                        foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                        {
                            //pickup location
                            cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                            f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                            {
                                existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                                    && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                                existingVal.Properties.ZoneUpdate = true;
                            });
                            //drop-off location
                            cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                            f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value).ToList().ForEach(existingVal =>
                            {
                                existingVal.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                                    && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                                existingVal.Properties.ZoneUpdate = true;
                            });
                        }

                        //foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        //f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value))
                        //{
                        //    existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                        //    && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                        //    existingVa.Properties.ZoneUpdate = true;
                        //}
                        //foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        //f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value))
                        //{
                        //    existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                        //    && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                        //    existingVa.Properties.ZoneUpdate = true;
                        //}

                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private void FLEET_STATUS(JObject data)
        {
            bool update = false;
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings.FACILITY_NASS_CODE)
                {
                    if (data.ContainsKey("VEHICLE"))
                    {
                        VehicleStatus newVehicleStatus = data.ToObject<VehicleStatus>(new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                        foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                        {
                            //get the key for the tag.
                            List<string> tag_id = cs.Locators.Where(f => f.Value.Properties.Name == newVehicleStatus.VEHICLE
                            && f.Value.Properties.TagType.EndsWith("Vehicle")).Select(y => y.Key).ToList();
                            //if tag is found then update data if tag is not found then at tag/vehicle 
                            if (tag_id.Count > 0)
                            {
                                for (int i = 0; i < tag_id.Count; i++)
                                {
                                    cs.Locators.Where(f => f.Key == tag_id[i]).Select(y => y.Value).ToList().ForEach(existingValue =>
                                    {
                                        if (existingValue.Properties.Vehicle_Status_Data != null)
                                        {
                                            //check the notifications 
                                            if (existingValue.Properties.Vehicle_Status_Data.STATE != newVehicleStatus.STATE)
                                            {

                                                existingValue.Properties.NotificationId = CheckNotification(existingValue.Properties.Vehicle_Status_Data.STATE, newVehicleStatus.STATE, "vehicle".ToLower(), existingValue.Properties, existingValue.Properties.NotificationId);
                                                update = true;
                                            }
                                            if (existingValue.Properties.Vehicle_Status_Data.BATTERYPERCENT != newVehicleStatus.BATTERYPERCENT)
                                            {
                                                update = true;
                                            }
                                            JObject tempVehicleStatus = JObject.Parse(JsonConvert.SerializeObject(existingValue.Properties.Vehicle_Status_Data, Formatting.Indented));
                                            tempVehicleStatus.Merge(data, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                            existingValue.Properties.Vehicle_Status_Data = tempVehicleStatus.ToObject<VehicleStatus>();

                                            if (update)
                                            {
                                                FOTFManager.Instance.BroadcastVehicleTagStatus(existingValue, cs.Id);
                                            }
                                        }
                                        else
                                        {
                                            existingValue.Properties.Vehicle_Status_Data = newVehicleStatus;
                                            existingValue.Properties.NotificationId = CheckNotification("", newVehicleStatus.STATE, "vehicle".ToLower(), existingValue.Properties, existingValue.Properties.NotificationId);
                                            existingValue.Properties.TagUpdate = true;
                                        }
                                    });
                                }
                            }
                            else
                            {
                                GeoMarker Lmarker = new GeoMarker();
                                Lmarker.Properties.Id = newVehicleStatus.VEHICLE_MAC_ADDRESS;
                                Lmarker.Properties.Name = newVehicleStatus.VEHICLE;
                                Lmarker.Properties.TagType = new GetTagType().Get("AGV");
                                Lmarker.Properties.TagTS = newVehicleStatus.TIME;
                                Lmarker.Properties.PositionTS = newVehicleStatus.TIME;
                                Lmarker.Geometry = GetVehicleGeometry(newVehicleStatus.X_LOCATION, newVehicleStatus.Y_LOCATION);
                                Lmarker.Properties.Vehicle_Status_Data = newVehicleStatus;
                                Lmarker.Properties.NotificationId = CheckNotification("", newVehicleStatus.STATE, "vehicle".ToLower(), Lmarker.Properties, Lmarker.Properties.NotificationId);

                                Lmarker.Properties.TagVisible = true;
                                FOTFManager.Instance.BroadcastVehicleTagStatus(Lmarker, cs.Id);
                                if (!cs.Locators.TryAdd(Lmarker.Properties.Id, Lmarker))
                                {
                                    new ErrorLogger().CustomLog("Unable to Add Marker " + newVehicleStatus.VEHICLE_MAC_ADDRESS, string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "_Applogs"));
                                }
                            }
                            //    cs.Locators.Where(f => f.Value.Properties.Name == newVehicleStatus.VEHICLE 
                            //&& f.Value.Properties.TagType.EndsWith("Vehicle")).Select(y => y.Value).ToList().ForEach(existingValue =>
                            //{
                            //    if (existingValue.Properties.Vehicle_Status_Data != null)
                            //    {
                            //        //check the notifications 
                            //        if (existingValue.Properties.Vehicle_Status_Data.STATE != newVehicleStatus.STATE)
                            //        {

                            //            existingValue.Properties.NotificationId = CheckNotification(existingValue.Properties.Vehicle_Status_Data.STATE, newVehicleStatus.STATE, "vehicle".ToLower(), existingValue.Properties, existingValue.Properties.NotificationId);
                            //            update = true;
                            //        }
                            //        if (existingValue.Properties.Vehicle_Status_Data.BATTERYPERCENT != newVehicleStatus.BATTERYPERCENT)
                            //        {
                            //            update = true;
                            //        }
                            //        JObject tempVehicleStatus = JObject.Parse(JsonConvert.SerializeObject(existingValue.Properties.Vehicle_Status_Data, Formatting.Indented));
                            //        tempVehicleStatus.Merge(data, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                            //        existingValue.Properties.Vehicle_Status_Data = tempVehicleStatus.ToObject<VehicleStatus>();

                            //        if (update)
                            //        {
                            //            existingValue.Properties.TagUpdate = true;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        existingValue.Properties.Vehicle_Status_Data = newVehicleStatus;
                            //        existingValue.Properties.NotificationId = CheckNotification("", newVehicleStatus.STATE, "vehicle".ToLower(), existingValue.Properties, existingValue.Properties.NotificationId);
                            //        existingValue.Properties.TagUpdate = true;
                            //    }

                            //});
                        }

                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private MarkerGeometry GetVehicleGeometry(string x, string y)
        {
            try
            {
                JObject geometry = new JObject();
                JArray temp = new JArray();
                if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
                {
                    temp = new JArray(0.0, 0.0);

                }
                else
                {
                    temp = new JArray(y, x);
                }
                geometry["coordinates"] = temp;
                MarkerGeometry result = geometry.ToObject<MarkerGeometry>();
                return result;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        private string CheckNotification(string currentState, string NewState, string type, Marker properties, string noteification_Id)
        {
            string noteification_id = "";
            try
            {
                if (currentState != NewState)
                {
                    if (!string.IsNullOrEmpty(noteification_Id) && AppParameters.NotificationList.ContainsKey(noteification_Id))
                    {
                        if (AppParameters.NotificationList.TryGetValue(noteification_Id, out Notification notification))
                        {

                            notification.Delete = true;
                            notification.Notification_Update = true;
                            noteification_id = "";

                        }
                    }
                    //new condition
                    AppParameters.NotificationConditionsList.Where(r => Regex.IsMatch(NewState, r.Value.Conditions, RegexOptions.IgnoreCase)
                  && r.Value.Type.ToLower() == type.ToLower()
                   && r.Value.ActiveCondition).Select(x => x.Value).ToList().ForEach(conditions =>
                   {
                       noteification_id = conditions.Id + properties.Id;

                       Notification newNotifi = JsonConvert.DeserializeObject<Notification>(JsonConvert.SerializeObject(conditions, Formatting.None));
                       newNotifi.Type_ID = properties.Id;
                       newNotifi.Type_Name = properties.Name;
                       newNotifi.Type_Duration = 0;
                       newNotifi.Type_Status = currentState;
                       newNotifi.Notification_ID = noteification_id;
                       newNotifi.Notification_Update = true;
                       newNotifi.Type_Time = properties.Vehicle_Status_Data.TIME;
                       AppParameters.NotificationList.TryAdd(noteification_id, newNotifi);

                   });

                }
                return noteification_id;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return noteification_id;
            }
        }

        internal void UpdateConnection(string conId, string type)
        {
            try
            {
                //if(AppParameters.ConnectionList.TryGetValue(conId, out Connection m ))
                // {
                //     var newConStatus = type != "error";
                //     if(m.ApiConnected != newConStatus)
                //     {
                //         m.ApiConnected = newConStatus;
                //         m.UpdateStatus = true;
                //     }
                //     //m.ApiConnected = type == "error" ? false : true;

                // }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        internal string GetBadgeId(string name)
        {
            try
            {
                int equalsIndex = name.IndexOf("_", 1);
                if ((equalsIndex > -1))
                {
                    string[] namesplit = name.Split('_');
                    if (namesplit.Length > 1)
                    {
                        return namesplit[1];
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }
        internal string GetCraftName(string name)
        {
            try
            {
                int equalsIndex = name.IndexOf("_", 1);
                if ((equalsIndex > -1))
                {
                    string[] namesplit = name.Split('_');
                    if (namesplit.Length > 1)
                    {
                        return namesplit[0];
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }
        private void CheckMachineNotifications(JObject machineData)
        {

            try
            {
                string zoneID = string.Empty;
                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                {
                    zoneID = cs.Zones.Where(x => x.Value.Properties.ZoneType == "Machine" &&
                            x.Value.Properties.MPEType == machineData["mpe_type"].ToString() &&
                            x.Value.Properties.MPENumber == (int)machineData["mpe_number"])
                               .Select(l => l.Key).FirstOrDefault();
                    if (!string.IsNullOrEmpty(zoneID))
                    {
                        string machine = machineData["mpe_type"].ToString().Trim() + machineData["mpe_number"].ToString().Trim();

                        CheckMachineThroughPutNotification(machineData, zoneID, machine);
                        CheckUnplannedMaintNotification(machineData, zoneID, machine);
                        CheckOPStartingLateNotification(machineData, zoneID, machine);
                        CheckOPRunningLateNatification(machineData, zoneID, machine);
                        CheckSortplanWrongNotification(machineData, zoneID, machine);
                    }
                }
                //string zoneID = AppParameters.ZoneList.Where(x => x.Value.Properties.ZoneType == "Machine" &&
                //            x.Value.Properties.MPEType == machineData["mpe_type"].ToString() &&
                //            x.Value.Properties.MPENumber == (int)machineData["mpe_number"])
                //               .Select(l => l.Key).FirstOrDefault();
                //if (!string.IsNullOrEmpty(zoneID))
                //{
                //    string machine = machineData["mpe_type"].ToString().Trim() + machineData["mpe_number"].ToString().Trim();

                //    CheckMachineThroughPutNotification(machineData, zoneID, machine);
                //    CheckUnplannedMaintNotification(machineData, zoneID, machine);
                //    CheckOPStartingLateNotification(machineData, zoneID, machine);
                //    CheckOPRunningLateNatification(machineData, zoneID, machine);
                //    CheckSortplanWrongNotification(machineData, zoneID, machine);
                //}
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void UpdateDeleteMachineNotifications(string notificationID, string notificationName, string timerName, string duration, string notificationValue, string timerValue)
        {
            if (AppParameters.NotificationList.TryGetValue(notificationID, out Notification ojbMerge))
            {
                if (notificationValue == "0" || string.IsNullOrEmpty(notificationValue))
                {
                    ojbMerge.Delete = true;
                    ojbMerge.Notification_Update = true;
                }
                else
                {
                    if (ojbMerge.Type_Duration != Convert.ToInt32(timerValue))
                    {
                        ojbMerge.Notification_Update = true;
                    }
                    ojbMerge.Type_Duration = Convert.ToInt32(timerValue);
                }
            }
        }

        private void AddNewMachineNotification(JObject machineData, string zoneID, string notificationID, string notificationType, string durationtext, string durationTime)
        {
            try
            {
                foreach (NotificationConditions newCondition in AppParameters.NotificationConditionsList.Where(r => Regex.IsMatch(notificationType, r.Value.Conditions, RegexOptions.IgnoreCase)
                            && r.Value.Type.ToLower() == "mpe".ToLower()
                            && (bool)r.Value.ActiveCondition).Select(x => x.Value).ToList())

                {
                    if (!AppParameters.NotificationList.ContainsKey(notificationID))
                    {
                        int.TryParse(durationTime, out int intStr);
                        string machineName = machineData["mpe_type"].ToString().Trim() + "-" + machineData["mpe_number"].ToString().Trim().PadLeft(3, '0');
                        Notification ojbMerge = new Notification
                        {
                            Type = newCondition.Type,
                            Name = newCondition.Name,
                            Type_ID = zoneID,
                            Notification_ID = notificationID,
                            Notification_Update = true,
                            Type_Duration = intStr,
                            Type_Status = "",
                            Type_Name = machineName,
                            Warning = newCondition.Warning,
                            Critical = newCondition.Critical,
                            WarningAction = newCondition.WarningAction,
                            CriticalAction = newCondition.CriticalAction
                        };
                        AppParameters.NotificationList.TryAdd(notificationID, ojbMerge);
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private string GetMachineNotificationDurationText(string durationSeconds)
        {
            string durartionText = "";
            if (Double.TryParse(durationSeconds, out Double dblSeconds))
            {
                TimeSpan ts = TimeSpan.FromSeconds(dblSeconds);
                var hour = ts.Hours;
                var min = ts.Minutes;
                var sec = ts.Seconds;
                Double decSec = (1 / 60) * sec;
                var secText = decSec != 0 ? "." + Math.Round(decSec, 0, MidpointRounding.AwayFromZero).ToString() : "";
                if (hour == 0)
                {
                    durartionText = min.ToString() + secText + " mins";
                }
                else
                {
                    string hourtext = hour > 1 ? "hrs" : "hr";
                    durartionText = hour.ToString() + " " + hourtext + ", " + min.ToString() + secText + " mins";
                }
            }
            return durartionText;
        }

        private void CheckOPStartingLateNotification(JObject machineData, string zoneID, string machine)
        {
            string notification_name = "op_started_late_status";
            string notification_timer = "op_started_late_timer";
            string notificationID = machine + "_op_started_late_status";
            string op_started_late_status = machineData.ContainsKey("op_started_late_status") ? machineData["op_started_late_status"].ToString().Trim() : "0";
            string op_started_late_timer = machineData.ContainsKey("op_started_late_timer") ? machineData["op_started_late_timer"].ToString().Trim() : "0";
            string duration = op_started_late_timer != "0" ? GetMachineNotificationDurationText(op_started_late_timer) : "";
            if (!string.IsNullOrEmpty(notificationID) && AppParameters.NotificationList.ContainsKey(notificationID))
            {
                UpdateDeleteMachineNotifications(notificationID, notification_name, notification_timer, duration, op_started_late_status, op_started_late_timer);
            }
            else if (op_started_late_status != "0" && !string.IsNullOrEmpty(op_started_late_status))
            {
                AddNewMachineNotification(machineData, zoneID, notificationID, notification_name, duration, op_started_late_timer);
            }
        }

        private void CheckUnplannedMaintNotification(JObject machineData, string zoneID, string machine)
        {
            string notification_name = "unplan_maint_sp_status";
            string notification_timer = "unplan_maint_sp_timer";
            string notificationID = machine + "_unplan_maint_sp_status";
            string unplan_maint_sp_status = machineData.ContainsKey("unplan_maint_sp_status") ? machineData["unplan_maint_sp_status"].ToString().Trim() : "0";
            string unplan_maint_sp_timer = machineData.ContainsKey("unplan_maint_sp_timer") ? machineData["unplan_maint_sp_timer"].ToString().Trim() : "0";
            string duration = unplan_maint_sp_timer != "0" ? GetMachineNotificationDurationText(unplan_maint_sp_timer) : "";
            if (!string.IsNullOrEmpty(notificationID) && AppParameters.NotificationList.ContainsKey(notificationID))
            {
                UpdateDeleteMachineNotifications(notificationID, notification_name, notification_timer, duration, unplan_maint_sp_status, unplan_maint_sp_timer);
            }
            else if (unplan_maint_sp_status != "0" && !string.IsNullOrEmpty(unplan_maint_sp_status))
            {
                AddNewMachineNotification(machineData, zoneID, notificationID, notification_name, duration, unplan_maint_sp_timer);
            }
        }

        private void CheckSortplanWrongNotification(JObject machineData, string zoneID, string machine)
        {
            string notification_name = "sortplan_wrong_status";
            string notification_timer = "sortplan_wrong_timer";
            string notificationID = machine + "_sortplan_wrong_status";
            string sortplan_wrong_status = machineData.ContainsKey("sortplan_wrong_status") ? machineData["sortplan_wrong_status"].ToString().Trim() : "0";
            string sortplan_wrong_timer = machineData.ContainsKey("sortplan_wrong_timer") ? machineData["sortplan_wrong_timer"].ToString().Trim() : "0";
            string duration = sortplan_wrong_timer != "0" ? GetMachineNotificationDurationText(sortplan_wrong_timer) : "";
            if (!string.IsNullOrEmpty(notificationID) && AppParameters.NotificationList.ContainsKey(notificationID))
            {
                UpdateDeleteMachineNotifications(notificationID, notification_name, notification_timer, duration, sortplan_wrong_status, sortplan_wrong_timer);
            }
            else if (sortplan_wrong_status != "0" && !string.IsNullOrEmpty(sortplan_wrong_status))
            {
                AddNewMachineNotification(machineData, zoneID, notificationID, notification_name, duration, sortplan_wrong_timer);
            }
        }

        private void CheckOPRunningLateNatification(JObject machineData, string zoneID, string machine)
        {
            string notification_name = "op_running_late_status";
            string notification_timer = "op_running_late_timer";
            string notificationID = machine + "_op_running_late_status";
            string op_running_late_status = machineData.ContainsKey("op_running_late_status") ? machineData["op_running_late_status"].ToString().Trim() : "0";
            string op_running_late_timer = machineData.ContainsKey("op_running_late_timer") ? machineData["op_running_late_timer"].ToString().Trim() : "0";
            string duration = op_running_late_timer != "0" ? GetMachineNotificationDurationText(op_running_late_timer) : "";
            if (!string.IsNullOrEmpty(notificationID) && AppParameters.NotificationList.ContainsKey(notificationID))
            {
                UpdateDeleteMachineNotifications(notificationID, notification_name, notification_timer, duration, op_running_late_status, op_running_late_timer);
            }
            else if (op_running_late_status != "0" && !string.IsNullOrEmpty(op_running_late_status))
            {
                AddNewMachineNotification(machineData, zoneID, notificationID, notification_name, duration, op_running_late_timer);
            }
        }

        private void CheckMachineThroughPutNotification(JObject machineData, string zoneID, string machine)
        {
            string notificationID = machine + "_throughput_status";
            string throughput_status = machineData.ContainsKey("throughput_status") ? machineData["throughput_status"].ToString().Trim() : "0";
            try
            {
                if (!string.IsNullOrEmpty(notificationID) && AppParameters.NotificationList.ContainsKey(notificationID))
                {
                    if (AppParameters.NotificationList.TryGetValue(notificationID, out Notification ojbMerge))
                    {
                        if (throughput_status == "1" || throughput_status == "0" || string.IsNullOrEmpty(throughput_status))
                        {
                            ojbMerge.Delete = true;
                            ojbMerge.Notification_Update = true;
                        }
                        else
                        {
                            string prev_throughput_status = ojbMerge.Type_Status.ToString().Trim();
                            if (prev_throughput_status != throughput_status)
                            {
                                ojbMerge.Type_Status = throughput_status;
                                ojbMerge.Notification_Update = true;
                            }
                            else
                            {
                                ojbMerge.Notification_Update = false;
                            }
                        }
                    }
                }
                else
                {
                    if (throughput_status != "1" && !string.IsNullOrEmpty(throughput_status) && throughput_status != "0")
                    {
                        string startdatetime = machineData.ContainsKey("current_run_start") ? machineData["current_run_start"].ToString().Trim() : "";
                        if (!string.IsNullOrEmpty(startdatetime))
                        {
                            DateTime dtSD = Convert.ToDateTime(startdatetime);
                            DateTime dtNow = DateTime.Now;
                            if (!string.IsNullOrEmpty(AppParameters.AppSettings.FACILITY_TIMEZONE))
                            {
                                if (AppParameters.TimeZoneConvert.TryGetValue(AppParameters.AppSettings.FACILITY_TIMEZONE, out string windowsTimeZoneId))
                                {
                                    dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                                }
                            }
                            TimeSpan ts = dtNow - dtSD;
                            if (ts.TotalMinutes >= 15)
                            {
                                AddNewMachineNotification(machineData, zoneID, notificationID, "throughput_status", "", "");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void CheckScanNotification()
        {
            string loadAfterDepartTypeName = "Load After Depart";
            string missingLoadTypeName = "Missing Closed Scan";
            string missingAssignedTypeName = "Missing Assigned Scan";
            string missingArrived = "Missing Arrived Scan";

            RemoveOldScanNotification(loadAfterDepartTypeName);
            RemoveOldScanNotification(missingLoadTypeName);
            RemoveOldScanNotification(missingAssignedTypeName);
            RemoveOldScanNotification(missingArrived);
            try
            {
                foreach (Container _container in AppParameters.Containers.Select(y => y.Value))
                {
                    if (_container.hasAssignScans && _container.hasLoadScans)
                    {
                        var notification_id = _container.PlacardBarcode + "_MissingClosed";
                        var notification_name = _container.PlacardBarcode;
                        if (!_container.hasCloseScans)
                        {
                            AddScanNotification(missingLoadTypeName, notification_id, _container.PlacardBarcode, notification_name, 0);
                        }
                        else
                        {
                            RemoveScanNotification(notification_id);
                        }
                    }
                    if (_container.hasCloseScans && _container.hasLoadScans)
                    {
                        var notification_id = _container.PlacardBarcode + "_MissingAssigned";
                        var notification_name = _container.PlacardBarcode;
                        if (!_container.hasAssignScans)
                        {
                            AddScanNotification(missingAssignedTypeName, notification_id, _container.PlacardBarcode, notification_name, 0);
                        }
                        else
                        {
                            RemoveScanNotification(notification_id);
                        }
                    }
                    foreach (ContainerHistory _scan in _container.ContainerHistory.OrderBy(o => o.EventDtmfmt))
                    {
                        if (_scan.Event == "LOAD")
                        {
                            var _trip = AppParameters.RouteTripsList.Where(z => z.Value.TrailerBarcode == _container.Trailer).Select(z => z.Value).FirstOrDefault();
                            if (_trip != null)
                            {
                                if ((_trip.Status == "DEPARTED" || _trip.LegStatus == "DEPARTED") && _trip.TripDirectionInd == "O")
                                {
                                    var _containerLoadTime = _scan.EventDtmfmt;
                                    var _trailerDepartTime = new DateTime(_trip.ActualDtm.Year, (_trip.ActualDtm.Month + 1), _trip.ActualDtm.DayOfMonth, _trip.ActualDtm.HourOfDay, _trip.ActualDtm.Minute, _trip.ActualDtm.Second);
                                    if (_containerLoadTime > _trailerDepartTime)
                                    {
                                        TimeSpan span = _containerLoadTime - _trailerDepartTime;
                                        var totalMinutes = (int)Math.Round(span.TotalMinutes);
                                        var notification_id = _container.PlacardBarcode + "_" + _trip.TrailerBarcode + "_LAD";
                                        var notification_name = _container.PlacardBarcode + "|" + _trip.TrailerBarcode + "|" + _containerLoadTime + "|" + _trailerDepartTime;
                                        AddScanNotification(loadAfterDepartTypeName, notification_id, _container.PlacardBarcode, notification_name, totalMinutes);
                                    }
                                }
                            }
                        }
                    }
                    if (_container.hasLoadScans || _container.hasUnloadScans)
                    {
                        foreach (var door in AppParameters.DockdoorList)
                        {
                            JObject _door = JObject.Parse(door.Value);
                            if (_door != null)
                            {
                                string _trailerbarcode = _door.ContainsKey("trailerBarcode") ? _door["trailerBarcode"].ToString().Trim() : "";
                                string _doorid = _door.ContainsKey("doorId") ? _door["doorId"].ToString().Trim() : "";
                                string _doornumber = _door.ContainsKey("doorNumber") ? _door["doorNumber"].ToString().Trim() : "";

                                var notification_id = _trailerbarcode + "_MissingArrived";
                                var notification_name = _trailerbarcode + "|" + _doorid + "|" + _doornumber;

                                if (_trailerbarcode == _container.Trailer)
                                {
                                    bool hasarrivalscan = false;
                                    if (_door.ContainsKey("events"))
                                    {
                                        foreach (var _event in _door["events"])
                                        {
                                            if (!string.IsNullOrEmpty(_event["eventName"].ToString().Trim()) && _event["eventName"].ToString().ToUpper().Contains("ARR"))
                                            {
                                                hasarrivalscan = true;
                                            }
                                        }
                                    }
                                    if (!hasarrivalscan)
                                    {
                                        AddScanNotification(missingArrived, notification_id, _trailerbarcode, notification_name, 0);
                                    }
                                    else
                                    {
                                        RemoveScanNotification(notification_id);
                                    }
                                }
                                else
                                {
                                    RemoveScanNotification(notification_id);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void AddScanNotification(string notificationType, string notificationID, string scanID, string typeName, int minutes)
        {

            foreach (NotificationConditions newCondition in AppParameters.NotificationConditionsList.Where(r => Regex.IsMatch(notificationType, r.Value.Conditions, RegexOptions.IgnoreCase)
                            && r.Value.Type.ToLower() == "dockdoor".ToLower()
                            && (bool)r.Value.ActiveCondition).Select(x => x.Value).ToList())
            {
                var warningMinutes = newCondition.Warning;
                var criticalMinutes = newCondition.Critical;
                string status = "";
                if (minutes >= criticalMinutes)
                {
                    status = "Critical";
                }
                else if (minutes > warningMinutes)
                {
                    status = "Warning";
                }

                if (AppParameters.NotificationList.TryGetValue(notificationID, out Notification ojbMerge))
                {
                    string prev_status = ojbMerge.Type_Status.ToString().Trim();
                    if (prev_status != status)
                    {
                        ojbMerge.Type_Status = status;
                        ojbMerge.Notification_Update = true;
                    }
                    else
                    {
                        ojbMerge.Notification_Update = false;
                    }
                }
                else
                {
                    Notification _notification = new Notification
                    {
                        ActiveCondition = newCondition.ActiveCondition,
                        Type = newCondition.Type,
                        Name = newCondition.Name,
                        Type_ID = scanID,
                        Notification_ID = notificationID,
                        Notification_Update = true,
                        Type_Status = status,
                        Type_Name = typeName,
                        Warning = newCondition.Warning,
                        Critical = newCondition.Critical,
                        WarningAction = newCondition.WarningAction,
                        CriticalAction = newCondition.CriticalAction,
                        Type_Duration = 0
                    };
                    AppParameters.NotificationList.TryAdd(notificationID, _notification);
                }
            }
        }

        private void RemoveScanNotification(string notification_id)
        {
            foreach (Notification _notification in AppParameters.NotificationList.Where(x => Regex.IsMatch(notification_id, x.Value.Notification_ID, RegexOptions.IgnoreCase)).Select(x => x.Value).ToList())
            {
                if (AppParameters.NotificationList.TryGetValue(_notification.Notification_ID, out Notification ojbMerge))
                {
                    ojbMerge.Delete = true;
                    ojbMerge.Notification_Update = true;
                }
            }
        }

        private void RemoveOldScanNotification(string scanNotificationType)
        {
            foreach (Notification _notification in AppParameters.NotificationList.Where(x => Regex.IsMatch(scanNotificationType, x.Value.Type, RegexOptions.IgnoreCase)).Select(x => x.Value).ToList())
            {
                if (scanNotificationType == "Missing Arrived Scan")
                {
                    foreach (var door in AppParameters.DockdoorList)
                    {
                        JObject _door = JObject.Parse(door.Value);
                        if (_door != null)
                        {
                            string _trailerbarcode = _door.ContainsKey("trailerBarcode") ? _door["trailerBarcode"].ToString().Trim() : "";
                            if (_trailerbarcode != _notification.Notification_ID)
                            {
                                if (AppParameters.NotificationList.TryGetValue(_notification.Notification_ID, out Notification ojbMerge))
                                {
                                    ojbMerge.Delete = true;
                                    ojbMerge.Notification_Update = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    var _scan = AppParameters.Containers.Where(z => z.Value.PlacardBarcode == _notification.Type_ID).Select(x => x.Key).ToList();
                    if (!_scan.Any())
                    {
                        if (AppParameters.NotificationList.TryGetValue(_notification.Notification_ID, out Notification ojbMerge))
                        {
                            ojbMerge.Delete = true;
                            ojbMerge.Notification_Update = true;
                        }
                    }
                }
            }
        }

        internal void ProcessDarvisAlert42(string message)
        {

            if (message.Contains("IG_") || message.Contains("DT_"))
            {
                try
                {
                    JArray msgJson = (JArray)JsonConvert.DeserializeObject(message);
                    if (msgJson[0].ToString() == "detections")
                    {
                        JArray cameraData = (JArray)msgJson[1]["data"];
                        foreach (JObject jo in cameraData.Cast<JObject>())
                        {
                            string camera_id = jo["camera_id"].ToString();


                            List<DarvisCameraAlert> alertList = new List<DarvisCameraAlert>();

                            JArray newDetections = (JArray)jo["detections"]["new"];
                            JArray removedDetections = (JArray)jo["detections"]["removed"];
                            JArray updatedDetections = (JArray)jo["detections"]["updated"];
                            foreach (JObject newObject in newDetections.Cast<JObject>())
                            {

                                ProcessNewOrExistingCameraData(newObject, ref alertList, camera_id, true);
                            }
                            foreach (JToken object_id in removedDetections.Cast<JObject>())
                            {

                            }
                            foreach (JObject updatedObject in updatedDetections.Cast<JObject>())
                            {
                                ProcessNewOrExistingCameraData(updatedObject, ref alertList, camera_id, false);
                            }
                            List<Tuple<GeoMarker, string>> camerasToBroadcast = new List<Tuple<GeoMarker, string>>();
                            foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                            {
                                cs.Locators.Where(f => f.Value.Properties.TagType == "Camera" &&
                                f.Value.Properties.Name == camera_id).Select(y => y.Value).
                                ToList().ForEach(Camera =>
                                {
                                    Camera.Properties.DarvisAlerts = alertList.ToArray<DarvisCameraAlert>().ToList<DarvisCameraAlert>();

                                    FOTFManager.Instance.BroadcastCameraStatus(Camera, cs.Id);

                                });

                            }


                        }
                    }

                }
                catch (Exception ex)
                {
                    new ErrorLogger().ExceptionLog(ex);
                }
            }
        }
        public void ProcessNewOrExistingCameraData(JObject thisObject, ref List<DarvisCameraAlert> alertList, string camera_id, bool isNew)
        {
            if (thisObject.ContainsKey("zones"))
            {
                JArray zones = (JArray)thisObject["zones"];
                foreach (JObject zo in zones.Cast<JObject>())
                {
                    string zoName = zo["name"].ToString();

                    if (zoName.StartsWith("IG_") || zoName.StartsWith("DT_"))
                    {
                        float dwelltime = (float)Convert.ToDouble(zo["dwell_time"].ToString());
                        DarvisCameraAlert alert = new DarvisCameraAlert
                        {
                            DwellTime = dwelltime,
                            Type = zoName.StartsWith("IG_") ? "IG" : "DT",
                            object_class = thisObject["clazz"].ToString(),
                            object_id = thisObject["object_id"].ToString(),
                            Top = Convert.ToInt32(thisObject["top"].ToString()),
                            Bottom = Convert.ToInt32(thisObject["bottom"].ToString()),
                            Left = Convert.ToInt32(thisObject["left"].ToString()),
                            Right = Convert.ToInt32(thisObject["right"].ToString())
                        };
                        alertList.Add(alert);
                    }
                }
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
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProcessRecvdMsg()
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