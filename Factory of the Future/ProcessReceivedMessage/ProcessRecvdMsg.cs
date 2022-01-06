using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Factory_of_the_Future
{
    public class ProcessRecvdMsg
    {
        public static string Message_type = string.Empty;

        public void StartProcess(dynamic data, JObject API)
        {
            try
            {
                string Message_type = data.ContainsKey("message") ? (string)data.Property("message").Value : "";
                if (string.IsNullOrEmpty(Message_type))
                {
                    Message_type = data.ContainsKey("MESSAGE") ? (string)data.Property("MESSAGE").Value : "";
                }
                if (string.IsNullOrEmpty(Message_type))
                {
                    Message_type = API.ContainsKey("MESSAGE_TYPE") ? (string)API.Property("MESSAGE_TYPE").Value : "";
                }
                if (!string.IsNullOrEmpty(Message_type))
                {
                    switch (Message_type)
                    {
                        /*Web cameras*/
                        case "Cameras":
                            CameraData(data, Message_type);
                            break;
                        /*Quuppa Data Start*/
                        case "TagPosition":
                            TagPosition(data);
                            break;
                        case "ProjectData":
                            ProjectData(data);
                            break;
                        /*Quuppa Data End*/
                        /*SVWeb Data Start*/
                        case "doors":
                            Doors(data, Message_type);
                            break;

                        case "trips":
                            Trips(data, Message_type);
                            break;

                        case "container":
                            Container(data, Message_type);
                            break;
                        /*SVWeb Data End*/
                        /*CTS Data Start*/
                        //case "outbound":
                        //    CTS_DockDeparted(data);
                        //    break;

                        //case "LocalTrips":
                        //    CTS_LocalDockDeparted(data);
                        //    break;

                        //case "inboundScheduled":
                        //    CTS_Inbound(data);
                        //    break;

                        //case "outboundScheduled":
                        //    CTS_Outbound(data);
                        //    break;
                        /*CTS Data End*/
                        /*SELS RT Data Start*/
                        case "P2PBySite":
                            P2PBySite(data, Message_type);
                            break;
                        case "getTacsVsSels":
                            TacsVsSels(data, Message_type);
                            break;

                        case "TacsVsSelsAnomaly":
                            TacsVsSelsLDCAnomaly(data, Message_type);
                            break;
                        /*SELS RT Data End*/
                        /*IV Data Start*/
                        case "getStaffBySite":
                            Staffing(data, Message_type);
                            break;
                        /*IV Data End*/
                        /*AGVM Data Start*/
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
                        /*AGVM Data End*/
                        /*MPEWatch Data Start*/
                        case "mpe_watch_id":
                            MPE_Watch_Id(data);
                            break;

                        case "rpg_run_perf":
                            MPEWatch_RPGPerf(data, API);
                            break;

                        case "rpg_plan":
                            MPEWatch_RPGPlan(data);
                            break;

                        case "dps_run_estm":
                            MPEWatch_DPSEst(data);
                            break;
                        /*MPEWatch Data End*/
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

        private void Staffing(JObject data, string message_type)
        {
            try
            {
                bool updatefile = false;
                if (data.HasValues)
                {
                    IEnumerable<JToken> staff = data.SelectTokens("$..DATA[*]");
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
                            if (!string.IsNullOrEmpty((string)Dataitem.Property("sortplan").Value))
                            {
                                string mach_type = Dataitem.ContainsKey("mach_type") ? (string)Dataitem.Property("mach_type").Value : "";
                                string machine_no = Dataitem.ContainsKey("machine_no") ? (string)Dataitem.Property("machine_no").Value : "";
                                string sortplan = Dataitem.ContainsKey("sortplan") ? (string)Dataitem.Property("sortplan").Value : "";
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
                                if (Regex.IsMatch(mach_type, "(DBCS|AFSM100|ATU|CIOSS)", RegexOptions.IgnoreCase))
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

                                Global.Sortplans.AddOrUpdate(mch_sortplan_id, Dataitem,
                                     (key, existingVal) =>
                                     {

                                         if (JsonConvert.SerializeObject(existingVal) == JsonConvert.SerializeObject(Dataitem))
                                             return existingVal;
                                         else
                                             updatefile = true;
                                         return Dataitem;
                                     });



                            }
                        }
                    }
                    if (updatefile)
                    {
                        new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "Staffing.json", JsonConvert.SerializeObject(Global.Sortplans.Select(x => x.Value).ToList(), Formatting.Indented));
                    }

                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void CameraData(JObject data, string message_type)
        {
            try
            {
                if (data.HasValues)
                {
                    JToken camera = data.SelectToken(message_type);
                    foreach (JObject camera_item in camera.Children())
                    {
                        if (Global.Camera_Info.ContainsKey(camera_item["CAMERA_NAME"].ToString()))
                        {
                            Global.Camera_Info.AddOrUpdate(camera_item["CAMERA_NAME"].ToString(), camera_item,
                                (key, existingVal) =>
                            {
                                if (camera_item != existingVal)
                                {
                                    existingVal = camera_item;
                                }
                                return existingVal;
                            });
                        }
                        else
                        {
                            if (!Global.Camera_Info.TryAdd(camera_item["CAMERA_NAME"].ToString(), camera_item))
                            {
                                new ErrorLogger().CustomLog("unable to add camera to array", string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "Appslogs"));
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

        private static void MPE_Watch_Id(JObject data)
        {
            try
            {
                string MpewatchID = "{\"MPE_WATCH_ID\":\"" + data.Property("id").Value + "\"}";
                FOTFManager.Instance.EditAppSettingdata(MpewatchID);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void Container(JObject jsonObject, string message_type)
        {
            try
            {
                
                if (jsonObject.HasValues)
                {
                    JToken Container = jsonObject.SelectToken(message_type);
                    if (Container != null)
                    {
                        //new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "ContainerHistory.json", JsonConvert.SerializeObject(Container, Formatting.Indented));
                        List<Container> Containers = JsonConvert.DeserializeObject<List<Container>>(JsonConvert.SerializeObject(Container));
                        string siteId = (string)Global.AppSettings.Property("FACILITY_NASS_CODE").Value;
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
                            if (!Global.Containers.ContainsKey(d.PlacardBarcode))
                            {
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
                                Global.Containers.TryAdd(d.PlacardBarcode, d);
                            }
                            else
                            {
                                if (Global.Containers.TryGetValue(d.PlacardBarcode, out Container dc))
                                {
                                    if (dc.ContainerHistory.Count() > d.ContainerHistory.Count)
                                    {
                                        dc.ContainerHistory = d.ContainerHistory;
                                        int sortindex = 0;
                                        foreach (ContainerHistory scan in dc.ContainerHistory.OrderBy(s => s.EventDtmfmt))
                                        {
                                            sortindex++;
                                            scan.sortind = sortindex;
                                            dc.binDisplay = scan.Event == "PASG" ? scan.BinName : "";
                                            if (scan.SiteId == siteId)
                                            {
                                                if (scan.Event == "PASG")
                                                {
                                                    dc.hasAssignScans = true;
                                                }
                                                if ((scan.Event == "CLOS" || scan.Event == "BCLS"))
                                                {
                                                    dc.hasCloseScans = true;
                                                }
                                                if (scan.Event == "LOAD")
                                                {
                                                    dc.hasLoadScans = true;
                                                    dc.Oroute = !string.IsNullOrEmpty(scan.Route) ? scan.Route : "";
                                                    dc.Otrip = !string.IsNullOrEmpty(scan.Trip) ? scan.Trip : "";
                                                    dc.Otrailer = !string.IsNullOrEmpty(scan.Trailer) ? scan.Trailer : "";
                                                }
                                                if (scan.Event == "UNLD")
                                                {
                                                    dc.hasUnloadScans = true;
                                                    dc.Iroute = !string.IsNullOrEmpty(scan.Route) ? scan.Route : "";
                                                    dc.Itrip = !string.IsNullOrEmpty(scan.Trip) ? scan.Trip : "";
                                                    dc.Itrailer = !string.IsNullOrEmpty(scan.Trailer) ? scan.Trailer : "";

                                                }
                                                if (scan.Event == "PRINT")
                                                {
                                                    dc.hasPrintScans = true;
                                                }
                                                if (scan.Event == "TERM")
                                                {
                                                    dc.containerTerminate = true;
                                                }

                                                if (!string.IsNullOrEmpty(scan.Location) && scan.SiteType == "Origin" && scan.Event == "PASG" && scan.Location != dc.Location)
                                                {
                                                    dc.Location = scan.Location;
                                                }
                                              
                                            }
                                            if (scan.Event == "TERM")
                                            {
                                                dc.containerTerminate = true;
                                            }
                                            if (scan.Event == "UNLD" && scan.RedirectInd == "Y" && dc.Dest == siteId)
                                            {
                                                dc.containerRedirectedDest = true;
                                            }

                                            if (scan.Event == "UNLD" && scan.SiteType == "Destination")
                                            {
                                                if (scan.SiteId == dc.Dest)
                                                {
                                                    dc.containerAtDest = true;
                                                }
                                            }
                                            if (scan.Event == "UNLD" && scan.SiteType == "Via")
                                            {
                                                if (scan.SiteId != dc.Dest)
                                                {
                                                    dc.containerRedirectedDest = true;
                                                    dc.Location = "X-Dock";
                                                }
                                            }
                                        }

                                    }

                                }
                            }
                        }

                    }
                    Container = null;
                    jsonObject = null;
                }
                if (Global.Containers.Count > 0)
                {
                    Global.Containers.Where(r => DateTime.Now.Subtract(r.Value.EventDtm).TotalDays > 2).Select(y => y.Key).ToList().ForEach( m => {
                        Global.Containers.TryRemove(m, out Container outc);
                        
                    });
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
           
        }

        private static void Trips(JObject jsonObject, string message_type)
        {
            try
            {
                if (jsonObject.HasValues)
                {
                    JToken trips = jsonObject.SelectToken(message_type);
                    if (trips != null)
                    {
                        List<Trips> Trips = JsonConvert.DeserializeObject<List<Trips>>(JsonConvert.SerializeObject(trips));
                        foreach (Trips tripitem in Trips)
                        {
                            string routtripid = string.Concat(tripitem.Route, tripitem.Trip, tripitem.TripDirectionInd);

                            if (!string.IsNullOrEmpty(tripitem.Status) && Regex.IsMatch(tripitem.Status, "(CANCELED|Omitted)", RegexOptions.IgnoreCase))
                            {
                                if (Global.Trips.ContainsKey(routtripid))
                                {
                                    Global.Trips.TryRemove(routtripid, out Trips temp1);
                                }
                            }
                            else
                            {
                                if (Global.Trips.ContainsKey(routtripid))
                                {
                                    Global.Trips.AddOrUpdate(routtripid, tripitem,
                                        (key, existingVal) =>
                                        {
                                            if (tripitem != existingVal)
                                            {
                                                if (existingVal.TripDirectionInd == "O")
                                                {

                                                    if (!string.IsNullOrEmpty(existingVal.DestSite))
                                                    {
                                                        IEnumerable<Container> alltrailercontent = null;
                                                        IEnumerable<Container> unloadedtrailerContent = Global.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Dest) && Regex.IsMatch(r.Value.Dest, existingVal.DestSite, RegexOptions.IgnoreCase)
                                                    && r.Value.hasLoadScans == false
                                                    && r.Value.containerTerminate == false
                                                    && r.Value.containerAtDest == false
                                                    && r.Value.hasCloseScans == true).Select(y => y.Value).ToList();
                                                        if (existingVal.UnloadedContainers != unloadedtrailerContent.Count())
                                                        {
                                                            existingVal.UnloadedContainers = unloadedtrailerContent.Count();
                                                        }
                                                        alltrailercontent = unloadedtrailerContent;
                                                        if (!string.IsNullOrEmpty(existingVal.TrailerBarcode))
                                                        {
                                                            IEnumerable<Container> loadedtrailerContent = null;
                                                            //for the loaded int the trailer
                                                            loadedtrailerContent = Global.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Otrailer)
                                                               && Regex.IsMatch(r.Value.Otrailer, existingVal.TrailerBarcode, RegexOptions.IgnoreCase)
                                                               && r.Value.hasLoadScans == true
                                                               ).Select(y => y.Value).ToList();
                                                            alltrailercontent = unloadedtrailerContent.Concat(loadedtrailerContent);
                                                            loadedtrailerContent = null;
                                                        }


                                                        existingVal.Containers = alltrailercontent;
                                                        unloadedtrailerContent = null;
                                                        alltrailercontent = null;
                                                    }
                                                }
                                                if (existingVal.TripDirectionInd == "I")
                                                {
                                                    if (!string.IsNullOrEmpty(existingVal.TrailerBarcode))
                                                    {
                                                        IEnumerable<Container> unloadedtrailerContent = Global.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Itrailer)
                                                        && Regex.IsMatch(r.Value.Itrailer, existingVal.TrailerBarcode, RegexOptions.IgnoreCase)

                                                         ).Select(y => y.Value).ToList();
                                                        existingVal.Containers = unloadedtrailerContent;
                                                        unloadedtrailerContent = null;
                                                    }
                                                }
                                                existingVal.Trip_Update = true;
                                                if (string.IsNullOrEmpty(existingVal.DestSite))
                                                {
                                                    new ItineraryTripUpdate(GetItinerary(tripitem.Route, tripitem.Trip, Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString(), existingVal.OperDate), existingVal.TripDirectionInd);
                                                }
                                            }
                                            return existingVal;
                                        });
                                }
                                else
                                {

                                    if (Global.Trips.TryAdd(routtripid, tripitem))
                                    {
                                        new ItineraryTripUpdate(GetItinerary(tripitem.Route, tripitem.Trip, Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString(), tripitem.OperDate), tripitem.TripDirectionInd);
                                    }
                                }
                            }
                        }
                    }
                }
                //remove older data this does the clean up.
                Global.Trips.Where(r => r.Value.ActDepartureDtm.Subtract(DateTime.Now).TotalHours > 3.0).Select(y => y.Key).ToList().ForEach(m =>
                {
                    if (!Global.Trips.TryRemove(m, out Trips valout))
                    {
                        new ErrorLogger().CustomLog(string.Concat("Unable to remove item from Trip Array", valout), string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "Applogs"));
                    }
                });
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static JArray GetItinerary(string route, string trip, string nasscode , DateTime start_time)
        {
            JArray temp = new JArray();
            try
            {
                //string start_time = string.Concat(DateTime.Now.ToString("yyyy-MM-dd'T'"), "00:00:00");

                Uri parURL = new Uri(string.Format((string)Global.AppSettings.Property("SV_ITINERARY").Value, route, trip, string.Concat(start_time.ToString("yyyy-MM-dd'T'"), "00:00:00")));
                string SV_Response = SendMessage.SV_Get(parURL.AbsoluteUri);
                if (!string.IsNullOrEmpty(SV_Response))
                {
                    if (Global.IsValidJson(SV_Response))
                    {
                        JArray itinerary = JArray.Parse(SV_Response);
                        if (itinerary.HasValues)
                        {
                           return itinerary;
                        }
                    } 
                }
                return temp;
            }
            catch (Exception e)
            {

                new ErrorLogger().ExceptionLog(e);
                return temp;
            }
        }

        private static void Doors(JObject jsonObject, string message_type)
        {
            try
            {
                bool update_info = false;
                if (jsonObject.HasValues)
                {
                    string siteId = (string)Global.AppSettings.Property("FACILITY_NASS_CODE").Value;
                    JToken doorstatus = jsonObject.SelectToken(message_type);
                    foreach (JObject item in doorstatus.Children())
                    {
                        Global.Zones.Where(x => x.Value["properties"]["Zone_Type"].ToString() == "DockDoor"
                        && x.Value["properties"]["doorNumber"].ToString() == item["doorNumber"].ToString().PadLeft(3, '0')).Select(l => l.Value).ToList().ForEach(m =>
                       {
                           if (item.ContainsKey("tripDirectionInd") && item.ContainsKey("route") && item.ContainsKey("trip"))
                           {
                               string routetripid = item.Property("route").Value.ToString() + item.Property("trip").Value.ToString() + item.Property("tripDirectionInd").Value.ToString();
                               if (Global.Trips.TryGetValue(routetripid, out Trips rtrip))
                               {

                                   if (item.ContainsKey("trailerBarcode"))
                                   {
                                       rtrip.TrailerBarcode = item.Property("trailerBarcode").Value.ToString();
                                   }
                                   if (item.ContainsKey("doorNumber"))
                                   {
                                       rtrip.DoorNumber = item["doorNumber"].ToString();
                                   }
                                   if (item.ContainsKey("doorId"))
                                   {
                                       rtrip.DoorId = item["doorId"].ToString();
                                   }

                                   DateTime dtNow = DateTime.Now;
                                   if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value))
                                   {
                                       if (Global.TimeZoneConvert.TryGetValue((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                                       {
                                           dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                                       }
                                   }
                                   double totalmin = Math.Round(rtrip.ScheduledDtm.Subtract(dtNow).TotalMinutes);
                                   if (totalmin != rtrip.TimeToDepart)
                                   {
                                       rtrip.TimeToDepart = totalmin;
                                       update_info = true;
                                   }
                                    JObject tempdata = JObject.Parse(JsonConvert.SerializeObject(rtrip, Formatting.Indented));
                                   if (m["properties"]["routetripData"].ToString() != tempdata.ToString() )
                                   {
                                       m["properties"]["routetripData"] = tempdata;
                                       update_info = true;
                                   }
                                   tempdata = null;
                               }
                               else
                               {
                                   DateTime dtNow = DateTime.Now;
                                   if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value))
                                   {
                                       if (Global.TimeZoneConvert.TryGetValue((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                                       {
                                           dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                                       }
                                   }
                                   DateTime scheuledDtm = new DateTime(1, 1, 1);
                                   if (item.ContainsKey("scheduledDtm"))
                                   {
                                       scheuledDtm = Global.SVdatetimeformat(JObject.Parse(JsonConvert.SerializeObject(item["scheduledDtm"])));

                                       double totalmin = Math.Round(scheuledDtm.Subtract(dtNow).TotalMinutes);
                                       if (m.ContainsKey("TimeToDepart"))
                                       {
                                           if (totalmin != (double)m["TimeToDepart"])
                                           {
                                               item["TimeToDepart"] = totalmin;
                                               update_info = true;
                                           }
                                       }
                                       else
                                       {
                                           m["TimeToDepart"] = totalmin;
                                       }
                                   }
                                   if (m["properties"]["routetripData"].ToString() != item.ToString())
                                   {
                                       m["properties"]["routetripData"] = item;
                                       update_info = true;
                                   }
                               }
                           }
                           else
                           {
                               if (m["properties"]["routetripData"].ToString() != item.ToString())
                               {
                                   m["properties"]["routetripData"] = item;
                                   update_info = true;
                               }
                           }

                           if (update_info)
                           {
                               m["properties"]["Zone_Update"] = true;

                           }

                           ///update trips info
                           if (item.ContainsKey("tripDirectionInd"))
                           {
                               if (Global.Trips.ContainsKey(string.Concat(item["route"], item["trip"], item["tripDirectionInd"])))
                               {
                                   new DoorTripsUpdate(item, item["tripDirectionInd"].ToString());
                               }
                           }
                       });
                    }
                    doorstatus = null;
                    jsonObject = null;

                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void TacsVsSelsLDCAnomaly(JObject data, string message_type)
        {
            /**
        * {
            "empId": "04692142",
            "tagId": "ca0800004878",
            "tagName": "Transportation_0086",
            "processed": "21-08-13 09:21:20",
            "hasTACS": true,
            "currentZones": [
                {
                    "id": "da09be6b-ac97-49d5-924d-1c6eae24e430",
                    "name": "Dock"
                }
            ],
            "selsToTacsTotalTime": 212,
            "isLdcAlert": true,
            "tacs": {
                "ldc": "34",
                "totalTime": 7991,
                "operationId": "766",
                "payLocation": "n/a",
                "isOvertimeAuth": false,
                "overtimeHours": 0,
                "isOvertime": false
            },
            "sels": {
                "totalTime": 16975,
                "currentLDCs": [
                    "17"
                ],
                "timeByLDC": {
                    "17": {
                        "ldc": 17,
                        "time": 16551,
                        "ldcToSelsTotalTime": 98,
                        "ldcToTacsTotalTime": 207
                    },
                    "19": {
                        "ldc": 19,
                        "time": 424,
                        "ldcToSelsTotalTime": 2,
                        "ldcToTacsTotalTime": 5
                    }
                }
            }
        }
        */
            try
            {
                if (data.HasValues)
                {
                    foreach (JObject item in data.Children())
                    {
                        if (Global.Tag.ContainsKey((string)item.Property("tagId").Value))
                        {
                            if (Global.Tag.TryGetValue((string)item.Property("tagId").Value, out JObject tag))
                            {
                                if ((bool)((JObject)tag["properties"]).Property("isLdcAlert").Value == false)
                                {
                                    ((JObject)tag["properties"]).Property("isLdcAlert").Value = true;
                                }
                                if (((JObject)tag["properties"]).Property("Employee_EIN").Value != item.Property("empId").Value)
                                {
                                    ((JObject)tag["properties"]).Property("Employee_EIN").Value = item.Property("empId").Value;
                                }
                                if (((JObject)tag["properties"]).Property("Tacs").Value.ToString() != item.Property("tacs").Value.ToString())
                                {
                                    ((JObject)tag["properties"]).Property("Tacs").Value = item.Property("tacs").Value;
                                }
                                if (((JObject)tag["properties"]).Property("currentLDCs").Value.ToString() != ((JObject)item["sels"]["currentLDCs"]).Property("currentLDCs").Value.ToString())
                                {
                                    ((JObject)tag["properties"]).Property("currentLDCs").Value = ((JObject)item["sels"]["currentLDCs"]).Property("currentLDCs").Value.ToString();
                                }
                               ((JObject)tag["properties"]).Property("Tag_Update").Value = true;
                            }
                        }
                        else
                        {
                            JObject GeoJsonType = new JObject_List().GeoJSON_Tag;
                            JArray temp_zone = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
                            ((JObject)GeoJsonType["geometry"]).Property("type").Value = "Point";
                            GeoJsonType["geometry"]["coordinates"] = new JArray(0, 0);
                            ((JObject)GeoJsonType["properties"]).Property("id").Value = (string)item.Property("tagId").Value;
                            ((JObject)GeoJsonType["properties"]).Property("zones").Value = temp_zone;
                            ((JObject)GeoJsonType["properties"]).Property("name").Value = item.Property("tagName").Value;
                            ((JObject)GeoJsonType["properties"]).Property("Tag_Type").Value = "Person";
                            ((JObject)GeoJsonType["properties"]).Property("Employee_EIN").Value = item.Property("empId").Value;
                            ((JObject)GeoJsonType["properties"]).Property("Tacs").Value = item.Property("tacs").Value;
                            ((JObject)GeoJsonType["properties"]).Property("positionTS").Value = DateTime.Now.AddDays(-10).ToUniversalTime();
                            ((JObject)GeoJsonType["properties"]).Property("Tag_Update").Value = true;
                            //add to the tags
                            if (!Global.Tag.ContainsKey((string)item.Property("tagId").Value))
                            {
                                Global.Tag.TryAdd((string)item.Property("tagId").Value, GeoJsonType);
                            }
                        }
                    }
                    if (Global.Tag.Count() > 0)
                    {
                        Global.Tag.Where(u => u.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")).Select(x => x.Value).ToList().ForEach(m =>
                        {
                            var exsiting = data.SelectTokens("[?(@.tagId)]").Where(i => (string)i["tagId"] == (string)((JObject)m["properties"]).Property("id").Value).ToList();

                            if (exsiting.Count == 0)
                            {
                                if ((bool)((JObject)m["properties"]).Property("isLdcAlert").Value == true)
                                {
                                    ((JObject)m["properties"]).Property("isLdcAlert").Value = false;
                                    ((JObject)m["properties"]).Property("Tag_Update").Value = true;
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void TacsVsSels(JObject data, string message_type)
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
                if (data.HasValues)
                {
                    if (data.ContainsKey("missedSels"))
                    {
                        JToken missedSels = data.SelectToken("missedSels");
                        if (missedSels != null)
                        {
                            foreach (JObject item in missedSels.Children())
                            {
                                if (Global.Tag.ContainsKey((string)item.Property("tagId").Value))
                                {
                                    Global.Tag.Where(r => r.Key == (string)item.Property("tagId").Value).Select(y => y.Value).ToList().ForEach(m =>
                                    {
                                        bool update = false;
                                        if ((bool)((JObject)m["properties"]).Property("isWearingTag").Value == true)
                                        {
                                            ((JObject)m["properties"]).Property("isWearingTag").Value = false;
                                            update = true;
                                        }
                                        if (((JObject)m["properties"]).Property("Employee_EIN").Value != item.Property("empId").Value)
                                        {
                                            ((JObject)m["properties"]).Property("Employee_EIN").Value = item.Property("empId").Value;
                                            update = true;
                                        }
                                        if (((JObject)m["properties"]).Property("Tacs").Value.ToString() != item.Property("tacs").Value.ToString())
                                        {
                                            ((JObject)m["properties"]).Property("Tacs").Value = item.Property("tacs").Value;
                                            update = true;
                                        }
                                        if (update)
                                        {
                                            if (!(bool)((JObject)m["properties"]).Property("Tag_Update").Value)
                                            {
                                                ((JObject)m["properties"]).Property("Tag_Update").Value = true;
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    JObject GeoJsonType = new JObject_List().GeoJSON_Tag;
                                    JArray temp_zone = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
                                    ((JObject)GeoJsonType["geometry"]).Property("type").Value = "Point";
                                    GeoJsonType["geometry"]["coordinates"] = new JArray(0, 0);
                                    ((JObject)GeoJsonType["properties"]).Property("id").Value = (string)item.Property("tagId").Value;
                                    ((JObject)GeoJsonType["properties"]).Property("zones").Value = temp_zone;
                                    ((JObject)GeoJsonType["properties"]).Property("name").Value = item.Property("tagName").Value;
                                    ((JObject)GeoJsonType["properties"]).Property("Tag_Type").Value = "Person";
                                    ((JObject)GeoJsonType["properties"]).Property("Employee_EIN").Value = item.Property("empId").Value;
                                    ((JObject)GeoJsonType["properties"]).Property("Tacs").Value = item.Property("tacs").Value;
                                    ((JObject)GeoJsonType["properties"]).Property("positionTS").Value = DateTime.Now.AddDays(-10).ToUniversalTime();
                                    ((JObject)GeoJsonType["properties"]).Property("Tag_Update").Value = true;
                                    //add to the tags
                                    if (!Global.Tag.ContainsKey((string)item.Property("tagId").Value))
                                    {
                                        Global.Tag.TryAdd((string)item.Property("tagId").Value, GeoJsonType);
                                    }
                                }
                            }
                            if (Global.Tag.Count() > 0)
                            {
                                Global.Tag.Where(u => u.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")).Select(x => x.Value).ToList().ForEach(m =>
                                {
                                    var exsiting = missedSels.SelectTokens("[?(@.tagId)]").Where(i => (string)i["tagId"] == (string)((JObject)m["properties"]).Property("id").Value).ToList();

                                    if (exsiting.Count == 0)
                                    {
                                        if ((bool)((JObject)m["properties"]).Property("isWearingTag").Value == false)
                                        {
                                            ((JObject)m["properties"]).Property("isWearingTag").Value = true;
                                            ((JObject)m["properties"]).Property("Tag_Update").Value = true;
                                        }
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        new ErrorLogger().CustomLog(data.ToString(), string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "_", Message_type.ToUpper() + "_Applogs"));
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        //private static void CTS_LocalDockDeparted(JObject data)
        //{
        //    //"Scheduled": "5/6/2021 1:45 PM",
        //    //"Departed": "",
        //    //"Door": null,
        //    //"Leg": "974",
        //    //"Route": "975L0",
        //    //"Trip": "116",
        //    //"Destination": "EUGENE P & D F",
        //    //"Assigned": 31,
        //    //"Closed": 47,
        //    //"Staged": 22,
        //    //"XDock": 19,
        //    //"MLD": 0,
        //    //"PLD": 0,
        //    //"MTLD": 0,
        //    //"Load": 0,
        //    //"LoadPercent": 0,
        //    //"Total": 88
        //    //}
        //    try
        //    {
        //        if (data.HasValues)
        //        {
        //            if (data.ContainsKey("Data"))
        //            {
        //                JToken cts_data = data.SelectToken("Data");
                    
        //                if (cts_data.Children().Count() > 0)
        //                {
        //                    foreach (JObject Dataitem in cts_data.Children())
        //                    {
        //                        bool update = false;
        //                        string trip = Dataitem.ContainsKey("Trip") ? (string)Dataitem.Property("Trip").Value : "";
        //                        string route = Dataitem.ContainsKey("Route") ? (string)Dataitem.Property("Route").Value : "";
        //                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
        //                        {
        //                            //if (cts_site.HasValues)
        //                            //{
        //                            //    if (cts_site.ContainsKey("TimeZone"))
        //                            //    {
        //                            //        DateTime scheduledTime = DateTime.Parse((string)Dataitem.Property("Scheduled").Value);
        //                            //        Dataitem.Property("Scheduled").Value = TimeZoneInfo.ConvertTime(scheduledTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site.Property("TimeZone").Value));

        //                            //        if (!string.IsNullOrEmpty((string)Dataitem.Property("Departed").Value))
        //                            //        {
        //                            //            DateTime departedTime = DateTime.Parse((string)Dataitem.Property("Departed").Value);
        //                            //            Dataitem.Property("Departed").Value = TimeZoneInfo.ConvertTime(departedTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site.Property("TimeZone").Value));
        //                            //        }
        //                            //    }

        //                            //}
        //                            if (!Global.CTS_LocalDockDeparted.ContainsKey(route + trip))
        //                            {
        //                                Dataitem.Add(new JProperty("CTS_Update", true));
        //                                Dataitem.Add(new JProperty("CTS_Remove", false));
        //                                Global.CTS_LocalDockDeparted.TryAdd(route + trip, Dataitem);
        //                            }
        //                            else
        //                            {
        //                                if (Global.CTS_LocalDockDeparted.TryGetValue(route + trip, out JObject cts_item))
        //                                {
        //                                    foreach (dynamic kv in Dataitem.Children())
        //                                    {
        //                                        if (cts_item.ContainsKey(kv.Name))
        //                                        {
        //                                            if (cts_item.Property(kv.Name).Value.ToString() != kv.Value.ToString())
        //                                            {
        //                                                cts_item.Property(kv.Name).Value = Dataitem.Property(kv.Name).Value.ToString();
        //                                                update = true;
        //                                            }
        //                                        }
        //                                    }
        //                                    if (update)
        //                                    {
        //                                        cts_item.Property("CTS_Update").Value = true;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                //check if the data have been removed.
        //                if (Global.CTS_LocalDockDeparted.Count() > 0)
        //                {
        //                    foreach (JObject item in Global.CTS_LocalDockDeparted.Select(x => x.Value))
        //                    {
        //                        string trip = item.ContainsKey("Trip") ? (string)item.Property("Trip").Value : "";
        //                        string route = item.ContainsKey("Route") ? (string)item.Property("Route").Value : "";
        //                        var exsiting = cts_data.SelectTokens("[?(@.Route)]").Where(i => (string)i["Route"] == (string)item.Property("Route").Value && (string)i["Trip"] == (string)item.Property("Trip").Value).ToList();

        //                        if (exsiting.Count == 0)
        //                        {
        //                            if (Global.CTS_LocalDockDeparted.TryGetValue(route + trip, out JObject cts_item))
        //                            {
        //                                if (cts_item.ContainsKey("CTS_Remove"))
        //                                {
        //                                    cts_item.Property("CTS_Remove").Value = true;
        //                                    cts_item.Property("CTS_Update").Value = true;
        //                                }
        //                                else
        //                                {
        //                                    cts_item.Add(new JProperty("CTS_Remove", true));
        //                                    cts_item.Property("CTS_Update").Value = true;
        //                                }
        //                            };
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
        //}

        //private static void CTS_Outbound(JObject data)
        //{
        //    //{
        //    //"Scheduled": "5/6/2021 12:25 PM",
        //    //"Actual": "5/6/2021 12:18 PM",
        //    //"RouteID": "972VS",
        //    //"TripID": "B3001",
        //    //"FirstLegDest": "97213",
        //    //"FirstLegSite": "ROSE CITY PARK",
        //    //"FinalDest": "972",
        //    //"FinalDestSite": "PORTLAND OR P&DC"
        //    //}
        //    try
        //    {
        //        if (data.HasValues)
        //        {
        //            if (data.ContainsKey("Data"))
        //            {
        //                JToken cts_data = data.SelectToken("Data");
                       
        //                if (cts_data.Children().Count() > 0)
        //                {
        //                    foreach (JObject Dataitem in cts_data.Children())
        //                    {
        //                        bool update = false;
        //                        string trip = Dataitem.ContainsKey("TripID") ? (string)Dataitem.Property("TripID").Value : "";
        //                        string route = Dataitem.ContainsKey("RouteID") ? (string)Dataitem.Property("RouteID").Value : "";
        //                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
        //                        {
        //                            //if (cts_site.HasValues)
        //                            //{
        //                            //    if (cts_site.ContainsKey("TimeZone"))
        //                            //    {
        //                            //        DateTime scheduledTime = DateTime.Parse((string)Dataitem.Property("Scheduled").Value);
        //                            //        Dataitem.Property("Scheduled").Value = TimeZoneInfo.ConvertTime(scheduledTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site.Property("TimeZone").Value));

        //                            //        if (!string.IsNullOrEmpty((string)Dataitem.Property("Actual").Value))
        //                            //        {
        //                            //            DateTime departedTime = DateTime.Parse((string)Dataitem.Property("Actual").Value);
        //                            //            Dataitem.Property("Actual").Value = TimeZoneInfo.ConvertTime(departedTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site.Property("TimeZone").Value));
        //                            //        }
        //                            //    }

        //                            //}
        //                            if (!Global.CTS_Outbound_Schedualed.ContainsKey(route + trip))
        //                            {
        //                                Dataitem.Add(new JProperty("CTS_Update", true));
        //                                Dataitem.Add(new JProperty("CTS_Remove", false));
        //                                Dataitem.Add(new JProperty("doorNumber", ""));
        //                                Global.CTS_Outbound_Schedualed.TryAdd(route + trip, Dataitem);
        //                            }
        //                            else
        //                            {
        //                                if (Global.CTS_Outbound_Schedualed.TryGetValue(route + trip, out JObject cts_item))
        //                                {
        //                                    foreach (dynamic kv in Dataitem.Children())
        //                                    {
        //                                        if (cts_item.ContainsKey(kv.Name))
        //                                        {
        //                                            if (cts_item.Property(kv.Name).Value.ToString() != kv.Value.ToString())
        //                                            {
        //                                                cts_item.Property(kv.Name).Value = Dataitem.Property(kv.Name).Value.ToString();
        //                                                update = true;
        //                                            }
        //                                        }
        //                                    }
        //                                    if (update)
        //                                    {
        //                                        cts_item.Property("CTS_Update").Value = true;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                // check if the data have been removed.
        //                if (Global.CTS_Outbound_Schedualed.Count() > 0)
        //                {
        //                    foreach (JObject item in Global.CTS_Outbound_Schedualed.Select(x => x.Value))
        //                    {
        //                        string trip = item.ContainsKey("TripID") ? (string)item.Property("TripID").Value : "";
        //                        string route = item.ContainsKey("RouteID") ? (string)item.Property("RouteID").Value : "";
        //                        var exsiting = cts_data.SelectTokens("[?(@.RouteID)]").Where(i => (string)i["RouteID"] == (string)item.Property("RouteID").Value && (string)i["TripID"] == (string)item.Property("TripID").Value).ToList();

        //                        if (exsiting.Count == 0)
        //                        {
        //                            if (Global.CTS_Outbound_Schedualed.TryGetValue(route + trip, out JObject cts_item))
        //                            {
        //                                if (cts_item.ContainsKey("CTS_Remove"))
        //                                {
        //                                    cts_item.Property("CTS_Remove").Value = true;
        //                                    cts_item.Property("CTS_Update").Value = true;
        //                                }
        //                                else
        //                                {
        //                                    cts_item.Add(new JProperty("CTS_Remove", true));
        //                                    cts_item.Property("CTS_Update").Value = true;
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
        //}

        //private static void CTS_Inbound(JObject data)
        //{
        //    //{
        //    //"Scheduled": "5/6/2021 12:15 PM",
        //    //"Actual": "",
        //    //"RouteID": "972VS",
        //    //"TripID": "B2142",
        //    //"LegOrigin": "972PS",
        //    //"SiteName": "PLANET EXPRESS SHIPPING LLC"
        //    //}
        //    try
        //    {
        //        if (data.HasValues)
        //        {
        //            if (data.ContainsKey("Data"))
        //            {
        //                JToken cts_data = data.SelectToken("Data");
        //                JObject cts_site = (JObject)data.SelectToken("Site");
        //                if (cts_data.Children().Count() > 0)
        //                {
        //                    foreach (JObject Dataitem in cts_data.Children())
        //                    {
        //                        bool update = false;
        //                        string trip = Dataitem.ContainsKey("TripID") ? (string)Dataitem.Property("TripID").Value : "";
        //                        string route = Dataitem.ContainsKey("RouteID") ? (string)Dataitem.Property("RouteID").Value : "";
        //                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
        //                        {
        //                            //if (cts_site.HasValues)
        //                            //{
        //                            //    if (cts_site.ContainsKey("TimeZone"))
        //                            //    {
        //                            //        DateTime scheduledTime = DateTime.Parse((string)Dataitem.Property("Scheduled").Value);
        //                            //        Dataitem.Property("Scheduled").Value = TimeZoneInfo.ConvertTime(scheduledTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site.Property("TimeZone").Value));

        //                            //        if (!string.IsNullOrEmpty((string)Dataitem.Property("Actual").Value))
        //                            //        {
        //                            //            DateTime departedTime = DateTime.Parse((string)Dataitem.Property("Actual").Value);
        //                            //            Dataitem.Property("Actual").Value = TimeZoneInfo.ConvertTime(departedTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site.Property("TimeZone").Value));
        //                            //        }
        //                            //    }

        //                            //}
        //                            if (!Global.CTS_Inbound_Schedualed.ContainsKey(route + trip))
        //                            {
        //                                Dataitem.Add(new JProperty("CTS_Update", true));
        //                                Dataitem.Add(new JProperty("CTS_Remove", false));
        //                                Dataitem.Add(new JProperty("doorNumber", ""));
        //                                Global.CTS_Inbound_Schedualed.TryAdd(route + trip, Dataitem);
        //                            }
        //                            else
        //                            {
        //                                if (Global.CTS_Inbound_Schedualed.TryGetValue(route + trip, out JObject cts_item))
        //                                {
        //                                    foreach (dynamic kv in Dataitem.Children())
        //                                    {
        //                                        if (cts_item.ContainsKey(kv.Name))
        //                                        {
        //                                            if (cts_item.Property(kv.Name).Value.ToString() != kv.Value.ToString())
        //                                            {
        //                                                cts_item.Property(kv.Name).Value = Dataitem.Property(kv.Name).Value.ToString();
        //                                                update = true;
        //                                            }
        //                                        }
        //                                    }
        //                                    if (update)
        //                                    {
        //                                        cts_item.Property("CTS_Update").Value = true;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                // check if the data have been removed.
        //                if (Global.CTS_Inbound_Schedualed.Count() > 0)
        //                {
        //                    foreach (JObject item in Global.CTS_Inbound_Schedualed.Select(x => x.Value))
        //                    {
        //                        string trip = item.ContainsKey("TripID") ? (string)item.Property("TripID").Value : "";
        //                        string route = item.ContainsKey("RouteID") ? (string)item.Property("RouteID").Value : "";
        //                        var exsiting = cts_data.SelectTokens("[?(@.RouteID)]").Where(i => (string)i["RouteID"] == (string)item.Property("RouteID").Value && (string)i["TripID"] == (string)item.Property("TripID").Value).ToList();

        //                        if (exsiting.Count == 0)
        //                        {
        //                            if (Global.CTS_Inbound_Schedualed.TryGetValue(route + trip, out JObject cts_item))
        //                            {
        //                                if (cts_item.ContainsKey("CTS_Remove"))
        //                                {
        //                                    cts_item.Property("CTS_Remove").Value = true;
        //                                    cts_item.Property("CTS_Update").Value = true;
        //                                }
        //                                else
        //                                {
        //                                    cts_item.Add(new JProperty("CTS_Remove", true));
        //                                    cts_item.Property("CTS_Update").Value = true;
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
        //}

        private static void P2PBySite(JObject data, string message_type)
        {
            try
            {
                if (data.HasValues)
                {
                    if (!data.ContainsKey("localdata"))
                    {
                        data.Add(new JProperty("localdata", true));
                        new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "P2PData.json", JsonConvert.SerializeObject(data, Formatting.Indented));
                    }
                    JToken sortplanlist = null;
                    if (data.ContainsKey(message_type))
                    {
                        sortplanlist = data.SelectToken(message_type);
                        if (sortplanlist != null)
                        {
                            if (sortplanlist.HasValues)
                            {
                                foreach (JObject Dataitem in sortplanlist.Children())
                                {
                                    if (!string.IsNullOrEmpty((string)Dataitem.Property("sortplan").Value))
                                    {
                                        string mach_type = Dataitem.ContainsKey("mach_type") ? (string)Dataitem.Property("mach_type").Value : "";
                                        string machine_no = Dataitem.ContainsKey("machine_no") ? (string)Dataitem.Property("machine_no").Value : "";
                                        string sortplan = Dataitem.ContainsKey("sortplan") ? (string)Dataitem.Property("sortplan").Value : "";
                                        string sortplan_name = "";
                                        int dotindex = sortplan.IndexOf(".", 1);
                                        if ((dotindex == -1))
                                        {
                                            sortplan_name = sortplan.Trim();
                                        }
                                        else
                                        {
                                            sortplan_name = sortplan.Substring(0, dotindex).Trim();
                                        }

                                        if (Global.Sortplans.ContainsKey(mach_type + "-" + machine_no + "-" + sortplan_name))
                                        {
                                            if (Global.Sortplans.TryGetValue(mach_type + "-" + machine_no + "-" + sortplan_name, out JObject existingVa))
                                            {
                                                foreach (dynamic kv in Dataitem.Children())
                                                {
                                                    if (existingVa.ContainsKey(kv.Name))
                                                    {
                                                        if (existingVa.Property(kv.Name).Value != kv.Value)
                                                        {
                                                            existingVa.Property(kv.Name).Value = kv.Value;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        existingVa.Add(new JProperty(kv.Name, kv.Value));
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Global.Sortplans.TryAdd(mach_type + "-" + machine_no + "-" + sortplan_name, Dataitem))
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //    if (Global.MachineZones.Count > 0)
                //{
                //    JToken machineInfo = data.SelectToken("Doors");
                //    foreach (var MachineZonesitem in Global.MachineZones)
                //    {
                //        if ((string)MachineZonesitem.Value["properties"]["Zone_Type"] == "Machine")
                //        {
                //            string mzname = (string)MachineZonesitem.Value["properties"]["name"];
                //            if (!string.IsNullOrEmpty(mzname))
                //            {
                //                string[] separators = { "_", "-" };
                //                string[] mznamesplit = mzname.Split(separators, StringSplitOptions.None);
                //                if (mznamesplit.Length > 1)
                //                {
                //                    string mname = mznamesplit[0];
                //                    int.TryParse(mznamesplit[1], out int mnumber);

                //                    IEnumerable<JToken> mchinesortplans = machineInfo.Where(i => (string)i["mach_type"] == mname && (int)i["machine_no"] == mnumber).ToList();
                //                    if (mchinesortplans.Count() > 0)
                //                    {
                //                        MachineZonesitem.Value["properties"]["P2PData"] = new JArray(mchinesortplans);
                //                        update_info = true;
                //                    }
                //                    if (update_info)
                //                    {
                //                        MachineZonesitem.Value["properties"]["Zone_Update"] = true;
                //                    }
                //                }

                //            }

                //        }
                //    }

                //}
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void MPEWatch_RPGPerf(JObject data, JObject Connection)
        {
            try
            {
                bool update_info = false;
                string machine_type = "";
                string machine_number = "";
                string total_volume = "";
                string sortplan = "";
                string estCompletionTime = "";
                if (data.HasValues)
                {
                    JToken machineInfo = data.SelectToken("data");
                    if (machineInfo != null)
                    {
                        foreach (JObject item in machineInfo.Children())
                        {
                            machine_type = item.ContainsKey("mpe_type") ? item.Property("mpe_type").Value.ToString().Trim() : "";
                            machine_number = item.ContainsKey("mpe_number") ? item.Property("mpe_number").Value.ToString().PadLeft(3, '0') : "";
                            sortplan = item.ContainsKey("cur_sortplan") ? item.Property("cur_sortplan").Value.ToString() : "";
                            //if (!string.IsNullOrEmpty(machine_type))
                            //{
                            //    if (machine_type.ToUpper().Trim() == "SPBSTS")
                            //    {
                            //        machine_type = "APBS";
                            //    }
                            //    if (machine_type.ToUpper().Trim() == "FSFSSC")
                            //    {
                            //        machine_type = "FSS";
                            //    }
                            //}

                            total_volume = item.ContainsKey("tot_sortplan_vol") ? item.Property("tot_sortplan_vol").Value.ToString().Trim() : "0";
                            int.TryParse(item.ContainsKey("rpg_est_vol") ? item.Property("rpg_est_vol").Value.ToString().Trim() : "0", out int rpg_volume);
                            double.TryParse(item.ContainsKey("rpg_est_vol") ? item.Property("cur_thruput_ophr").Value.ToString().Trim() : "0", out double throughput);
                            int.TryParse(item.ContainsKey("rpg_est_vol") ? item.Property("tot_sortplan_vol").Value.ToString().Trim() : "0", out int piecesfed);

                            if (rpg_volume > 0 && throughput > 0)
                            {
                                DateTime dtNow = DateTime.Now;
                                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value))
                                {
                                    if (Global.TimeZoneConvert.TryGetValue((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                                    {
                                        dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                                    }
                                }

                                double intMinuteToCompletion = (rpg_volume - piecesfed) / (throughput / 60);
                                DateTime dtEstCompletion = dtNow.AddMinutes(intMinuteToCompletion);
                                estCompletionTime = dtEstCompletion.ToString("yyyy-MM-dd HH:mm:ss");
                                item.Add("rpg_est_comp_time", estCompletionTime);
                            }
                            else
                            {
                                item.Add("rpg_est_comp_time", "");
                            }
                            //Don't need to store run performance data at this time--TODO redo insert for better performance.
                            //if (!string.IsNullOrEmpty(total_volume))
                            //{
                            //    if (total_volume != "0")
                            //    {
                            //        new Oracle_DB_Calls().Insert_RPG_Run_Perf(item);
                            //    }
                            //}
                            if (item.Property("current_run_end").Value.ToString() == "" && item.Property("current_run_start").Value.ToString() != "")
                            {
                                JObject results = new Oracle_DB_Calls().Get_RPG_Plan_Info(item);
                                item.Add("rpg_start_dtm", results.ContainsKey("rpg_start_dtm") ? results.Property("rpg_start_dtm").Value.ToString().Trim() : "");
                                item.Add("rpg_end_dtm", results.ContainsKey("rpg_end_dtm") ? results.Property("rpg_end_dtm").Value.ToString().Trim() : "");
                                item.Add("expected_throughput", results.ContainsKey("expected_throughput") ? results.Property("expected_throughput").Value.ToString().Trim() : "");
                            }
                            else
                            {
                                item.Add("rpg_start_dtm", "");
                                item.Add("rpg_end_dtm", "");
                            }
                            Global.Zones.Where(x => x.Value["properties"]["Zone_Type"].ToString() == "Machine" &&
                            x.Value["properties"]["MPE_Type"].ToString() == item["mpe_type"].ToString() && x.Value["properties"]["MPE_Number"].ToString() == item["mpe_number"].ToString())
                               .Select(l => l.Value).ToList()
                               .ForEach(existingVa =>
                               {
                                   if (!string.IsNullOrEmpty(sortplan))
                                   {
                                       existingVa["properties"]["P2PData"] = GetP2PSortplan(machine_type, machine_number, sortplan);
                                   }
                                   if (!string.IsNullOrEmpty(existingVa["properties"]["MPEWatchData"].ToString()))
                                   {
                                       foreach (dynamic kv in item.Children())
                                       {
                                           if (((JObject)existingVa["properties"]["MPEWatchData"]).ContainsKey(kv.Name))
                                           {
                                               if (existingVa["properties"]["MPEWatchData"][kv.Name].ToString() != kv.Value.ToString())
                                               {
                                                   existingVa["properties"]["MPEWatchData"][kv.Name] = kv.Value;
                                                   update_info = true;
                                               }
                                           }
                                           else
                                           {
                                               existingVa["properties"]["MPEWatchData"][kv.Name] = kv.Value;
                                           }
                                       }
                                   }
                                   else
                                   {
                                       existingVa["properties"]["MPEWatchData"] = item;
                                       update_info = true;
                                   }
                                   if (update_info)
                                   {
                                       existingVa["properties"]["Zone_Update"] = true;
                                   }

                               });
                        }
                    }
                    machineInfo = null;
                    if (machineInfo == null)
                    {
                        JToken Errorinfo = data.SelectToken("error_message");
                        if (Errorinfo != null)
                        {
                            Global.API_List.Where(x => (int)x.Value["ID"] == (int)Connection["ID"]).Select(y => y.Value).ToList().ForEach(m =>
                            {
                                if ((bool)m["API_CONNECTED"])
                                {
                                    m["API_CONNECTED"] = false;
                                }
                                m["UPDATE_STATUS"] = true;
                            });
                            Global.AppSettings.Property("MPE_WATCH_ID").Value = "";

                        }
                    }

                    data = null;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private static JToken GetP2PSortplan(string machine_type, string machine_number, string sortplan)
        {
            try
            {
                JObject return_result = new JObject();
                if (Regex.IsMatch(machine_type, "(DBCS|AFSM100|ATU|CIOSS)", RegexOptions.IgnoreCase))
                {
                    string sortplan_name = "";
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
                string id = machine_type + "-" + Convert.ToInt32(machine_number) + "-" + sortplan;
                if (Global.Sortplans.ContainsKey(id))
                {
                    if (Global.Sortplans.TryGetValue(id, out JObject sp))
                    {
                        return sp;
                    }
                }

                return return_result;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return new JObject();
            }
        }

        private static void MPEWatch_RPGPlan(JObject data)
        {
            try
            {
                if (data.HasValues)
                {
                    JToken planInfo = data.SelectToken("data");
                    if (planInfo != null && planInfo.HasValues)
                    {
                        new Oracle_DB_Calls().Insert_RPG_Plan(data);
                    }
                    planInfo = null;
                    data = null;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        private static void MPEWatch_DPSEst(JObject data)
        {
            try
            {
                int time_to_comp_optimal = 0;
                int time_to_comp_actual = 0;
                string time_to_comp_optimal_DateTime = "";
                string time_to_comp_actual_DateTime = "";
                DateTime dtNow = DateTime.Now;
                if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value))
                {
                    if (Global.TimeZoneConvert.TryGetValue((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                    {
                        dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                    }
                }

                if (data.HasValues && Global.Zones.Count > 0)
                {
                    JToken dpsInfo = data.SelectToken("data");
                    if (dpsInfo != null)
                    {
                        if (dpsInfo.HasValues)
                        {
                            foreach (JObject item in dpsInfo.Children())
                            {
                                int.TryParse(item.ContainsKey("time_to_comp_optimal") ? item.Property("time_to_comp_optimal").Value.ToString().Trim() : "0", out time_to_comp_optimal);
                                DateTime dtCompOptimal = dtNow.AddSeconds(time_to_comp_optimal);
                                time_to_comp_optimal_DateTime = dtCompOptimal.ToString("yyyy-MM-dd HH:mm:ss");
                                item.Add("time_to_comp_optimal_DateTime", time_to_comp_optimal_DateTime);

                                int.TryParse(item.ContainsKey("time_to_comp_actual") ? item.Property("time_to_comp_actual").Value.ToString().Trim() : "0", out time_to_comp_actual);
                                DateTime dtCompActual = dtNow.AddSeconds(time_to_comp_actual);
                                time_to_comp_actual_DateTime = dtCompActual.ToString("MM/dd/yyyy HH:mm:ss");
                                item.Add("time_to_comp_actual_DateTime", time_to_comp_actual_DateTime);

                                string strSortPlan = item.ContainsKey("sortplan_name_perf") ? item.Property("sortplan_name_perf").Value.ToString().Trim() : "";
                                string[] strSortPlanList = strSortPlan.Split(',').Select(x => x.Trim()).ToArray();
                                foreach (string strSP in strSortPlanList)
                                {
                                    string strSortPlanItem = strSP;

                                    if (!string.IsNullOrEmpty(strSortPlanItem))
                                    {
                                      
                                        Global.Zones.Where(u => u.Value["properties"]["Zone_Type"].ToString() == "Machine").Select(x => x.Value).ToList().ForEach(machineZone =>
                                        {
                                            if (machineZone["properties"]["MPEWatchData"].HasValues)
                                            {

                                                string currSortPlan = machineZone["properties"]["MPEWatchData"]["cur_sortplan"].ToString();
                                                if (!string.IsNullOrEmpty(currSortPlan))
                                                {
                                                    if (currSortPlan.Length > 7) { currSortPlan = currSortPlan.Substring(0, 7); }
                                                    if (strSortPlanItem.Length > 7) { strSortPlanItem = strSortPlanItem.Substring(0, 7); }

                                                    if (currSortPlan == strSortPlanItem)
                                                    {
                                                        machineZone["properties"]["DPSData"] = item;
                                                        machineZone["properties"]["Zone_Update"] = true;
                                                    }
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    }
                    dpsInfo = null;
                    data = null;
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
            }
        }

        //private static void CTS_DockDeparted(JObject data)
        //{
        //    //"Scheduled": "5/6/2021 1:45 PM",
        //    //"Departed": "",
        //    //"Door": null,
        //    //"Leg": "974",
        //    //"Route": "975L0",
        //    //"Trip": "116",
        //    //"Destination": "EUGENE P & D F",
        //    //"Assigned": 31,
        //    //"Closed": 47,
        //    //"Staged": 22,
        //    //"XDock": 19,
        //    //"MLD": 0,
        //    //"PLD": 0,
        //    //"MTLD": 0,
        //    //"Load": 0,
        //    //"LoadPercent": 0,
        //    //"Total": 88
        //    //}
        //    try
        //    {
        //        if (data.HasValues)
        //        {
        //            if (data.ContainsKey("Data"))
        //            {
        //                JToken cts_data = data.SelectToken("Data");
                       
        //                if (cts_data.Children().Count() > 0)
        //                {
        //                    foreach (JObject Dataitem in cts_data.Children())
        //                    {
        //                        bool update = false;
        //                        string trip = Dataitem.ContainsKey("Trip") ? (string)Dataitem.Property("Trip").Value : "";
        //                        string route = Dataitem.ContainsKey("Route") ? (string)Dataitem.Property("Route").Value : "";
        //                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
        //                        {
        //                            //if (cts_site.HasValues)
        //                            //{
        //                            //    if (cts_site.ContainsKey("TimeZone"))
        //                            //    {
        //                            //        DateTime scheduledTime = DateTime.Parse((string)Dataitem.Property("Scheduled").Value);
        //                            //        Dataitem.Property("Scheduled").Value = TimeZoneInfo.ConvertTime(scheduledTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site.Property("TimeZone").Value));

        //                            //        if (!string.IsNullOrEmpty((string)Dataitem.Property("Departed").Value))
        //                            //        {
        //                            //            DateTime departedTime = DateTime.Parse((string)Dataitem.Property("Departed").Value);
        //                            //            Dataitem.Property("Departed").Value = TimeZoneInfo.ConvertTime(departedTime, TimeZoneInfo.FindSystemTimeZoneById((string)cts_site.Property("TimeZone").Value));
        //                            //        }
        //                            //    }

        //                            //}
        //                            if (!Global.CTS_DockDeparted.ContainsKey(route + trip))
        //                            {
        //                                Dataitem.Add(new JProperty("CTS_Update", true));
        //                                Dataitem.Add(new JProperty("CTS_Remove", false));
        //                                Global.CTS_DockDeparted.TryAdd(route + trip, Dataitem);
        //                            }
        //                            else
        //                            {
        //                                if (Global.CTS_DockDeparted.TryGetValue(route + trip, out JObject cts_item))
        //                                {
        //                                    foreach (dynamic kv in Dataitem.Children())
        //                                    {
        //                                        if (cts_item.ContainsKey(kv.Name))
        //                                        {
        //                                            if (cts_item.Property(kv.Name).Value.ToString() != kv.Value.ToString())
        //                                            {
        //                                                cts_item.Property(kv.Name).Value = Dataitem.Property(kv.Name).Value.ToString();
        //                                                update = true;
        //                                            }
        //                                        }
        //                                    }
        //                                    if (update)
        //                                    {
        //                                        cts_item.Property("CTS_Update").Value = true;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                //check if the data have been removed.
        //                if (Global.CTS_DockDeparted.Count() > 0)
        //                {
        //                    foreach (JObject item in Global.CTS_DockDeparted.Select(x => x.Value))
        //                    {
        //                        string trip = item.ContainsKey("Trip") ? (string)item.Property("Trip").Value : "";
        //                        string route = item.ContainsKey("Route") ? (string)item.Property("Route").Value : "";
        //                        var exsiting = cts_data.SelectTokens("[?(@.Route)]").Where(i => (string)i["Route"] == (string)item.Property("Route").Value && (string)i["Trip"] == (string)item.Property("Trip").Value).ToList();

        //                        if (exsiting.Count == 0)
        //                        {
        //                            if (Global.CTS_DockDeparted.TryGetValue(route + trip, out JObject cts_item))
        //                            {
        //                                if (cts_item.ContainsKey("CTS_Remove"))
        //                                {
        //                                    cts_item.Property("CTS_Remove").Value = true;
        //                                    cts_item.Property("CTS_Update").Value = true;
        //                                }
        //                                else
        //                                {
        //                                    cts_item.Add(new JProperty("CTS_Remove", true));
        //                                    cts_item.Property("CTS_Update").Value = true;
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
        //}

        private static void ERRORWITHWORK(JObject data)
        {
            try
            {
                string dropoff = string.Empty;
                string pickup = string.Empty;
                if (data.ContainsKey("VEHICLE"))
                {
                    Global.Tag.Where(f => f.Value["properties"]["name"].ToString() == (string)data.Property("VEHICLE").Value).Select(y => y.Value).ToList().ForEach(existingVa =>
                        {
                            existingVa["properties"]["state"] = "Error";
                            existingVa["properties"]["errorCode"] = (string)data.Property("errorCode".ToUpper()).Value;
                            existingVa["properties"]["errorDiscription"] = (string)data.Property("error_Discription".ToUpper()).Value;
                            existingVa["properties"]["errorLocation"] = (string)data.Property("errorCode".ToUpper()).Value;
                            existingVa["properties"]["errorTime"] = (DateTime)data.Property("time".ToUpper()).Value;
                            existingVa["properties"]["Tag_Update"] = true;
                            existingVa["properties"]["inMission"] = false;
                        });
                }
                if (!string.IsNullOrEmpty(pickup))
                {
                    //update AGV zone location
                    Global.Zones.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" &&  f.Value["properties"]["name"].ToString() == pickup).Select(y => y.Value).ToList().ForEach(existingVa =>
                        {
                            existingVa["properties"]["successfulPickup"] = false;
                            existingVa["properties"]["eta"] = "";
                            existingVa["properties"]["errorCode"] = (string)data.Property("errorCode".ToUpper()).Value;
                            existingVa["properties"]["errorDiscription"] = (string)data.Property("error_Discription".ToUpper()).Value;
                            //existingVa["properties"]["errorLocation"] = (string)data.Property("error_Location".ToUpper()).Value;
                            existingVa["properties"]["Zone_Update"] = true;
                        });
                }
                //update AGV zone location
                if (!string.IsNullOrEmpty(dropoff))
                {
                    Global.Zones.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" && f.Value["properties"]["name"].ToString() == dropoff).Select(y => y.Value).ToList().ForEach(existingVa =>
                     {
                         existingVa["properties"]["successfulDropoff"] = false;
                         existingVa["properties"]["eta"] = "";
                         existingVa["properties"]["errorCode"] = (string)data.Property("errorCode".ToUpper()).Value;
                         existingVa["properties"]["errorDiscription"] = (string)data.Property("error_Discription".ToUpper()).Value;
                         // existingVa["properties"]["errorLocation"] = (string)data.Property("error_Location".ToUpper()).Value;
                         existingVa["properties"]["Zone_Update"] = true;
                     });
                }
                ////send to database
                //if (!data.ContainsKey("TAG_ID"))
                //{
                //    data.AddAfterSelf(new JProperty("TAG_ID", tag_id));
                //    // new Oracle_DB_Calls().Update_AGVTagdata(data);

                //}
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void ERRORWITHOUTWORK(JObject data)
        {
            try
            {
                if (data.ContainsKey("VEHICLE"))
                {
                    Global.Tag.Where(f => f.Value["properties"]["name"].ToString() == (string)data.Property("VEHICLE").Value).Select(y => y.Value).ToList().ForEach(existingVa =>
                         {
                             existingVa["properties"]["state"] = "Error";
                             existingVa["properties"]["errorCode"] = (string)data.Property("errorCode".ToUpper()).Value;
                             existingVa["properties"]["errorDiscription"] = (string)data.Property("error_Discription".ToUpper()).Value;
                             //existingVa["properties"]["errorLocation"] = (string)data.Property("errorCode".ToUpper()).Value;
                             existingVa["properties"]["errorTime"] = (DateTime)data.Property("time".ToUpper()).Value;
                             existingVa["properties"]["Tag_Update"] = true;
                         });

                    ////send to database
                    //if (!data.ContainsKey("TAG_ID"))
                    //{
                    //    data.AddAfterSelf(new JProperty("TAG_ID", tag_id));
                    //    //new Oracle_DB_Calls().Update_AGVTagdata(data);

                    //}
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void SUCCESSFULDROP(JObject data)
        {
            try
            {
                string dropoff = string.Empty;
                if (data.ContainsKey("VEHICLE"))
                {
                    Global.Tag.Where(f => f.Value["properties"]["name"].ToString() == (string)data.Property("VEHICLE").Value).Select(y => y.Value).ToList().ForEach(existingVa =>

                         {
                             dropoff = (string)existingVa["properties"]["dropoffLocation"];
                             existingVa["properties"]["inMission"] = false;
                             existingVa["properties"]["state"] = "Drop Off";
                             existingVa["properties"]["successfulDropoff"] = true;
                             existingVa["properties"]["Tag_Update"] = true;
                         });

                    if (string.IsNullOrEmpty(dropoff))
                    {
                        //update AGV zone location
                        Global.Zones.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" && f.Value["properties"]["name"].ToString() == dropoff).Select(y => y.Value).ToList().ForEach(existingVa =>
                            {
                                existingVa["properties"]["vehicleNumber"] = "";
                                existingVa["properties"]["successfulDropoff"] = true;
                                existingVa["properties"]["eta"] = "";
                                existingVa["properties"]["errorCode"] = "";
                                existingVa["properties"]["errorDiscription"] = "";
                                existingVa["properties"]["errorLocation"] = "";
                                existingVa["properties"]["Zone_Update"] = true;
                            });
                    }
                    ////send to database
                    //if (!data.ContainsKey("TAG_ID"))
                    //{
                    //    data.AddAfterSelf(new JProperty("TAG_ID", tag_id));
                    //    //new Oracle_DB_Calls().Update_AGVTagdata(data);

                    //}
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void SUCCESSFULPICKUP(JObject data)
        {
            try
            {
                string dropoff = string.Empty;
                string pickup = string.Empty;
                if (data.ContainsKey("VEHICLE"))
                {
                    Global.Tag.Where(f => f.Value["properties"]["name"].ToString() == (string)data.Property("VEHICLE").Value).Select(y => y.Value).ToList().ForEach(existingVa =>
                       {
                           dropoff = (string)existingVa["properties"]["pickupLocation"];
                           pickup = (string)existingVa["properties"]["dropoffLocation"];
                           existingVa["properties"]["state"] = "Picked UP";
                           existingVa["properties"]["successfulPickup"] = true;
                           existingVa["properties"]["Tag_Update"] = true;
                       });
                    if (!string.IsNullOrEmpty(pickup))
                    {
                        //update AGV zone location
                        Global.Zones.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" &&  f.Value["properties"]["name"].ToString() == pickup).Select(y => y.Value).ToList().ForEach(existingVa =>
                         {
                             existingVa["properties"]["successfulPickup"] = true;
                             existingVa["properties"]["eta"] = "";
                             existingVa["properties"]["errorCode"] = "";
                             existingVa["properties"]["errorDiscription"] = "";
                             existingVa["properties"]["errorLocation"] = "";
                             existingVa["properties"]["Zone_Update"] = true;
                         });
                    }
                    //update AGV zone location
                    if (!string.IsNullOrEmpty(dropoff))
                    {
                        Global.Zones.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" &&  f.Value["properties"]["name"].ToString() == dropoff).Select(y => y.Value).ToList().ForEach(existingVa =>
                        {
                            existingVa["properties"]["successfulDropoff"] = false;
                            existingVa["properties"]["eta"] = "";
                            existingVa["properties"]["errorCode"] = "";
                            existingVa["properties"]["errorDiscription"] = "";
                            existingVa["properties"]["errorLocation"] = "";
                            existingVa["properties"]["Zone_Update"] = true;
                        });
                    }
                    ////send to database
                    //if (!data.ContainsKey("TAG_ID"))
                    //{
                    //    data.AddAfterSelf(new JProperty("TAG_ID", tag_id));
                    //    //new Oracle_DB_Calls().Update_AGVTagdata(data);

                    //}
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void MATCHEDWITHWORK(JObject data)
        {
            try
            {
                if (data.ContainsKey("VEHICLE"))
                {
                    Global.Tag.Where(f => f.Value["properties"]["name"].ToString() == data["VEHICLE"].ToString()).Select(y => y.Value).ToList().ForEach(existingVa =>
                    {
                        existingVa["properties"]["vehicleNumber"] = (string)data.Property("vehicle_Number".ToUpper()).Value;
                        existingVa["properties"]["inMission"] = true;
                        existingVa["properties"]["state"] = "MatchedWithWork";
                        existingVa["properties"]["door"] = data.ContainsKey("door".ToUpper()) ? (string)data.Property("door".ToUpper()).Value : "";
                        existingVa["properties"]["etaToPickup"] = data.ContainsKey("eta".ToUpper()) ? (string)data.Property("eta".ToUpper()).Value : "";
                        existingVa["properties"]["etaToDropoff"] = "";
                        existingVa["properties"]["placard"] = data.ContainsKey("mtel".ToUpper()) ? (string)data.Property("mtel".ToUpper()).Value : "";
                        existingVa["properties"]["successfulPickup"] = false;
                        existingVa["properties"]["etaToDropoff"] = "";
                        existingVa["properties"]["successfulDropoff"] = false;
                        existingVa["properties"]["errorCode"] = "";
                        existingVa["properties"]["errorLocation"] = "";
                        existingVa["properties"]["errorTime"] = "";
                        existingVa["properties"]["pickupLocation"] = data.ContainsKey("pickup_location".ToUpper()) ? (string)data.Property("pickup_location".ToUpper()).Value : "";
                        existingVa["properties"]["dropoffLocation"] = data.ContainsKey("dropoff_location".ToUpper()) ? (string)data.Property("dropoff_location".ToUpper()).Value : "";
                        existingVa["properties"]["endLocation"] = data.ContainsKey("end_location".ToUpper()) ? (string)data.Property("end_location".ToUpper()).Value : "";
                        existingVa["properties"]["requestId"] = data.ContainsKey("requestId".ToUpper()) ? (string)data.Property("requestId".ToUpper()).Value : "";
                        existingVa["properties"]["Tag_Update"] = true;
                    }) ;
                    //update AGV zone location
                    Global.Zones.Where(f => f.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" &&  
                    f.Value["properties"]["name"].ToString() == data["pickup_location".ToUpper()].ToString().ToUpper()).Select(y => y.Value).ToList().ForEach(existingVa =>
                    {
                        existingVa["properties"]["inMission"] = true;
                        existingVa["properties"]["vehicleNumber"] = (string)data.Property("vehicle_Number".ToUpper()).Value;
                        existingVa["properties"]["successfulPickup"] = false;
                        existingVa["properties"]["eta"] = (string)data.Property("eta".ToUpper()).Value;
                        existingVa["properties"]["Zone_Update"] = true;
                    });
                    ////send to database
                    //if (!data.ContainsKey("TAG_ID"))
                    //{
                    //    data.AddAfterSelf(new JProperty("TAG_ID", tag_id));
                    //    //new Oracle_DB_Calls().Insert_AGVTagdata(data);

                    //}
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static void FLEET_STATUS(JObject data)
        {
            try
            {
                if (data.ContainsKey("NASS_CODE"))
                {
                    if ((string)data.Property("NASS_CODE").Value == Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString())
                    {
                        bool Update = false;
                        if (data.ContainsKey("VEHICLE"))
                        {
                            if (Global.Tag.Count() > 0)
                            {
                                Global.Tag.Where(u => u.Value["properties"]["Tag_Type"].ToString().ToString().ToLower() == "Autonomous Vehicle".ToLower() &&
                                u.Value["properties"]["name"].ToString() == (string)data.Property("VEHICLE").Value).Select(x => x.Value).ToList().ForEach(existingVa =>
                                {
                                    if (((JObject)existingVa["properties"]).ContainsKey("vehicleBatteryPercent"))
                                    {
                                        if ((string)existingVa["properties"]["vehicleBatteryPercent"] != (string)data.Property("batterypercent".ToUpper()).Value)
                                        {
                                            existingVa["properties"]["vehicleBatteryPercent"] = (string)data.Property("batterypercent".ToUpper()).Value;
                                            if (!Update)
                                            {
                                                Update = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingVa["properties"]["vehicleBatteryPercent"] = (string)data.Property("batterypercent".ToUpper()).Value;
                                        if (!Update)
                                        {
                                            Update = true;
                                        }
                                    }
                                    if (((JObject)existingVa["properties"]).ContainsKey("MacAddress"))
                                    {
                                        if ((string)existingVa["properties"]["MacAddress"] != (string)data.Property("VEHICLE_MAC_ADDRESS".ToUpper()).Value)
                                        {
                                            existingVa["properties"]["MacAddress"] = (string)data.Property("VEHICLE_MAC_ADDRESS".ToUpper()).Value;
                                            if (!Update)
                                            {
                                                Update = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingVa["properties"]["MacAddress"] = (string)data.Property("VEHICLE_MAC_ADDRESS".ToUpper()).Value;
                                        if (!Update)
                                        {
                                            Update = true;
                                        }
                                    }
                                    if (((JObject)existingVa["properties"]).ContainsKey("vehicleNumber"))
                                    {
                                        if ((string)existingVa["properties"]["vehicleNumber"] != (string)data.Property("vehicle_number".ToUpper()).Value)
                                        {
                                            existingVa["properties"]["vehicleNumber"] = (string)data.Property("vehicle_number".ToUpper()).Value;
                                            if (!Update)
                                            {
                                                Update = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingVa["properties"]["vehicleNumber"] = (string)data.Property("vehicle_number".ToUpper()).Value;
                                        if (!Update)
                                        {
                                            Update = true;
                                        }
                                    }
                                    if (((JObject)existingVa["properties"]).ContainsKey("vehicleCategory"))
                                    {
                                        if (data.ContainsKey("Cateogry".ToUpper()))
                                        {
                                            if ((string)existingVa["properties"]["vehicleCategory"] != (string)data.Property("Cateogry".ToUpper()).Value)
                                            {
                                                existingVa["properties"]["vehicleCategory"] = (string)data.Property("Cateogry".ToUpper()).Value;
                                                if (!Update)
                                                {
                                                    Update = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((string)existingVa["properties"]["vehicleCategory"] != (string)data.Property("Category".ToUpper()).Value)
                                            {
                                                existingVa["properties"]["vehicleCategory"] = (string)data.Property("Category".ToUpper()).Value;
                                                if (!Update)
                                                {
                                                    Update = true;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (data.ContainsKey("Cateogry".ToUpper()))
                                        {
                                            existingVa["properties"]["vehicleCategory"] = (string)data.Property("Cateogry".ToUpper()).Value;
                                            if (!Update)
                                            {
                                                Update = true;
                                            }
                                        }
                                        else
                                        {
                                            existingVa["properties"]["vehicleCategory"] = (string)data.Property("Category".ToUpper()).Value;
                                            if (!Update)
                                            {
                                                Update = true;
                                            }
                                        }
                                    }
                                    if (((JObject)existingVa["properties"]).ContainsKey("vehicleTime"))
                                    {
                                        if ((string)existingVa["properties"]["vehicleTime"] != (string)data.Property("time".ToUpper()).Value)
                                        {
                                            existingVa["properties"]["vehicleTime"] = ((DateTime)data.Property("time".ToUpper()).Value).ToUniversalTime();
                                            if (!Update)
                                            {
                                                Update = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingVa["properties"]["vehicleTime"] = ((DateTime)data.Property("time".ToUpper()).Value).ToUniversalTime();
                                        if (!Update)
                                        {
                                            Update = true;
                                        }
                                    }
                                    if (((JObject)existingVa["properties"]).ContainsKey("state"))
                                    {
                                        if ((string)existingVa["properties"]["state"] != (string)data.Property("state".ToUpper()).Value)
                                        {
                                            //current condition
                                            if (Global.Notification_Conditions.Where(r => Regex.IsMatch((string)existingVa["properties"]["state"], r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                                            && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                                             && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList().Count() > 0)
                                            {
                                                foreach (JObject conditionitem in Global.Notification_Conditions.Where(r => Regex.IsMatch((string)existingVa["properties"]["state"], r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                                            && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                                             && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList())
                                                {
                                                    if (Global.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)existingVa["properties"]["id"]))
                                                    {
                                                        if (Global.Notification.TryGetValue((string)conditionitem.Property("ID").Value + (string)existingVa["properties"]["id"], out JObject ojbMerge))
                                                        {
                                                            ojbMerge.Add(new JProperty("DELETE", true));
                                                        }
                                                    }
                                                }
                                            }
                                            existingVa["properties"]["state"] = (string)data.Property("state".ToUpper()).Value;
                                            //new condition
                                            if (Global.Notification_Conditions.Where(r => Regex.IsMatch((string)existingVa["properties"]["state"], r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                                        && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                                         && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList().Count() > 0)
                                            {
                                                foreach (var conditionitem in Global.Notification_Conditions.Where(r => Regex.IsMatch((string)existingVa["properties"]["state"], r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                                            && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                                             && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList())
                                                {
                                                    if (!Global.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)existingVa["properties"]["id"]))
                                                    {
                                                        JObject ojbMerge = (JObject)conditionitem.DeepClone();
                                                        ojbMerge.Merge(data, new JsonMergeSettings
                                                        {
                                                            // union array values together to avoid duplicates
                                                            MergeArrayHandling = MergeArrayHandling.Union
                                                        });
                                                        ojbMerge.Add(new JProperty("VEHICLETIME", ((DateTime)((JObject)existingVa["properties"]).Property("vehicleTime").Value).ToUniversalTime()));
                                                        ojbMerge.Add(new JProperty("VEHICLENAME", (string)((JObject)existingVa["properties"]).Property("name").Value));
                                                        ojbMerge.Add(new JProperty("SHOWTOAST", true));
                                                        ojbMerge.Add(new JProperty("TAGID", (string)existingVa["properties"]["id"]));
                                                        ojbMerge.Add(new JProperty("NOTIFICATIONGID", (string)conditionitem.Property("ID").Value + (string)existingVa["properties"]["id"]));
                                                        ojbMerge.Add(new JProperty("UPDATE", true));
                                                        if (Global.Notification.TryAdd((string)conditionitem.Property("ID").Value + (string)existingVa["properties"]["id"], ojbMerge))
                                                        {
                                                        }
                                                    }
                                                }
                                            }
                                            if (!Update)
                                            {
                                                Update = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        existingVa["properties"]["state"] = (string)data.Property("state".ToUpper()).Value;
                                        //new condition
                                        if (Global.Notification_Conditions.Where(r => Regex.IsMatch((string)existingVa["properties"]["state"], r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                                    && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                                     && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList().Count() > 0)
                                        {
                                            foreach (var conditionitem in Global.Notification_Conditions.Where(r => Regex.IsMatch((string)existingVa["properties"]["state"], r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                                        && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                                         && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList())
                                            {
                                                if (!Global.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)existingVa["properties"]["id"]))
                                                {
                                                    JObject ojbMerge = (JObject)conditionitem.DeepClone();
                                                    ojbMerge.Merge(data, new JsonMergeSettings
                                                    {
                                                        // union array values together to avoid duplicates
                                                        MergeArrayHandling = MergeArrayHandling.Union
                                                    });
                                                    ojbMerge.Add(new JProperty("VEHICLETIME", ((DateTime)((JObject)existingVa["properties"]).Property("vehicleTime").Value).ToUniversalTime()));
                                                    ojbMerge.Add(new JProperty("VEHICLENAME", (string)((JObject)existingVa["properties"]).Property("name").Value));
                                                    ojbMerge.Add(new JProperty("TAGID", (string)existingVa["properties"]["id"]));
                                                    ojbMerge.Add(new JProperty("NOTIFICATIONGID", (string)conditionitem.Property("ID").Value + (string)existingVa["properties"]["id"]));
                                                    ojbMerge.Add(new JProperty("UPDATE", true));
                                                    if (Global.Notification.TryAdd((string)conditionitem.Property("ID").Value + (string)existingVa["properties"]["id"], ojbMerge))
                                                    {
                                                    }
                                                }
                                            }
                                        }
                                        if (!Update)
                                        {
                                            Update = true;
                                        }
                                    }
                                    if (Update)
                                    {
                                        existingVa["properties"]["Tag_Update"] = true;
                                    }
                                });
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

        private static void ProjectData(JObject jsonObject)
        {
            try
            {
                if (jsonObject.HasValues)
                {
                  
                    if (jsonObject.ContainsKey("coordinateSystems"))
                    {

                        JToken locators = jsonObject.SelectToken("coordinateSystems[0].locators");
                        if (locators.Count() > 0)
                        {
                            foreach (JObject locatorsitem in locators.Children())
                            {
                                JArray temp = new JArray();
                                bool update_info = false;
                                try
                                {
                                    if (locatorsitem.ContainsKey("location"))
                                    {
                                        if (locatorsitem.Property("location").FirstOrDefault().HasValues)
                                        {
                                            JArray tagitemsplit = (JArray)locatorsitem.Property("location").Value;
                                            if (tagitemsplit.HasValues)
                                            {
                                                temp = new JArray(tagitemsplit[0], tagitemsplit[1]);
                                            }
                                        }
                                    }
                                    if (Global.Tag.ContainsKey(locatorsitem.Property("id").Value.ToString()))
                                    {
                                        if (Global.Zones.TryGetValue(locatorsitem.Property("id").Value.ToString(), out JObject existingVa))
                                        {
                                            if (existingVa["geometry"]["coordinates"].ToString() != temp.ToString())
                                            {
                                                existingVa["geometry"]["coordinates"] = temp;
                                                update_info = true;
                                            }
                                             ((JObject)existingVa["properties"]).Merge(locatorsitem, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

                                            if (existingVa["properties"]["visible"].ToString() != locatorsitem["visible"].ToString())
                                            {
                                                update_info = true;
                                            }
                                            if (existingVa["properties"]["name"].ToString() != locatorsitem["name"].ToString())
                                            { 
                                                update_info = true;
                                            }
                                            if (update_info)
                                            {
                                                existingVa["properties"]["Tag_Update"] = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        JObject GeoJsonType = new JObject_List().GeoJSON_Locators;
                                        GeoJsonType["geometry"]["type"] = "Point";

                                        if (temp.HasValues)
                                        {
                                            GeoJsonType["geometry"]["coordinates"] = temp;
                                        }
                                        ((JObject)GeoJsonType["properties"]).Merge(locatorsitem, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                     
                                        GeoJsonType["properties"]["Tag_Type"] = "Locater";
                                        GeoJsonType["properties"]["Tag_Update"] = true;
                                        if (!Global.Tag.ContainsKey(locatorsitem["id"].ToString()))
                                        {
                                            Global.Tag.TryAdd(locatorsitem["id"].ToString(), GeoJsonType);
                                        }
                                        GeoJsonType = null;
                                    }
                                }
                                catch (Exception e)
                                {
                                    new ErrorLogger().ExceptionLog(e);
                                }
                            }
                        }
                        JToken zones = jsonObject.SelectToken("coordinateSystems[0].zones");
                        if (zones.Count() > 0)
                        {
                            foreach (JObject zoneitem in zones.Children())
                            {
                                JArray temp = new JArray();
                                bool update_info = false;
                                try
                                {
                                    if (zoneitem.ContainsKey("polygonData"))
                                    {
                                        if (zoneitem.Property("polygonData").HasValues)
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
                                                if (xyar != null)
                                                {
                                                    temp.Add(new JArray(xyar));
                                                }

                                            }
                                        }
                                    }
                                    if (zoneitem.ContainsKey("polygonHoles"))
                                    {
                                       if (zoneitem.Property("polygonHoles").FirstOrDefault().HasValues)
                                        {
                                            foreach (JObject holeitem in zoneitem.Property("polygonHoles").FirstOrDefault().Children())
                                            {
                                                if (holeitem.ContainsKey("polygonData"))
                                                {
                                                    if (holeitem["polygonData"].HasValues)
                                                    {
                                                        string[] polygonholeDatasplit = zoneitem["polygonData"].ToString().Split('|');
                                                        if (polygonholeDatasplit.Length > 0)
                                                        {
                                                            JArray xyar = new JArray();
                                                            foreach (var polygonitem in polygonholeDatasplit)
                                                            {
                                                                string[] polygonitemsplit = polygonitem.Split(',');
                                                                xyar.Add(new JArray(Convert.ToDouble(polygonitemsplit[0]), Convert.ToDouble(polygonitemsplit[1])));
                                                            }
                                                            temp.Add(new JArray(xyar));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (Global.Zones.ContainsKey(zoneitem.Property("id").Value.ToString()))
                                    {
                                        if (Global.Zones.TryGetValue(zoneitem.Property("id").Value.ToString(), out JObject existingVa))
                                        {
                                            if (existingVa["geometry"]["coordinates"].ToString() != temp.ToString())
                                            {
                                                existingVa["geometry"]["coordinates"] = temp;
                                                update_info = true;
                                            }
                                            if (existingVa["properties"]["visible"].ToString() != zoneitem["visible"].ToString())
                                            {
                                                existingVa["properties"]["visible"] = zoneitem["visible"];
                                                update_info = true;
                                            }
                                            if (existingVa["properties"]["color"].ToString() != zoneitem["color"].ToString())
                                            {
                                                existingVa["properties"]["color"] = zoneitem["color"];
                                                update_info = true;
                                            }
                                            if (existingVa["properties"]["Zone_Type"].ToString().StartsWith("Machine"))
                                            {
                                                if (Global.Zone_Info.TryGetValue(existingVa["properties"]["id"].ToString(), out JObject zoneinfodata))
                                                {
                                                    if (existingVa["properties"]["name"].ToString() != zoneinfodata["name"].ToString())
                                                    {
                                                        ((JObject)existingVa["properties"]).Merge(zoneinfodata, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                                    }
                                                    
                                                }
                                            }
                                            if (update_info)
                                            {
                                                existingVa["properties"]["Zone_Update"] = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        JObject GeoJsonType = new JObject_List().GeoJSON_Zone;
                                        GeoJsonType["geometry"]["type"] = "Polygon";

                                        if (temp.HasValues)
                                        {
                                            GeoJsonType["geometry"]["coordinates"] = temp;
                                        }
                                        GeoJsonType["properties"]["id"] = zoneitem.ContainsKey("id") ? zoneitem["id"] : "";
                                        GeoJsonType["properties"]["visible"] = zoneitem.ContainsKey("visible") ? zoneitem["visible"] : false;
                                        GeoJsonType["properties"]["color"] = zoneitem.ContainsKey("color") ? zoneitem["color"] : "";
                                        GeoJsonType["properties"]["name"] = zoneitem.ContainsKey("name") ? zoneitem["name"] : "";
                                        GeoJsonType["properties"]["Zone_Type"] = GetZoneType(zoneitem["name"].ToString());

                                        if (GeoJsonType["properties"]["Zone_Type"].ToString().StartsWith("DockDoor"))
                                        {
                                            string tempdoor = zoneitem.ContainsKey("name") ? string.Join(string.Empty, Regex.Matches((string)zoneitem.Property("name").Value, @"\d+").OfType<Match>().Select(m => m.Value)) : "";
                                            if (!string.IsNullOrEmpty(tempdoor))
                                            {
                                                GeoJsonType["properties"]["doorNumber"] = tempdoor;
                                            }
                                            else
                                            {
                                                GeoJsonType["properties"]["doorNumber"] = tempdoor;
                                            }

                                            GeoJsonType["properties"]["routetripData"] = "";
                                        }
                                        if (GeoJsonType["properties"]["Zone_Type"].ToString().StartsWith("Machine"))
                                        {
                                            if (Global.Zone_Info.TryGetValue(GeoJsonType["properties"]["id"].ToString(), out JObject zoneinfodata))
                                            {
                                                ((JObject)GeoJsonType["properties"]).Merge(zoneinfodata, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(GeoJsonType["properties"]["MPE_Number"].ToString()))
                                                {
                                                    string tempnumber = zoneitem.ContainsKey("name") ? string.Join(string.Empty, Regex.Matches((string)zoneitem.Property("name").Value, @"\d+").OfType<Match>().Select(m => m.Value)) : "";
                                                    GeoJsonType["properties"]["MPE_Number"] = int.TryParse(tempnumber, out int n) ? n.ToString() : "0";
                                                }
                                                if (string.IsNullOrEmpty(GeoJsonType["properties"]["MPE_Type"].ToString()))
                                                {
                                                    string tempname = zoneitem.ContainsKey("name") ? string.Join(string.Empty, Regex.Matches((string)zoneitem.Property("name").Value, @"\p{L}+").OfType<Match>().Select(m => m.Value)) : "";
                                                    GeoJsonType["properties"]["MPE_Type"] = !string.IsNullOrEmpty(tempname) ? tempname : "";
                                                }
                                            }
                                        }
                                        if (jsonObject.ContainsKey("localdata"))
                                        {
                                            GeoJsonType["properties"]["Zone_Update"] = false;
                                        }
                                        else
                                        {
                                            GeoJsonType["properties"]["Zone_Update"] = true;
                                        }
                                        if (!Global.Zones.ContainsKey(zoneitem.Property("id").Value.ToString()))
                                        {
                                            Global.Zones.TryAdd(zoneitem.Property("id").Value.ToString(), GeoJsonType);
                                        }

                                    }
                                }
                                catch (Exception e)
                                {
                                    new ErrorLogger().ExceptionLog(e);
                                }
                            }
                        }
                        JToken polygons = jsonObject.SelectToken("coordinateSystems[0].polygons");
                        if (polygons.HasValues)
                        {
                            foreach (JObject zoneitem in polygons.Children())
                            {
                                try
                                {
                                    JArray temp = new JArray();
                                    bool update_info = false;
                                    if (zoneitem.ContainsKey("polygonData"))
                                    {
                                        if (zoneitem.Property("polygonData").HasValues)
                                        {
                                            string[] polygonDatasplit = zoneitem.Property("polygonData").Value.ToString().Split('|');
                                            if (polygonDatasplit.Length > 0)
                                            {
                                                JArray xyar = new JArray();
                                                foreach (var polygonitem in polygonDatasplit)
                                                {
                                                    string[] polygonitemsplit = polygonitem.Split(',');
                                                    xyar.Add(new JArray(Convert.ToDouble(polygonitemsplit[0]), Convert.ToDouble(polygonitemsplit[1])));
                                                }
                                                temp.Add(new JArray(xyar));
                                            }
                                        }
                                    }
                                    if (zoneitem.ContainsKey("polygonHoles"))
                                    {
                                        if (zoneitem.Property("polygonHoles").FirstOrDefault().HasValues)
                                        {
                                            foreach (JObject holeitem in zoneitem.Property("polygonHoles").FirstOrDefault().Children())
                                            {
                                                if (holeitem.ContainsKey("polygonData"))
                                                {
                                                    if (holeitem.Property("polygonData").HasValues)
                                                    {
                                                        string[] polygonholeDatasplit = holeitem.Property("polygonData").Value.ToString().Split('|');
                                                        if (polygonholeDatasplit.Length > 0)
                                                        {
                                                            JArray xyar = new JArray();
                                                            foreach (var polygonitem in polygonholeDatasplit)
                                                            {
                                                                string[] polygonitemsplit = polygonitem.Split(',');
                                                                xyar.Add(new JArray(Convert.ToDouble(polygonitemsplit[0]), Convert.ToDouble(polygonitemsplit[1])));
                                                            }
                                                            temp.Add(new JArray(xyar));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (Global.Zones.ContainsKey(zoneitem["id"].ToString()))
                                    {
                                        if (Global.Zones.TryGetValue(zoneitem["id"].ToString(), out JObject existingVa))
                                        {
                                            if (existingVa["geometry"]["coordinates"].ToString() != temp.ToString())
                                            {
                                                existingVa["geometry"]["coordinates"] = temp;
                                                update_info = true;
                                            }
                                            if (existingVa["properties"]["visible"].ToString() != zoneitem["visible"].ToString())
                                            {
                                                existingVa["properties"]["visible"] = zoneitem["visible"];
                                                update_info = true;
                                            }
                                            if (existingVa["properties"]["color"].ToString() != zoneitem["color"].ToString())
                                            {
                                                existingVa["properties"]["color"] = zoneitem["color"];
                                                update_info = true;
                                            }
                                            if (existingVa["properties"]["name"].ToString() != zoneitem["name"].ToString())
                                            {
                                                existingVa["properties"]["name"] = zoneitem["name"];
                                                update_info = true;
                                            }
                                            if (update_info)
                                            {
                                                existingVa["properties"]["Zone_Update"] = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        JObject GeoJsonType = new JObject_List().GeoJSON_Zone;

                                        GeoJsonType["geometry"]["type"] = "Polygon";
                                        if (temp.HasValues)
                                        {
                                            GeoJsonType["geometry"]["coordinates"] = temp;
                                        }
                                        GeoJsonType["properties"]["id"] = zoneitem.ContainsKey("id") ? zoneitem["id"] : "";
                                        GeoJsonType["properties"]["visible"] = zoneitem.ContainsKey("visible") ? zoneitem["visible"] : false;
                                        GeoJsonType["properties"]["color"] = zoneitem.ContainsKey("color") ? zoneitem["color"] : "";
                                        GeoJsonType["properties"]["name"] = zoneitem.ContainsKey("name") ? zoneitem["name"].ToString() : "";
                                        GeoJsonType["properties"]["Zone_Type"] = GetZoneType(value: zoneitem["name"].ToString());
                                        if (jsonObject.ContainsKey("localdata"))
                                        {
                                            GeoJsonType["properties"]["Zone_Update"] = false;
                                        }
                                        else
                                        {
                                            GeoJsonType["properties"]["Zone_Update"] = true;
                                        }
                                        if (!Global.Zones.ContainsKey(zoneitem["id"].ToString()))
                                        {
                                            Global.Zones.TryAdd(zoneitem["id"].ToString(), GeoJsonType);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    new ErrorLogger().ExceptionLog(e);
                                }
                            }
                        }
                        JToken backgroundImages = jsonObject.SelectToken("coordinateSystems[0].backgroundImages");
                        if (backgroundImages.Count() > 0)
                        {
                            bool backgroundupdate_info = false;
                            foreach (JObject imageitem in backgroundImages.Children())
                            {

                                if (!Global.Map.ContainsKey(imageitem["id"].ToString()))
                                {
                                    JObject map = new JObject_List().Map;
                                    map["Id"] = imageitem["id"];
                                    map["OrigoY"] = imageitem["origoY"];
                                    map["OrigoX"] = imageitem["origoX"];
                                    map["MetersPerPixelY"] = imageitem["metersPerPixelY"];
                                    map["MetersPerPixelX"] = imageitem["metersPerPixelX"];
                                    map["WidthMeter"] = imageitem["widthMeter"];
                                    map["HeightMeter"] = imageitem["heightMeter"];
                                    map["YMeter"] = imageitem["yMeter"];
                                    map["XMeter"] = imageitem["xMeter"];
                                    map["Base64Img"] = imageitem.ContainsKey("base64") ? imageitem["base64"] : "";
                                    map["Facility_Name"] = Global.AppSettings.ContainsKey("FACILITY_NAME") ? Global.AppSettings["FACILITY_NAME"] : "";
                                    map["Facility_TimeZone"] = Global.AppSettings.ContainsKey("FACILITY_TIMEZONE") ? Global.AppSettings["FACILITY_TIMEZONE"] : "";
                                    map["Environment"] = !string.IsNullOrEmpty(Global.Application_Environment) ? Global.Application_Environment : "";
                                    map["Software_Version"] = !string.IsNullOrEmpty(Global.VersionInfo) ? Global.VersionInfo : "";
                                    map["NASS_Code"] = Global.AppSettings.ContainsKey("FACILITY_NASS_CODE") ? Global.AppSettings["FACILITY_NASS_CODE"] : "";
                                    map["Map_Update"] = true;

                                    if (!Global.Map.ContainsKey(imageitem["id"].ToString()))
                                    {
                                        Global.Map.TryAdd(imageitem["id"].ToString(), map);
                                    }
                                }
                                else
                                {
                                    if (Global.Map.ContainsKey(imageitem["id"].ToString()))
                                    {
                                        if (Global.Map.TryGetValue(imageitem["id"].ToString(), out JObject existingVa))
                                        {
                                            foreach (dynamic kv in imageitem.Children())
                                            {
                                                if (existingVa.ContainsKey(kv.Name))
                                                {
                                                    if (existingVa.Property(kv.Name).Value != kv.Value)
                                                    {
                                                        existingVa.Property(kv.Name).Value = kv.Value;
                                                        backgroundupdate_info = true;
                                                    }
                                                }
                                                else
                                                {
                                                    existingVa.Add(new JProperty(kv.Name, kv.Value));
                                                    backgroundupdate_info = true;
                                                }
                                            }
                                            if (backgroundupdate_info)
                                            {
                                                existingVa["Map_Update"] = true;
                                            }
                                        }
                                    }
                                }

                            }
                        }

                    }
                    if (!jsonObject.ContainsKey("localdata"))
                    {
                        jsonObject["localdata"] = true;
                        new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "ProjectData.json", JsonConvert.SerializeObject(jsonObject, Formatting.Indented));
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static string GetZoneType(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Regex.IsMatch(value, (string)Global.AppSettings.Property("AGV_ZONE").Value, RegexOptions.IgnoreCase))
                    {
                        return "AGVLocation";
                    }
                    else if (Regex.IsMatch(value, (string)Global.AppSettings.Property("DOCKDOOR_ZONE").Value, RegexOptions.IgnoreCase))
                    {
                        return "DockDoor";
                    }
                    else if (Regex.IsMatch(value, (string)Global.AppSettings.Property("MANUAL_ZONE").Value, RegexOptions.IgnoreCase))
                    {
                        return "Area";
                    }
                    else if (Regex.IsMatch(value, (string)Global.AppSettings.Property("VIEWPORTS").Value, RegexOptions.IgnoreCase))
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

        private static void TagPosition(JObject item)
        {
            try
            {
                if (item.HasValues)
                {
                    if (item.ContainsKey("tags"))
                    {
                        double tagtimeout = Global.AppSettings.ContainsKey("TAG_TIMEOUTMILS") ? !string.IsNullOrEmpty((string)Global.AppSettings.Property("TAG_TIMEOUTMILS").Value) ? (long)Global.AppSettings.Property("TAG_TIMEOUTMILS").Value : 300000 : 300000;
                        JArray temp_zone = new JArray();
                        foreach (JObject tagitem in ((JArray)item.Property("tags").Value).Children())
                        {
                            //if (tagitem.Property("name").Value.ToString().StartsWith("Maint"))
                            //{
                            try
                            {
                                temp_zone = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
                                DateTime positionTS = Global.UnixTimeStampToDateTime((long)tagitem.Property("positionTS").Value);
                                DateTime responseTS = Global.UnixTimeStampToDateTime((long)item.Property("responseTS").Value);
                                System.TimeSpan tagdiffResult = responseTS.ToUniversalTime().Subtract(positionTS.ToUniversalTime());
                                JArray tempcoordinates = new JArray();
                                bool update_info = false;
                                if (tagitem.ContainsKey("smoothedPosition"))
                                {
                                    if (tagitem.Property("smoothedPosition").FirstOrDefault().HasValues)
                                    {
                                        JArray tagitemsplit = (JArray)tagitem.Property("smoothedPosition").Value;
                                        if (tagitemsplit.HasValues)
                                        {
                                            tempcoordinates = new JArray(tagitemsplit[0], tagitemsplit[1]);
                                        }
                                    }
                                }
                                if (Global.Tag.ContainsKey(tagitem["id"].ToString()))
                                {
                                    if (Global.Tag.TryGetValue(tagitem["id"].ToString(), out JObject existingVa))
                                    {
                                       
                                        if (existingVa["geometry"]["coordinates"].ToString() != tempcoordinates.ToString())
                                        {
                                            existingVa["geometry"]["coordinates"] = tempcoordinates;
                                            update_info = true;
                                        }
                                        if (tagitem["zones"].HasValues)
                                        {
                                            //default time is 5 minutes
                                            if (tagdiffResult.TotalMilliseconds > tagtimeout)
                                            {
                                                tagitem["zones"] = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
                                                temp_zone = new JArray(new JObject(new JProperty("name", "NOTINZONE"), new JProperty("id", "NOTINZONE")));
                                            }
                                            else
                                            {
                                                temp_zone = (JArray)tagitem["zones"];
                                            }

                                            if (existingVa["properties"]["zones"].ToString() != temp_zone.ToString())
                                            {
                                                existingVa["properties"]["zones"] = temp_zone;
                                                update_info = true;
                                            }
                                        }
                                        if (existingVa["properties"]["name"].ToString() != tagitem["name"].ToString() )
                                        {
                                            existingVa["properties"]["name"] = tagitem["name"];
                                            update_info = true;
                                        }
                                        string tagtype = GetTagType((string)tagitem.Property("name").Value);
                                        if (existingVa["properties"]["Tag_Type"].ToString() != tagtype)
                                        {
                                            existingVa["properties"]["Tag_Type"] = tagtype;
                                            update_info = true;
                                        }
                                        existingVa["properties"]["Raw_Data"] = tagitem;
                                        existingVa["properties"]["Tag_TS"] = responseTS;
                                        existingVa["properties"]["positionTS"] = positionTS;
                                        if (update_info)
                                        {
                                            existingVa["properties"]["Tag_Update"] = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //JObject GeoJson = new JObject_List().GeoJSON;

                                    JObject GeoJsonType = new JObject_List().GeoJSON_Tag;
                                    GeoJsonType["geometry"]["type"] = "Point";
                                    GeoJsonType["geometry"]["coordinates"] = tempcoordinates;
                                    if (tagitem.ContainsKey("zones"))
                                    {
                                        if (tagitem.Property("zones").Value.HasValues)
                                        {
                                            if (tagdiffResult.TotalMilliseconds > tagtimeout)
                                            {
                                                tagitem.Property("zones").Value = temp_zone;
                                                GeoJsonType["properties"]["zones"] = temp_zone;
                                            }
                                            else
                                            {
                                                GeoJsonType["properties"]["zones"] = (JArray)tagitem["zones"];
                                            }
                                        }
                                        else
                                        {
                                            GeoJsonType["properties"]["zones"] = temp_zone;
                                        }
                                    }
                                    GeoJsonType["properties"]["id"] = tagitem.ContainsKey("id") ? tagitem["id"] : "";
                                    GeoJsonType["properties"]["visible"] = tagitem.ContainsKey("visible") ? tagitem["visible"] : false;
                                    GeoJsonType["properties"]["color"] = tagitem.ContainsKey("color") ? tagitem["color"] : "";
                                    GeoJsonType["properties"]["name"] = tagitem.ContainsKey("name") ? tagitem.Property("name").Value : "";
                                    GeoJsonType["properties"]["positionTS"] = positionTS;
                                    GeoJsonType["properties"]["Tag_TS"]= responseTS;
                                    GeoJsonType["properties"]["Tag_Type"] = GetTagType(tagitem["name"].ToString());
                                    GeoJsonType["properties"]["Raw_Data"] = tagitem;
                                    GeoJsonType["properties"]["Tag_Update"] = true;
                                    if (!Global.Tag.ContainsKey(tagitem["id"].ToString()))
                                    {
                                        Global.Tag.TryAdd(tagitem["id"].ToString(), GeoJsonType);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                new ErrorLogger().ExceptionLog(e);
                            }
                        }
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private static string GetTagType(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Regex.IsMatch(value, (string)Global.AppSettings.Property("TAG_AGV").Value, RegexOptions.IgnoreCase))
                    //if (value.ToLower().StartsWith("agv"))
                    {
                        return "Autonomous Vehicle";
                    }
                    else if (Regex.IsMatch(value, (string)Global.AppSettings.Property("TAG_PIV").Value, RegexOptions.IgnoreCase))
                    //if (value.ToLower().StartsWith("walkingrider") || value.ToLower().StartsWith("mule") || value.ToLower().StartsWith("forklift"))
                    {
                        return "Vehicle";
                    }
                    else if (Regex.IsMatch(value, (string)Global.AppSettings.Property("TAG_PERSON").Value, RegexOptions.IgnoreCase))
                    // else if (value.ToLower().StartsWith("mail") || value.ToLower().StartsWith("clerk") || value.ToLower().StartsWith("mha") || value.ToLower().StartsWith("maint") || value.ToLower().StartsWith("supervisor"))
                    {
                        return "Person";
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
    }
}