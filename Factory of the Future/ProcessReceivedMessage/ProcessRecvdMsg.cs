using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
                            CameraData(data, connID);
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
                            Doors(data, connID);
                            break;
                        case "trips":
                            Trips(data, Message_type, connID);
                            break;
                        case "container":
                            Container(data, connID);
                            break;
                        case "getTacsVsSels":
                            TacsVsSels(data, Message_type, connID);
                            break;
                        case "getTacsVsSels_Summary":
                            TacsVsSels(data, Message_type, connID);
                            break;
                        //case "TacsVsSelsAnomaly":
                        //    TacsVsSelsLDCAnomaly(data, Message_type);
                        //    break;
                        ///*SELS RT Data End*/
                        ///*IV Data Start*/
                        case "getStaffBySite":
                            Staffing(data, connID);
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
                            MPEWatch_RPGPerf(data, connID);
                            break;
                        case "rpg_plan":
                            MPEWatch_RPGPlan(data, connID);
                            break;
                        case "dps_run_estm":
                            MPEWatch_DPSEst(data, connID);
                            break;
                        ///*MPEWatch Data End*/
                        
                        case "getSVZones":
                            SVZones(data, connID);
                            break;
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
        private void Staffing(dynamic data ,string conID)
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
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
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
                                if (mach_type == "APBS")
                                {
                                    mach_type = "SPBSTS";
                                }
                                
                                sortplan = AppParameters.SortPlan_Name_Trimer(sortplan);
                                string mch_sortplan_id = mach_type + "-" + machine_no + "-" + sortplan;
                                string newtempData = JsonConvert.SerializeObject(Dataitem, Formatting.None);
                                AppParameters.StaffingSortplansList.AddOrUpdate(mch_sortplan_id, newtempData,
                                     (key, existingVal) =>
                                     {
                                         updatefile = true;
                                         return newtempData;
                                     });
                            }
                        }
                        Task.Run(() => updateConnection(conID, "good"));
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                    if (updatefile)
                    {
                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Staffing.json", JsonConvert.SerializeObject(AppParameters.StaffingSortplansList.Select(x => x.Value).ToList(), Formatting.Indented));
                    }

                }
                else
                {
                    Task.Run(() => updateConnection(conID, "error"));
                }
            }
            catch (Exception e)
            {
                Task.Run(() => updateConnection(conID, "error"));
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private void CameraData(dynamic data, string conID)
        {
            try
            {
                if (data != null)
                {
                    List<Cameras> newCameras = JsonConvert.DeserializeObject<List<Cameras>>(data);
                    if (newCameras.Count > 0)
                    {
                        foreach (Cameras camera_item in newCameras)
                        {
                            //camera_item.Base64 = GetImgae(camera_item);
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
                        Task.Run(() => updateConnection(conID, "good"));
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                }
               
            }
            catch (Exception e)
            {
                Task.Run(() => updateConnection(conID, "error"));
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private string GetImgae(Cameras camera_item)
        {
            try
            {
                string formatUrl = string.Concat("http://", camera_item.CameraName, "/axis-cgi/jpg/image.cgi?resolution=640x480");
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(formatUrl);
                using (HttpWebResponse Response = (HttpWebResponse)request.GetResponse())
                {
                    if (Response.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new System.IO.StreamReader(Response.GetResponseStream(), ASCIIEncoding.ASCII))
                        {
                            return "";
                        }
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

        private static void Container(dynamic data, string conID)
        {

            try
            {

                if (data != null)
                {
                    List<Container> Containers = JsonConvert.DeserializeObject<List<Container>>(data);
                    if (Containers.Count > 0)
                    {
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
                        Task.Run(() => updateConnection(conID, "good"));
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                }
                else
                {
                    Task.Run(() => updateConnection(conID, "error"));
                }

                if (AppParameters.Containers.Count > 0)
                {
                    foreach (string m in AppParameters.Containers.Where(r => DateTime.Now.Subtract(r.Value.EventDtm).TotalDays >= 3).Select(y => y.Key))
                    {
                        AppParameters.Containers.TryRemove(m, out Container outc);

                    }
                }
                CheckScanNotification();
            }
            catch (Exception e)
            {
                Task.Run(() => updateConnection(conID, "error"));
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }

        }
        private static void Trips(dynamic data, string message_type, string conID)
        {

            try
            {
                if (data != null)
                {
                    JToken jsonObject = JToken.Parse(data);
                    if (jsonObject != null && jsonObject.HasValues)
                    {
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
                        Task.Run(() => updateConnection(conID, "good"));
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                    jsonObject = null;
                }
                else
                {
                    Task.Run(() => updateConnection(conID, "error"));
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
                Task.Run(() => updateConnection(conID, "error"));
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
        private static void SVZones(dynamic data, string conID)
        {

            try
            {
                if (data != null)
                {
                    List<SV_Bullpen> SV_Bullpen = JsonConvert.DeserializeObject<List<SV_Bullpen>>(data);
                    if (SV_Bullpen.Count > 0)
                    {
                        foreach (SV_Bullpen Bullpen_item in SV_Bullpen)
                        {

                            AppParameters.SVZoneNameList.AddOrUpdate(Bullpen_item.LocationId, Bullpen_item,
                               (key, oldValue) =>
                               {
                                   return Bullpen_item;
                               });
                        }
                        //JToken tempData = JToken.Parse(data);
                        //if (tempData != null && tempData.HasValues)
                        //{
                        //    foreach (JObject item in tempData.Children())
                        //    {
                        //        string svzone_id = item.ContainsKey("locationId") ? item["locationId"].ToString() : "";
                        //        string zoneName = item["locationName"].ToString();
                        //        if (!string.IsNullOrEmpty(svzone_id))
                        //        {

                        //            AppParameters.SVZoneNameList.AddOrUpdate(svzone_id, zoneName,
                        //               (key, oldValue) =>
                        //               {
                        //                   return zoneName;
                        //               });
                        //        }
                        //    }
                        //}
                        Task.Run(() => updateConnection(conID, "good"));
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                }
            }

            catch (Exception e)
            {
                Task.Run(() => updateConnection(conID, "error"));
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static void Doors(dynamic data, string conID)
        {
            try
            {
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    if (tempData !=null && tempData.HasValues)
                    {
                        foreach (JObject item in tempData.Children())
                        {
                            string dockdoor_id = item.ContainsKey("doorNumber") ? item["doorNumber"].ToString() : "";
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
                                    trip.RawData = JsonConvert.SerializeObject(item, Formatting.None);
                                    //dooritem = JsonConvert.SerializeObject(trip, Formatting.None);
                                    Task.Run(() => UpdateDoorZone(trip));

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
                                item["rawData"] = JsonConvert.SerializeObject(item, Formatting.None);
                                Task.Run(() => UpdateDoorZone(item.ToObject<RouteTrips>()));
                            }
                        }
                        Task.Run(() => updateConnection(conID, "good"));
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                }
                else
                {
                    Task.Run(() => updateConnection(conID, "error"));
                }
                data = null;
            }
            catch (Exception e)
            {
                Task.Run(() => updateConnection(conID, "error"));
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private static void UpdateDoorZone(RouteTrips trip)
        {
            try
            {

                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => f.Value.Properties.ZoneType == "DockDoor"
                    && f.Value.Properties.DoorNumber == trip.DoorNumber
                    ).Select(y => y.Value).ToList().ForEach(DockDoor =>
                    {
                        if(DockDoor.Properties.DockDoorData.RawData != trip.RawData)
                        {
                            DockDoor.Properties.ZoneUpdate = true;
                        }
                        DockDoor.Properties.DockDoorData = trip;
                        
                    });
                }

                //AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "DockDoor" 
                //&& r.Value.Properties.DoorNumber == trip.DoorNumber).Select(y => y.Key).ToList().ForEach(key =>
                //{
                //    if (AppParameters.ZoneList.TryGetValue(key, out GeoZone doorZone))
                //    {
                //        doorZone.Properties.DockDoorData = trip;
                //        doorZone.Properties.ZoneUpdate = true;
                //    }
                //});
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
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
                    if(message_type == "getTacsVsSels_Summary")
                    {
                        JToken tempData = JToken.Parse(data);
                        //JArray ja = JArray.Parse(data);
                        foreach(JObject item in tempData)
                        {
                            string badgeID = !string.IsNullOrEmpty(item["tagId"].ToString()) ? item["tagId"].ToString() : "";

                            string ldc = !string.IsNullOrEmpty(item["tacs"]["ldc"].ToString()) ? item["tacs"]["ldc"].ToString() : "";
                            string finance = !string.IsNullOrEmpty(item["tacs"]["finance"].ToString()) ? item["tacs"]["finance"].ToString() : "";
                            string fnAlert = !string.IsNullOrEmpty(item["tacs"]["fnAlert"].ToString()) ? item["tacs"]["fnAlert"].ToString() : "";
                            int totalTime = !string.IsNullOrEmpty(item["tacs"]["totalTime"].ToString()) ? (Int32)item["tacs"]["totalTime"] : 0;
                            string operationId = !string.IsNullOrEmpty(item["tacs"]["operationId"].ToString()) ? item["tacs"]["operationId"].ToString() : "";
                            string payLocation = !string.IsNullOrEmpty(item["tacs"]["payLocation"].ToString()) ? item["tacs"]["payLocation"].ToString() : "";
                            int overtimeHours = !string.IsNullOrEmpty(item["tacs"]["overtimeHours"].ToString()) ? (Int32)item["tacs"]["overtimeHours"] : 0;
                            bool isOverTime = !string.IsNullOrEmpty(item["tacs"]["isOvertime"].ToString()) ? (bool)item["tacs"]["isOvertime"] : false;
                            bool isOverTimeAuth = !string.IsNullOrEmpty(item["tacs"]["isOvertimeAuth"].ToString()) ? (bool)item["tacs"]["isOvertimeAuth"] : false;
                            if (!string.IsNullOrEmpty(badgeID))
                            {
                                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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



                            //    if (AppParameters.TagsList.TryGetValue(MissedSelitem.TagId, out GeoMarker geoLmarker))
                            //    {
                            //        geoLmarker.Properties.Tacs = JsonConvert.SerializeObject(MissedSelitem.Tacs, Formatting.None);
                            //        geoLmarker.Properties.IsWearingTag = false;
                            //        geoLmarker.Properties.EmpId = MissedSelitem.EmpId;
                            //        geoLmarker.Properties.CraftName = GetCraftName(MissedSelitem.TagName);
                            //        geoLmarker.Properties.BadgeId = GetBadgeId(MissedSelitem.TagName);
                            //        geoLmarker.Properties.TagUpdate = true;
                            //    }
                            //    else
                            //    {
                            //        GeoMarker Lmarker = new GeoMarker();
                            //        Lmarker.Geometry.Coordinates = new List<double> { 0, 0 };
                            //        Lmarker.Properties.Id = MissedSelitem.TagId;
                            //        Lmarker.Properties.Name = MissedSelitem.TagName;
                            //        Lmarker.Properties.EmpId = MissedSelitem.EmpId;
                            //        Lmarker.Properties.TagType = "Person";
                            //        Lmarker.Properties.CraftName = GetCraftName(MissedSelitem.TagName);
                            //        Lmarker.Properties.BadgeId = GetBadgeId(MissedSelitem.TagName);
                            //        Lmarker.Properties.PositionTS = AppParameters.UnixTimeStampToDateTime((long)MissedSelitem.ProcessedTs);
                            //        Lmarker.Properties.TagVisible = false;
                            //        Lmarker.Properties.IsWearingTag = false;
                            //        Lmarker.Properties.TagUpdate = true;
                            //        if (!AppParameters.TagsList.TryAdd(MissedSelitem.TagId, Lmarker))
                            //        {
                            //            new ErrorLogger().CustomLog("Unable to Add Marker" + MissedSelitem.TagId, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                            //        }
                            //    }
                        //}
                        Task.Run(() => updateConnection(conID, "good"));
                    //}
                    
                }
                else
                {
                    Task.Run(() => updateConnection(conID, "error"));
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
        private static void MPEWatch_RPGPerf(dynamic data, string conID)
        {
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
                        foreach (JObject item in machineInfo.Children())
                        {
                            item["cur_sortplan"] = AppParameters.SortPlan_Name_Trimer(item["cur_sortplan"].ToString());
                            item["cur_operation_id"] = !string.IsNullOrEmpty(item["cur_operation_id"].ToString()) ? item["cur_operation_id"].ToString() : "0";
                            item["rpg_start_dtm"] = "";
                            item["rpg_end_dtm"] = "";
                            item["expected_throughput"] = "0";
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
                            if (item.ContainsKey("rpg_expected_thruput"))
                            {
                                item["expected_throughput"] = !string.IsNullOrEmpty(item["rpg_expected_thruput"].ToString()) ? item["rpg_expected_thruput"].ToString().Split(' ').FirstOrDefault() : "0";
                                if (!string.IsNullOrEmpty(item["expected_throughput"].ToString()) && item["expected_throughput"].ToString() != "0")
                                {
                                    int.TryParse(item.ContainsKey("cur_thruput_ophr") ? item["cur_thruput_ophr"].ToString().Trim() : "0", out int cur_thruput);
                                    int.TryParse(item.ContainsKey("expected_throughput") ? item["expected_throughput"].ToString().Trim() : "0", out int expected_throughput);
                                    double thrper = (double)cur_thruput / (double)expected_throughput * 100;
                                    string throughputState = "1";
                                    if (item["current_run_end"].ToString() != "" && item["current_run_end"].ToString() != "0")
                                    {
                                        throughputState = "0";
                                    }
                                    else if (thrper >= 100)
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
                                if ((item["current_run_end"].ToString() == "" || item["current_run_end"].ToString() == "0") && item["current_run_start"].ToString() != "")
                                {
                                    // JObject results = new Oracle_DB_Calls().Get_RPG_Plan_Info(item);
                                    JObject results = Get_RPG_Plan_Info(item);
                                    if (results != null && results.HasValues)
                                    {
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
                                }
                            }

                            CheckMachineNotifications(item);
                            string MpeName = string.Concat(item["mpe_type"].ToString().Trim(), "-", item["mpe_number"].ToString().PadLeft(3, '0'));
                            string newMPEPerf = JsonConvert.SerializeObject(item, Formatting.Indented);
                            AppParameters.MPEPerformanceList.AddOrUpdate(MpeName, newMPEPerf,
                                  (key, oldValue) =>
                                  {
                                      return newMPEPerf;
                                  });

                            foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                            {
                                cs.Zones.Where(f => f.Value.Properties.ZoneType == "Machine" &&
                                f.Value.Properties.Name == MpeName).Select(y => y.Value).ToList().ForEach(existingVal =>
                                {
                                    string temp = JsonConvert.SerializeObject(existingVal.Properties.MPEWatchData, Formatting.None);
                                    string tempItem = JsonConvert.SerializeObject(item, Formatting.None);
                                    if (temp != tempItem)
                                    {
                                        existingVal.Properties.ZoneUpdate = true;
                                    }
                                });
                            }
                             //AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Machine" && r.Value.Properties.Name == MpeName).Select(y => y.Key).ToList().ForEach(key =>
                             //{
                             //    if (AppParameters.ZoneList.TryGetValue(key, out GeoZone machineZone))
                             //    {
                             //        //convert to string
                             //        string temp = JsonConvert.SerializeObject(machineZone.Properties.MPEWatchData, Formatting.None);
                             //        string tempItem = JsonConvert.SerializeObject(item, Formatting.None);
                             //        if (temp != tempItem)
                             //        {
                             //            machineZone.Properties.ZoneUpdate = true;
                             //        }
                                     
                             //    }
                             //});

                        }
                        Task.Run(() => updateConnection(conID, "good"));
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                    machineInfo = null;
                    data = null;
                  
                }
                else
                {
                    Task.Run(() => updateConnection(conID, "error"));
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => updateConnection(conID, "error"));
                new ErrorLogger().ExceptionLog(ex);
            }

        }
        private static JObject Get_RPG_Plan_Info(JObject item)
        {
            try
            {
                RPGPlan tempRPG = AppParameters.MPEPRPGList.Where(x => x.Value.mpe_type == item["mpe_type"].ToString() &&
                             Convert.ToInt32(x.Value.machine_num) == (int)item["mpe_number"] &&
                             x.Value.sort_program_name == item["cur_sortplan"].ToString() &&
                             Convert.ToInt32((x.Value.mail_operation_nbr).ToString().PadRight(6,'0')) == Convert.ToInt32(((int)item["cur_operation_id"]).ToString().PadRight(6,'0')) &&
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

                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                {
                    foreach (string key in cs.Zones.Where(f => f.Value.Properties.ZoneType == "Bin" &&
                    f.Value.Properties.MPEType.Trim() == MpeType && f.Value.Properties.MPENumber.ToString().PadLeft(3, '0') == MpeNumber).Select(y => y.Key).ToList())
                    {
                        List<string> FullBinList = new List<string>();
                        if(cs.Zones.TryGetValue(key, out GeoZone binZone))
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





                //    foreach (string key in AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Bin" && r.Value.Properties.MPEType.Trim() == MpeType && r.Value.Properties.MPENumber.ToString().PadLeft(3, '0') == MpeNumber).Select(y => y.Key).ToList())
                //{
                //    List<string> FullBinList = new List<string>();
                //    if (AppParameters.ZoneList.TryGetValue(key, out GeoZone binZone))
                //    {
                //        if (FullBins.Any())
                //        {
                //            binZone.Properties.MPEBins = null;
                //            for (int i = 0; i < FullBins.Count; i++)
                //            {
                //                if (binZone.Properties.Bins.Split(',').Select(p => p.Trim()).ToList().Contains(FullBins[i]))
                //                {
                //                    FullBinList.Add(FullBins[i]);
                //                }
                //            }
                //            binZone.Properties.MPEBins = FullBinList;
                //            binZone.Properties.ZoneUpdate = true;
                //        }
                //        else
                //        {
                //            if (binZone.Properties.MPEBins.Count() != FullBinList.Count())
                //            {
                //                binZone.Properties.MPEBins = FullBinList;
                //                binZone.Properties.ZoneUpdate = true;
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }
        private static void MPEWatch_RPGPlan(dynamic data, string conID)
        {
            try
            {
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    JToken planInfo = tempData.SelectToken("data");
                    if (planInfo != null && planInfo.HasValues)
                    {
                        List<RPGPlan> RPG_collection = planInfo.ToObject<List<RPGPlan>>();
                        foreach (RPGPlan RPG_item in RPG_collection)
                        {
                            if (!string.IsNullOrEmpty(RPG_item.line_4_text))
                            {
                                RPG_item.expected_throughput = !string.IsNullOrEmpty(RPG_item.line_4_text) ? RPG_item.line_4_text.Split(' ').FirstOrDefault() : "0";
                            }
                            else
                            {
                                RPG_item.expected_throughput = !string.IsNullOrEmpty(RPG_item.rpg_expected_thruput) ? RPG_item.rpg_expected_thruput.Split(' ').FirstOrDefault() : "0";
                            }
          
                            RPG_item.sort_program_name = AppParameters.SortPlan_Name_Trimer(RPG_item.sort_program_name);

                            string RPGKey = AppParameters.MPEPRPGList.Where(x => x.Value.mpe_type == RPG_item.mpe_type &&
                            x.Value.machine_num == RPG_item.machine_num &&
                            x.Value.sort_program_name == RPG_item.sort_program_name &&
                            x.Value.mail_operation_nbr == RPG_item.mail_operation_nbr &&
                            x.Value.rpg_start_dtm == RPG_item.rpg_start_dtm
                            )
                               .Select(l => l.Key).FirstOrDefault();

                            if (!string.IsNullOrEmpty(RPGKey))
                            {
                                if (AppParameters.MPEPRPGList.TryGetValue(RPGKey, out RPGPlan OldRPG_item))
                                {
                                    if (!AppParameters.MPEPRPGList.TryUpdate(RPGKey, RPG_item, OldRPG_item))
                                    {
                                        new ErrorLogger().CustomLog("Unable to update RPG Data" + RPG_item.mpe_name + " " + RPG_item.mpe_type + " " + RPG_item.machine_num, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                    }
                                }
                            }
                            else
                            {
                                string newKey = Guid.NewGuid().ToString();
                                if (!AppParameters.MPEPRPGList.TryAdd(newKey, RPG_item))
                                {
                                    new ErrorLogger().CustomLog("Unable to update RPG Data" + RPG_item.mpe_name + " " + RPG_item.mpe_type + " " + RPG_item.machine_num, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                }

                            }
                        }
                        Task.Run(() => updateConnection(conID, "good"));
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                    planInfo = null;
                    data = null;
                    tempData = null;

                }
                else
                {
                    Task.Run(() => updateConnection(conID, "error"));
                }
                //remove old data
                if (AppParameters.MPEPRPGList.Keys.Count > 0)
                    {
                        foreach (string existingkey in AppParameters.MPEPRPGList.Where(f => f.Value.rpg_start_dtm.Date <= DateTime.Now.AddDays(-2).Date).Select(y => y.Key))
                        {
                            if (!AppParameters.MPEPRPGList.TryRemove(existingkey, out RPGPlan existingValue))
                            {
                                new ErrorLogger().CustomLog("Unable to update RPG Data" + existingValue.mpe_name + " " + existingValue.mpe_type + " " + existingValue.machine_num, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                            }
                        }
                    }
                
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }
        private static void MPEWatch_DPSEst(dynamic data, string conID)
        {
            try
            {
                if (data != null)
                {
                    JToken tempData = JToken.Parse(data);
                    JToken dpsInfo = tempData.SelectToken("data");
                    if (dpsInfo != null && dpsInfo.HasValues)
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

                        Task.Run(() => updateConnection(conID, "good"));
                    }
                    else
                    {
                        Task.Run(() => updateConnection(conID, "error"));
                    }
                    tempData = null;
                    dpsInfo = null;
                    data = null;
                }
                else
                {
                    Task.Run(() => updateConnection(conID, "error"));
                }

            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                Task.Run(() => updateConnection(conID, "error"));
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
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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
                                    existingValue.Properties.Misison = null;
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
                            new ErrorLogger().CustomLog("unable to remove Mission " + mission.REQUEST_ID + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
                        }
                    }
                    //update AGV zone location
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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
        private static void ERRORWITHOUTWORK(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE") && (string)data["NASS_CODE"] == AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString())
                {
                    if (data.ContainsKey("VEHICLE"))
                    {
                        //match with vehicle
                        foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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
                                        existingValue.Properties.Misison = null;
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
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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
                                    existingValue.Properties.Misison = null;
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
                            new ErrorLogger().CustomLog("unable to remove Mission " + mission.REQUEST_ID + " to list", string.Concat((string)AppParameters.AppSettings["APPLICATION_NAME"], "Appslogs"));
                        }
                    }

                  
                    //update AGV zone location
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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
                 
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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

                                    existingValue.Properties.Misison = AppParameters.MissionList.Where(r => r.Value.VEHICLE == tempMission.VEHICLE
                                    && r.Value.STATE == tempMission.STATE
                                    && r.Value.REQUEST_ID == tempMission.REQUEST_ID).Select(y => y.Value).FirstOrDefault();
                                    existingValue.Properties.TagUpdate = true;

                                });
                            }
                        }
                    }
               
                    //update AGV zone location
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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

                                    existingValue.Properties.Misison = AppParameters.MissionList.Where(r => r.Value.VEHICLE == tempMission.VEHICLE
                                    && r.Value.STATE == tempMission.STATE
                                    && r.Value.REQUEST_ID == tempMission.REQUEST_ID).Select(y => y.Value).FirstOrDefault();
                                    existingValue.Properties.TagUpdate = true;

                                });
                            }
                        }
                    }
                  
                    //update AGV zone location
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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
                        foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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
                        foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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
                                                existingValue.Properties.TagUpdate = true;
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
                                Lmarker.Properties.TagType = GetTagType("AGV");
                                Lmarker.Properties.TagTS = newVehicleStatus.TIME;
                                Lmarker.Properties.PositionTS = newVehicleStatus.TIME;
                                Lmarker.Geometry = GetVehicleGeometry(newVehicleStatus.X_LOCATION, newVehicleStatus.Y_LOCATION);
                                Lmarker.Properties.Vehicle_Status_Data = newVehicleStatus;
                                Lmarker.Properties.NotificationId = CheckNotification("", newVehicleStatus.STATE, "vehicle".ToLower(), Lmarker.Properties, Lmarker.Properties.NotificationId);

                                Lmarker.Properties.TagVisible = true;
                                Lmarker.Properties.TagUpdate = true;
                                if (!cs.Locators.TryAdd(Lmarker.Properties.Id, Lmarker))
                                {
                                    new ErrorLogger().CustomLog("Unable to Add Marker " + newVehicleStatus.VEHICLE_MAC_ADDRESS, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
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
        private static MarkerGeometry GetVehicleGeometry(string x, string y)
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
        private static void ProjectData(dynamic jsonObject, string conID)
        {
            bool saveToFile = false;
            try
            {
                if (jsonObject != null)
                {
                    JToken tempData = JToken.Parse(jsonObject);
                    if (tempData.HasValues)
                    {
                        if (tempData.Type != JTokenType.Array)
                        {
                            if (((JObject)tempData).ContainsKey("coordinateSystems"))
                            {
                                if (AppParameters.CoordinateSystem.FirstOrDefault().Key == "temp")
                                {
                                    AppParameters.CoordinateSystem.TryRemove("temp", out CoordinateSystem tep);
                                }
                                // loop though the Coordinate system
                                JToken CoordinateSystem = tempData.SelectToken("coordinateSystems");
                                for (int i = 0; i < CoordinateSystem.Count(); i++)
                                {
                                    if (AppParameters.CoordinateSystem.ContainsKey(CoordinateSystem[i]["id"].ToString()))
                                    {
                                        if (AppParameters.CoordinateSystem.TryGetValue(CoordinateSystem[i]["id"].ToString(), out CoordinateSystem updateCS))
                                        {
                                            //the background image
                                            LoadBcagroundImage(CoordinateSystem[i].SelectToken("backgroundImages"), updateCS.Id, CoordinateSystem[i]["name"].ToString(), out saveToFile);
                                            //this is for Zones
                                            LoadZones(CoordinateSystem[i].SelectToken("zones"), updateCS.Id, out saveToFile);
                                            //this is for Locators
                                            LoadLocators(CoordinateSystem[i].SelectToken("locators"), updateCS.Id, out saveToFile);

                                        }
                                    }
                                    else
                                    {
                                        CoordinateSystem CSystem = new CoordinateSystem();
                                        CSystem.Name = CoordinateSystem[i]["name"].ToString();
                                        CSystem.Id = CoordinateSystem[i]["id"].ToString();
                                        ///this is used to add new Coordinate System images
                                        if (AppParameters.CoordinateSystem.TryAdd(CSystem.Id, CSystem))
                                        {
                                            //the background image
                                            LoadBcagroundImage(CoordinateSystem[i].SelectToken("backgroundImages"), CSystem.Id, CSystem.Name, out saveToFile);
                                            //this is for Zones
                                            LoadZones(CoordinateSystem[i].SelectToken("zones"), CSystem.Id, out saveToFile);
                                            //this is for Locators
                                            LoadLocators(CoordinateSystem[i].SelectToken("locators"), CSystem.Id, out saveToFile);
                                        }
                                        else
                                        {
                                            new ErrorLogger().CustomLog("Unable to add CoordinateSystem " + CSystem.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                        }
                                    }
                                }
                                Task.Run(() => updateConnection(conID, "good"));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < tempData.Count(); i++)
                            {
                                if (AppParameters.CoordinateSystem.ContainsKey(tempData[i]["id"].ToString()))
                                {
                                    if (AppParameters.CoordinateSystem.TryGetValue(tempData[i]["id"].ToString(), out CoordinateSystem updateCS))
                                    {
                                        //the background image
                                        LoadlocalBcagroundImage(tempData[i].SelectToken("backgroundImages"), updateCS.Id, tempData[i]["name"].ToString(), out saveToFile);
                                        //this is for Zones
                                        LoadlocalZones(tempData[i].SelectToken("zones"), updateCS.Id, out saveToFile);
                                        //this is for Locators
                                        LoadlocalLocators(tempData[i].SelectToken("locators"), updateCS.Id, out saveToFile);

                                    }
                                }
                                else
                                {
                                    CoordinateSystem CSystem = new CoordinateSystem();
                                    CSystem.Name = tempData[i]["name"].ToString();
                                    CSystem.Id = tempData[i]["id"].ToString();
                                    ///this is used to add new Coordinate System images
                                    if (AppParameters.CoordinateSystem.TryAdd(CSystem.Id, CSystem))
                                    {
                                        //the background image
                                        LoadlocalBcagroundImage(tempData[i].SelectToken("backgroundImages"), CSystem.Id, CSystem.Name, out saveToFile);
                                        //this is for Zones
                                        LoadlocalZones(tempData[i].SelectToken("zones"), CSystem.Id, out saveToFile);
                                        //this is for Locators
                                        LoadlocalLocators(tempData[i].SelectToken("locators"), CSystem.Id, out saveToFile);
                                    }
                                    else
                                    {
                                        new ErrorLogger().CustomLog("Unable to add CoordinateSystem " + CSystem.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                    }
                                }
                            }
                        }
                    }
                    //log Project Data to locale drive.
                    if (saveToFile)
                    {
                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(AppParameters.CoordinateSystem.Select(x => x.Value).ToList()));

                   //     new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", JsonConvert.SerializeObject(AppParameters.CoordinateSystem, Formatting.Indented));
                    }
                }
            }
            catch (Exception e)
            {
                Task.Run(() => updateConnection(conID, "error"));
                new ErrorLogger().ExceptionLog(e);
            }
            
        }
        private static void LoadlocalLocators(JToken locatorlist, string csid, out bool saveToFile)
        {
            saveToFile = false;
            try
            {
                if (locatorlist != null && locatorlist.Count() > 0)
                {
                    foreach (JToken locatorsitem in locatorlist)
                    {
                        GeoMarker Lmarker = locatorsitem.FirstOrDefault().ToObject<GeoMarker>();
                        if (AppParameters.CoordinateSystem[csid].Locators.TryAdd(Lmarker.Properties.Id, Lmarker))
                        {
                            Lmarker.Properties.TagUpdate = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                saveToFile = false;
            }
        }

        private static void LoadlocalZones(JToken zoneslist, string csid, out bool saveToFile)
        {
            saveToFile = false;
            try
            {
                if (zoneslist != null && zoneslist.Count() > 0)
                {
                    foreach (JToken zoneitem in zoneslist)
                    {
                        GeoZone newGZone = zoneitem.FirstOrDefault().ToObject<GeoZone>();
                        if (AppParameters.CoordinateSystem[csid].Zones.TryAdd(newGZone.Properties.Id, newGZone))
                        {
                            newGZone.Properties.ZoneUpdate = true;
                        }

                    }
                }

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                saveToFile = false;
            }
        }
  
        private static void LoadlocalBcagroundImage(JToken backgroundImages, string csid, string csname, out bool saveToFile)
        {
            saveToFile = false;
            try
            {
                if (backgroundImages != null && backgroundImages.Count() > 0)
                {
                    BackgroundImage newbckimg = backgroundImages.ToObject<BackgroundImage>();
                    AppParameters.CoordinateSystem[csid].BackgroundImage = newbckimg;
                    newbckimg.UpdateStatus = true;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                saveToFile = false;
            }
        }

        private static void LoadBcagroundImage(JToken backgroundImages, string csid, string csname, out bool saveToFile)
        {
            saveToFile = false;
            try
            {
                if (backgroundImages != null && backgroundImages.Count() > 0)
                {
                    foreach (JObject bgItem in backgroundImages.Children())
                    {
                        BackgroundImage newbckimg = bgItem.ToObject<BackgroundImage>();
                        //newbckimg.FacilityName = !string.IsNullOrEmpty(AppParameters.AppSettings["FACILITY_NAME"].ToString()) ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "Site Not Configured";
                        //newbckimg.ApplicationFullName = AppParameters.AppSettings["APPLICATION_FULLNAME"].ToString();
                        //newbckimg.ApplicationAbbr = AppParameters.AppSettings["APPLICATION_NAME"].ToString();
                        newbckimg.Name = csname;
                        AppParameters.CoordinateSystem[csid].BackgroundImage = newbckimg;
                        newbckimg.UpdateStatus = true;
                        saveToFile = true;

                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                saveToFile = false;
            }
        }
        private static void LoadZones(JToken zoneslist, string csid, out bool saveToFile)
        {
            saveToFile = false;
            try
            {
                if (zoneslist != null && zoneslist.Count() > 0)
                {
                    foreach (JObject zoneitem in zoneslist.Children())
                    {
                        bool zoneUpdate = false;
                      
                        if (AppParameters.CoordinateSystem[csid].Zones.TryGetValue(zoneitem["id"].ToString(), out GeoZone gZone))
                        {
                            ZoneGeometry tempGeometry = GetQuuppaZoneGeometry(zoneitem["polygonData"]);
                            if (JsonConvert.SerializeObject(gZone.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(tempGeometry.Coordinates, Formatting.None))
                            {
                                gZone.Geometry.Coordinates = tempGeometry.Coordinates;
                                zoneUpdate = true;
                                saveToFile = true;
                            }
                            if (!gZone.Properties.QuuppaOverride)
                            {
                                if (gZone.Properties.Name != zoneitem["name"].ToString())
                                {
                                    gZone.Properties.Name = zoneitem["name"].ToString();
                                    zoneUpdate = true;
                                    saveToFile = true;
                                }
                            }
                            string temptype = GetZoneType(gZone.Properties.Name);
                            if (temptype != gZone.Properties.ZoneType)
                            {
                                gZone.Properties.Name = temptype;
                                zoneUpdate = true;
                                saveToFile = true;
                            }
                            if (zoneUpdate)
                            {
                                gZone.Properties.ZoneUpdate = true;
                                saveToFile = true;
                                saveToFile = true;
                            }
                        }
                        else
                        {
                            GeoZone newGZone = new GeoZone();
                            newGZone.Geometry = GetQuuppaZoneGeometry(zoneitem["polygonData"]);
                            newGZone.Properties.FloorId = csid;
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
                           
                            if (AppParameters.CoordinateSystem[csid].Zones.TryAdd(newGZone.Properties.Id, newGZone))
                            {
                                newGZone.Properties.ZoneUpdate = true;
                                saveToFile = true;
                                //new ErrorLogger().CustomLog("Unable to Add Zone" + newGZone.Properties.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                            }
                        }

                    }
                }
                
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                saveToFile = false;
            }
        }
        private static void LoadLocators(JToken locatorlist, string csid, out bool saveToFile)
        {
            saveToFile = false;
            try
            {
                if (locatorlist != null && locatorlist.Count() > 0)
                {
                    foreach (JObject locatorsitem in locatorlist.Children())
                    {
                        bool locatorupdate = false;
                        if (AppParameters.CoordinateSystem[csid].Locators.TryGetValue(locatorsitem["id"].ToString(), out GeoMarker geoLmarker))
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
                                geoLmarker.Properties.Name = "Locator";
                                string tempTagype = GetTagType(geoLmarker.Properties.Name);
                                if (geoLmarker.Properties.TagType != tempTagype)
                                {
                                    geoLmarker.Properties.TagType = tempTagype;
                                    locatorupdate = true;
                                    saveToFile = true;
                                }
                                if (geoLmarker.Properties.TagType == "Person")
                                {
                                    geoLmarker.Properties.CraftName = GetCraftName(geoLmarker.Properties.Name);
                                    geoLmarker.Properties.BadgeId = GetBadgeId(geoLmarker.Properties.Name);
                                    locatorupdate = true;
                                    saveToFile = true;
                                }

                                locatorupdate = true;
                            }
                            if (locatorupdate)
                            {
                                geoLmarker.Properties.TagUpdate = true;
                                saveToFile = true;
                            }

                        }
                        else
                        {
                            GeoMarker Lmarker = new GeoMarker();
                            Lmarker.Type = "Feature";
                            Lmarker.Properties.Id = locatorsitem["id"].ToString();
                            Lmarker.Properties.Name = locatorsitem.ContainsKey("name") ? locatorsitem["name"].ToString() : "Locator";
                            Lmarker.Properties.Color = locatorsitem.ContainsKey("color") ? locatorsitem["color"].ToString() : "";
                            Lmarker.Properties.TagType = GetTagType(Lmarker.Properties.Name);
                            Lmarker.Geometry = GetQuuppaTagGeometry(locatorsitem["location"]);
                            Lmarker.Properties.TagVisible = (bool)locatorsitem["visible"];
                            Lmarker.Properties.Source = "other";
                            if (AppParameters.CoordinateSystem[csid].Locators.TryAdd(Lmarker.Properties.Id, Lmarker))
                            {
                                Lmarker.Properties.TagUpdate = true;
                                saveToFile = true;
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                saveToFile = false;
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
                                //new tag data format version of Quuppa = "locationCoordSysId": "20a2d551-4b2a-4b4f-ab80-0caa4b250b38"
                                //old tag data format version of Quuppa "coordinateSystemId": "e11a3dd5-2e97-405c-9dfd-e63eb810964f",
                                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                                {
                                    List<string> tag_id = cs.Locators.Where(f => f.Key == tagid).Select(y => y.Key).ToList();
                                    if (tag_id.Count > 0)
                                    {
                                        cs.Locators.Where(f => f.Key == tag_id[0]).Select(y => y.Value).ToList().ForEach(geoLmarker =>
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
                                           });
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
                                        Lmarker.Properties.TagVisible = tagitem.ContainsKey("locationMovementStatus") ? tagitem["locationMovementStatus"].ToString() == "noData" ? false : true : false;
                                        Lmarker.Properties.TagUpdate = true;
                                        if (!cs.Locators.TryAdd(Lmarker.Properties.Id, Lmarker))
                                        {
                                            new ErrorLogger().CustomLog("Unable to Add Marker" + tagid, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                                        }
                                    }
                                }

                            }    
                            Task.Run(() => updateConnection(conID, "good"));
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
                Task.Run(() => updateConnection(conID, "error"));
                new ErrorLogger().ExceptionLog(e);
            }
        }
        internal static void updateConnection(string conId,string type)
        {
            try
            {
               if(AppParameters.ConnectionList.TryGetValue(conId, out Connection m ))
                {
                    var newConStatus = type == "error" ? false : true;
                    if(m.ApiConnected != newConStatus)
                    {
                        m.ApiConnected = newConStatus;
                        m.UpdateStatus = true;
                    }
                    //m.ApiConnected = type == "error" ? false : true;
                    
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
                    {
                        return "Autonomous Vehicle";
                    }
                    else if (Regex.IsMatch(value, (string)AppParameters.AppSettings["TAG_PIV"], RegexOptions.IgnoreCase))
                    {
                        return "Vehicle";
                    }
                    else if (Regex.IsMatch(value, (string)AppParameters.AppSettings["TAG_PERSON"], RegexOptions.IgnoreCase))
                    {
                        return "Person";
                    }
                    else if (Regex.IsMatch(value, (string)AppParameters.AppSettings["TAG_LOCATOR"], RegexOptions.IgnoreCase))
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

        private static void CheckMachineNotifications(JObject machineData)
        {
 
            try
            {
                string zoneID = string.Empty;
                foreach(CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
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

        private static void UpdateDeleteMachineNotifications(string notificationID, string notificationName, string timerName, string duration, string notificationValue, string timerValue)
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
                    if(ojbMerge.Type_Duration != Convert.ToInt32(timerValue))
                    {
                        ojbMerge.Notification_Update = true;
                    }
                    ojbMerge.Type_Duration = Convert.ToInt32(timerValue);
                }
            }
        }

        private static void AddNewMachineNotification(JObject machineData, string zoneID, string notificationID, string notificationType, string durationtext, string durationTime)
        {
            try
            {
                foreach (NotificationConditions newCondition in AppParameters.NotificationConditionsList.Where(r => Regex.IsMatch(notificationType, r.Value.Conditions, RegexOptions.IgnoreCase)
                            && r.Value.Type.ToLower() == "mpe".ToLower()
                            && (bool)r.Value.ActiveCondition).Select(x => x.Value).ToList())

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

        private static void CheckScanNotification()
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
                    if(_container.hasAssignScans && _container.hasLoadScans)
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
                    if(_container.hasCloseScans && _container.hasLoadScans)
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
                    if(_container.hasLoadScans || _container.hasUnloadScans)
                    {
                        foreach(var door in AppParameters.DockdoorList)
                        {
                            JObject _door = JObject.Parse(door.Value);
                            if(_door != null)
                            {
                                string _trailerbarcode = _door.ContainsKey("trailerBarcode") ? _door["trailerBarcode"].ToString().Trim() : "";
                                string _doorid = _door.ContainsKey("doorId") ? _door["doorId"].ToString().Trim() : "";
                                string _doornumber = _door.ContainsKey("doorNumber") ? _door["doorNumber"].ToString().Trim() : "";

                                var notification_id = _trailerbarcode + "_MissingArrived";
                                var notification_name = _trailerbarcode+ "|" + _doorid + "|" + _doornumber;

                                if (_trailerbarcode == _container.Trailer)
                                {
                                    bool hasarrivalscan = false;
                                    if (_door.ContainsKey("events"))
                                    {
                                        foreach (var _event in _door["events"])
                                        {
                                            if(!string.IsNullOrEmpty(_event["eventName"].ToString().Trim()) && _event["eventName"].ToString().ToUpper().Contains("ARR"))
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

        private static void AddScanNotification(string notificationType, string notificationID, string scanID, string typeName, int minutes)
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
                    if(prev_status != status)
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

        private static void RemoveScanNotification(string notification_id)
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

        private static void RemoveOldScanNotification(string scanNotificationType)
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
                            if(_trailerbarcode != _notification.Notification_ID)
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
                        foreach (JObject jo in cameraData)
                        {
                            string camera_id = jo["camera_id"].ToString();


                            List<DarvisCameraAlert> alertList = new List<DarvisCameraAlert>();

                            JArray newDetections = (JArray)jo["detections"]["new"];
                            JArray removedDetections = (JArray)jo["detections"]["removed"];
                            JArray updatedDetections = (JArray)jo["detections"]["updated"];
                            foreach (JObject newObject in newDetections)
                            {

                                ProcessNewOrExistingCameraData(newObject, ref alertList, camera_id, true);
                            }
                            foreach (JToken object_id in removedDetections)
                            {

                            }
                            foreach (JObject updatedObject in updatedDetections)
                            {
                                ProcessNewOrExistingCameraData(updatedObject, ref alertList, camera_id, false);
                            }
                            List<Tuple<GeoMarker, string>> camerasToBroadcast = new List<Tuple<GeoMarker, string>>();
                            foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                            {
                                cs.Locators.Where(f => f.Value.Properties.TagType == "Camera" &&
                                f.Value.Properties.Name == camera_id).Select(y => y.Value).
                                ToList().ForEach(Camera =>
                                {
                                    Camera.Properties.DarvisAlerts = alertList.ToArray<DarvisCameraAlert>().ToList<DarvisCameraAlert>();
                                    //FOTFManager.Instance.BroadcastCameraStatus(Camera, cs.Id);
                                    Tuple<GeoMarker, string> newData = new Tuple<GeoMarker, string>(Camera, cs.Id);
                                    camerasToBroadcast.Add(newData);
                                });

                            }
                            FOTFManager.Instance.BroadcastCameraStatus(camerasToBroadcast);

                        }
                    }

                }
                catch (Exception ex)
                {
                    new ErrorLogger().ExceptionLog(ex);
                }
            }
        }
        public void ProcessNewOrExistingCameraData(JObject thisObject, ref List<DarvisCameraAlert> alertList,string camera_id, bool isNew)
        {
            if (thisObject.ContainsKey("zones"))
            {
                JArray zones = (JArray)thisObject["zones"];
                foreach (JObject zo in zones)
                {
                    string zoName = zo["name"].ToString();

                    if (zoName.StartsWith("IG_") || zoName.StartsWith("DT_"))
                    {
                        float dwelltime = (float)Convert.ToDouble(zo["dwell_time"].ToString());
                        DarvisCameraAlert alert = new DarvisCameraAlert();
                        alert.DwellTime = dwelltime;
                        alert.Type = zoName.StartsWith("IG_") ? "IG" : "DT";
                        alert.object_class = thisObject["clazz"].ToString();
                        alert.object_id = thisObject["object_id"].ToString();
                        alert.Top = Convert.ToInt32(thisObject["top"].ToString());
                        alert.Bottom = Convert.ToInt32(thisObject["bottom"].ToString());
                        alert.Left = Convert.ToInt32(thisObject["left"].ToString());

                        alert.Right = Convert.ToInt32(thisObject["right"].ToString());
                        alertList.Add(alert);
                    }
                }
            }
        }
    }
}