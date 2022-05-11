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
        public void StartProcess(dynamic data, string Message_type, string connID)
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
                        case "getTagPosition":
                            TagPosition(data,connID);
                            break;
                        case "getProjectInfo":
                            ProjectData(data, connID);
                            break;
                        ///*Quuppa Data End*/
                        ///*SVWeb Data Start*/
                        case "doors":
                            Doors(data);
                            break;
                        case "trips":
                            Trips(data, Message_type);
                            break;
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
                        case "getTacsVsSels":
                            TacsVsSels(data, Message_type, connID);
                            break;
                        //case "TacsVsSelsAnomaly":
                        //    TacsVsSelsLDCAnomaly(data, Message_type);
                        //    break;
                        ///*SELS RT Data End*/
                        ///*IV Data Start*/
                        case "getStaffBySite":
                            Staffing(data);
                            break;
                        ///*IV Data End*/
                        ///*AGVM Data Start*/
                        case "FLEET_STATUS":
                            FLEET_STATUS(data);
                            break;
                        case "MATCHEDWITHWORK":
                            MATCHEDWITHWORK(data);
                            break;
                        case "SUCCESSFULPICKUP":
                            SUCCESSFULPICKUP(data);
                            break;
                        case "SUCCESSFULDROP":
                            SUCCESSFULDROP(data);
                            break;
                        case "ERRORWITHOUTWORK":
                            ERRORWITHOUTWORK(data);
                            break;
                        case "ERRORWITHWORK":
                            ERRORWITHWORK(data);
                            break;
                        case "MOVEREQUEST":
                            MOVEREQUEST(data);
                            break;
                        case "MISSIONCANCELED":
                            if (!data.ContainsKey("NASS_CODE"))
                            {
                                data["NASS_CODE"] = AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString();
                            } 
                            ERRORWITHWORK(data);
                            break;
                        case "MISSIONFAILED":
                            ERRORWITHWORK(data);
                            break;
                        ///*AGVM Data End*/
                        ///*MPEWatch Data Start*/
                        case "mpe_watch_id":
                            MPE_Watch_Id(data);
                            break;
                        case "rpg_run_perf":
                            MPEWatch_RPGPerf(data);
                            break;
                        case "rpg_plan":
                            MPEWatch_RPGPlan(data);
                            break;
                        case "dps_run_estm":
                            MPEWatch_DPSEst(data);
                            break;
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
        private void Staffing(dynamic data)
        {
            try
            {
                bool updatefile = false;
                if (data != null )
                {
                    JToken tempData = JToken.Parse(data);
                    IEnumerable<JToken> staff = tempData.SelectTokens("$..DATA[*]");
                    JArray sortplanlist = new JArray();
                    if (staff.Count() > 0)
                    {
                        foreach (JToken stafff_item in staff)
                        {
                            sortplanlist.Add(new JObject(new JProperty("mach_type", (string)stafff_item[0]),
                                new JProperty("machine_no", (int)stafff_item[1]),
                                new JProperty("sortplan", (string)stafff_item[2]),
                                new JProperty("clerk", stafff_item[3]),
                                new JProperty("mh", stafff_item[4])));
                        }

                    }
                    if (sortplanlist.HasValues)
                    {
                        foreach (JObject Dataitem in sortplanlist.Children())
                        {
                            if (!string.IsNullOrEmpty((string)Dataitem["sortplan"]))
                            {
                                string mach_type = Dataitem.ContainsKey("mach_type") ? (string)Dataitem["mach_type"] : "";
                                string machine_no = Dataitem.ContainsKey("machine_no") ? (string)Dataitem["machine_no"] : "";
                                string sortplan = Dataitem.ContainsKey("sortplan") ? (string)Dataitem["sortplan"] : "";
                                string sortplan_name = "";
                                if (mach_type == "APBS")
                                {
                                    mach_type = "SPBSTS";
                                }
                                //int dotindex = sortplan.IndexOf(".", 1);
                                //if ((dotindex == -1))
                                //{
                                //    sortplan_name = sortplan.Trim();
                                //}
                                //else
                                //{
                                //    sortplan_name = sortplan.Substring(0, dotindex).Trim();
                                //}
                                if (Regex.IsMatch(mach_type, "(DBCS|AFSM100|ATU|CIOSS|DIOSS)", RegexOptions.IgnoreCase))
                                {
                                    int dotindex = sortplan.IndexOf(".", 1);
                                    if ((dotindex == -1))
                                    {
                                        sortplan_name = sortplan;
                                    }
                                    else
                                    {
                                        sortplan_name = sortplan.Substring(0, dotindex);
                                    }
                                    sortplan = sortplan_name;
                                }
                                string mch_sortplan_id = mach_type + "-" + machine_no + "-" + sortplan;
                                string newtempData = JsonConvert.SerializeObject(Dataitem, Formatting.None);
                                AppParameters.StaffingSortplansList.AddOrUpdate(mch_sortplan_id, newtempData,
                                     (key, existingVal) =>
                                     {
                                         return newtempData;
                                     });
                            }
                        }
                    }
                    if (updatefile)
                    {
                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Staffing.json", JsonConvert.SerializeObject(AppParameters.StaffingSortplansList.Select(x => x.Value).ToList(), Formatting.Indented));
                    }

                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private void CameraData(dynamic data)
        {
            try
            {
                if (data != null)
                {
                    List<Cameras> newCameras = JsonConvert.DeserializeObject<List<Cameras>>(data);
                    foreach (Cameras camera_item in newCameras)
                    {
                        if (AppParameters.CameraInfoList.TryGetValue(camera_item.CameraName, out Cameras existingValue))
                        {
                            existingValue.FacilitySubtypeDesc = camera_item.FacilitySubtypeDesc; 
                            existingValue.AuthKey = camera_item.AuthKey;
                            existingValue.Description = camera_item.Description;
                            existingValue.FacilitiyLatitudeNum = camera_item.FacilitiyLatitudeNum;
                            existingValue.FacilitiyLongitudeNum = camera_item.FacilitiyLongitudeNum;
                            existingValue.FacilityDisplayName = camera_item.FacilityDisplayName;
                            existingValue.FacilityPhysAddrTxt = camera_item.FacilityPhysAddrTxt;
                            existingValue.GeoProcDivisionNm = camera_item.GeoProcDivisionNm;
                            existingValue.GeoProcRegionNm = camera_item.GeoProcRegionNm;
                            existingValue.LocaleKey = camera_item.LocaleKey;
                            existingValue.ModelNum = camera_item.ModelNum;
                            existingValue.Reachable = camera_item.Reachable;

                        }
                        else
                        {
                            if (!AppParameters.CameraInfoList.TryAdd(camera_item.CameraName, camera_item))
                            {
                                new ErrorLogger().CustomLog("Unable to Able to add Camera" + camera_item.CameraName, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));

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
                        foreach (ContainerHistory scan in d.ContainerHistory.OrderBy(o => o.EventDtmfmt))
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
                                    d.Trailer = scan.Trailer;
                                }
                                if (scan.Event == "UNLD")
                                {
                                    d.hasUnloadScans = true;
                                    d.Trailer = scan.Trailer;
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
                                    d.hasUnloadScans = true;
                                    d.Location = "X-Dock";
                                }
                            }
                        }
                        if (AppParameters.Containers.TryGetValue(d.PlacardBarcode, out Container exisitingContainer))
                        {
                            exisitingContainer.hasAssignScans = d.hasAssignScans;
                            exisitingContainer.hasCloseScans = d.hasCloseScans;
                            exisitingContainer.hasLoadScans = d.hasLoadScans;
                            exisitingContainer.hasUnloadScans = d.hasUnloadScans;
                            exisitingContainer.hasPrintScans = d.hasPrintScans;
                            exisitingContainer.containerTerminate = d.containerTerminate;
                            exisitingContainer.Location = d.Location;
                            exisitingContainer.containerTerminate = d.containerTerminate;
                            exisitingContainer.containerRedirectedDest = d.containerRedirectedDest;
                            exisitingContainer.containerAtDest = d.containerAtDest;
                            exisitingContainer.containerRedirectedDest = d.containerRedirectedDest;
                            exisitingContainer.ContainerHistory = d.ContainerHistory;
                        }
                        else
                        {
                            AppParameters.Containers.TryAdd(d.PlacardBarcode, d);
                        }
                     
                    }
                    Containers = null;
                    data = null;
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
        private static void Trips(dynamic data, string message_type)
        {

            try
            {
                if (data != null)
                {

                    JToken jsonObject = JToken.Parse(data);

                    foreach (JObject rt in jsonObject.Children())
                    {
                        if (rt.ContainsKey("routeTripId") && rt.ContainsKey("routeTripLegId") && rt.ContainsKey("tripDirectionInd"))
                        {
                            string routetripid = string.Concat(rt["routeTripId"].ToString(), rt["routeTripLegId"].ToString(), rt["tripDirectionInd"].ToString());
                            rt["id"] = routetripid;
                           
                            rt["rawData"] = JsonConvert.SerializeObject(rt, Formatting.None);
                            RouteTrips newRTData = rt.ToObject<RouteTrips>();
                            newRTData.TripMin = AppParameters.Get_TripMin(newRTData.ScheduledDtm);
                            // if trip does not exist and does not have CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE in LegStatus
                            // then add to list
                            if (AppParameters.RouteTripsList.ContainsKey(routetripid))
                            {
                                Task.Run(() => AddTriptoList(routetripid, newRTData));
                            }
                            else if (!AppParameters.RouteTripsList.ContainsKey(routetripid) &&
                                !(Regex.IsMatch(newRTData.LegStatus, "(CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE)", RegexOptions.IgnoreCase)
                                || Regex.IsMatch(newRTData.Status, "(CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE)", RegexOptions.IgnoreCase)))
                            {
                                Task.Run(() => AddTriptoList(routetripid, newRTData));
                            }
                            

                                //if (AppParameters.RouteTripsList.ContainsKey(routetripid))
                            //{

                            //    if (AppParameters.RouteTripsList.AddOrUpdate(routetripid, out RouteTrips existingVal))
                            //    {
                            //        if (rt.ToString() != existingVal.RawData)
                            //        {
                            //            existingVal.TripUpdate = true;
                            //            existingVal.Merge(rt, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                            //            //Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt["route"].ToString(), rt["trip"].ToString(), AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), GetSvDate((JObject)rt["operDate"])), rt["tripDirectionInd"].ToString(), routetripid));
                            //        }
                            //    }
                            //}
                            //else
                            //{

                            //    if (AppParameters.RouteTripsList.TryAdd(routetripid, rt.ToObject<RouteTrips>()))
                            //    {
                            //        // Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt["route"].ToString(), rt["trip"].ToString(), AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), GetSvDate((JObject)rt["operDate"])), rt["tripDirectionInd"].ToString(), routetripid));
                            //    }
                            //}
                        }

                    }
                    jsonObject = null;
                }
                if (AppParameters.RouteTripsList.Count > 0)
                {
                    foreach (string m in AppParameters.RouteTripsList.Where(r => r.Value.TripMin < -1440 ).Select(y => y.Key))
                    {
                        AppParameters.RouteTripsList.TryRemove(m, out RouteTrips value);
                    }
                }


            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static void AddTriptoList(string routetripid, RouteTrips newRTData)
        {
            try
            {
                AppParameters.RouteTripsList.AddOrUpdate(routetripid, newRTData,
                    (key, existingVal) =>
                    {
                        if (existingVal.RawData != newRTData.RawData)
                        {
                            newRTData.RawData = JsonConvert.SerializeObject(newRTData, Formatting.None);
                            newRTData.TripUpdate = true;
                            newRTData.State = existingVal.State;
                            newRTData.NotificationId = existingVal.NotificationId;
                            return newRTData;
                        }
                        else
                        {
                            return existingVal;
                        }

                    });
                if (newRTData.OperDate != null)
                {
                    Task.Run(() => new ItineraryTrip_Update(GetItinerary(newRTData.Route, newRTData.Trip, AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), AppParameters.GetSvDate(newRTData.OperDate)), routetripid));
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static string GetItinerary(string route, string trip, string nasscode, DateTime start_time)
        {
            string temp = "";
            try
            {
                //string start_time = string.Concat(DateTime.Now.ToString("yyyy-MM-dd'T'"), "00:00:00");

                Uri parURL = new Uri(string.Format((string)AppParameters.AppSettings["SV_ITINERARY"], route, trip, string.Concat(start_time.ToString("yyyy-MM-dd'T'"), "00:00:00")));
                string SV_Response = SendMessage.SV_Get(parURL.AbsoluteUri);
                if (!string.IsNullOrEmpty(SV_Response))
                {
                    temp = SV_Response;
                }
                return temp;
            }
            catch (Exception e)
            {   
                new ErrorLogger().ExceptionLog(e);
                return temp;
            }
        }
        private static void Doors(dynamic data)
        {
            try
            {
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    foreach (JObject item in tempData.Children())
                    {
                        string dockdoor_id = item.ContainsKey("doorNumber") ? item["doorNumber"].ToString() :"";
                        if (!string.IsNullOrEmpty(dockdoor_id))
                        {
                            string doorInfo = JsonConvert.SerializeObject(item, Formatting.None);
                            AppParameters.DockdoorList.AddOrUpdate(dockdoor_id, doorInfo,
                               (key, oldValue) =>
                               {
                                   return doorInfo;
                               });
                            doorInfo = null;
                        }
                     
                        string routetripid = "";
                        if (item.ContainsKey("routeTripId") && item.ContainsKey("routeTripLegId") && item.ContainsKey("tripDirectionInd"))
                        {
                            routetripid = string.Concat(item["routeTripId"].ToString(), item["routeTripLegId"].ToString(), item["tripDirectionInd"].ToString());
                        }

                        if (!string.IsNullOrEmpty(routetripid))
                        {
                            if (AppParameters.RouteTripsList.TryGetValue(routetripid, out RouteTrips trip))
                            {
                                trip.DoorId = item["doorId"].ToString();
                                trip.DoorNumber = item["doorNumber"].ToString();
                                //dooritem = JsonConvert.SerializeObject(trip, Formatting.None);
                                Task.Run(() => UpdateDoorZone(trip) );
                              
                            }
                            else
                            {
                                item["id"] = routetripid;
                                item["rawData"] = JsonConvert.SerializeObject(item, Formatting.None);
                                RouteTrips newRTData = item.ToObject<RouteTrips>();
                                newRTData.OperDate = newRTData.ScheduledDtm;
                                newRTData.TripMin = AppParameters.Get_TripMin(newRTData.ScheduledDtm);
                                Task.Run(() => AddTriptoList(routetripid, newRTData));
                                Task.Run(() => UpdateDoorZone(newRTData));
                            }
                        }
                        else
                        {
                            Task.Run(() => UpdateDoorZone(item.ToObject<RouteTrips>()));
                        }
                    }

                }
                data = null;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static void UpdateDoorZone(RouteTrips trip)
        {
            try
            {
         
                AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "DockDoor" && r.Value.Properties.DoorNumber == trip.DoorNumber).Select(y => y.Key).ToList().ForEach(key =>
                {
                    if (AppParameters.ZoneList.TryGetValue(key, out GeoZone doorZone))
                    {
                        doorZone.Properties.DockDoorData = trip;
                        doorZone.Properties.ZoneUpdate = true;
                    }
                });
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
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
        private static void TacsVsSels(dynamic data, string message_type, string conID)
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

                    TacsTags tag = JsonConvert.DeserializeObject<TacsTags>(data);
                    if (tag.MissedSels != null)
                    {
                        foreach (var MissedSelitem in tag.MissedSels)
                        {
                            if (AppParameters.TagsList.TryGetValue(MissedSelitem.TagId, out GeoMarker geoLmarker))
                            {
                                geoLmarker.Properties.Tacs = JsonConvert.SerializeObject(MissedSelitem.Tacs, Formatting.None);
                                geoLmarker.Properties.IsWearingTag = false;
                                geoLmarker.Properties.EmpId = MissedSelitem.EmpId;
                                geoLmarker.Properties.CraftName = GetCraftName(MissedSelitem.TagName);
                                geoLmarker.Properties.BadgeId = GetBadgeId(MissedSelitem.TagName);
                                geoLmarker.Properties.TagUpdate = true;
                            }
                            else
                            {
                                GeoMarker Lmarker = new GeoMarker();
                                Lmarker.Geometry.Coordinates = new List<double> { 0, 0 };
                                Lmarker.Properties.Id = MissedSelitem.TagId;
                                Lmarker.Properties.Name = MissedSelitem.TagName;
                                Lmarker.Properties.EmpId = MissedSelitem.EmpId;
                                Lmarker.Properties.TagType = "Person";
                                Lmarker.Properties.CraftName = GetCraftName(MissedSelitem.TagName);
                                Lmarker.Properties.BadgeId = GetBadgeId(MissedSelitem.TagName);
                                Lmarker.Properties.PositionTS = AppParameters.UnixTimeStampToDateTime((long)MissedSelitem.ProcessedTs);
                                Lmarker.Properties.TagVisible = false;
                                Lmarker.Properties.IsWearingTag = false;
                                Lmarker.Properties.TagUpdate = true;
                                if (!AppParameters.TagsList.TryAdd(MissedSelitem.TagId, Lmarker))
                                {
                                    new ErrorLogger().CustomLog("Unable to Add Marker" + MissedSelitem.TagId, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                }
                            }
                        }
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                    //JToken tempData = JToken.Parse(data);
                    //if (((JObject)tempData).ContainsKey("missedSels"))
                    //{
                    //    JToken missedSels = tempData.SelectToken("missedSels");
                    //    if (missedSels.HasValues)
                    //    {
                    //        foreach (JObject item in missedSels.Children())
                    //        {
                    //            if (AppParameters.TagsList.TryGetValue((string)item["tagId"], out GeoMarker geoLmarker))
                    //            {
                    //                geoLmarker.Properties.Tacs = item["tacs"].ToString();
                    //                geoLmarker.Properties.IsWearingTag = false;
                    //                geoLmarker.Properties.CraftName = GetCraftName((string)item["tagName"]);
                    //                geoLmarker.Properties.BadgeId = GetBadgeId((string)item["tagName"]);
                    //                geoLmarker.Properties.TagUpdate = true;

                    //            }
                    //            else
                    //            {
                    //                GeoMarker Lmarker = new GeoMarker();
                    //                Lmarker.Properties.Id = (string)item["tagId"];
                    //                Lmarker.Properties.Name = (string)item["tagName"];
                    //                Lmarker.Properties.TagType = "Person";

                    //                Lmarker.Properties.CraftName = GetCraftName(Lmarker.Properties.Name);
                    //                Lmarker.Properties.BadgeId = GetBadgeId(Lmarker.Properties.Name);

                    //                Lmarker.Properties.PositionTS = DateTime.Now.AddDays(-10).ToUniversalTime();
                    //                Lmarker.Properties.TagVisible = false;
                    //                Lmarker.Properties.IsWearingTag = false;
                    //                Lmarker.Properties.TagUpdate = true;
                    //                if (!AppParameters.TagsList.TryAdd(Lmarker.Properties.Id, Lmarker))
                    //                {
                    //                    new ErrorLogger().CustomLog("Unable to Add Marker" + Lmarker.Properties.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                    //                }
                    //            }
                    //            //if (AppParameters.Tag.ContainsKey((string)item["tagId"]))
                    //            //{
                    //            //    foreach (JObject m in AppParameters.Tag.Where(r => r.Key == (string)item["tagId"]).Select(y => y.Value))
                    //            //    {

                    //            //        if ((bool)m["properties"]["isWearingTag"] == true)
                    //            //        {
                    //            //            m["properties"]["isWearingTag"] = false;
                    //            //        }
                    //            //        if (m["properties"]["empId"] != item["empId"])
                    //            //        {
                    //            //            m["properties"]["empId"] = item["empId"];
                    //            //        }
                    //            //        if (m["properties"]["tacs"].ToString() != item["tacs"].ToString())
                    //            //        {
                    //            //            m["properties"]["tacs"] = item["tacs"];
                    //            //        }

                    //            //    }
                    //            //}
                    //            //else
                    //            //{
                    //            //    JObject GeoJsonType = new JObject_List().GeoJSON_Tag;
                    //            //    JArray temp_zone = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
                    //            //    ((JObject)GeoJsonType["geometry"])["type"] = "Point";
                    //            //    GeoJsonType["geometry"]["coordinates"] = new JArray(0, 0);
                    //            //    GeoJsonType["properties"]["id"] = (string)item["tagId"];
                    //            //    GeoJsonType["properties"]["zones"] = temp_zone;
                    //            //    GeoJsonType["properties"]["name"] = item["tagName"];
                    //            //    GeoJsonType["properties"]["Tag_Type"] = "Person";
                    //            //    GeoJsonType["properties"]["empId"] = item["empId"];
                    //            //    GeoJsonType["properties"]["tacs"] = item["tacs"];
                    //            //    GeoJsonType["properties"]["positionTS"] = DateTime.Now.AddDays(-10).ToUniversalTime();

                    //            //    //add to the tags
                    //            //    if (!AppParameters.Tag.ContainsKey((string)item["tagId"]))
                    //            //    {
                    //            //        AppParameters.Tag.TryAdd((string)item["tagId"], GeoJsonType);
                    //            //    }
                    //            //}
                    //        }
                    //        //if (AppParameters.Tag.Count() > 0)
                    //        //{
                    //        //    foreach (JObject m in AppParameters.Tag.Where(u => u.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")).Select(x => x.Value))
                    //        //    {
                    //        //        if (missedSels.SelectTokens("[?(@.tagId)]").Where(i => (string)i["tagId"] == (string)m["properties"]["id"]).ToList().Count == 0)
                    //        //        {
                    //        //            if (!(bool)m["properties"]["isWearingTag"])
                    //        //            {
                    //        //                m["properties"]["isWearingTag"] = true;

                    //        //            }
                    //        //        }
                    //        //    }
                    //        //}
                    //    }
                    //}
                    //else
                    //{
                    //    if (!((JObject)tempData).ContainsKey("missedSels") && ((JObject)tempData).ContainsKey("msg"))
                    //    {
                    //        if (tempData["msg"].ToString().StartsWith("No data"))
                    //        {
                    //            Task.Run(() => updateConnection(conID, "error"));
                    //        }
                    //    }
                    //}
                }

              
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                Task.Run(() => updateConnection(conID, "error"));
            }
             finally
            {
                data = null;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }
        }
        private static void MPE_Watch_Id(string data)
        {
            try
            {
                if (data != null)
                {
                    JToken jsonObject = JToken.Parse(data);
                    if (jsonObject.HasValues)
                    {
                        string MpewatchID = "{\"MPE_WATCH_ID\":\"" + jsonObject["id"] + "\"}";
                        FOTFManager.Instance.EditAppSettingdata(MpewatchID);

                    }
                }

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static void MPEWatch_RPGPerf(dynamic data)
        {
            try
            {
                string total_volume = "";
                string estCompletionTime = "";
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    JToken machineInfo = tempData.SelectToken("data");
                    if (machineInfo.HasValues)
                    {
                        DateTime dtNow = DateTime.Now;
                        string windowsTimeZoneId = "";
                        if (!string.IsNullOrEmpty((string)AppParameters.AppSettings["FACILITY_TIMEZONE"]))
                        {
                            AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings["FACILITY_TIMEZONE"], out windowsTimeZoneId);
                        }
                        foreach (JObject item in machineInfo.Children())
                        {
                            MPEWatch_FullBins(item);
                            total_volume = item.ContainsKey("tot_sortplan_vol") ? item["tot_sortplan_vol"].ToString().Trim() : "0";
                            int.TryParse(item.ContainsKey("rpg_est_vol") ? item["rpg_est_vol"].ToString().Trim() : "0", out int rpg_volume);
                            double.TryParse(item.ContainsKey("rpg_est_vol") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out double throughput);
                            int.TryParse(item.ContainsKey("rpg_est_vol") ? item["tot_sortplan_vol"].ToString().Trim() : "0", out int piecesfed);

                            if (rpg_volume > 0 && throughput > 0)
                            {
                                if (!string.IsNullOrEmpty(windowsTimeZoneId))
                                {
                                    dtNow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                                }
                                double intMinuteToCompletion = (rpg_volume - piecesfed) / (throughput / 60);
                                DateTime dtEstCompletion = dtNow.AddMinutes(intMinuteToCompletion);
                                estCompletionTime = dtEstCompletion.ToString("yyyy-MM-dd HH:mm:ss");
                                item["rpg_est_comp_time"] = estCompletionTime;
                            }
                            else
                            {
                                item["rpg_est_comp_time"] = "";
                            }
                            if (item["current_run_end"].ToString() == "" && item["current_run_start"].ToString() != "")
                            {
                                JObject results = new Oracle_DB_Calls().Get_RPG_Plan_Info(item);
                                item["rpg_start_dtm"] = results.ContainsKey("rpg_start_dtm") ? results["rpg_start_dtm"].ToString().Trim() : "";
                                item["rpg_end_dtm"] = results.ContainsKey("rpg_end_dtm") ? results["rpg_end_dtm"].ToString().Trim() : "";
                                item["expected_throughput"] = results.ContainsKey("expected_throughput") ? results["expected_throughput"].ToString().Trim() : "";
                                //item["throughput_status"] = "1";
                                if (!string.IsNullOrEmpty(item["expected_throughput"].ToString()) && item["expected_throughput"].ToString() != "0")
                                {
                                    int.TryParse(item.ContainsKey("cur_thruput_ophr") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out int cur_thruput);
                                    int.TryParse(item.ContainsKey("expected_throughput") ? item["expected_throughput"].ToString().Trim() : "0", out int expected_throughput);
                                    double thrper = (double)cur_thruput / (double)expected_throughput * 100;
                                    string throughputState = "1";
                                    if (thrper >= 100)
                                    {
                                        throughputState = "1";
                                    }
                                    else if (thrper >= 90)
                                    {
                                        throughputState = "2";
                                    }
                                    else if (thrper < 90)
                                    {
                                        throughputState = "3";
                                    }
                                    item["throughput_status"] = throughputState;
                                }
                            }
                            else
                            {
                                item["rpg_start_dtm"] = "";
                                item["rpg_end_dtm"] = "";
                                item["expected_throughput"] = "";
                                item["throughput_status"] = "1";
                            }

                            CheckMachineNotifications(item);
                            string MpeName = string.Concat(item["mpe_type"].ToString().Trim(), "-", item["mpe_number"].ToString().PadLeft(3, '0'));
                            string newMPEPerf = JsonConvert.SerializeObject(item, Formatting.Indented);
                            AppParameters.MPEPerformanceList.AddOrUpdate(MpeName, newMPEPerf,
                                  (key, oldValue) =>
                                  {
                                      return newMPEPerf;
                                  });
                            AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Machine" && r.Value.Properties.Name == MpeName).Select(y => y.Key).ToList().ForEach(key =>
                             {
                                 if (AppParameters.ZoneList.TryGetValue(key, out GeoZone machineZone))
                                 {
                                     //convert to string
                                     string temp = JsonConvert.SerializeObject(machineZone.Properties.MPEWatchData, Formatting.None);
                                     string tempItem = JsonConvert.SerializeObject(item, Formatting.None);
                                     if (temp != tempItem)
                                     {
                                         machineZone.Properties.ZoneUpdate = true;
                                     }
                                     
                                 }
                             });

                        }
                    }
                    machineInfo = null;
                    data = null;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }

        }
        private static void MPEWatch_FullBins(JObject data)
        {
            try
            {
                string MpeType = data["mpe_type"].ToString().Trim();
                string MpeNumber = data["mpe_number"].ToString().PadLeft(3, '0');
                List<string> FullBins = !string.IsNullOrEmpty(data["bin_full_bins"].ToString()) ? data["bin_full_bins"].ToString().Split(',').Select(p => p.Trim().TrimStart('0')).ToList() : new List<string>();
                foreach (string key in AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Bin" && r.Value.Properties.MPEType.Trim() == MpeType && r.Value.Properties.MPENumber.ToString().PadLeft(3, '0') == MpeNumber).Select(y => y.Key).ToList())
                {
                    List<string> FullBinList = new List<string>();
                    if (AppParameters.ZoneList.TryGetValue(key, out GeoZone binZone))
                    {
                        if (FullBins.Any())
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
        private static void MPEWatch_RPGPlan(dynamic data)
        {
            try
            {
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    JToken planInfo = tempData.SelectToken("data");
                    if (planInfo != null && planInfo.HasValues)
                    {
                        new Oracle_DB_Calls().Insert_RPG_Plan((JObject)tempData);
                    }
                    planInfo = null;
                    data = null;
                    tempData = null;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }
        private static void MPEWatch_DPSEst(dynamic data)
        {
            try
            {
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    JToken dpsInfo = tempData.SelectToken("data");
                    if (dpsInfo.HasValues)
                    {
                        int time_to_comp_optimal = 0;
                        int time_to_comp_actual = 0;
                        string time_to_comp_optimal_DateTime = "";
                        string time_to_comp_actual_DateTime = "";
                        DateTime dtNow = DateTime.Now;
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

                            int.TryParse(item.ContainsKey("time_to_comp_optimal") ? item["time_to_comp_optimal"].ToString().Trim() : "0", out time_to_comp_optimal);
                            DateTime dtCompOptimal = dtNow.AddSeconds(time_to_comp_optimal);
                            time_to_comp_optimal_DateTime = dtCompOptimal.ToString("yyyy-MM-dd HH:mm:ss");
                            item["time_to_comp_optimal_DateTime"] = time_to_comp_optimal_DateTime;

                            int.TryParse(item.ContainsKey("time_to_comp_actual") ? item["time_to_comp_actual"].ToString().Trim() : "0", out time_to_comp_actual);
                            DateTime dtCompActual = dtNow.AddSeconds(time_to_comp_actual);
                            time_to_comp_actual_DateTime = dtCompActual.ToString("MM/dd/yyyy HH:mm:ss");
                            item["time_to_comp_actual_DateTime"] = time_to_comp_actual_DateTime;
                            for (int i = 0; i < strSortPlanList.Length; i++)
                            {
                                string newDPS = JsonConvert.SerializeObject(item, Formatting.Indented);
                                AppParameters.DPSList.AddOrUpdate(strSortPlanList[i].Substring(0, 7), newDPS,
                                    (key, oldValue) =>
                                    {
                                        return newDPS;
                                    });

                            }
                            
                        }
                    }
                    tempData = null;
                    dpsInfo = null;
                    data = null;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }
        private static void ERRORWITHWORK(JObject data)
        {
            try
            {

                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
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
                    foreach (string existingkey in AppParameters.TagsList.Where(f => f.Value.Properties.Name == tempMission.VEHICLE
                   && f.Value.Properties.TagType.ToLower().EndsWith("Vehicle".ToLower())).Select(y => y.Key))
                    {
                        if (AppParameters.TagsList.TryGetValue(existingkey, out GeoMarker existingValue))
                        {
                            // get list of request for the pickup location.
                            existingValue.Properties.Misison = null;
                            existingValue.Properties.TagUpdate = true;
                        }
                    }
                    //remove request id
                    if (AppParameters.MissionList.Keys.Count > 0)
                    {
                        if (AppParameters.MissionList.ContainsKey(tempMission.REQUEST_ID) && !AppParameters.MissionList.TryRemove(tempMission.REQUEST_ID, out Mission mission))
                        {
                            new ErrorLogger().CustomLog("unable to remove Mission " + mission.REQUEST_ID + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
                        }
                    }
                    //update AGV zone location
                    foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                    f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value))
                    {
                        existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                        && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                        existingVa.Properties.ZoneUpdate = true;
                    }
                    foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                    f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value))
                    {
                        existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                        && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                        existingVa.Properties.ZoneUpdate = true;
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
        private static void ERRORWITHOUTWORK(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
                {
                    if (data.ContainsKey("VEHICLE"))
                    {
                        //match with vehicle
                        foreach (string existingkey in AppParameters.TagsList.Where(f => f.Value.Properties.Name == (string)data["VEHICLE"]
                       && f.Value.Properties.TagType.ToLower().EndsWith("Vehicle".ToLower())).Select(y => y.Key))
                        {
                            if (AppParameters.TagsList.TryGetValue(existingkey, out GeoMarker existingValue))
                            {
                                // get list of request for the pickup location.
                                existingValue.Properties.Misison = null;
                                existingValue.Properties.Vehicle_Status_Data.ERRORCODE = (string)data["errorCode".ToUpper()];
                                existingValue.Properties.Vehicle_Status_Data.ERRORCODE_DISCRIPTION = (string)data["error_Discription".ToUpper()];
                                existingValue.Properties.Vehicle_Status_Data.TIME = (DateTime)data["time".ToUpper()];
                                existingValue.Properties.TagUpdate = true;
                            }
                        }
                        //foreach (JObject existingVa in AppParameters.Tag.Where(f => f.Value["properties"]["name"].ToString() == (string)data["VEHICLE"]).Select(y => y.Value))
                        //{
                        //    if ((string)existingVa["properties"]["state"] != "Error")
                        //    {
                        //        existingVa["properties"]["notificationId"] = CheckNotification(existingVa["properties"]["state"].ToString(), "Error", "vehicle", (JObject)existingVa["properties"], existingVa["properties"]["notificationId"].ToString());
                        //        existingVa["properties"]["state"] = "Error";
                        //        existingVa["properties"]["errorCode"] = (string)data["errorCode".ToUpper()];
                        //        existingVa["properties"]["errorDiscription"] = (string)data["error_Discription".ToUpper()];
                        //        existingVa["properties"]["errorTime"] = (DateTime)data["time".ToUpper()];
                        //        existingVa["properties"]["Tag_Update"] = true;
                        //    }
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
                data = null;
            }
        }
        private static void SUCCESSFULDROP(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
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
                    foreach (string existingkey in AppParameters.TagsList.Where(f => f.Value.Properties.Name == tempMission.VEHICLE
                   && f.Value.Properties.TagType.ToLower().EndsWith("Vehicle".ToLower())).Select(y => y.Key))
                    {
                        if (AppParameters.TagsList.TryGetValue(existingkey, out GeoMarker existingValue))
                        {
                            // get list of request for the pickup location.
                            existingValue.Properties.Misison = null;
                            existingValue.Properties.TagUpdate = true;
                        }
                    }
                    //remove request id
                    if (AppParameters.MissionList.Keys.Count > 0)
                    {
                        if (AppParameters.MissionList.ContainsKey(tempMission.REQUEST_ID) && !AppParameters.MissionList.TryRemove(tempMission.REQUEST_ID, out Mission mission))
                        {
                            new ErrorLogger().CustomLog("unable to remove Mission " + mission.REQUEST_ID + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
                        }
                    }
                  
                    //update AGV zone location
                    foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                    f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value))
                    {
                        existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                        && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                        existingVa.Properties.ZoneUpdate = true;
                    }
                    foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                    f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value))
                    {
                        existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                        && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                        existingVa.Properties.ZoneUpdate = true;
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
        private static void SUCCESSFULPICKUP(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
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
                    foreach (string existingkey in AppParameters.TagsList.Where(f => f.Value.Properties.Name == tempMission.VEHICLE
                   && f.Value.Properties.TagType.ToLower().EndsWith("Vehicle".ToLower())).Select(y => y.Key))
                    {
                        if (AppParameters.TagsList.TryGetValue(existingkey, out GeoMarker existingValue))
                        {
                            // get list of request for the pickup location.
                            existingValue.Properties.Misison = AppParameters.MissionList.Where(r => r.Value.VEHICLE == tempMission.VEHICLE
                            && r.Value.STATE == tempMission.STATE
                            && r.Value.REQUEST_ID == tempMission.REQUEST_ID).Select(y => y.Value).FirstOrDefault();
                            existingValue.Properties.TagUpdate = true;
                        }
                    }
                    //update AGV zone location
                    foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                    f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value))
                    {
                        existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                        && r.Value.STATE == "Active").Select(y => y.Value).ToList();
                        existingVa.Properties.ZoneUpdate = true;
                    }
                    foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                    f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value))
                    {
                        existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                        && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                        existingVa.Properties.ZoneUpdate = true;
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
        private static void MATCHEDWITHWORK(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
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
                            new ErrorLogger().CustomLog("unable to add Mission " + tempMission.REQUEST_ID + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
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
                    foreach (string existingkey in AppParameters.TagsList.Where(f => f.Value.Properties.Name == tempMission.VEHICLE
                   && f.Value.Properties.TagType.ToLower().EndsWith("Vehicle".ToLower())).Select(y => y.Key))
                    {
                        if (AppParameters.TagsList.TryGetValue(existingkey, out GeoMarker existingValue))
                        {
                            // get list of request for the pickup location.
                            existingValue.Properties.Misison = AppParameters.MissionList.Where(r => r.Value.VEHICLE == tempMission.VEHICLE
                            && r.Value.STATE == tempMission.STATE
                            && r.Value.REQUEST_ID == tempMission.REQUEST_ID).Select(y => y.Value).FirstOrDefault();
                            existingValue.Properties.TagUpdate = true;
                        }
                    }
                    //update AGV zone location
                    foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                    f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value))
                    {
                        existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                        && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                        existingVa.Properties.ZoneUpdate = true;
                    }
                    foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                    f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value))
                    {
                        existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                        && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                        existingVa.Properties.ZoneUpdate = true;
                    }

                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static void MOVEREQUEST(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
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
                                new ErrorLogger().CustomLog("unable to add Mission " + tempMission.REQUEST_ID + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
                            }
                        }
                        else
                        {
                            if (AppParameters.MissionList.TryRemove(tempMission.REQUEST_ID,  out Mission valueOut))
                            {
                                if (!AppParameters.MissionList.TryAdd(tempMission.REQUEST_ID, tempMission))
                                {
                                    new ErrorLogger().CustomLog("unable to add Mission " + tempMission.REQUEST_ID + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
                                }
                            }
                        }
                        //update AGV zone location
                        foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.PICKUP_LOCATION).Select(y => y.Value))
                        {
                            existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.PICKUP_LOCATION == tempMission.PICKUP_LOCATION
                            && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                            existingVa.Properties.ZoneUpdate = true;
                        }
                        foreach (GeoZone existingVa in AppParameters.ZoneList.Where(f => f.Value.Properties.ZoneType == "AGVLocation" &&
                        f.Value.Properties.Name == tempMission.DROPOFF_LOCATION).Select(y => y.Value))
                        {
                            existingVa.Properties.MissionList = AppParameters.MissionList.Where(r => r.Value.DROPOFF_LOCATION == tempMission.DROPOFF_LOCATION
                            && r.Value.STATE == tempMission.STATE).Select(y => y.Value).ToList();
                            existingVa.Properties.ZoneUpdate = true;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static void FLEET_STATUS(JObject data)
        {
            bool update = false;
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
                {
                    if (data.ContainsKey("VEHICLE"))
                    {
                        VehicleStatus newVehicleStatus = data.ToObject<VehicleStatus>(new JsonSerializer { NullValueHandling = NullValueHandling.Ignore });
                        foreach (string existingKey in AppParameters.TagsList.Where(u => u.Value.Properties.TagType.ToLower().EndsWith("Vehicle".ToLower()) &&
                         u.Value.Properties.Name == newVehicleStatus.VEHICLE).Select(x => x.Key))
                        {
                            if (AppParameters.TagsList.TryGetValue(existingKey, out GeoMarker existingValue))
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
                                        existingValue.Properties.TagUpdate = true;
                                    }
                                }
                                else
                                {
                                    existingValue.Properties.Vehicle_Status_Data = newVehicleStatus;
                                    existingValue.Properties.NotificationId = CheckNotification("", newVehicleStatus.STATE, "vehicle".ToLower(), existingValue.Properties, existingValue.Properties.NotificationId);
                                    existingValue.Properties.TagUpdate = true;
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

        private static string CheckNotification(string currentState, string NewState, string type, Marker properties, string noteification_Id)
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
                       newNotifi.Type_ID = properties.Id ; 
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

        private static void ProjectData(dynamic jsonObject, string connID)
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
                            backgroundImages["rawData"] = JsonConvert.SerializeObject(backgroundImages, Formatting.None);
                            //this is for existing images
                            if (AppParameters.IndoorMap.TryGetValue(backgroundImages["id"].ToString(), out BackgroundImage bckimg))
                            {
                                if (bckimg.RawData != backgroundImages["rawData"].ToString())
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
                                newbckimg.RawData = JsonConvert.SerializeObject(backgroundImages.ToString(), Formatting.None);
                                newbckimg.FacilityName = AppParameters.AppSettings["FACILITY_NAME"].ToString();
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
                                bool zoneUpdate = false;
                                if (AppParameters.ZoneInfo.TryGetValue(zoneitem["id"].ToString(), out ZoneInfo zoneinfodata))
                                {
                                    JObject zinfo = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(zoneinfodata, Formatting.Indented));
                                    zoneitem.Merge(zinfo, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                }
                            if (AppParameters.ZoneList.TryGetValue(zoneitem["id"].ToString(), out GeoZone gZone))
                                {
                                    ZoneGeometry tempGeometry = GetQuuppaZoneGeometry(zoneitem["polygonData"]);
                                    if (JsonConvert.SerializeObject(gZone.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(tempGeometry.Coordinates, Formatting.None))
                                    {
                                        gZone.Geometry.Coordinates = tempGeometry.Coordinates;
                                        zoneUpdate = true;
                                    }
                                    if (gZone.Properties.Name != zoneitem["name"].ToString())
                                    {
                                        gZone.Properties.Name = zoneitem["name"].ToString();
                                        zoneUpdate = true;
                                    }
                                    string temptype = GetZoneType(gZone.Properties.Name);
                                    if (temptype != gZone.Properties.ZoneType)
                                    {
                                        gZone.Properties.Name = temptype;
                                        zoneUpdate = true;
                                    }
                                    if (zoneUpdate)
                                    {
                                        gZone.Properties.ZoneUpdate = true;
                                    }
                                    
                                }
                                else
                                {
                                    GeoZone newGZone = new GeoZone();
                                    newGZone.Geometry = GetQuuppaZoneGeometry(zoneitem["polygonData"]);
                                    newGZone.Properties.Id = zoneitem["id"].ToString();
                                    newGZone.Properties.Name = zoneitem["name"].ToString();
                                    newGZone.Properties.Color = zoneitem["color"].ToString();
                                    newGZone.Properties.Visible = (bool)zoneitem["visible"];
                                    newGZone.Properties.ZoneType = GetZoneType(newGZone.Properties.Name);
                                    newGZone.Properties.Source = "other";

                                    if (newGZone.Properties.ZoneType.StartsWith("DockDoor"))
                                    {
                                        //get the DockDoor Number
                                        if (int.TryParse(string.Join(string.Empty, Regex.Matches(newGZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                                        {
                                            newGZone.Properties.DoorNumber = n.ToString();
                                        }
                                    }

                                    if (newGZone.Properties.ZoneType == "Machine")
                                    {
                                        //get the MPE Number
                                        if (int.TryParse(string.Join(string.Empty, Regex.Matches(newGZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                                        {
                                            newGZone.Properties.MPENumber = n;
                                        }
                                        //get the MPE Name
                                        newGZone.Properties.MPEType = string.Join(string.Empty, Regex.Matches(newGZone.Properties.Name, @"\p{L}+").OfType<Match>().Select(m => m.Value));
                                    }
                                    newGZone.Properties.ZoneUpdate = true;
                                    if (!AppParameters.ZoneList.TryAdd(newGZone.Properties.Id, newGZone))
                                    {
                                        new ErrorLogger().CustomLog("Unable to Add Zone" + newGZone.Properties.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
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
                                bool locatorupdate = false;
                                if (AppParameters.TagsList.TryGetValue(locatorsitem["id"].ToString(), out GeoMarker geoLmarker))
                                {
                                    // check if position changed
                                
                                    MarkerGeometry tempGeometry = GetQuuppaTagGeometry(locatorsitem["location"]);
                                    if (JsonConvert.SerializeObject(geoLmarker.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(tempGeometry.Coordinates, Formatting.None))
                                    {
                                        geoLmarker.Geometry.Coordinates = tempGeometry.Coordinates;
                                        locatorupdate = true;
                                    }

                                    if (geoLmarker.Properties.Name != locatorsitem["name"].ToString())
                                    {
                                        geoLmarker.Properties.Name = locatorsitem["name"].ToString();
                                        string tempTagype = GetTagType(geoLmarker.Properties.Name);
                                        if (geoLmarker.Properties.TagType != tempTagype)
                                        {
                                            geoLmarker.Properties.TagType = tempTagype;
                                            locatorupdate = true;
                                        }
                                        if (geoLmarker.Properties.TagType == "Person")
                                        {
                                            geoLmarker.Properties.CraftName = GetCraftName(geoLmarker.Properties.Name);
                                            geoLmarker.Properties.BadgeId = GetBadgeId(geoLmarker.Properties.Name);
                                            locatorupdate = true;
                                        }

                                        locatorupdate = true;
                                    }
                                    if (locatorupdate)
                                    {
                                        geoLmarker.Properties.TagUpdate = true;
                                    }
                                  
                                }
                                else
                                {
                                    GeoMarker Lmarker = new GeoMarker();
                                    Lmarker.Type = "Feature";
                                    Lmarker.Properties.Id = locatorsitem["id"].ToString();
                                    Lmarker.Properties.Name = locatorsitem["name"].ToString();
                                    Lmarker.Properties.Color = locatorsitem["color"].ToString();
                                    Lmarker.Properties.TagType = GetTagType(Lmarker.Properties.Name);
                                    Lmarker.Geometry = GetQuuppaTagGeometry(locatorsitem["location"]);
                                    Lmarker.Properties.TagVisible = (bool)locatorsitem["visible"];
                                    Lmarker.Properties.Source = "other";
                                    Lmarker.Properties.TagUpdate = true;
                                    if (!AppParameters.TagsList.TryAdd(Lmarker.Properties.Id, Lmarker))
                                    {
                                        new ErrorLogger().CustomLog("Unable to Add Locater" + Lmarker.Properties.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                    }

                                }
                            }
                        }
                    }
                    //log Project Data to locale drive.
                    if (!((JObject)tempData).ContainsKey("localdata"))
                    {
                        tempData["localdata"] = true;
                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "ProjectData.json", JsonConvert.SerializeObject(tempData, Formatting.Indented));
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            
        }
        private void TagPosition(dynamic data, string conID)
        {
            try
            {
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    if (((JObject)tempData).ContainsKey("tags"))
                    {
                        //this is for tags
                        JToken tags = tempData.SelectToken("tags");
                        if (tags.Count() > 0)
                        {
                        
                            DateTime responseTS = AppParameters.UnixTimeStampToDateTime((long)tempData["responseTS"]);
                        
                            foreach (JObject tagitem in tags.Children())
                            {
                                bool update = false;
                                tagitem["rawData"] = JsonConvert.SerializeObject(tagitem, Formatting.None);
                                string tagid = tagitem.ContainsKey("id") ? tagitem["id"].ToString() : tagitem["tagId"].ToString();
                                if (AppParameters.TagsList.TryGetValue(tagid, out GeoMarker geoLmarker))
                                {
                                    // check if position changed
                                    JToken position = tagitem.ContainsKey("smoothedPosition") ? tagitem["smoothedPosition"] : tagitem["location"];
                                    MarkerGeometry tempGeometry = GetQuuppaTagGeometry(position);
                                    if (JsonConvert.SerializeObject(geoLmarker.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(tempGeometry.Coordinates, Formatting.None))
                                    {
                                        geoLmarker.Geometry.Coordinates = tempGeometry.Coordinates;
                                        update = true;
                                    }

                                    JToken positionTs = tagitem.ContainsKey("positionTS") ? tagitem["positionTS"] : tagitem["locationTS"];
                                    geoLmarker.Properties.PositionTS = AppParameters.UnixTimeStampToDateTime((long)positionTs);
                                    geoLmarker.Properties.TagTS = responseTS;
                                    geoLmarker.Properties.Zones = tagitem["zones"].ToObject<List<Zone>>();
                                    string tempName = tagitem.ContainsKey("name") ? tagitem["name"].ToString() : tagitem["tagName"].ToString();
                                    if (geoLmarker.Properties.Name != tempName)
                                    {
                                        geoLmarker.Properties.Name = tempName;
                                        string tempTagype = GetTagType(geoLmarker.Properties.Name);
                                        if (geoLmarker.Properties.TagType != tempTagype)
                                        {
                                            geoLmarker.Properties.TagType = tempTagype;
                                            update = true;
                                        }
                                        if (geoLmarker.Properties.TagType == "Person")
                                        {
                                            geoLmarker.Properties.CraftName = GetCraftName(geoLmarker.Properties.Name);
                                            geoLmarker.Properties.BadgeId = GetBadgeId(geoLmarker.Properties.Name);
                                            update = true;
                                        }

                                        update = true;
                                    }
                                    if (update)
                                    {
                                        geoLmarker.Properties.TagUpdate = true;
                                    }
                                }
                                else
                                {

                                    GeoMarker Lmarker = new GeoMarker();
                                    Lmarker.Properties.Id = tagid;
                                    Lmarker.Properties.Name = tagitem.ContainsKey("name") ? tagitem["name"].ToString() : tagitem["tagName"].ToString();
                                    Lmarker.Properties.Color = tagitem["color"].ToString();
                                    Lmarker.Properties.TagType = GetTagType(Lmarker.Properties.Name);
                                    if (Lmarker.Properties.TagType == "Person")
                                    {
                                        Lmarker.Properties.CraftName = GetCraftName(Lmarker.Properties.Name);
                                        Lmarker.Properties.BadgeId = GetBadgeId(Lmarker.Properties.Name);
                                    }
                                  
                                    Lmarker.Properties.TagTS = responseTS;
                                    JToken positionTs = tagitem.ContainsKey("positionTS") ? tagitem["positionTS"] : tagitem["locationTS"];
                                    Lmarker.Properties.PositionTS = AppParameters.UnixTimeStampToDateTime((long)positionTs);
                                    JToken position = tagitem.ContainsKey("smoothedPosition") ? tagitem["smoothedPosition"] : tagitem["location"];
                                    Lmarker.Geometry = GetQuuppaTagGeometry(position);
                                    Lmarker.Properties.RawData = tagitem["rawData"].ToString();
                                    Lmarker.Properties.TagVisible = tagitem.ContainsKey("locationMovementStatus") ? tagitem["locationMovementStatus"].ToString() == "noData" ? false : true  : false;
                                    Lmarker.Properties.TagUpdate = true;
                                    if (!AppParameters.TagsList.TryAdd(tagid, Lmarker))
                                    {
                                        new ErrorLogger().CustomLog("Unable to Add Marker" + tagid, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                    }
                                }
                            }
                        }
                    }
                    if (!((JObject)tempData).ContainsKey("tags") && ((JObject)tempData).ContainsKey("status"))
                    {
                        if (tempData["status"].ToString() == "GeneralFailure")
                        {
                            Task.Run(() => updateConnection(conID, "error"));
                        } 
                    }
                }
            }
            catch (Exception e)
            {

                new ErrorLogger().ExceptionLog(e);
            }
        }
        internal static void updateConnection(string conId,string type)
        {
            try
            {
               if(AppParameters.ConnectionList.TryGetValue(conId, out Connection m ))
                {
                    m.ApiConnected = type == "error" ? false : true;
                    m.UpdateStatus = true;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        internal static string GetBadgeId(string name)
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
        internal static string GetCraftName(string name)
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
        private static MarkerGeometry GetQuuppaTagGeometry(JToken tagitemsplit)
        {
            try
            {
                JObject geometry = new JObject();
                JArray temp = new JArray();
                if (tagitemsplit.HasValues)
                {
                  temp = new JArray(tagitemsplit[0], tagitemsplit[1]);
                }
                else
                {
                    temp = new JArray(0.0, 0.0);
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
        private static ZoneGeometry GetQuuppaZoneGeometry(JToken zoneitem)
        {
            try
            {
                JObject geometry = new JObject();
                JArray temp = new JArray();

                string[] polygonDatasplit = zoneitem.ToString().Split('|');
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

                geometry["coordinates"] = temp;

                ZoneGeometry result = geometry.ToObject<ZoneGeometry>();

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

        private static void CheckMachineNotifications(JObject machineData)
        {
                try
                {
                string zoneID = AppParameters.ZoneList.Where(x => x.Value.Properties.ZoneType == "Machine" &&
                            x.Value.Properties.MPEType == machineData["mpe_type"].ToString() &&
                            x.Value.Properties.MPENumber.ToString() == machineData["mpe_number"].ToString())
                               .Select(l => l.Value.Properties.Id).FirstOrDefault().ToString();
                string machine = machineData["mpe_type"].ToString().Trim() + machineData["mpe_number"].ToString().Trim();

                CheckMachineThroughPutNotification(machineData, zoneID, machine);
                CheckUnplannedMaintNotification(machineData, zoneID, machine);
                CheckOPStartingLateNotification(machineData, zoneID, machine);
                CheckOPRunningLateNatification(machineData, zoneID, machine);
                CheckSortplanWrongNotification(machineData, zoneID, machine);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void UpdateDeleteMachineNotifications(string notificationID, string notificationName, string timerName, string duration, string notificationValue, string timerValue)
        {
            //Notification existingNotifiaction = AppParameters.NotificationList.Where(x => x.Value.Notification_ID == notificationID).FirstOrDefault();
            if (AppParameters.NotificationList.TryGetValue(notificationID, out Notification ojbMerge))
            {
                if (notificationValue == "0" || string.IsNullOrEmpty(notificationValue))
                {
                    ojbMerge.Delete = true;
                    ojbMerge.Notification_Update = true;
                    //ojbMerge["DELETE"] = true;
                    //ojbMerge["UPDATE"] = true;
                }
                else
                {
                    //ojbMerge.Name = notificationValue;
                    ojbMerge.Type_Duration = Convert.ToInt32(timerValue);
                    ojbMerge.Notification_Update = true;
                    //ojbMerge[notificationName] = notificationValue;
                    //ojbMerge["durationTime"] = timerValue;
                    //ojbMerge["durationText"] = duration;
                    //ojbMerge["UPDATE"] = true;
                }
            }
        }

        private static void AddNewMachineNotification(JObject machineData, string zoneID, string notificationID, string notificationType, string durationtext, string durationTime)
        {
            try
            {
                foreach (NotificationConditions newCondition in AppParameters.NotificationConditionsList.Where(r => Regex.IsMatch(notificationType, r.Value.Conditions, RegexOptions.IgnoreCase)
                            //&& r.Value.Type.ToLower() == "automation".ToLower()
                            && r.Value.Type.ToLower() == "mpe".ToLower()
                            && (bool)r.Value.ActiveCondition).Select(x => x.Value).ToList())
                //foreach (JObject newCondition in Global.Notification_Conditions.Where(r => Regex.IsMatch(notificationType, r.Value["CONDITIONS"].ToString(), RegexOptions.IgnoreCase)
                //            && r.Value["TYPE"].ToString().ToLower() == "automation".ToLower()
                //            && (bool)r.Value["ACTIVE_CONDITION"]).Select(x => x.Value))
                {
                    if (!AppParameters.NotificationList.ContainsKey(notificationID))
                    {
                        int intStr = 0;
                        int.TryParse(durationTime, out intStr);
                        string machineName = machineData["mpe_type"].ToString().Trim() + "-" + machineData["mpe_number"].ToString().Trim().PadLeft(3, '0');
                        Notification ojbMerge = new Notification
                        {
                            Type = newCondition.Type,
                            Name = newCondition.Name,
                            Type_ID = zoneID,
                            Notification_ID = notificationID,
                            Notification_Update = true,
                            Type_Duration = intStr,//Convert.ToInt32(durationTime)
                            Type_Name = machineName,
                            Warning = newCondition.Warning,
                            Critical = newCondition.Critical,
                            WarningAction = newCondition.WarningAction,
                            CriticalAction = newCondition.CriticalAction
                        };
                        AppParameters.NotificationList.TryAdd(notificationID, ojbMerge);
                        //JObject ojbMerge = (JObject)newCondition.DeepClone();
                        //ojbMerge.Merge(machineData, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                        //ojbMerge["SHOWTOAST"] = true;
                        //ojbMerge["ZONEID"] = zoneID;
                        //ojbMerge["NOTIFICATIONGID"] = notificationID;
                        //ojbMerge["UPDATE"] = true;
                        //ojbMerge["durationTime"] = durationTime;
                        //ojbMerge["durationText"] = durationtext;
                        //Global.Notification.TryAdd(notificationID, ojbMerge);

                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static string GetMachineNotificationDurationText(string durationSeconds)
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

        private static void CheckOPStartingLateNotification(JObject machineData, string zoneID, string machine)
        {
            string notification_name = "op_started_late_status";
            string notification_timer = "op_started_late_timer";
            //string notificationID = zoneID + "_op_started_late_status";
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

        private static void CheckUnplannedMaintNotification(JObject machineData, string zoneID, string machine)
        {
            string notification_name = "unplan_maint_sp_status";
            string notification_timer = "unplan_maint_sp_timer";
            //string notificationID = zoneID + "_unplan_maint_sp_status";
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

        private static void CheckSortplanWrongNotification(JObject machineData, string zoneID, string machine)
        {
            string notification_name = "sortplan_wrong_status";
            string notification_timer = "sortplan_wrong_timer";
            //string notificationID = zoneID + "_sortplan_wrong_status";
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

        private static void CheckOPRunningLateNatification(JObject machineData, string zoneID, string machine)
        {
            string notification_name = "op_running_late_status";
            string notification_timer = "op_running_late_timer";
            //string notificationID = zoneID + "_op_running_late_status";
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

        private static void CheckMachineThroughPutNotification(JObject machineData, string zoneID, string machine)
        {
            //string notificationID = zoneID + "_throughput_status";
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
                            //ojbMerge["DELETE"] = true;
                            //ojbMerge["UPDATE"] = true;
                            ojbMerge.Delete = true;
                            ojbMerge.Notification_Update = true;
                        }
                        else
                        {
                            //string prev_throughput_status = ojbMerge.ContainsKey("throughput_status") ? ojbMerge["throughput_status"].ToString().Trim() : "1";
                            string prev_throughput_status = ojbMerge.Type_Status.ToString().Trim();
                            if (prev_throughput_status != throughput_status)
                            {
                                ojbMerge.Type_Status = throughput_status;
                                ojbMerge.Notification_Update = true;
                                //ojbMerge["throughput_status"] = throughput_status;
                                //ojbMerge["UPDATE"] = true;
                            }
                            else
                            {
                                ojbMerge.Notification_Update = true;
                                //ojbMerge["UPDATE"] = true;
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
                            if (!string.IsNullOrEmpty((string)AppParameters.AppSettings["FACILITY_TIMEZONE"]))
                            {
                                if (AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings["FACILITY_TIMEZONE"], out string windowsTimeZoneId))
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
    }
}