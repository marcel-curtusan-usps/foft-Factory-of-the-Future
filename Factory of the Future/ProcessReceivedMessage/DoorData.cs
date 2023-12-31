﻿using Factory_of_the_Future.Models;
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
    internal class DoorData : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public string dockdoor_id { get; protected set; }
        public string doorInfo { get; protected set; }
        public bool update;
        public int NotloadedContainers { get; set; } = 0;
        public JToken tempData = null;
        public RouteTrips currenttrip = null;
        public RouteTrips newRTData = null;
        public List<RouteTrips> doortempData;
        public List<DockDoorStatus> door_temp_Data;


        internal async Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            update = true;
            try
            {
                if (_data != null)
                {

                    tempData = JToken.Parse(_data);
                    if (tempData != null && tempData.HasValues)
                    {
                        if (AppParameters.AppSettings.LOG_API_DATA)
                        {
                            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.LogFloder), string.Concat(message_type, DateTime.Now.ToString("yyyyMMdd"), ".txt"), JsonConvert.SerializeObject(tempData, Formatting.Indented));
                        }
                        if (tempData[0]["SQLTypeName"].ToString() == "SV.LOCATION_STATUS_OBJ")
                        {
                            door_temp_Data = tempData.ToObject<List<DockDoorStatus>>().OrderBy(o => o.LocationBarcode).ToList();
                            foreach (DockDoorStatus door in door_temp_Data)
                            {
                            
                                if (!AppParameters.DockdoorList.ContainsKey(door.LocationNumber) && AppParameters.DockdoorList.TryAdd(door.LocationNumber, door.LocationNumber))
                                {
                                    //
                                }
                               
                                if (!string.IsNullOrEmpty(door.TripReport.Id))
                                {
                                    door.TripReport.AtDoor = true;
                                    door.TripReport.DoorId = door.LocationBarcode;
                                    door.TripReport.DoorNumber = door.LocationNumber;
                                    door.TripReport.OperDate = new Models.EventDtm { Year = DateTime.Now.Year, Month = DateTime.Now.Month - 1, DayOfMonth = DateTime.Now.Day, HourOfDay = DateTime.Now.Hour, Minute = 0,  Second = 0 };
                                    if (new TripData().ProcessTrip(JsonConvert.DeserializeObject<RouteTrips>(JsonConvert.SerializeObject(door.TripReport))))
                                    {
                                        FOTFManager.Instance.UpdateDoorData(door.LocationNumber);
                                    }
                                   ;
                                    //await Task.Run(() => FOTFManager.Instance.saveDoorTripAssociation(door.LocationNumber, door.TripReport.Route, door.TripReport.Trip)).ConfigureAwait(true);
                                    //rt = JsonConvert.DeserializeObject<RouteTrips>(JsonConvert.SerializeObject(door.TripReport));
                                    //rt.AtDoor = true;
                                    //rt.DoorId = door.LocationBarcode;
                                    //rt.DoorNumber = door.LocationNumber;
                                    //if (AppParameters.RouteTripsList.ContainsKey(rt.Id) && AppParameters.RouteTripsList.TryGetValue(rt.Id, out currenttrip))
                                    //{
                                    //    currenttrip.AtDoor = true;
                                    //    currenttrip.DoorId = door.LocationBarcode;
                                    //    currenttrip.DoorNumber = door.LocationNumber;
                                    //    //get trip Itinerary
                                    //    if (!currenttrip.Legs.Any())
                                    //    {
                                    //        await Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt.Route, rt.Trip, AppParameters.AppSettings.FACILITY_NASS_CODE, new Utility().GetSvDate(rt.OperDate)), rt.Id)).ConfigureAwait(true);
                                    //    }
                                    //    List<Container> temp = Task.Run(() => FOTFManager.Instance.GetTripContainer(currenttrip.DestSites, rt.TrailerBarcode, out int NotloadedContainers, out int loaded)).Result;
                                    //    if (temp != null && temp.Count() > currenttrip.Containers.Count() )
                                    //    {
                                    //        currenttrip.Containers = temp;
                                    //        currenttrip.NotloadedContainers = NotloadedContainers;
                                    //        update = true;
                                    //    }
                                    //    if (update)
                                    //    {
                                    //        await Task.Run(() => FOTFManager.Instance.UpdateDoorData(currenttrip.DoorNumber)).ConfigureAwait(true);
                                    //    }
                                    //}
                                    //else if (!AppParameters.RouteTripsList.ContainsKey(rt.Id))
                                    //{
                                    //    rt.DestSites = ("(^" + rt.LegSiteId + "$)");

                                    //    rt.Containers = FOTFManager.Instance.GetTripContainer(rt.DestSites, rt.TrailerBarcode, out int NotloadedContainers, out int loaded);
                                    //    rt.NotloadedContainers = NotloadedContainers;
                                    //    rt.RawData = JsonConvert.SerializeObject(rt, Formatting.None);
                                    //    rt.Status = "ACTIVE";
                                    //    if (AppParameters.RouteTripsList.TryAdd(rt.Id, rt))
                                    //    {
                                     //    FOTFManager.Instance.UpdateDoorData(door.LocationNumber);
                                    //    }

                                    //}
                                }
                                else
                                {
                                    foreach (string rtId in AppParameters.RouteTripsList.Where(x => x.Value.DoorNumber == door.LocationNumber && x.Value.AtDoor).Select(y => y.Key))
                                    {
                                        if (AppParameters.RouteTripsList.TryRemove(rtId, out currenttrip))
                                        {
                                            await Task.Run(() => FOTFManager.Instance.UpdateDoorData(door.LocationNumber)).ConfigureAwait(false);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            doortempData = tempData.ToObject<List<RouteTrips>>();
                            foreach (RouteTrips rt in doortempData)
                            {
                                if (!AppParameters.DockdoorList.ContainsKey(rt.DoorNumber))
                                {
                                    if (AppParameters.DockdoorList.TryAdd(rt.DoorNumber, rt.DoorNumber))
                                    {
                                        //
                                    }
                                }

                                if (rt.Id != "00")
                                {
                                    await Task.Run(() => FOTFManager.Instance.saveDoorTripAssociation(rt.DoorNumber, rt.Route, rt.Trip)).ConfigureAwait(false);
                                    rt.AtDoor = true;

                                    if (AppParameters.RouteTripsList.ContainsKey(rt.Id) && AppParameters.RouteTripsList.TryGetValue(rt.Id, out currenttrip))
                                    {

                                        List<Container> temp = Task.Run(() => (List<Container>)FOTFManager.Instance.GetTripContainer(currenttrip.DestSites, rt.TrailerBarcode, out int NotloadedContainers, out int loaded)).Result;
                                        if (temp != null && currenttrip.Containers.Count() >= temp.Count())
                                        {
                                            currenttrip.Containers = temp;
                                            currenttrip.NotloadedContainers = NotloadedContainers;
                                        }
                                        currenttrip.RawData = JsonConvert.SerializeObject(rt, Formatting.None);
                                        foreach (PropertyInfo prop in currenttrip.GetType().GetProperties())
                                        {

                                            if (!new Regex("^(Containers|Legs|RawData)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                            {
                                                if (prop.GetValue(rt, null) != prop.GetValue(currenttrip, null))
                                                {
                                                    update = true;
                                                    prop.SetValue(currenttrip, prop.GetValue(rt, null));

                                                }
                                            }
                                        }
                                        if (update)
                                        {
                                            await Task.Run(() => FOTFManager.Instance.UpdateDoorData(currenttrip.DoorNumber)).ConfigureAwait(false);
                                        }
                                    }
                                    else if (!AppParameters.RouteTripsList.ContainsKey(rt.Id))
                                    {
                                        rt.DestSites = ("(^" + rt.LegSiteId + "$)");

                                        rt.Containers = (List<Container>)FOTFManager.Instance.GetTripContainer(rt.DestSites, rt.TrailerBarcode, out int NotloadedContainers, out int loaded);
                                        rt.NotloadedContainers = NotloadedContainers;
                                        rt.RawData = JsonConvert.SerializeObject(rt, Formatting.None);
                                        rt.Status = "ACTIVE";
                                        if (AppParameters.RouteTripsList.TryAdd(rt.Id, rt))
                                        {
                                            await Task.Run(() => FOTFManager.Instance.UpdateDoorData(rt.DoorNumber)).ConfigureAwait(false);
                                        }

                                    }

                                }
                                else
                                {
                                    foreach (string rtId in AppParameters.RouteTripsList.Where(x => x.Value.DoorNumber == rt.DoorNumber && x.Value.AtDoor).Select(y => y.Key))
                                    {
                                        if (AppParameters.RouteTripsList.TryRemove(rtId, out currenttrip))
                                        {
                                            await Task.Run(() => FOTFManager.Instance.UpdateDoorData(rt.DoorNumber)).ConfigureAwait(false);
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                return update;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return update;
            }
            finally
            {
                Dispose();
            }
        }
      
        private bool getDefaultDockDoor(string rt, out string door)
        {
            string _door = "";
            try
            {
                _door = AppParameters.DoorTripAssociation.Where(f => f.Key == rt).Select(y => y.Value.DoorNumber).FirstOrDefault();
                door = !string.IsNullOrEmpty(_door) ? _door.Trim() : "";

                return true;
            }
            catch (Exception e)
            {
                door = !string.IsNullOrEmpty(_door) ? _door.Trim() : "";
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }
        
        private string GetItinerary(string route, string trip, string nasscode, DateTime start_time)
        {
            string temp = "";
            try
            {
                //string start_time = string.Concat(DateTime.Now.ToString("yyyy-MM-dd'T'"), "00:00:00");

                Uri parURL = new Uri(string.Format(AppParameters.AppSettings.SV_ITINERARY, route, trip, string.Concat(start_time.ToString("yyyy-MM-dd'T'"), "00:00:00")));
                string SV_Response = new SendMessage().Get(parURL, new JObject());
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
                dockdoor_id = string.Empty;
                doorInfo = string.Empty;
                doortempData = null;
                tempData = null;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}