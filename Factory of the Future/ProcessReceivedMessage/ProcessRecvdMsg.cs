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
    public class ProcessRecvdMsg
    {
        public void StartProcess(dynamic data, string Message_type)
        {
            try
            {
                if (!string.IsNullOrEmpty(Message_type))
                {
                    switch (Message_type)
                    {
                        ///*Web cameras*/
                        case "Cameras":
                            CameraData(data);
                            break;
                        /*Quuppa Data Start*/
                        //case "getTagPosition":
                        //    TagPosition(data);
                        //    break;
                        case "getProjectInfo":
                            ProjectData(data);
                            break;
                        ///*Quuppa Data End*/
                        ///*SVWeb Data Start*/
                        //case "doors":
                        //    Doors(data, Message_type);
                        //    break;

                        //case "trips":
                        //    Trips(data, Message_type);
                        //break;

                        case "container":
                            Container(data);
                            break;
                        ///*SVWeb Data End*/
                        ///*CTS Data Start*/
                        ////case "outbound":
                        ////    CTS_DockDeparted(data);
                        ////    break;

                        ////case "LocalTrips":
                        ////    CTS_LocalDockDeparted(data);
                        ////    break;

                        ////case "inboundScheduled":
                        ////    CTS_Inbound(data);
                        ////    break;

                        ////case "outboundScheduled":
                        ////    CTS_Outbound(data);
                        ////    break;
                        ///*CTS Data End*/
                        ///*SELS RT Data Start*/
                        ////case "P2PBySite":
                        ////    P2PBySite(data, Message_type);
                        ////    break;
                        //case "getTacsVsSels":
                        //    TacsVsSels(data, Message_type);
                        //    break;

                        //case "TacsVsSelsAnomaly":
                        //    TacsVsSelsLDCAnomaly(data, Message_type);
                        //    break;
                        ///*SELS RT Data End*/
                        ///*IV Data Start*/
                        //case "getStaffBySite":
                        //    Staffing(data, Message_type);
                        //    break;
                        ///*IV Data End*/
                        ///*AGVM Data Start*/
                        //case "FLEET_STATUS":
                        //    FLEET_STATUS(data);
                        //    break;
                        //case "MATCHEDWITHWORK":
                        //    MATCHEDWITHWORK(data);
                        //    break;
                        //case "SUCCESSFULPICKUP":
                        //    SUCCESSFULPICKUP(data);
                        //    break;
                        //case "SUCCESSFULDROP":
                        //    SUCCESSFULDROP(data);
                        //    break;
                        //case "ERRORWITHOUTWORK":
                        //    ERRORWITHOUTWORK(data);
                        //    break;
                        //case "ERRORWITHWORK":
                        //    ERRORWITHWORK(data);
                        //    break;
                        //case "MOVEREQUEST":
                        //    MOVEREQUEST(data);
                        //    break;
                        //case "MISSIONCANCELED":
                        //    ERRORWITHWORK(data);
                        //    break;
                        //case "MISSIONFAILED":
                        //    ERRORWITHWORK(data);
                        //    break;
                        ///*AGVM Data End*/
                        ///*MPEWatch Data Start*/
                        //case "mpe_watch_id":
                        //    MPE_Watch_Id(data);
                        //    break;

                        //case "rpg_run_perf":
                        //    MPEWatch_RPGPerf(data);
                        //    break;

                        //case "rpg_plan":
                        //    MPEWatch_RPGPlan(data);
                        //    break;

                        //case "dps_run_estm":
                        //    MPEWatch_DPSEst(data);
                        //    break;
                        ///*MPEWatch Data End*/
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }



        //private void Staffing(JObject data, string message_type)
        //{
        //    try
        //    {
        //        bool updatefile = false;
        //        if (data.HasValues)
        //        {
        //            IEnumerable<JToken> staff = data.SelectTokens("$..DATA[*]");
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
        //            if (sortplanlist.HasValues)
        //            {
        //                foreach (JObject Dataitem in sortplanlist.Children())
        //                {
        //                    if (!string.IsNullOrEmpty((string)Dataitem["sortplan"]))
        //                    {
        //                        string mach_type = Dataitem.ContainsKey("mach_type") ? (string)Dataitem["mach_type"] : "";
        //                        string machine_no = Dataitem.ContainsKey("machine_no") ? (string)Dataitem["machine_no"] : "";
        //                        string sortplan = Dataitem.ContainsKey("sortplan") ? (string)Dataitem["sortplan"] : "";
        //                        string sortplan_name = "";
        //                        if (mach_type == "APBS")
        //                        {
        //                            mach_type = "SPBSTS";
        //                        }
        //                        //int dotindex = sortplan.IndexOf(".", 1);
        //                        //if ((dotindex == -1))
        //                        //{
        //                        //    sortplan_name = sortplan.Trim();
        //                        //}
        //                        //else
        //                        //{
        //                        //    sortplan_name = sortplan.Substring(0, dotindex).Trim();
        //                        //}
        //                        if (Regex.IsMatch(mach_type, "(DBCS|AFSM100|ATU|CIOSS)", RegexOptions.IgnoreCase))
        //                        {
        //                            int dotindex = sortplan.IndexOf(".", 1);
        //                            if ((dotindex == -1))
        //                            {
        //                                sortplan_name = sortplan;
        //                            }
        //                            else
        //                            {
        //                                sortplan_name = sortplan.Substring(0, dotindex);
        //                            }
        //                            sortplan = sortplan_name;
        //                        }
        //                        string mch_sortplan_id = mach_type + "-" + machine_no + "-" + sortplan;

        //                        AppParameters.SortplansList.AddOrUpdate(mch_sortplan_id, Dataitem,
        //                             (key, existingVal) =>
        //                             {

        //                                 if (JsonConvert.SerializeObject(existingVal) == JsonConvert.SerializeObject(Dataitem))
        //                                     return existingVal;
        //                                 else
        //                                     updatefile = true;
        //                                 return Dataitem;
        //                             });



        //                    }
        //                }
        //            }
        //            if (updatefile)
        //            {
        //                new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Staffing.json", JsonConvert.SerializeObject(AppParameters.SortplansList.Select(x => x.Value).ToList(), Formatting.Indented));
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}

        private void CameraData(dynamic data)
        {
            try
            {
                if (data != null)
                {
                    List<Cameras> newCameras = JsonConvert.DeserializeObject<List<Cameras>>(data);
                    foreach (Cameras camera_item in newCameras)
                    {
                        AppParameters.CameraInfoList.AddOrUpdate(camera_item.CameraName, camera_item, (key, oldCameras) => {
                            return camera_item;
                        });
                    }
                }
               
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        //private static void MPE_Watch_Id(JObject data)
        //{
        //    try
        //    {
        //        string MpewatchID = "{\"MPE_WATCH_ID\":\"" + data["id"] + "\"}";
        //        FOTFManager.Instance.EditAppSettingdata(MpewatchID);
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}

        private static void Container(dynamic data)
        {

            try
            {

                if (data != null)
                {
                    //string siteId = (string)AppParameters.AppSettings["FACILITY_NASS_CODE"];
                    //JToken containerData = JToken.Parse(data);
                    //foreach (JObject containeritem in containerData)
                    //{
                    //    JObject _container = new JObject_List().Container;
                    //    _container["placardBarcode"] = containeritem["placardBarcode"].ToString();
                    //    _container["dest"] = containeritem.ContainsKey("dest") ? containeritem["dest"].ToString() :"";
                    //    _container["destinationName"] = containeritem.ContainsKey("destinationName") ? containeritem["destinationName"].ToString() : "";
                    //    if (containeritem["placardBarcode"].ToString().StartsWith("99M"))
                    //    {
                    //        _container["mailClass"] = "99M";
                    //        _container["mailClassDisplay"] = "Mailer";
                    //        _container["originName"] = "Mailer";
                    //    }
                    //    else
                    //    {
                    //        if (containeritem.ContainsKey("containerHistory"))
                    //        {
                    //            foreach (JObject scan in containeritem["containerHistory"])
                    //            {
                    //                _container["binDisplay"] = scan.ContainsKey("binName") || scan.ContainsKey("binNumber") ? scan.ContainsKey("binNumber") ? scan["binName"].ToString() : scan.ContainsKey("binNumber") ? scan["binNumber"].ToString() : "" : "";
                    //                if (scan["siteId"].ToString() == siteId)
                    //                {
                    //                    if (scan.ContainsKey("location") && scan["siteType"].ToString() == "Origin" && scan["event"].ToString() == "PASG" && scan["location"] != containeritem["location"])
                    //                    {
                    //                        _container["location"] = scan["location"];
                    //                    }
                    //                    _container["hasPrintScans"] = scan["event"].ToString() == "PRINT";
                    //                    _container["hasAssignScans"] = scan["event"].ToString() == "CLOS" || scan["event"].ToString() == "BCLS";
                    //                    _container["hasLoadScans"] = scan["event"].ToString() == "LOAD" ;
                    //                    _container["hasUnloadScans"] = scan["event"].ToString() == "UNLD";
                    //                    _container["hasAssignScans"] = scan["event"].ToString() == "PASG";
                    //                    _container["containerTerminate"] = scan["event"].ToString() == "TERM";

                    //                }
                    //            }

                    //        }
                    //    }
                    //    if (AppParameters.Containers.Count() > 0)
                    //    {
                    //        JObject temp = AppParameters.Containers.Where(r => r.ContainsKey("placardBarcode") && r["placardBarcode"].ToString() == containeritem["placardBarcode"].ToString()).FirstOrDefault();
                    //        if (temp != null && temp.Count > 0)
                    //        {
                    //           AppParameters.Containers.First(r => r["placardBarcode"].ToString() == containeritem["placardBarcode"].ToString())
                    //                .Merge(_container, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

                    //        }
                    //        else
                    //        {
                    //            AppParameters.Containers.Add(_container);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        AppParameters.Containers.Add(_container);
                    //    }
                    //    //if (AppParameters.Containers.ContainsKey(containeritem["placardBarcode"].ToString()))
                    //    //{
                    //    //    if(AppParameters.Containers.TryGetValue(containeritem["placardBarcode"].ToString(), out JToken m))
                    //    //    {
                    //    //        ((JObject)m).Merge(_container, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });  
                    //    //    }

                    //    //}
                    //    //else
                    //    //{
                    //    //    AppParameters.Containers.Add(containeritem["placardBarcode"].ToString(), _container);
                    //    //}

                    //}
                    //containerData = null;



                    List<Container> Containers = JsonConvert.DeserializeObject<List<Container>>(data);
                    string siteId = (string)AppParameters.AppSettings["FACILITY_NASS_CODE"];
                    foreach (Container d in Containers)
                    {
                        if (d.PlacardBarcode.StartsWith("99M"))
                        {
                            d.MailClass = "99M";
                            d.MailClassDisplay = "Mailer";
                            d.OriginName = "Mailer";
                            d.Origin = "";
                            d.Dest = "";
                            d.DestinationName = "";
                        }
                        int sortindex = 0;
                        foreach (ContainerHistory scan in d.ContainerHistory.OrderBy(s => s.EventDtmfmt))
                        {
                            if (d.EventDtm.Year == 1)
                            {
                                d.EventDtm = scan.EventDtmfmt;
                            }
                            sortindex++;
                            scan.sortind = sortindex;
                            d.binDisplay = scan.Event == "PASG" ? scan.BinName : "";
                            if (scan.SiteId == siteId)
                            {
                                if (scan.Event == "PASG")
                                {
                                    d.hasAssignScans = true;
                                }
                                if ((scan.Event == "CLOS" || scan.Event == "BCLS"))
                                {
                                    d.hasCloseScans = true;
                                }
                                if (scan.Event == "LOAD")
                                {
                                    d.hasLoadScans = true;
                                    d.Oroute = !string.IsNullOrEmpty(scan.Route) ? scan.Route : "";
                                    d.Otrip = !string.IsNullOrEmpty(scan.Trip) ? scan.Trip : "";
                                    d.Otrailer = !string.IsNullOrEmpty(scan.Trailer) ? scan.Trailer : "";
                                }
                                if (scan.Event == "UNLD")
                                {
                                    d.hasUnloadScans = true;
                                    d.Iroute = !string.IsNullOrEmpty(scan.Route) ? scan.Route : "";
                                    d.Itrip = !string.IsNullOrEmpty(scan.Trip) ? scan.Trip : "";
                                    d.Itrailer = !string.IsNullOrEmpty(scan.Trailer) ? scan.Trailer : "";

                                }
                                if (scan.Event == "PRINT")
                                {
                                    d.hasPrintScans = true;
                                }
                                if (scan.Event == "TERM")
                                {
                                    d.containerTerminate = true;
                                }
                                if (!string.IsNullOrEmpty(scan.Location) && scan.SiteType == "Origin" && scan.Event == "PASG" && scan.Location != d.Location)
                                {
                                    d.Location = scan.Location;
                                }

                            }
                            if (scan.Event == "TERM")
                            {
                                d.containerTerminate = true;
                            }
                            if (scan.Event == "UNLD" && scan.RedirectInd == "Y" && d.Dest == siteId)
                            {
                                d.containerRedirectedDest = true;
                            }

                            if (scan.Event == "UNLD" && scan.SiteType == "Destination")
                            {
                                if (scan.SiteId == d.Dest)
                                {
                                    d.containerAtDest = true;
                                }
                            }
                            if (scan.Event == "UNLD" && scan.SiteType == "Via")
                            {
                                if (scan.SiteId != d.Dest)
                                {
                                    d.containerRedirectedDest = true;
                                    d.Location = "X-Dock";
                                }
                            }
                        }
                        AppParameters.Containers.AddOrUpdate(d.PlacardBarcode, d, (key, oldD) =>
                        {

                            return d;
                        });
                    }
                    Containers = null;
                }

                if (AppParameters.Containers.Count > 0)
                {
                    foreach (string m in AppParameters.Containers.Where(r => DateTime.Now.Subtract(r.Value.EventDtm).TotalDays >= 3).Select(y => y.Key))
                    {
                        AppParameters.Containers.TryRemove(m, out Container outc);

                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }
        }

        //private static void Trips(dynamic jsonObject, string message_type)
        //{

        //    try
        //    {
        //        if (jsonObject != null)
        //        {
        //            foreach (JObject rt in jsonObject)
        //            {
        //                if (rt.ContainsKey("routeTripId") && rt.ContainsKey("routeTripLegId"))
        //                {
        //                    string routetripid = string.Concat(rt["routeTripId"].ToString(), rt["routeTripLegId"].ToString());
        //                    if (AppParameters.RouteTripsList.ContainsKey(routetripid))
        //                    {
        //                        if (AppParameters.RouteTripsList.TryGetValue(routetripid, out JObject existingVal))
        //                        {
        //                            if (rt.ToString() != existingVal["rawData"].ToString())
        //                            {
        //                                existingVal["Trip_Update"] = true;
        //                                existingVal.Merge(rt, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        //                                Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt["route"].ToString(), rt["trip"].ToString(), AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), GetSvDate((JObject)rt["operDate"])), rt["tripDirectionInd"].ToString(), routetripid));
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        rt["id"] = routetripid;
        //                        rt["notificationId"] = "";
        //                        rt["state"] = "";
        //                        rt["destSite"] = "";
        //                        rt["tripMin"] = 0;
        //                        rt["containers"] = "";
        //                        rt["Trip_Update"] = false;
        //                        rt["unloadedContainers"] = 0;
        //                        rt["rawData"] = rt;
        //                        if (AppParameters.RouteTripsList.TryAdd(routetripid, rt))
        //                        {
        //                            Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt["route"].ToString(), rt["trip"].ToString(), AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), GetSvDate((JObject)rt["operDate"])), rt["tripDirectionInd"].ToString(), routetripid));
        //                        }
        //                    }
        //                }

        //            }
        //        }
        //        if (AppParameters.RouteTripsList.Count > 0)
        //        {
        //            foreach(string m in AppParameters.RouteTripsList.Where(r => (int)r.Value["tripMin"] < -1440).Select(y => y.Key))
        //            {
        //                AppParameters.RouteTripsList.TryRemove(m, out JObject value);
        //            }
        //        }


        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        jsonObject = null;
        //    }

        //}

        //private static DateTime GetSvDate(JObject triptime)
        //{
        //    DateTime tripDtm = new DateTime((int)triptime["year"], ((int)triptime["month"] + 1), (int)triptime["dayOfMonth"], (int)triptime["hourOfDay"], (int)triptime["minute"], (int)triptime["second"]);
        //    return tripDtm;
        //}

        //private static JArray GetItinerary(string route, string trip, string nasscode , DateTime start_time)
        //{
        //    JArray temp = new JArray();
        //    try
        //    {
        //        //string start_time = string.Concat(DateTime.Now.ToString("yyyy-MM-dd'T'"), "00:00:00");

        //        Uri parURL = new Uri(string.Format((string)AppParameters.AppSettings["SV_ITINERARY"], route, trip, string.Concat(start_time.ToString("yyyy-MM-dd'T'"), "00:00:00")));
        //        string SV_Response = SendMessage.SV_Get(parURL.AbsoluteUri);
        //        if (!string.IsNullOrEmpty(SV_Response))
        //        {
        //            if (AppParameters.IsValidJson(SV_Response))
        //            {
        //                JArray itinerary = JArray.Parse(SV_Response);
        //                if (itinerary.HasValues)
        //                {
        //                   return itinerary;
        //                }
        //            } 
        //        }
        //        return temp;
        //    }
        //    catch (Exception e)
        //    {

        //        new ErrorLogger().ExceptionLog(e);
        //        return temp;
        //    }
        //}

        //private static void Doors(dynamic jsonObject, string message_type)
        //{
        //    try
        //    {
        //        bool update_info = false;
        //        if (jsonObject.HasValues)
        //        {
        //            string siteId = (string)AppParameters.AppSettings["FACILITY_NASS_CODE"];

        //            foreach (JObject item in jsonObject)
        //            {
        //                update_info = false;
        //                foreach (JObject m in AppParameters.ZonesList.Where(x => x.Value["properties"]["Zone_Type"].ToString() == "DockDoor"
        //                 && (int)x.Value["properties"]["doorNumber"] == (int)item["doorNumber"]).Select(l => l.Value))
        //                {
        //                    if (m["properties"]["Raw_Data"].ToString() != item.ToString())
        //                    {
        //                        if (item.ContainsKey("routeTripId") && item.ContainsKey("routeTripLegId"))
        //                        {
        //                            string routetripid = string.Concat(item["routeTripId"].ToString(), item["routeTripLegId"].ToString());
        //                            if (AppParameters.RouteTripsList.ContainsKey(routetripid))
        //                            {
        //                                if (AppParameters.RouteTripsList.TryGetValue(routetripid, out JObject trip))
        //                                {
        //                                    trip.Merge(item, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

        //                                    if (!string.IsNullOrEmpty(trip["destSite"].ToString()) && item.ContainsKey("trailerBarcode") && !string.IsNullOrEmpty(trip["trailerBarcode"].ToString()))
        //                                    {
        //                                        if (trip["tripDirectionInd"].ToString() == "O")
        //                                        {
        //                                            IEnumerable<Container> alltrailercontent = null;
        //                                            IEnumerable<Container> unloadedtrailerContent = AppParameters.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Dest)
        //                                            && Regex.IsMatch(r.Value.Dest, trip["destSite"].ToString(), RegexOptions.IgnoreCase)
        //                                            && r.Value.hasLoadScans == false
        //                                            && r.Value.containerTerminate == false
        //                                            && r.Value.containerAtDest == false
        //                                            && r.Value.hasCloseScans == true).Select(y => y.Value).ToList();

        //                                            if ((int)trip["unloadedContainers"] != unloadedtrailerContent.Count())
        //                                            {
        //                                                trip["unloadedContainers"] = unloadedtrailerContent.Count();
        //                                            }
        //                                            alltrailercontent = unloadedtrailerContent;
        //                                            if (!string.IsNullOrEmpty(item["trailerBarcode"].ToString()))
        //                                            {
        //                                                IEnumerable<Container> loadedtrailerContent = null;
        //                                                //for the loaded int the trailer
        //                                                loadedtrailerContent = AppParameters.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Otrailer)
        //                                                     && Regex.IsMatch(r.Value.Otrailer, item["trailerBarcode"].ToString(), RegexOptions.IgnoreCase)
        //                                                     && r.Value.hasLoadScans == true
        //                                                     ).Select(y => y.Value).ToList();
        //                                                alltrailercontent = unloadedtrailerContent.Concat(loadedtrailerContent);
        //                                                loadedtrailerContent = null;
        //                                            }
        //                                            trip["containers"] = JArray.Parse(JsonConvert.SerializeObject(alltrailercontent, Formatting.Indented));
        //                                            unloadedtrailerContent = null;
        //                                            alltrailercontent = null;
        //                                        }
        //                                        if (trip["tripDirectionInd"].ToString() == "I")
        //                                        {
        //                                            if (!string.IsNullOrEmpty(item["trailerBarcode"].ToString()))
        //                                            {
        //                                                IEnumerable<Container> unloadedtrailerContent = AppParameters.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Itrailer)
        //                                                && Regex.IsMatch(r.Value.Itrailer, item["trailerBarcode"].ToString(), RegexOptions.IgnoreCase)

        //                                                 ).Select(y => y.Value).ToList();
        //                                                trip["containers"] = JArray.Parse(JsonConvert.SerializeObject(unloadedtrailerContent, Formatting.Indented));
        //                                                unloadedtrailerContent = null;

        //                                            }
        //                                        }
        //                                    }
        //                                    m["properties"]["dockDoorData"] = trip;
        //                                    m["properties"]["Raw_Data"] = trip;

        //                                }
        //                            }
        //                            else
        //                            {
        //                                item["id"] = routetripid;
        //                                item["state"] = "";
        //                                item["destSite"] = "";
        //                                item["tripMin"] = 0;
        //                                item["containers"] = "";
        //                                item["unloadedContainers"] = 0;
        //                                if (item.ContainsKey("scheduledDtm"))
        //                                {
        //                                    item["tripMin"] = AppParameters.Get_TripMin((JObject)item["scheduledDtm"]);
        //                                }

        //                                m["properties"]["dockDoorData"] = item;
        //                                m["properties"]["Raw_Data"] = item;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            m["properties"]["dockDoorData"] = item;
        //                            m["properties"]["Raw_Data"] = item;
        //                        }

        //                        update_info = true;
        //                    }

        //                    if (update_info)
        //                    {
        //                        m["properties"]["Zone_Update"] = true;
        //                    }

        //                }
        //            }
        //            jsonObject = null;

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        jsonObject = null;
        //    }
        //}

        //private static void TacsVsSelsLDCAnomaly(JArray data, string message_type)
        //{
        //    /**
        //* {
        //    "empId": "04692142",
        //    "tagId": "ca0800004878",
        //    "tagName": "Transportation_0086",
        //    "processed": "21-08-13 09:21:20",
        //    "hasTACS": true,
        //    "currentZones": [
        //        {
        //            "id": "da09be6b-ac97-49d5-924d-1c6eae24e430",
        //            "name": "Dock"
        //        }
        //    ],
        //    "selsToTacsTotalTime": 212,
        //    "isLdcAlert": true,
        //    "tacs": {
        //        "ldc": "34",
        //        "totalTime": 7991,
        //        "operationId": "766",
        //        "payLocation": "n/a",
        //        "isOvertimeAuth": false,
        //        "overtimeHours": 0,
        //        "isOvertime": false
        //    },
        //    "sels": {
        //        "totalTime": 16975,
        //        "currentLDCs": [
        //            "17"
        //        ],
        //        "timeByLDC": {
        //            "17": {
        //                "ldc": 17,
        //                "time": 16551,
        //                "ldcToSelsTotalTime": 98,
        //                "ldcToTacsTotalTime": 207
        //            },
        //            "19": {
        //                "ldc": 19,
        //                "time": 424,
        //                "ldcToSelsTotalTime": 2,
        //                "ldcToTacsTotalTime": 5
        //            }
        //        }
        //    }
        //}
        //*/
        //    try
        //    {
        //        if (data.HasValues)
        //        {

        //            foreach (JObject item in data)
        //            {
        //                if (AppParameters.Tag.ContainsKey((string)item["tagId"]))
        //                {
        //                   foreach(JObject m in AppParameters.Tag.Where(r => r.Key == (string)item["tagId"]).Select(y => y.Value))
        //                    {
        //                        //tag.Merge(item, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace });
        //                        //tag["properties"]["Tag_Update"] = true;

        //                        if (m["properties"]["empId"] != item["empId"])
        //                        {
        //                            m["properties"]["empId"] = item["empId"];

        //                        }
        //                        if (m["properties"]["tacs"].ToString() != item["tacs"].ToString())
        //                        {
        //                            m["properties"]["tacs"] = item["tacs"];

        //                        }
        //                        if (m["properties"]["sels"].ToString() != item["sels"].ToString())
        //                        {
        //                            m["properties"]["sels"] = item["sels"];

        //                        }
        //                        if (m["properties"]["isLdcAlert"].ToString() != item["isLdcAlert"].ToString())
        //                        {
        //                            m["properties"]["isLdcAlert"] = item["isLdcAlert"];

        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    JObject GeoJsonType = new JObject_List().GeoJSON_Tag;
        //                    JArray temp_zone = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
        //                    ((JObject)GeoJsonType["geometry"])["type"] = "Point";
        //                    GeoJsonType["geometry"]["coordinates"] = new JArray(0, 0);
        //                    //((JObject)GeoJsonType["properties"]).Merge(item, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Replace });

        //                    GeoJsonType["properties"]["id"] = (string)item["tagId"];
        //                    GeoJsonType["properties"]["zones"] = temp_zone;
        //                    GeoJsonType["properties"]["name"] = item["tagName"];
        //                    GeoJsonType["properties"]["Tag_Type"] = "Person";
        //                    GeoJsonType["properties"]["isLdcAlert"] = item["isLdcAlert"];
        //                    GeoJsonType["properties"]["empId"] = item["empId"];
        //                    GeoJsonType["properties"]["tacs"] = item["tacs"];
        //                    GeoJsonType["properties"]["sels"] = item["sels"];
        //                    GeoJsonType["properties"]["positionTS"] = DateTime.Now.AddDays(-10).ToUniversalTime();
        //                    //add to the tags
        //                    if (!AppParameters.Tag.ContainsKey((string)item["tagId"]))
        //                    {
        //                        AppParameters.Tag.TryAdd((string)item["tagId"], GeoJsonType);
        //                    }
        //                }
        //            }

        //            if (AppParameters.Tag.Count() > 0)
        //            {
        //                foreach(JObject m in AppParameters.Tag.Where(u => u.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")).Select(x => x.Value))
        //                {
        //                    if (data.SelectTokens("[?(@.tagId)]").Where(i => (string)i["tagId"] == (string)m["properties"]["id"]).ToList().Count == 0)
        //                    {
        //                        if (((JObject)m["properties"]).ContainsKey("isLdcAlert") && (bool)m["properties"]["isLdcAlert"])
        //                        {
        //                            m["properties"]["isLdcAlert"] = false;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        //private static void TacsVsSels(JObject data, string message_type)
        //{
        //    // "processedSince": "21-08-12 09:08:42",
        //    //"missedSelsCount": 561,
        //    //"missedSels": [
        //    //    {
        //    //        "empId": "01055142",
        //    //        "tagId": "ca0800004400",
        //    //        "tagName": "n/a",
        //    //        "type": "n/a",
        //    //        "tacs": {
        //    //            "ldc": "36",
        //    //            "totalTime": 8393,
        //    //            "operationId": "750",
        //    //            "payLocation": "n/a",
        //    //            "isOvertimeAuth": false,
        //    //            "overtimeHours": 0,
        //    //            "isOvertime": false
        //    //        }
        //    //    }}
        //    try
        //    {
        //        if (data.HasValues)
        //        {
        //            if (data.ContainsKey("missedSels"))
        //            {
        //                JToken missedSels = data.SelectToken("missedSels");
        //                if (missedSels != null)
        //                {
        //                    foreach (JObject item in missedSels.Children())
        //                    {
        //                        if (AppParameters.Tag.ContainsKey((string)item["tagId"]))
        //                        {
        //                            foreach(JObject m in AppParameters.Tag.Where(r => r.Key == (string)item["tagId"]).Select(y => y.Value))
        //                            {

        //                                if ((bool)m["properties"]["isWearingTag"] == true)
        //                                {
        //                                    m["properties"]["isWearingTag"] = false;
        //                                }
        //                                if (m["properties"]["empId"] != item["empId"])
        //                                {
        //                                    m["properties"]["empId"] = item["empId"];
        //                                }
        //                                if (m["properties"]["tacs"].ToString() != item["tacs"].ToString())
        //                                {
        //                                    m["properties"]["tacs"] = item["tacs"];
        //                                }

        //                            }
        //                        }
        //                        else
        //                        {
        //                            JObject GeoJsonType = new JObject_List().GeoJSON_Tag;
        //                            JArray temp_zone = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
        //                            ((JObject)GeoJsonType["geometry"])["type"] = "Point";
        //                            GeoJsonType["geometry"]["coordinates"] = new JArray(0, 0);
        //                            GeoJsonType["properties"]["id"] = (string)item["tagId"];
        //                            GeoJsonType["properties"]["zones"] = temp_zone;
        //                            GeoJsonType["properties"]["name"] = item["tagName"];
        //                            GeoJsonType["properties"]["Tag_Type"] = "Person";
        //                            GeoJsonType["properties"]["empId"] = item["empId"];
        //                            GeoJsonType["properties"]["tacs"] = item["tacs"];
        //                            GeoJsonType["properties"]["positionTS"] = DateTime.Now.AddDays(-10).ToUniversalTime();

        //                            //add to the tags
        //                            if (!AppParameters.Tag.ContainsKey((string)item["tagId"]))
        //                            {
        //                                AppParameters.Tag.TryAdd((string)item["tagId"], GeoJsonType);
        //                            }
        //                        }
        //                    }
        //                    if (AppParameters.Tag.Count() > 0)
        //                    {
        //                       foreach(JObject m in  AppParameters.Tag.Where(u => u.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")).Select(x => x.Value))
        //                        {
        //                            if (missedSels.SelectTokens("[?(@.tagId)]").Where(i => (string)i["tagId"] == (string)m["properties"]["id"]).ToList().Count == 0)
        //                            {
        //                                if (!(bool)m["properties"]["isWearingTag"])
        //                                {
        //                                    m["properties"]["isWearingTag"] = true;

        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        ////private static void CTS_LocalDockDeparted(JObject data)
        ////{
        ////    //"Scheduled": "5/6/2021 1:45 PM",
        ////    //"Departed": "",
        ////    //"Door": null,
        ////    //"Leg": "974",
        ////    //"Route": "975L0",
        ////    //"Trip": "116",
        ////    //"Destination": "EUGENE P & D F",
        ////    //"Assigned": 31,
        ////    //"Closed": 47,
        ////    //"Staged": 22,
        ////    //"XDock": 19,
        ////    //"MLD": 0,
        ////    //"PLD": 0,
        ////    //"MTLD": 0,
        ////    //"Load": 0,
        ////    //"LoadPercent": 0,
        ////    //"Total": 88
        ////    //}
        ////    try
        ////    {
        ////        if (data.HasValues)
        ////        {
        ////            if (data.ContainsKey("Data"))
        ////            {
        ////                JToken cts_data = data.SelectToken("Data");

        ////                if (cts_data.Children().Count() > 0)
        ////                {
        ////                    foreach (JObject Dataitem in cts_data.Children())
        ////                    {
        ////                        bool update = false;
        ////                        string trip = Dataitem.ContainsKey("Trip") ? (string)Dataitem["Trip"] : "";
        ////                        string route = Dataitem.ContainsKey("Route") ? (string)Dataitem["Route"] : "";
        ////                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
        ////                        {
        ////                            //if (cts_site.HasValues)
        ////                            //{
        ////                            //    if (cts_site.ContainsKey("TimeZone"))
        ////                            //    {
        ////                            //        DateTime scheduledTime = DateTime.Parse((string)Dataitem["Scheduled"]);
        ////                            //        Dataitem["Scheduled"] = TimeZoneInfo.ConvertTime(scheduledTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site["TimeZone"]));

        ////                            //        if (!string.IsNullOrEmpty((string)Dataitem["Departed"]))
        ////                            //        {
        ////                            //            DateTime departedTime = DateTime.Parse((string)Dataitem["Departed"]);
        ////                            //            Dataitem["Departed"] = TimeZoneInfo.ConvertTime(departedTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site["TimeZone"]));
        ////                            //        }
        ////                            //    }

        ////                            //}
        ////                            if (!AppParameters.CTS_LocalDockDeparted.ContainsKey(route + trip))
        ////                            {
        ////                                Dataitem.Add(new JProperty("CTS_Update", true));
        ////                                Dataitem.Add(new JProperty("CTS_Remove", false));
        ////                                AppParameters.CTS_LocalDockDeparted.TryAdd(route + trip, Dataitem);
        ////                            }
        ////                            else
        ////                            {
        ////                                if (AppParameters.CTS_LocalDockDeparted.TryGetValue(route + trip, out JObject cts_item))
        ////                                {
        ////                                    foreach (dynamic kv in Dataitem.Children())
        ////                                    {
        ////                                        if (cts_item.ContainsKey(kv.Name))
        ////                                        {
        ////                                            if (cts_item[kv.Name].ToString() != kv.Value.ToString())
        ////                                            {
        ////                                                cts_item[kv.Name] = Dataitem[kv.Name].ToString();
        ////                                                update = true;
        ////                                            }
        ////                                        }
        ////                                    }
        ////                                    if (update)
        ////                                    {
        ////                                        cts_item["CTS_Update"] = true;
        ////                                    }
        ////                                }
        ////                            }
        ////                        }
        ////                    }
        ////                }

        ////                //check if the data have been removed.
        ////                if (AppParameters.CTS_LocalDockDeparted.Count() > 0)
        ////                {
        ////                    foreach (JObject item in AppParameters.CTS_LocalDockDeparted.Select(x => x.Value))
        ////                    {
        ////                        string trip = item.ContainsKey("Trip") ? (string)item["Trip"] : "";
        ////                        string route = item.ContainsKey("Route") ? (string)item["Route"] : "";
        ////                        var exsiting = cts_data.SelectTokens("[?(@.Route)]").Where(i => (string)i["Route"] == (string)item["Route"] && (string)i["Trip"] == (string)item["Trip"]).ToList();

        ////                        if (exsiting.Count == 0)
        ////                        {
        ////                            if (AppParameters.CTS_LocalDockDeparted.TryGetValue(route + trip, out JObject cts_item))
        ////                            {
        ////                                if (cts_item.ContainsKey("CTS_Remove"))
        ////                                {
        ////                                    cts_item["CTS_Remove"] = true;
        ////                                    cts_item["CTS_Update"] = true;
        ////                                }
        ////                                else
        ////                                {
        ////                                    cts_item.Add(new JProperty("CTS_Remove", true));
        ////                                    cts_item["CTS_Update"] = true;
        ////                                }
        ////                            };
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////    }
        ////}

        ////private static void CTS_Outbound(JObject data)
        ////{
        ////    //{
        ////    //"Scheduled": "5/6/2021 12:25 PM",
        ////    //"Actual": "5/6/2021 12:18 PM",
        ////    //"RouteID": "972VS",
        ////    //"TripID": "B3001",
        ////    //"FirstLegDest": "97213",
        ////    //"FirstLegSite": "ROSE CITY PARK",
        ////    //"FinalDest": "972",
        ////    //"FinalDestSite": "PORTLAND OR P&DC"
        ////    //}
        ////    try
        ////    {
        ////        if (data.HasValues)
        ////        {
        ////            if (data.ContainsKey("Data"))
        ////            {
        ////                JToken cts_data = data.SelectToken("Data");

        ////                if (cts_data.Children().Count() > 0)
        ////                {
        ////                    foreach (JObject Dataitem in cts_data.Children())
        ////                    {
        ////                        bool update = false;
        ////                        string trip = Dataitem.ContainsKey("TripID") ? (string)Dataitem["TripID"] : "";
        ////                        string route = Dataitem.ContainsKey("RouteID") ? (string)Dataitem["RouteID"] : "";
        ////                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
        ////                        {
        ////                            //if (cts_site.HasValues)
        ////                            //{
        ////                            //    if (cts_site.ContainsKey("TimeZone"))
        ////                            //    {
        ////                            //        DateTime scheduledTime = DateTime.Parse((string)Dataitem["Scheduled"]);
        ////                            //        Dataitem["Scheduled"] = TimeZoneInfo.ConvertTime(scheduledTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site["TimeZone"]));

        ////                            //        if (!string.IsNullOrEmpty((string)Dataitem["Actual"]))
        ////                            //        {
        ////                            //            DateTime departedTime = DateTime.Parse((string)Dataitem["Actual"]);
        ////                            //            Dataitem["Actual"] = TimeZoneInfo.ConvertTime(departedTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site["TimeZone"]));
        ////                            //        }
        ////                            //    }

        ////                            //}
        ////                            if (!AppParameters.CTS_Outbound_Schedualed.ContainsKey(route + trip))
        ////                            {
        ////                                Dataitem.Add(new JProperty("CTS_Update", true));
        ////                                Dataitem.Add(new JProperty("CTS_Remove", false));
        ////                                Dataitem.Add(new JProperty("doorNumber", ""));
        ////                                AppParameters.CTS_Outbound_Schedualed.TryAdd(route + trip, Dataitem);
        ////                            }
        ////                            else
        ////                            {
        ////                                if (AppParameters.CTS_Outbound_Schedualed.TryGetValue(route + trip, out JObject cts_item))
        ////                                {
        ////                                    foreach (dynamic kv in Dataitem.Children())
        ////                                    {
        ////                                        if (cts_item.ContainsKey(kv.Name))
        ////                                        {
        ////                                            if (cts_item[kv.Name].ToString() != kv.Value.ToString())
        ////                                            {
        ////                                                cts_item[kv.Name] = Dataitem[kv.Name].ToString();
        ////                                                update = true;
        ////                                            }
        ////                                        }
        ////                                    }
        ////                                    if (update)
        ////                                    {
        ////                                        cts_item["CTS_Update"] = true;
        ////                                    }
        ////                                }
        ////                            }
        ////                        }
        ////                    }
        ////                }

        ////                // check if the data have been removed.
        ////                if (AppParameters.CTS_Outbound_Schedualed.Count() > 0)
        ////                {
        ////                    foreach (JObject item in AppParameters.CTS_Outbound_Schedualed.Select(x => x.Value))
        ////                    {
        ////                        string trip = item.ContainsKey("TripID") ? (string)item["TripID"] : "";
        ////                        string route = item.ContainsKey("RouteID") ? (string)item["RouteID"] : "";
        ////                        var exsiting = cts_data.SelectTokens("[?(@.RouteID)]").Where(i => (string)i["RouteID"] == (string)item["RouteID"] && (string)i["TripID"] == (string)item["TripID"]).ToList();

        ////                        if (exsiting.Count == 0)
        ////                        {
        ////                            if (AppParameters.CTS_Outbound_Schedualed.TryGetValue(route + trip, out JObject cts_item))
        ////                            {
        ////                                if (cts_item.ContainsKey("CTS_Remove"))
        ////                                {
        ////                                    cts_item["CTS_Remove"] = true;
        ////                                    cts_item["CTS_Update"] = true;
        ////                                }
        ////                                else
        ////                                {
        ////                                    cts_item.Add(new JProperty("CTS_Remove", true));
        ////                                    cts_item["CTS_Update"] = true;
        ////                                }
        ////                            }
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////    }
        ////}

        ////private static void CTS_Inbound(JObject data)
        ////{
        ////    //{
        ////    //"Scheduled": "5/6/2021 12:15 PM",
        ////    //"Actual": "",
        ////    //"RouteID": "972VS",
        ////    //"TripID": "B2142",
        ////    //"LegOrigin": "972PS",
        ////    //"SiteName": "PLANET EXPRESS SHIPPING LLC"
        ////    //}
        ////    try
        ////    {
        ////        if (data.HasValues)
        ////        {
        ////            if (data.ContainsKey("Data"))
        ////            {
        ////                JToken cts_data = data.SelectToken("Data");
        ////                JObject cts_site = (JObject)data.SelectToken("Site");
        ////                if (cts_data.Children().Count() > 0)
        ////                {
        ////                    foreach (JObject Dataitem in cts_data.Children())
        ////                    {
        ////                        bool update = false;
        ////                        string trip = Dataitem.ContainsKey("TripID") ? (string)Dataitem["TripID"] : "";
        ////                        string route = Dataitem.ContainsKey("RouteID") ? (string)Dataitem["RouteID"] : "";
        ////                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
        ////                        {
        ////                            //if (cts_site.HasValues)
        ////                            //{
        ////                            //    if (cts_site.ContainsKey("TimeZone"))
        ////                            //    {
        ////                            //        DateTime scheduledTime = DateTime.Parse((string)Dataitem["Scheduled"]);
        ////                            //        Dataitem["Scheduled"] = TimeZoneInfo.ConvertTime(scheduledTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site["TimeZone"]));

        ////                            //        if (!string.IsNullOrEmpty((string)Dataitem["Actual"]))
        ////                            //        {
        ////                            //            DateTime departedTime = DateTime.Parse((string)Dataitem["Actual"]);
        ////                            //            Dataitem["Actual"] = TimeZoneInfo.ConvertTime(departedTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site["TimeZone"]));
        ////                            //        }
        ////                            //    }

        ////                            //}
        ////                            if (!AppParameters.CTS_Inbound_Schedualed.ContainsKey(route + trip))
        ////                            {
        ////                                Dataitem.Add(new JProperty("CTS_Update", true));
        ////                                Dataitem.Add(new JProperty("CTS_Remove", false));
        ////                                Dataitem.Add(new JProperty("doorNumber", ""));
        ////                                AppParameters.CTS_Inbound_Schedualed.TryAdd(route + trip, Dataitem);
        ////                            }
        ////                            else
        ////                            {
        ////                                if (AppParameters.CTS_Inbound_Schedualed.TryGetValue(route + trip, out JObject cts_item))
        ////                                {
        ////                                    foreach (dynamic kv in Dataitem.Children())
        ////                                    {
        ////                                        if (cts_item.ContainsKey(kv.Name))
        ////                                        {
        ////                                            if (cts_item[kv.Name].ToString() != kv.Value.ToString())
        ////                                            {
        ////                                                cts_item[kv.Name] = Dataitem[kv.Name].ToString();
        ////                                                update = true;
        ////                                            }
        ////                                        }
        ////                                    }
        ////                                    if (update)
        ////                                    {
        ////                                        cts_item["CTS_Update"] = true;
        ////                                    }
        ////                                }
        ////                            }
        ////                        }
        ////                    }
        ////                }

        ////                // check if the data have been removed.
        ////                if (AppParameters.CTS_Inbound_Schedualed.Count() > 0)
        ////                {
        ////                    foreach (JObject item in AppParameters.CTS_Inbound_Schedualed.Select(x => x.Value))
        ////                    {
        ////                        string trip = item.ContainsKey("TripID") ? (string)item["TripID"] : "";
        ////                        string route = item.ContainsKey("RouteID") ? (string)item["RouteID"] : "";
        ////                        var exsiting = cts_data.SelectTokens("[?(@.RouteID)]").Where(i => (string)i["RouteID"] == (string)item["RouteID"] && (string)i["TripID"] == (string)item["TripID"]).ToList();

        ////                        if (exsiting.Count == 0)
        ////                        {
        ////                            if (AppParameters.CTS_Inbound_Schedualed.TryGetValue(route + trip, out JObject cts_item))
        ////                            {
        ////                                if (cts_item.ContainsKey("CTS_Remove"))
        ////                                {
        ////                                    cts_item["CTS_Remove"] = true;
        ////                                    cts_item["CTS_Update"] = true;
        ////                                }
        ////                                else
        ////                                {
        ////                                    cts_item.Add(new JProperty("CTS_Remove", true));
        ////                                    cts_item["CTS_Update"] = true;
        ////                                }
        ////                            }
        ////                        }
        ////                    }
        ////                }
        ////            }
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////    }
        ////}

        //private static void P2PBySite(JObject data, string message_type)
        //{
        //    try
        //    {
        //        if (data.HasValues)
        //        {
        //            if (!data.ContainsKey("localdata"))
        //            {
        //                data.Add(new JProperty("localdata", true));
        //                new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "P2PData.json", JsonConvert.SerializeObject(data, Formatting.Indented));
        //            }
        //            JToken sortplanlist = null;
        //            if (data.ContainsKey(message_type))
        //            {
        //                sortplanlist = data.SelectToken(message_type);
        //                if (sortplanlist != null)
        //                {
        //                    if (sortplanlist.HasValues)
        //                    {
        //                        foreach (JObject Dataitem in sortplanlist.Children())
        //                        {
        //                            if (!string.IsNullOrEmpty((string)Dataitem["sortplan"]))
        //                            {
        //                                string mach_type = Dataitem.ContainsKey("mach_type") ? (string)Dataitem["mach_type"] : "";
        //                                string machine_no = Dataitem.ContainsKey("machine_no") ? (string)Dataitem["machine_no"] : "";
        //                                string sortplan = Dataitem.ContainsKey("sortplan") ? (string)Dataitem["sortplan"] : "";
        //                                string sortplan_name = "";
        //                                int dotindex = sortplan.IndexOf(".", 1);
        //                                if ((dotindex == -1))
        //                                {
        //                                    sortplan_name = sortplan.Trim();
        //                                }
        //                                else
        //                                {
        //                                    sortplan_name = sortplan.Substring(0, dotindex).Trim();
        //                                }

        //                                if (AppParameters.SortplansList.ContainsKey(mach_type + "-" + machine_no + "-" + sortplan_name))
        //                                {
        //                                    if (AppParameters.SortplansList.TryGetValue(mach_type + "-" + machine_no + "-" + sortplan_name, out JObject existingVa))
        //                                    {
        //                                        foreach (dynamic kv in Dataitem.Children())
        //                                        {
        //                                            if (existingVa.ContainsKey(kv.Name))
        //                                            {
        //                                                if (existingVa[kv.Name] != kv.Value)
        //                                                {
        //                                                    existingVa[kv.Name] = kv.Value;
        //                                                }
        //                                            }
        //                                            else
        //                                            {
        //                                                existingVa.Add(new JProperty(kv.Name, kv.Value));
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (AppParameters.SortplansList.TryAdd(mach_type + "-" + machine_no + "-" + sortplan_name, Dataitem))
        //                                    {
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        //    if (AppParameters.MachineZones.Count > 0)
        //        //{
        //        //    JToken machineInfo = data.SelectToken("Doors");
        //        //    foreach (var MachineZonesitem in AppParameters.MachineZones)
        //        //    {
        //        //        if ((string)MachineZonesitem.Value["properties"]["Zone_Type"] == "Machine")
        //        //        {
        //        //            string mzname = (string)MachineZonesitem.Value["properties"]["name"];
        //        //            if (!string.IsNullOrEmpty(mzname))
        //        //            {
        //        //                string[] separators = { "_", "-" };
        //        //                string[] mznamesplit = mzname.Split(separators, StringSplitOptions.None);
        //        //                if (mznamesplit.Length > 1)
        //        //                {
        //        //                    string mname = mznamesplit[0];
        //        //                    int.TryParse(mznamesplit[1], out int mnumber);

        //        //                    IEnumerable<JToken> mchinesortplans = machineInfo.Where(i => (string)i["mach_type"] == mname && (int)i["machine_no"] == mnumber).ToList();
        //        //                    if (mchinesortplans.Count() > 0)
        //        //                    {
        //        //                        MachineZonesitem.Value["properties"]["P2PData"] = new JArray(mchinesortplans);
        //        //                        update_info = true;
        //        //                    }
        //        //                    if (update_info)
        //        //                    {
        //        //                        MachineZonesitem.Value["properties"]["Zone_Update"] = true;
        //        //                    }
        //        //                }

        //        //            }

        //        //        }
        //        //    }

        //        //}
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        //private static void MPEWatch_RPGPerf(JObject data)
        //{
        //    try
        //    {
        //        bool update_info = false;
        //        string machine_type = "";
        //        string machine_number = "";
        //        string total_volume = "";
        //        string sortplan = "";
        //        string estCompletionTime = "";
        //        if (data != null && data.HasValues)
        //        {
        //            JToken machineInfo = data.SelectToken("data");
        //            if (machineInfo != null)
        //            {
        //                foreach (JObject item in machineInfo.Children())
        //                {
        //                    machine_type = item.ContainsKey("mpe_type") ? item["mpe_type"].ToString().Trim() : "";
        //                    machine_number = item.ContainsKey("mpe_number") ? item["mpe_number"].ToString().PadLeft(3, '0') : "";
        //                    sortplan = item.ContainsKey("cur_sortplan") ? item["cur_sortplan"].ToString() : "";
        //                    //if (!string.IsNullOrEmpty(machine_type))
        //                    //{
        //                    //    if (machine_type.ToUpper().Trim() == "SPBSTS")
        //                    //    {
        //                    //        machine_type = "APBS";
        //                    //    }
        //                    //    if (machine_type.ToUpper().Trim() == "FSFSSC")
        //                    //    {
        //                    //        machine_type = "FSS";
        //                    //    }
        //                    //}

        //                    total_volume = item.ContainsKey("tot_sortplan_vol") ? item["tot_sortplan_vol"].ToString().Trim() : "0";
        //                    int.TryParse(item.ContainsKey("rpg_est_vol") ? item["rpg_est_vol"].ToString().Trim() : "0", out int rpg_volume);
        //                    double.TryParse(item.ContainsKey("rpg_est_vol") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out double throughput);
        //                    int.TryParse(item.ContainsKey("rpg_est_vol") ? item["tot_sortplan_vol"].ToString().Trim() : "0", out int piecesfed);

        //                    if (rpg_volume > 0 && throughput > 0)
        //                    {
        //                        DateTime dtNow = DateTime.Now;
        //                        if (!string.IsNullOrEmpty((string)AppParameters.AppSettings["FACILITY_TIMEZONE"]))
        //                        {
        //                            if (AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings["FACILITY_TIMEZONE"], out string windowsTimeZoneId))
        //                            {
        //                                dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
        //                            }
        //                        }

        //                        double intMinuteToCompletion = (rpg_volume - piecesfed) / (throughput / 60);
        //                        DateTime dtEstCompletion = dtNow.AddMinutes(intMinuteToCompletion);
        //                        estCompletionTime = dtEstCompletion.ToString("yyyy-MM-dd HH:mm:ss");
        //                        item.Add("rpg_est_comp_time", estCompletionTime);
        //                    }
        //                    else
        //                    {
        //                        item.Add("rpg_est_comp_time", "");
        //                    }

        //                    if (item["current_run_end"].ToString() == "" && item["current_run_start"].ToString() != "")
        //                    {
        //                        JObject results = new Oracle_DB_Calls().Get_RPG_Plan_Info(item);
        //                        item.Add("rpg_start_dtm", results.ContainsKey("rpg_start_dtm") ? results["rpg_start_dtm"].ToString().Trim() : "");
        //                        item.Add("rpg_end_dtm", results.ContainsKey("rpg_end_dtm") ? results["rpg_end_dtm"].ToString().Trim() : "");
        //                        item.Add("expected_throughput", results.ContainsKey("expected_throughput") ? results["expected_throughput"].ToString().Trim() : "");
        //                    }
        //                    else
        //                    {
        //                        item.Add("rpg_start_dtm", "");
        //                        item.Add("rpg_end_dtm", "");
        //                    }
        //                    AppParameters.ZonesList.Where(x => x.Value["properties"]["Zone_Type"].ToString() == "Machine" &&
        //                    x.Value["properties"]["MPE_Type"].ToString() == item["mpe_type"].ToString() && x.Value["properties"]["MPE_Number"].ToString() == item["mpe_number"].ToString())
        //                       .Select(l => l.Value).ToList()
        //                       .ForEach(existingVa =>
        //                       {
        //                           if (!string.IsNullOrEmpty(sortplan))
        //                           {
        //                               existingVa["properties"]["P2PData"] = GetP2PSortplan(machine_type, machine_number, sortplan);
        //                           }
        //                           if (existingVa["properties"]["MPEWatchData"].ToString() != item.ToString())
        //                           {
        //                               existingVa["properties"]["MPEWatchData"] = item;
        //                               update_info = true;
        //                           }

        //                           if (update_info)
        //                           {
        //                               existingVa["properties"]["Zone_Update"] = true;
        //                           }

        //                       });
        //                }
        //            }
        //            machineInfo = null;
        //            data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogger().ExceptionLog(ex);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        //private static JToken GetP2PSortplan(string machine_type, string machine_number, string sortplan)
        //{
        //    try
        //    {
        //        JObject return_result = new JObject();
        //        if (Regex.IsMatch(machine_type, "(DBCS|AFSM100|ATU|CIOSS)", RegexOptions.IgnoreCase))
        //        {
        //            string sortplan_name = "";
        //            int dotindex = sortplan.IndexOf(".", 1);
        //            if ((dotindex == -1))
        //            {
        //                sortplan_name = sortplan;
        //            }
        //            else
        //            {
        //                sortplan_name = sortplan.Substring(0, dotindex);
        //            }
        //            sortplan = sortplan_name;
        //        }
        //        string id = machine_type + "-" + Convert.ToInt32(machine_number) + "-" + sortplan;
        //        if (AppParameters.SortplansList.ContainsKey(id))
        //        {
        //            if (AppParameters.SortplansList.TryGetValue(id, out JObject sp))
        //            {
        //                return sp;
        //            }
        //        }

        //        return return_result;
        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogger().ExceptionLog(ex);
        //        return new JObject();
        //    }
        //}

        //private static void MPEWatch_RPGPlan(JObject data)
        //{
        //    try
        //    {
        //        if (data.HasValues)
        //        {
        //            JToken planInfo = data.SelectToken("data");
        //            if (planInfo != null && planInfo.HasValues)
        //            {
        //                new Oracle_DB_Calls().Insert_RPG_Plan(data);
        //            }
        //            planInfo = null;
        //            data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogger().ExceptionLog(ex);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        //private static void MPEWatch_DPSEst(JObject data)
        //{
        //    try
        //    {
        //        int time_to_comp_optimal = 0;
        //        int time_to_comp_actual = 0;
        //        string time_to_comp_optimal_DateTime = "";
        //        string time_to_comp_actual_DateTime = "";
        //        DateTime dtNow = DateTime.Now;
        //        if (!string.IsNullOrEmpty((string)AppParameters.AppSettings["FACILITY_TIMEZONE"]))
        //        {
        //            if (AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings["FACILITY_TIMEZONE"], out string windowsTimeZoneId))
        //            {
        //                dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
        //            }
        //        }

        //        if (data.HasValues && AppParameters.ZonesList.Count > 0)
        //        {
        //            JToken dpsInfo = data.SelectToken("data");
        //            if (dpsInfo != null)
        //            {
        //                if (dpsInfo.HasValues)
        //                {
        //                    foreach (JObject item in dpsInfo.Children())
        //                    {
        //                        int.TryParse(item.ContainsKey("time_to_comp_optimal") ? item["time_to_comp_optimal"].ToString().Trim() : "0", out time_to_comp_optimal);
        //                        DateTime dtCompOptimal = dtNow.AddSeconds(time_to_comp_optimal);
        //                        time_to_comp_optimal_DateTime = dtCompOptimal.ToString("yyyy-MM-dd HH:mm:ss");
        //                        item.Add("time_to_comp_optimal_DateTime", time_to_comp_optimal_DateTime);

        //                        int.TryParse(item.ContainsKey("time_to_comp_actual") ? item["time_to_comp_actual"].ToString().Trim() : "0", out time_to_comp_actual);
        //                        DateTime dtCompActual = dtNow.AddSeconds(time_to_comp_actual);
        //                        time_to_comp_actual_DateTime = dtCompActual.ToString("MM/dd/yyyy HH:mm:ss");
        //                        item.Add("time_to_comp_actual_DateTime", time_to_comp_actual_DateTime);

        //                        string strSortPlan = item.ContainsKey("sortplan_name_perf") ? item["sortplan_name_perf"].ToString().Trim() : "";
        //                        string[] strSortPlanList = strSortPlan.Split(',').Select(x => x.Trim()).ToArray();
        //                        foreach (string strSP in strSortPlanList)
        //                        {
        //                            string strSortPlanItem = strSP;

        //                            if (!string.IsNullOrEmpty(strSortPlanItem))
        //                            {

        //                                foreach(JObject machineZone in AppParameters.ZonesList.Where(u => u.Value["properties"]["Zone_Type"].ToString() == "Machine").Select(x => x.Value))
        //                                {
        //                                    if (machineZone["properties"]["MPEWatchData"].HasValues)
        //                                    {

        //                                        string currSortPlan = machineZone["properties"]["MPEWatchData"]["cur_sortplan"].ToString();
        //                                        if (!string.IsNullOrEmpty(currSortPlan))
        //                                        {
        //                                            if (currSortPlan.Length > 7) { currSortPlan = currSortPlan.Substring(0, 7); }
        //                                            if (strSortPlanItem.Length > 7) { strSortPlanItem = strSortPlanItem.Substring(0, 7); }

        //                                            if (currSortPlan == strSortPlanItem)
        //                                            {
        //                                                machineZone["properties"]["DPSData"] = item;
        //                                                machineZone["properties"]["Zone_Update"] = true;
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            dpsInfo = null;
        //            data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new ErrorLogger().ExceptionLog(ex);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        //private static void ERRORWITHWORK(JObject data)
        //{
        //    try
        //    {
        //        if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
        //        {
        //            JObject mission = new JObject
        //            {
        //                ["Request_Id"] = (string)data["requestId".ToUpper()],
        //                ["Vehicle"] = (string)data["vehicle".ToUpper()],
        //                ["Vehicle_Number"] = (string)data["vehicle_Number".ToUpper()],
        //                ["Error_Discription"] = (string)data["Error_Discription".ToUpper()],
        //                ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
        //                ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
        //                ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
        //                ["State"] = "Error",
        //                ["MissionType"] = (string)data["message".ToUpper()],
        //                ["MissionErrorTime"] = (DateTime)data["time".ToUpper()]
        //            };
        //            foreach (JObject existingVa in AppParameters.Tag.Where(f => f.Value["properties"]["name"].ToString() == (string)mission["Vehicle"]).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["inMission"] = false;
        //                // get list of request for the pickup location.
        //                existingVa["properties"]["Mission"] = JToken.Parse(JsonConvert.SerializeObject(new JObject(), Formatting.Indented));
        //                existingVa["properties"]["Tag_Update"] = true;
        //            }
        //            //merge changes 
        //            foreach (JObject request in AppParameters.MissionList.Where(r => (int)r.Value["Request_Id"] == (int)mission["Request_Id"]).Select(y => y.Value))
        //            {
        //                request.Merge(mission, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        //            }
        //            //update AGV zone location
        //            foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation"
        //             && f.Value["properties"]["name"].ToString() == mission["Pickup_Location"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Pickup_Location"].ToString() == mission["Pickup_Location"].ToString()
        //                && r.Value["State"].ToString() == "Active"
        //                && (int)r.Value["Request_Id"] != (int)mission["Request_Id"]).Select(y => y.Value).ToList(), Formatting.Indented));
        //                existingVa["properties"]["Zone_Update"] = true;

        //            }
        //            foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation"
        //             && f.Value["properties"]["name"].ToString() == mission["Dropoff_Location"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Dropoff_Location"].ToString() == mission["Dropoff_Location"].ToString()
        //                && r.Value["State"].ToString() == "Active"
        //                && (int)r.Value["Request_Id"] != (int)mission["Request_Id"]).Select(y => y.Value).ToList(), Formatting.Indented));
        //                existingVa["properties"]["Zone_Update"] = true;
        //            }


        //            //remove request id
        //            if (!AppParameters.MissionList.TryRemove(mission["Request_Id"].ToString(), out JObject value))
        //            {
        //                new ErrorLogger().CustomLog("unable to remove Mission " + mission["Request_Id"].ToString() + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
        //            }
        //            mission = null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        //private static void ERRORWITHOUTWORK(JObject data)
        //{
        //    try
        //    {
        //        if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
        //        {
        //            if (data.ContainsKey("VEHICLE"))
        //            {
        //                foreach (JObject existingVa in AppParameters.Tag.Where(f => f.Value["properties"]["name"].ToString() == (string)data["VEHICLE"]).Select(y => y.Value))
        //                {
        //                    if ((string)existingVa["properties"]["state"] != "Error")
        //                    {
        //                        existingVa["properties"]["notificationId"] = CheckNotification(existingVa["properties"]["state"].ToString(), "Error", "vehicle", (JObject)existingVa["properties"], existingVa["properties"]["notificationId"].ToString());
        //                        existingVa["properties"]["state"] = "Error";
        //                        existingVa["properties"]["errorCode"] = (string)data["errorCode".ToUpper()];
        //                        existingVa["properties"]["errorDiscription"] = (string)data["error_Discription".ToUpper()];
        //                        existingVa["properties"]["errorTime"] = (DateTime)data["time".ToUpper()];
        //                        existingVa["properties"]["Tag_Update"] = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        //private static void SUCCESSFULDROP(JObject data)
        //{
        //    try
        //    {
        //        if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
        //        {
        //            JObject mission = new JObject
        //            {
        //                ["Request_Id"] = (string)data["requestId".ToUpper()],
        //                ["Vehicle"] = (string)data["vehicle".ToUpper()],
        //                ["Vehicle_Number"] = (string)data["vehicle_Number".ToUpper()],
        //                ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
        //                ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
        //                ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
        //                ["Placard"] = (string)data["mtel".ToUpper()],
        //                ["State"] = "Complete",
        //                ["MissionType"] = (string)data["message".ToUpper()],
        //                ["MissionDropOffTime"] = (DateTime)data["time".ToUpper()]
        //            };


        //            foreach (JObject existingVa in AppParameters.Tag.Where(f => f.Value["properties"]["name"].ToString() == (string)mission["Vehicle"]).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["inMission"] = false;
        //                // get list of request for the pickup location.
        //                existingVa["properties"]["Mission"] = JToken.Parse(JsonConvert.SerializeObject(new JObject(), Formatting.Indented));
        //                existingVa["properties"]["Tag_Update"] = true;
        //            }

        //            //merge changes 
        //            foreach (JObject request in AppParameters.MissionList.Where(r => (int)r.Value["Request_Id"] == (int)mission["Request_Id"]).Select(y => y.Value))
        //            {
        //                request.Merge(mission, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        //            }
        //            //update AGV zone location
        //            foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation"
        //            && f.Value["properties"]["name"].ToString() == mission["Pickup_Location"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Pickup_Location"].ToString() == mission["Pickup_Location"].ToString()
        //                && r.Value["State"].ToString() == "Active"
        //                && (int)r.Value["Request_Id"] != (int)mission["Request_Id"]).Select(y => y.Value).ToList(), Formatting.Indented));
        //                existingVa["properties"]["Zone_Update"] = true;

        //            }
        //            foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation"
        //            && f.Value["properties"]["name"].ToString() == mission["Dropoff_Location"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Dropoff_Location"].ToString() == mission["Dropoff_Location"].ToString()
        //                 && r.Value["State"].ToString() == "Active"
        //                && (int)r.Value["Request_Id"] != (int)mission["Request_Id"]).Select(y => y.Value).ToList(), Formatting.Indented));

        //                existingVa["properties"]["Zone_Update"] = true;
        //            }

        //            //remove request id
        //            if (!AppParameters.MissionList.TryRemove(mission["Request_Id"].ToString(), out JObject value))
        //            {
        //                new ErrorLogger().CustomLog("unable to remove Mission " + mission["Request_Id"].ToString() + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
        //            }

        //            mission = null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        //private static void SUCCESSFULPICKUP(JObject data)
        //{
        //    try
        //    {
        //        if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
        //        {
        //            JObject mission = new JObject
        //            {
        //                ["Request_Id"] = (string)data["requestId".ToUpper()],
        //                ["Vehicle"] = data["vehicle".ToUpper()].ToString().ToUpper(),
        //                ["Vehicle_Number"] = (string)data["vehicle_Number".ToUpper()],
        //                ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
        //                ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
        //                ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
        //                ["Door"] = (string)data["door".ToUpper()],
        //                ["Placard"] = (string)data["mtel".ToUpper()],
        //                ["State"] = "Active",
        //                ["MissionType"] = (string)data["message".ToUpper()],
        //                ["MissionPickupTime"] = (DateTime)data["time".ToUpper()]
        //            };

        //            foreach (JObject existingVa in AppParameters.Tag.Where(f => f.Value["properties"]["name"].ToString() == mission["Vehicle"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["inMission"] = true;
        //                existingVa["properties"]["Mission"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Vehicle"].ToString() == mission["Vehicle"].ToString()
        //                 && r.Value["State"].ToString() == mission["State"].ToString()
        //                 && (int)r.Value["Request_Id"] == (int)mission["Request_Id"]).Select(y => y.Value).ToList(), Formatting.Indented));
        //                existingVa["properties"]["Tag_Update"] = true;
        //            }
        //            //merge changes 
        //            foreach (JObject request in AppParameters.MissionList.Where(r => (int)r.Value["Request_Id"] == (int)mission["Request_Id"]).Select(y => y.Value))
        //            {
        //                request.Merge(mission, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        //            }
        //            //update AGV Pickup zone location
        //            foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation"
        //             && f.Value["properties"]["name"].ToString() == mission["Pickup_Location"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Pickup_Location"].ToString() == mission["Pickup_Location"].ToString()
        //                && (int)r.Value["Request_Id"] != (int)mission["Request_Id"]).Select(y => y.Value).ToList(), Formatting.Indented));
        //                existingVa["properties"]["Zone_Update"] = true;
        //            }

        //            //update AGV Dropoff zone location
        //            foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation"
        //            && f.Value["properties"]["name"].ToString() == mission["Dropoff_Location"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Dropoff_Location"].ToString() == mission["Dropoff_Location"].ToString()
        //                && r.Value["State"].ToString() == "Active").Select(y => y.Value).ToList(), Formatting.Indented));
        //                existingVa["properties"]["Zone_Update"] = true;
        //            }


        //            mission = null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        data = null;
        //    }
        //}

        //private static void MATCHEDWITHWORK(JObject data)
        //{
        //    try
        //    {
        //        if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
        //        {
        //            JObject mission = new JObject
        //            {
        //                ["Request_Id"] = (string)data["requestId".ToUpper()],
        //                ["Vehicle"] = data["vehicle".ToUpper()].ToString().ToUpper(),
        //                ["Vehicle_Number"] = (string)data["vehicle_Number".ToUpper()],
        //                ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
        //                ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
        //                ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
        //                ["ETA"] = (string)data["eta".ToUpper()],
        //                ["Door"] = (string)data["door".ToUpper()],
        //                ["Placard"] = (string)data["mtel".ToUpper()],
        //                ["State"] = "Active",
        //                ["MissionType"] = (string)data["message".ToUpper()],
        //                ["MissionAssignedTime"] = (DateTime)data["time".ToUpper()]
        //            };
        //            if (!AppParameters.MissionList.ContainsKey(mission["Request_Id"].ToString()))
        //            {
        //                if (!AppParameters.MissionList.TryAdd(mission["Request_Id"].ToString(), mission))
        //                {
        //                    new ErrorLogger().CustomLog("unable to add Mission " + mission["Request_Id"].ToString() + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
        //                }
        //            }
        //            else
        //            {
        //                //merge changes 
        //                AppParameters.MissionList.Where(r => (int)r.Value["Request_Id"] == (int)mission["Request_Id"]).Select(y => y.Value).FirstOrDefault().Merge(mission, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        //            }

        //           foreach( JObject existingVa in AppParameters.Tag.Where(f => f.Value["properties"]["name"].ToString() == mission["Vehicle"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["inMission"] = true;
        //                // get list of request for the pickup location.
        //                existingVa["properties"]["Mission"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Vehicle"].ToString() == mission["Vehicle"].ToString()
        //                && r.Value["State"].ToString() == "Active"
        //                && (int)r.Value["Request_Id"] == (int)mission["Request_Id"]).Select(y => y.Value).ToList(), Formatting.Indented));
        //                existingVa["properties"]["Tag_Update"] = true;
        //            }

        //            //update AGV zone location
        //            foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" &&
        //            f.Value["properties"]["name"].ToString() == mission["Pickup_Location"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Pickup_Location"].ToString() == mission["Pickup_Location"].ToString()
        //                && r.Value["State"].ToString() == "Active").Select(y => y.Value).ToList(), Formatting.Indented));
        //                existingVa["properties"]["Zone_Update"] = true;
        //            }
        //            foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" &&
        //            f.Value["properties"]["name"].ToString() == mission["Dropoff_Location"].ToString()).Select(y => y.Value))
        //            {
        //                existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Dropoff_Location"].ToString() == mission["Dropoff_Location"].ToString()
        //                && r.Value["State"].ToString() == "Active").Select(y => y.Value).ToList(), Formatting.Indented));
        //                existingVa["properties"]["Zone_Update"] = true;
        //            }
        //            mission = null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}

        //private static void MOVEREQUEST(JObject data)
        //{
        //    try
        //    {
        //        if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
        //        {
        //            if (data.HasValues && data.ContainsKey("requestId".ToUpper()))
        //            {
        //                JObject mission = new JObject
        //                {
        //                    ["Request_Id"] = (string)data["requestId".ToUpper()],
        //                    ["Vehicle"] = "",
        //                    ["Vehicle_Number"] = "",
        //                    ["Pickup_Location"] = data["pickup_location".ToUpper()].ToString().ToUpper(),
        //                    ["Dropoff_Location"] = data["dropoff_location".ToUpper()].ToString().ToUpper(),
        //                    ["End_Location"] = data["end_location".ToUpper()].ToString().ToUpper(),
        //                    ["Door"] = (string)data["door".ToUpper()],
        //                    ["ETA"] = "",
        //                    ["Placard"] = (string)data["mtel".ToUpper()],
        //                    ["QueuePosition"] = (string)data["QueuePosition".ToUpper()],
        //                    ["State"] = "Active",
        //                    ["MissionType"] = (string)data["message".ToUpper()],
        //                    ["MissionRequestTime"] = (DateTime)data["time".ToUpper()]
        //                };

        //                if (!AppParameters.MissionList.ContainsKey(mission["Request_Id"].ToString()))
        //                {
        //                    if (!AppParameters.MissionList.TryAdd(mission["Request_Id"].ToString(), mission))
        //                    {
        //                        new ErrorLogger().CustomLog("unable to add Mission " + mission["Request_Id"].ToString() + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
        //                    }
        //                }
        //                else
        //                {
        //                    if (AppParameters.MissionList.TryRemove(mission["Request_Id"].ToString(), out JObject valueOut))
        //                    {
        //                        if (!AppParameters.MissionList.TryAdd(mission["Request_Id"].ToString(), mission))
        //                        {
        //                            new ErrorLogger().CustomLog("unable to add Mission " + mission["Request_Id"].ToString() + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
        //                        }
        //                    }
        //                }
        //                //update AGV zone location
        //                foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" &&
        //                f.Value["properties"]["name"].ToString() == mission["Pickup_Location"].ToString()).Select(y => y.Value))
        //                {
        //                    existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Pickup_Location"].ToString() == mission["Pickup_Location"].ToString()
        //                    && r.Value["State"].ToString() == mission["State"].ToString()).Select(y => y.Value).ToList(), Formatting.Indented));
        //                    existingVa["properties"]["Zone_Update"] = true;
        //                }
        //                foreach (JObject existingVa in AppParameters.ZonesList.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" &&
        //                f.Value["properties"]["name"].ToString() == mission["Dropoff_Location"].ToString()).Select(y => y.Value))
        //                {
        //                    existingVa["properties"]["MissionList"] = JToken.Parse(JsonConvert.SerializeObject(AppParameters.MissionList.Where(r => r.Value["Dropoff_Location"].ToString() == mission["Dropoff_Location"].ToString()
        //                    && r.Value["State"].ToString() == mission["State"].ToString()).Select(y => y.Value).ToList(), Formatting.Indented));
        //                    existingVa["properties"]["Zone_Update"] = true;
        //                }
        //                mission = null;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}

        //private static void FLEET_STATUS(JObject data)
        //{
        //    try
        //    {
        //        if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
        //        {
        //            bool Update = false;
        //            if (data.ContainsKey("VEHICLE"))
        //            {
        //                foreach(JObject existingVa in AppParameters.Tag.Where(u => u.Value["properties"]["Tag_Type"].ToString().ToLower() == "Autonomous Vehicle".ToLower() &&
        //                u.Value["properties"]["name"].ToString() == (string)data["VEHICLE"]).Select(x => x.Value))
        //                {
        //                    if (((JObject)existingVa["properties"]).ContainsKey("vehicleBatteryPercent"))
        //                    {
        //                        if ((string)existingVa["properties"]["vehicleBatteryPercent"] != (string)data["batterypercent".ToUpper()])
        //                        {
        //                            existingVa["properties"]["vehicleBatteryPercent"] = (string)data["batterypercent".ToUpper()];
        //                            if (!Update)
        //                            {
        //                                Update = true;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        existingVa["properties"]["vehicleBatteryPercent"] = (string)data["batterypercent".ToUpper()];
        //                        if (!Update)
        //                        {
        //                            Update = true;
        //                        }
        //                    }
        //                    if (((JObject)existingVa["properties"]).ContainsKey("MacAddress"))
        //                    {
        //                        if ((string)existingVa["properties"]["MacAddress"] != (string)data["VEHICLE_MAC_ADDRESS".ToUpper()])
        //                        {
        //                            existingVa["properties"]["MacAddress"] = (string)data["VEHICLE_MAC_ADDRESS".ToUpper()];
        //                            if (!Update)
        //                            {
        //                                Update = true;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        existingVa["properties"]["MacAddress"] = (string)data["VEHICLE_MAC_ADDRESS".ToUpper()];
        //                        if (!Update)
        //                        {
        //                            Update = true;
        //                        }
        //                    }
        //                    if (((JObject)existingVa["properties"]).ContainsKey("vehicleNumber"))
        //                    {
        //                        if ((string)existingVa["properties"]["vehicleNumber"] != (string)data["vehicle_number".ToUpper()])
        //                        {
        //                            existingVa["properties"]["vehicleNumber"] = (string)data["vehicle_number".ToUpper()];
        //                            if (!Update)
        //                            {
        //                                Update = true;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        existingVa["properties"]["vehicleNumber"] = (string)data["vehicle_number".ToUpper()];
        //                        if (!Update)
        //                        {
        //                            Update = true;
        //                        }
        //                    }
        //                    if (((JObject)existingVa["properties"]).ContainsKey("vehicleCategory"))
        //                    {
        //                        if ((string)existingVa["properties"]["vehicleCategory"] != (string)data["Category".ToUpper()])
        //                        {
        //                            existingVa["properties"]["vehicleCategory"] = (string)data["Category".ToUpper()];
        //                            if (!Update)
        //                            {
        //                                Update = true;
        //                            }
        //                        }

        //                    }
        //                    else
        //                    {
        //                        existingVa["properties"]["vehicleCategory"] = (string)data["Category".ToUpper()];
        //                        if (!Update)
        //                        {
        //                            Update = true;
        //                        }

        //                    }
        //                    if (((JObject)existingVa["properties"]).ContainsKey("vehicleTime"))
        //                    {
        //                        if ((string)existingVa["properties"]["vehicleTime"] != (string)data["time".ToUpper()])
        //                        {
        //                            existingVa["properties"]["vehicleTime"] = ((DateTime)data["time".ToUpper()]).ToUniversalTime();
        //                            if (!Update)
        //                            {
        //                                Update = true;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        existingVa["properties"]["vehicleTime"] = ((DateTime)data["time".ToUpper()]).ToUniversalTime();
        //                        if (!Update)
        //                        {
        //                            Update = true;
        //                        }
        //                    }
        //                    if (((JObject)existingVa["properties"]).ContainsKey("state"))
        //                    {
        //                        if ((string)existingVa["properties"]["state"] != (string)data["state".ToUpper()])
        //                        {

        //                            existingVa["properties"]["notificationId"] = CheckNotification(existingVa["properties"]["state"].ToString(), data["state".ToUpper()].ToString(), "vehicle".ToLower(), (JObject)existingVa["properties"], existingVa["properties"]["notificationId"].ToString());
        //                            existingVa["properties"]["state"] = (string)data["state".ToUpper()];
        //                            if (!Update)
        //                            {
        //                                Update = true;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        existingVa["properties"]["state"] = "";
        //                        existingVa["properties"]["notificationId"] = "";
        //                        existingVa["properties"]["notificationId"] = CheckNotification(existingVa["properties"]["state"].ToString(), data["state".ToUpper()].ToString(), "vehicle".ToLower(), (JObject)existingVa["properties"], "");
        //                        existingVa["properties"]["state"] = (string)data["state".ToUpper()];
        //                    }
        //                    if (Update)
        //                    {
        //                        existingVa["properties"]["Tag_Update"] = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}

        private static void ProjectData(dynamic jsonObject)
        {
            try
            {
                if (jsonObject != null)
                {
                    JToken tempData = JToken.Parse(jsonObject);
                    if (((JObject)tempData).ContainsKey("coordinateSystems"))
                    {
                        //the background image
                        JToken backgroundImages = tempData.SelectToken("coordinateSystems[0].backgroundImages[0]");
                        if (backgroundImages.HasValues)
                        {
                            backgroundImages["rawData"] = backgroundImages.ToString();
                            //this is for existing images
                            if (AppParameters.IndoorMap.TryGetValue(backgroundImages["id"].ToString(), out BackgroundImage bckimg))
                            {
                                if (bckimg.RawData != backgroundImages.ToString())
                                {
                                    JObject tempbckimg = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(bckimg, Formatting.Indented));
                                    tempbckimg["updateStatus"] = true;
                                    tempbckimg.Merge((JObject)backgroundImages, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                    BackgroundImage newtempbckimg = tempbckimg.ToObject<BackgroundImage>();
                                    if (!AppParameters.IndoorMap.TryUpdate(newtempbckimg.Id, newtempbckimg, bckimg))
                                    {
                                        new ErrorLogger().CustomLog("Unable to Update Image" + newtempbckimg.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                    }
                                }

                            }
                            ////this for new images
                            else
                            {
                                BackgroundImage newbckimg = backgroundImages.ToObject<BackgroundImage>();
                                newbckimg.RawData = backgroundImages.ToString();
                                newbckimg.UpdateStatus = true;
                                if (!AppParameters.IndoorMap.TryAdd(newbckimg.Id, newbckimg))
                                {
                                    new ErrorLogger().CustomLog("Unable to Image " + newbckimg.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                }

                            }
                        }

                        //this is for Zones
                        JToken zones = tempData.SelectToken("coordinateSystems[0].zones");
                        if (zones.Count() > 0)
                        {
                            foreach (JObject zoneitem in zones.Children())
                            {
                                //this for new zones
                                zoneitem["rawData"] = JsonConvert.SerializeObject(zoneitem, Formatting.None);
                                if (AppParameters.ZoneList.TryGetValue(zoneitem["id"].ToString(), out GeoZone gZone))
                                {
                                    if (gZone.Properties.RawData != zoneitem["rawData"].ToString())
                                    {
                                        Geometry newGeomery = GetQuuppaZoneGeometry(zoneitem, "Polygon"); 
                                        JObject tempgZone = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(gZone, Formatting.Indented));
                                        tempgZone["Zone_Update"] = true;
                                        ((JObject)tempgZone["properties"]).Merge(zoneitem, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                        GeoZone newtempgZone = tempgZone.ToObject<GeoZone>();
                                        newtempgZone.Geometry = newGeomery;
                                        if (!AppParameters.ZoneList.TryUpdate(newtempgZone.Properties.Id, newtempgZone, gZone))
                                        {
                                            new ErrorLogger().CustomLog("Unable to Update Zone" + newtempgZone.Properties.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                        }
                                    }
                                }
                                else
                                {
                                    GeoZone newGZone = new GeoZone();
                                    newGZone.Type = "Feature";
                                    JObject tempgZone = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(newGZone, Formatting.Indented));

                                    tempgZone["properties"] = zoneitem;
                                    GeoZone newtempgZone = tempgZone.ToObject<GeoZone>();
                                    newtempgZone.Properties.ZoneType = GetZoneType(newtempgZone.Properties.Name);
                                    newtempgZone.Properties.ZoneUpdate = true;
                                    newtempgZone.Geometry = GetQuuppaZoneGeometry(zoneitem, "Polygon");
                                    if (!AppParameters.ZoneList.TryAdd(newtempgZone.Properties.Id, newtempgZone))
                                    {
                                        new ErrorLogger().CustomLog("Unable to Update Zone" + newtempgZone.Properties.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                    }
                                }
                            }
                        }
                        //this is for Zones
                        JToken locators = tempData.SelectToken("coordinateSystems[0].locators");
                        if (locators.Count() > 0)
                        {
                            foreach (JObject locatorsitem in locators.Children())
                            {
                                locatorsitem["rawData"] = JsonConvert.SerializeObject(locatorsitem, Formatting.None);
                                if (AppParameters.TagsList.TryGetValue(locatorsitem["id"].ToString(), out GeoMarker geoLmarker)) 
                                {
                                    if (geoLmarker.Properties.RawData != locatorsitem["rawData"].ToString() )
                                    {

                                    }
                                }
                                else
                                {
                                    GeoMarker Lmarker = new GeoMarker();
                                    Lmarker.Type = "Feature";
                                    JObject tempLmarker = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(Lmarker, Formatting.Indented));

                                    tempLmarker["properties"] = locatorsitem;
                                    GeoMarker newtempLmarker = tempLmarker.ToObject<GeoMarker>();
                                    newtempLmarker.Properties.TagType = GetTagType(newtempLmarker.Properties.Name);
                                    newtempLmarker.Properties.TagUpdate = true;
                                    newtempLmarker.Geometry = GetQuuppaTagGeometry(locatorsitem["location"], "Point", "location");
                                    if (!AppParameters.TagsList.TryAdd(newtempLmarker.Properties.Id, newtempLmarker))
                                    {
                                        new ErrorLogger().CustomLog("Unable to Update Zone" + newtempLmarker.Properties.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                    }
                                }
                            }
                        }
                    }



                    //    ProjectInfo projectData = JsonConvert.DeserializeObject<ProjectInfo>(jsonObject);
                    //foreach (CoordinateSystem CoordinateSystemitem in projectData.CoordinateSystems)
                    //{
                    //    AppParameters.IndoorMap.AddOrUpdate(CoordinateSystemitem.Id, CoordinateSystemitem.BackgroundImages[0], (key, oldData) =>
                    //    {
                    //        CoordinateSystemitem.BackgroundImages[0].UpdateStatus = true;
                    //        return CoordinateSystemitem.BackgroundImages[0];
                    //    });

                    //}
                    //    foreach (Zone zoneitem in projectData.CoordinateSystems[0].Zones)
                    //    {

                    //        GeoZone tempGeoZone = new GeoZone();

                    //        tempGeoZone.Geometry = "";
                    //        tempGeoZone.Type = "Feature";
                    //        tempGeoZone.Properties = 
                    //        AppParameters.ZoneList.AddOrUpdate(tempGeoZone.Properties.Id, tempGeoZone, (key, oldZoneItem) =>
                    //        {
                    //            tempGeoZone.Properties.ZoneUpdate = true;
                    //            return null;
                    //        });
                    //    }

                    //if (jsonObject.ContainsKey("coordinateSystems"))
                    //{
                    //    string temp_map_id = "";
                    //    JToken backgroundImages = jsonObject.SelectToken("coordinateSystems[0].backgroundImages");

                    //    foreach (JObject imageitem in backgroundImages)
                    //    {
                    //        temp_map_id = imageitem["id"].ToString();
                    //        if (!AppParameters.IndoorMap.ContainsKey(temp_map_id))
                    //        {

                    //            imageitem["Facility_Name"] = AppParameters.AppSettings.ContainsKey("FACILITY_NAME") ? AppParameters.AppSettings["FACILITY_NAME"] : "";
                    //            imageitem["Facility_TimeZone"] = AppParameters.AppSettings.ContainsKey("FACILITY_TIMEZONE") ? AppParameters.AppSettings["FACILITY_TIMEZONE"] : "";
                    //            imageitem["Environment"] = !string.IsNullOrEmpty(AppParameters.ApplicationEnvironment) ? AppParameters.ApplicationEnvironment : "";
                    //            imageitem["Software_Version"] = !string.IsNullOrEmpty(AppParameters.VersionInfo) ? AppParameters.VersionInfo : "";
                    //            imageitem["NASS_Code"] = AppParameters.AppSettings.ContainsKey("FACILITY_NASS_CODE") ? AppParameters.AppSettings["FACILITY_NASS_CODE"] : "";
                    //            imageitem["Map_Update"] = true;
                    //            AppParameters.IndoorMap.TryAdd(imageitem["id"].ToString(), imageitem);

                    //        }
                    //        else
                    //        {
                    //            if (AppParameters.IndoorMap.TryGetValue(temp_map_id, out JObject existingVal))
                    //            {
                    //                existingVal.Merge(imageitem, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                    //                existingVal["Map_Update"] = true;
                    //            }

                    //        }

                    //    }

                    //    JToken locators = jsonObject.SelectToken("coordinateSystems[0].locators");
                    //    if (locators.Count() > 0)
                    //    {
                    //        foreach (JObject locatorsitem in locators.Children())
                    //        {
                    //            JArray temp = new JArray();
                    //            bool update_info = false;
                    //            try
                    //            {
                    //                if (locatorsitem.ContainsKey("location"))
                    //                {
                    //                    if (locatorsitem["location"].HasValues)
                    //                    {
                    //                        JArray tagitemsplit = (JArray)locatorsitem["location"];
                    //                        if (tagitemsplit.HasValues)
                    //                        {
                    //                            temp = new JArray(tagitemsplit[0], tagitemsplit[1]);
                    //                        }
                    //                    }
                    //                }
                    //                if (AppParameters.Tag.ContainsKey(locatorsitem["id"].ToString()))
                    //                {
                    //                    if (AppParameters.ZonesList.TryGetValue(locatorsitem["id"].ToString(), out JObject existingVa))
                    //                    {
                    //                        if (existingVa["geometry"]["coordinates"].ToString() != temp.ToString())
                    //                        {
                    //                            existingVa["geometry"]["coordinates"] = temp;
                    //                            update_info = true;
                    //                        }
                    //                         ((JObject)existingVa["properties"]).Merge(locatorsitem, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

                    //                        if (existingVa["properties"]["visible"].ToString() != locatorsitem["visible"].ToString())
                    //                        {
                    //                            update_info = true;
                    //                        }
                    //                        if (existingVa["properties"]["name"].ToString() != locatorsitem["name"].ToString())
                    //                        {
                    //                            update_info = true;
                    //                        }
                    //                        if (update_info)
                    //                        {
                    //                            existingVa["properties"]["Tag_Update"] = true;
                    //                        }
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    JObject GeoJsonType = new JObject_List().GeoJSON_Locators;
                    //                    GeoJsonType["geometry"]["type"] = "Point";

                    //                    if (temp.HasValues)
                    //                    {
                    //                        GeoJsonType["geometry"]["coordinates"] = temp;
                    //                    }
                    //                    ((JObject)GeoJsonType["properties"]).Merge(locatorsitem, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

                    //                    GeoJsonType["properties"]["Tag_Type"] = "Locater";
                    //                    GeoJsonType["properties"]["Tag_Update"] = true;
                    //                    if (!AppParameters.Tag.ContainsKey(locatorsitem["id"].ToString()))
                    //                    {
                    //                        AppParameters.Tag.TryAdd(locatorsitem["id"].ToString(), GeoJsonType);
                    //                    }
                    //                    GeoJsonType = null;
                    //                }
                    //            }
                    //            catch (Exception e)
                    //            {
                    //                new ErrorLogger().ExceptionLog(e);
                    //            }
                    //        }
                    //    }
                    //    JToken zones = jsonObject.SelectToken("coordinateSystems[0].zones");
                    //    if (zones.Count() > 0)
                    //    {

                    //        foreach (JObject zoneitem in zones.Children())
                    //        {
                    //            JArray temp = new JArray();
                    //            bool update_info = false;
                    //            try
                    //            {
                    //                if (zoneitem.ContainsKey("polygonData") && zoneitem.Property("polygonData").HasValues)
                    //                {

                    //                    string[] polygonDatasplit = zoneitem["polygonData"].ToString().Split('|');
                    //                    if (polygonDatasplit.Length > 0)
                    //                    {
                    //                        JArray xyar = new JArray();
                    //                        foreach (var polygonitem in polygonDatasplit)
                    //                        {
                    //                            string[] polygonitemsplit = polygonitem.Split(',');
                    //                            xyar.Add(new JArray(Convert.ToDouble(polygonitemsplit[0]), Convert.ToDouble(polygonitemsplit[1])));
                    //                        }
                    //                        if (xyar != null)
                    //                        {
                    //                            temp.Add(new JArray(xyar));
                    //                        }

                    //                    }

                    //                }
                    //                if (AppParameters.ZonesList.ContainsKey(zoneitem["id"].ToString()))
                    //                {
                    //                    if (AppParameters.ZonesList.TryGetValue(zoneitem["id"].ToString(), out JObject existingVa))
                    //                    {
                    //                        if (existingVa["geometry"]["coordinates"].ToString() != temp.ToString())
                    //                        {
                    //                            existingVa["geometry"]["coordinates"] = temp;
                    //                            update_info = true;
                    //                        }
                    //                        if (existingVa["properties"]["visible"].ToString() != zoneitem["visible"].ToString())
                    //                        {
                    //                            existingVa["properties"]["visible"] = zoneitem["visible"];
                    //                            update_info = true;
                    //                        }
                    //                        if (existingVa["properties"]["color"].ToString() != zoneitem["color"].ToString())
                    //                        {
                    //                            existingVa["properties"]["color"] = zoneitem["color"];
                    //                            update_info = true;
                    //                        }
                    //                        if (existingVa["properties"]["Zone_Type"].ToString().StartsWith("Machine"))
                    //                        {
                    //                            if (AppParameters.ZoneInfo.TryGetValue(existingVa["properties"]["id"].ToString(), out JObject zoneinfodata))
                    //                            {
                    //                                if (existingVa["properties"]["name"].ToString() != zoneinfodata["name"].ToString())
                    //                                {
                    //                                    ((JObject)existingVa["properties"]).Merge(zoneinfodata, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                    //                                }

                    //                            }
                    //                        }
                    //                        if (update_info)
                    //                        {
                    //                            existingVa["properties"]["Zone_Update"] = true;
                    //                        }
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    JObject GeoJsonType = new JObject_List().GeoJSON_Zone;
                    //                    GeoJsonType["geometry"]["type"] = "Polygon";

                    //                    if (temp.HasValues)
                    //                    {
                    //                        GeoJsonType["geometry"]["coordinates"] = temp;
                    //                    }
                    //                    GeoJsonType["properties"]["id"] = zoneitem.ContainsKey("id") ? zoneitem["id"] : "";
                    //                    GeoJsonType["properties"]["visible"] = zoneitem.ContainsKey("visible") ? zoneitem["visible"] : false;
                    //                    GeoJsonType["properties"]["color"] = zoneitem.ContainsKey("color") ? zoneitem["color"] : "";
                    //                    GeoJsonType["properties"]["name"] = zoneitem.ContainsKey("name") ? zoneitem["name"] : "";
                    //                    GeoJsonType["properties"]["Zone_Type"] = GetZoneType(zoneitem["name"].ToString());

                    //                    if (GeoJsonType["properties"]["Zone_Type"].ToString().StartsWith("DockDoor"))
                    //                    {
                    //                        string tempdoor = zoneitem.ContainsKey("name") ? string.Join(string.Empty, Regex.Matches((string)zoneitem["name"], @"\d+").OfType<Match>().Select(m => m.Value)) : "";
                    //                        if (!string.IsNullOrEmpty(tempdoor))
                    //                        {
                    //                            GeoJsonType["properties"]["doorNumber"] = tempdoor;
                    //                        }
                    //                        else
                    //                        {
                    //                            GeoJsonType["properties"]["doorNumber"] = tempdoor;
                    //                        }

                    //                        GeoJsonType["properties"]["routetripData"] = "";
                    //                    }
                    //                    if (GeoJsonType["properties"]["Zone_Type"].ToString().StartsWith("Machine"))
                    //                    {
                    //                        if (AppParameters.ZoneInfo.TryGetValue(GeoJsonType["properties"]["id"].ToString(), out JObject zoneinfodata))
                    //                        {
                    //                            ((JObject)GeoJsonType["properties"]).Merge(zoneinfodata, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                    //                        }
                    //                        else
                    //                        {
                    //                            if (string.IsNullOrEmpty(GeoJsonType["properties"]["MPE_Number"].ToString()))
                    //                            {
                    //                                string tempnumber = zoneitem.ContainsKey("name") ? string.Join(string.Empty, Regex.Matches((string)zoneitem["name"], @"\d+").OfType<Match>().Select(m => m.Value)) : "";
                    //                                GeoJsonType["properties"]["MPE_Number"] = int.TryParse(tempnumber, out int n) ? n.ToString() : "0";
                    //                            }
                    //                            if (string.IsNullOrEmpty(GeoJsonType["properties"]["MPE_Type"].ToString()))
                    //                            {
                    //                                string tempname = zoneitem.ContainsKey("name") ? string.Join(string.Empty, Regex.Matches((string)zoneitem["name"], @"\p{L}+").OfType<Match>().Select(m => m.Value)) : "";
                    //                                GeoJsonType["properties"]["MPE_Type"] = !string.IsNullOrEmpty(tempname) ? tempname : "";
                    //                            }
                    //                        }
                    //                    }

                    //                    GeoJsonType["properties"]["Zone_Update"] = true;

                    //                    if (!AppParameters.ZonesList.ContainsKey(zoneitem["id"].ToString()))
                    //                    {
                    //                        AppParameters.ZonesList.TryAdd(zoneitem["id"].ToString(), GeoJsonType);
                    //                    }

                    //                }
                    //            }
                    //            catch (Exception e)
                    //            {
                    //                new ErrorLogger().ExceptionLog(e);
                    //            }
                    //        }
                    //    }
                    //    JToken polygons = jsonObject.SelectToken("coordinateSystems[0].polygons");
                    //    if (polygons.HasValues)
                    //    {
                    //        foreach (JObject zoneitem in polygons.Children())
                    //        {
                    //            try
                    //            {
                    //                JArray temp = new JArray();

                    //                if (zoneitem.ContainsKey("polygonData") && (zoneitem.Property("polygonData").HasValues))
                    //                {

                    //                    string[] polygonDatasplit = zoneitem["polygonData"].ToString().Split('|');
                    //                    if (polygonDatasplit.Length > 0)
                    //                    {
                    //                        JArray xyar = new JArray();
                    //                        foreach (var polygonitem in polygonDatasplit)
                    //                        {
                    //                            string[] polygonitemsplit = polygonitem.Split(',');
                    //                            xyar.Add(new JArray(Convert.ToDouble(polygonitemsplit[0]), Convert.ToDouble(polygonitemsplit[1])));
                    //                        }
                    //                        temp.Add(new JArray(xyar));
                    //                    }

                    //                }

                    //                if (AppParameters.IndoorMap.TryGetValue(temp_map_id, out JObject mapvalue))
                    //                {
                    //                    if (mapvalue.ContainsKey("TrackingArea"))
                    //                    {
                    //                        mapvalue["TrackingArea"] = temp;
                    //                    }
                    //                }

                    //            }
                    //            catch (Exception e)
                    //            {
                    //                new ErrorLogger().ExceptionLog(e);
                    //            }
                    //        }
                    //    }


                    //}
                    //if (!jsonObject.ContainsKey("localdata"))
                    //{
                    //    jsonObject["localdata"] = true;
                    //    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "ProjectData.json", JsonConvert.SerializeObject(jsonObject, Formatting.Indented));
                    //}
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            
        }

        private static Geometry GetQuuppaTagGeometry(JToken location, string type, string dataType)
        {
            try
            {
                JObject geometry = new JObject();
                geometry["type"] = type;
                JArray temp = new JArray();
                JArray tagitemsplit = (JArray)location[dataType];
                if (tagitemsplit.HasValues)
                {
                    temp = new JArray(tagitemsplit[0], tagitemsplit[1]);
                }
                geometry["coordinates"] = temp;
                Geometry result = geometry.ToObject<Geometry>();
                return result;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        private static Geometry GetQuuppaZoneGeometry(JObject zoneitem,string type)
        {
            try
            {
                JObject geometry = new JObject();
                geometry["type"] = type;
                JArray temp = new JArray();
                
              
                if (zoneitem.ContainsKey("polygonData") && zoneitem.Property("polygonData").HasValues)
                {
                    string[] polygonDatasplit = zoneitem["polygonData"].ToString().Split('|');
                    if (polygonDatasplit.Length > 0)
                    {
                        JArray xyar = new JArray();
                        foreach (var polygonitem in polygonDatasplit)
                        {
                            string[] polygonitemsplit = polygonitem.Split(',');
                            xyar.Add(new JArray(Convert.ToDouble(polygonitemsplit[0]), Convert.ToDouble(polygonitemsplit[1])));
                        }
                        temp.Add(xyar);
                    }
                }
                geometry["coordinates"] = temp;

                Geometry result = geometry.ToObject<Geometry>();

                return result;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        private static string GetZoneType(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Regex.IsMatch(value, (string)AppParameters.AppSettings["AGV_ZONE"], RegexOptions.IgnoreCase))
                    {
                        return "AGVLocation";
                    }
                    else if (Regex.IsMatch(value, (string)AppParameters.AppSettings["DOCKDOOR_ZONE"], RegexOptions.IgnoreCase))
                    {
                        return "DockDoor";
                    }
                    else if (Regex.IsMatch(value, (string)AppParameters.AppSettings["MANUAL_ZONE"], RegexOptions.IgnoreCase))
                    {
                        return "Area";
                    }
                    else if (Regex.IsMatch(value, (string)AppParameters.AppSettings["VIEWPORTS"], RegexOptions.IgnoreCase))
                    {
                        return "ViewPorts";
                    }
                    else
                    {
                        return "Machine";
                    }
                }
                else
                {
                    return "None";
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "None";
            }
        }

        //private static void TagPosition(JObject item)
        //{
        //    try
        //    {
        //        if (item.HasValues)
        //        {
        //            if (item.ContainsKey("tags"))
        //            {
        //                double tagtimeout = AppParameters.AppSettings.ContainsKey("TAG_TIMEOUTMILS") ? !string.IsNullOrEmpty((string)AppParameters.AppSettings["TAG_TIMEOUTMILS"]) ? (long)AppParameters.AppSettings["TAG_TIMEOUTMILS"] : 300000 : 300000;
        //                JArray temp_zone = new JArray();
        //                foreach (JObject tagitem in ((JArray)item["tags"]).Children())
        //                {
        //                    //if (tagitem["name"].ToString().StartsWith("Maint"))
        //                    //{
        //                    try
        //                    {
        //                        temp_zone = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
        //                        DateTime positionTS = AppParameters.UnixTimeStampToDateTime((long)tagitem["positionTS"]);
        //                        DateTime responseTS = AppParameters.UnixTimeStampToDateTime((long)item["responseTS"]);
        //                        System.TimeSpan tagdiffResult = responseTS.ToUniversalTime().Subtract(positionTS.ToUniversalTime());
        //                        JArray tempcoordinates = new JArray();
        //                        bool update_info = false;
        //                        if (tagitem.ContainsKey("smoothedPosition"))
        //                        {
        //                            if (tagitem["smoothedPosition"].HasValues)
        //                            {
        //                                JArray tagitemsplit = (JArray)tagitem["smoothedPosition"];
        //                                if (tagitemsplit.HasValues)
        //                                {
        //                                    tempcoordinates = new JArray(tagitemsplit[0], tagitemsplit[1]);
        //                                }
        //                            }
        //                        }
        //                        if (AppParameters.Tag.ContainsKey(tagitem["id"].ToString()))
        //                        {
        //                            if (AppParameters.Tag.TryGetValue(tagitem["id"].ToString(), out JObject existingVa))
        //                            {

        //                                if (existingVa["geometry"]["coordinates"].ToString() != tempcoordinates.ToString())
        //                                {
        //                                    existingVa["geometry"]["coordinates"] = tempcoordinates;
        //                                    update_info = true;
        //                                }
        //                                if (tagitem["zones"].HasValues)
        //                                {
        //                                    //default time is 5 minutes
        //                                    if (tagdiffResult.TotalMilliseconds > tagtimeout)
        //                                    {
        //                                        tagitem["zones"] = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
        //                                        temp_zone = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
        //                                    }
        //                                    else
        //                                    {
        //                                        temp_zone = (JArray)tagitem["zones"];
        //                                    }

        //                                    if (existingVa["properties"]["zones"].ToString() != temp_zone.ToString())
        //                                    {
        //                                        existingVa["properties"]["zones"] = temp_zone;
        //                                        update_info = true;
        //                                    }
        //                                }
        //                                if (existingVa["properties"]["name"].ToString() != tagitem["name"].ToString() )
        //                                {
        //                                    existingVa["properties"]["name"] = tagitem["name"];
        //                                    update_info = true;
        //                                }
        //                                string tagtype = GetTagType((string)tagitem["name"]);
        //                                if (existingVa["properties"]["Tag_Type"].ToString() != tagtype)
        //                                {
        //                                    existingVa["properties"]["Tag_Type"] = tagtype;
        //                                    update_info = true;
        //                                }
        //                                existingVa["properties"]["Raw_Data"] = tagitem;
        //                                existingVa["properties"]["Tag_TS"] = responseTS;
        //                                existingVa["properties"]["positionTS"] = positionTS;
        //                                if (update_info)
        //                                {
        //                                    existingVa["properties"]["Tag_Update"] = true;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //JObject GeoJson = new JObject_List().GeoJSON;

        //                            JObject GeoJsonType = new JObject_List().GeoJSON_Tag;
        //                            GeoJsonType["geometry"]["type"] = "Point";
        //                            GeoJsonType["geometry"]["coordinates"] = tempcoordinates;
        //                            if (tagitem.ContainsKey("zones"))
        //                            {
        //                                if (tagitem["zones"].HasValues)
        //                                {
        //                                    if (tagdiffResult.TotalMilliseconds > tagtimeout)
        //                                    {
        //                                        tagitem["zones"] = temp_zone;
        //                                        GeoJsonType["properties"]["zones"] = temp_zone;
        //                                    }
        //                                    else
        //                                    {
        //                                        GeoJsonType["properties"]["zones"] = (JArray)tagitem["zones"];
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    GeoJsonType["properties"]["zones"] = temp_zone;
        //                                }
        //                            }
        //                            GeoJsonType["properties"]["id"] = tagitem.ContainsKey("id") ? tagitem["id"] : "";
        //                            GeoJsonType["properties"]["visible"] = tagitem.ContainsKey("visible") ? tagitem["visible"] : false;
        //                            GeoJsonType["properties"]["color"] = tagitem.ContainsKey("color") ? tagitem["color"] : "";
        //                            GeoJsonType["properties"]["name"] = tagitem.ContainsKey("name") ? tagitem["name"] : "";
        //                            GeoJsonType["properties"]["positionTS"] = positionTS;
        //                            GeoJsonType["properties"]["Tag_TS"]= responseTS;
        //                            GeoJsonType["properties"]["Tag_Type"] = GetTagType(tagitem["name"].ToString());
        //                            GeoJsonType["properties"]["Raw_Data"] = tagitem;
        //                            GeoJsonType["properties"]["Tag_Update"] = true;
        //                            if (!AppParameters.Tag.ContainsKey(tagitem["id"].ToString()))
        //                            {
        //                                AppParameters.Tag.TryAdd(tagitem["id"].ToString(), GeoJsonType);
        //                            }
        //                        }
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        new ErrorLogger().ExceptionLog(e);
        //                    }
        //                }
        //                //}
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //    finally
        //    {
        //        item = null;
        //    }
        //}

        private static string GetTagType(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Regex.IsMatch(value, (string)AppParameters.AppSettings["TAG_AGV"], RegexOptions.IgnoreCase))
                    //if (value.ToLower().StartsWith("agv"))
                    {
                        return "Autonomous Vehicle";
                    }
                    else if (Regex.IsMatch(value, (string)AppParameters.AppSettings["TAG_PIV"], RegexOptions.IgnoreCase))
                    //if (value.ToLower().StartsWith("walkingrider") || value.ToLower().StartsWith("mule") || value.ToLower().StartsWith("forklift"))
                    {
                        return "Vehicle";
                    }
                    else if (Regex.IsMatch(value, (string)AppParameters.AppSettings["TAG_PERSON"], RegexOptions.IgnoreCase))
                    // else if (value.ToLower().StartsWith("mail") || value.ToLower().StartsWith("clerk") || value.ToLower().StartsWith("mha") || value.ToLower().StartsWith("maint") || value.ToLower().StartsWith("supervisor"))
                    {
                        return "Person";
                    }
                    else if (Regex.IsMatch(value, (string)AppParameters.AppSettings["TAG_LOCATOR"], RegexOptions.IgnoreCase))
                    // else if (value.ToLower().StartsWith("mail") || value.ToLower().StartsWith("clerk") || value.ToLower().StartsWith("mha") || value.ToLower().StartsWith("maint") || value.ToLower().StartsWith("supervisor"))
                    {
                        return "Locator";
                    }
                    else
                    {
                        return value;
                    }
                }
                else
                {
                    return "Unknown_Tag";
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "Error_Unknown_Tag";
            }
        }
        //private static string CheckNotification(string currentState, string NewState, string type, JObject data, string noteifi_id)
        //{
        //    string noteification_id = noteifi_id;
        //    try
        //    {
        //        if (currentState != NewState)
        //        {
        //            if (!string.IsNullOrEmpty(noteification_id) && AppParameters.NotificationList.ContainsKey(noteification_id))
        //            {
        //                if (AppParameters.NotificationList.TryGetValue(noteification_id, out JObject ojbMerge))
        //                {
        //                    if (!ojbMerge.ContainsKey("DELETE"))
        //                    {
        //                        ojbMerge["DELETE"] = true;
        //                        ojbMerge["UPDATE"] = true;
        //                        noteification_id = "";
        //                    }
        //                }
        //            }
        //            //new condition
        //            foreach (JObject newCondition in AppParameters.NotificationConditionsList.Where(r => Regex.IsMatch(NewState, r.Value["CONDITIONS"].ToString(), RegexOptions.IgnoreCase)
        //                && r.Value["TYPE"].ToString().ToLower() == type.ToLower()
        //                && (bool)r.Value["ACTIVE_CONDITION"]).Select(x => x.Value))
        //            {
        //                noteification_id = (string)newCondition["ID"] + (string)data["id"];
        //                if (!AppParameters.NotificationList.ContainsKey(noteification_id))
        //                {
        //                    JObject ojbMerge = (JObject)newCondition.DeepClone();
        //                    ojbMerge.Merge(data, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        //                    ojbMerge["SHOWTOAST"] = true;
        //                    ojbMerge["TAGID"] = (string)data["id"];
        //                    ojbMerge["notificationId"] = (string)newCondition["ID"] + (string)data["id"];
        //                    ojbMerge["UPDATE"] = true;
        //                    AppParameters.NotificationList.TryAdd((string)newCondition["ID"] + (string)data["id"], ojbMerge);
        //                }
        //            }
        //        }
        //        return noteification_id;
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return noteification_id;
        //    }
        //}
    }
}