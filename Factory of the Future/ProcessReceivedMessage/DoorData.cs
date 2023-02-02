using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public JToken tempData = null;
        public RouteTrips currenttrip = null;
        public RouteTrips newRTData = null;
        public List<RouteTrips> doortempData;
        private bool saveToFile;

        internal bool Load(dynamic data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (_data != null)
                {
                    tempData = JToken.Parse(data);
                    if (tempData != null && tempData.HasValues)
                    {
                        doortempData = tempData.ToObject<List<RouteTrips>>();
                        foreach (RouteTrips rt in doortempData)
                        {
                            AppParameters.DockdoorList.AddOrUpdate(rt.DoorNumber, rt.DoorNumber, (key, oldValue) =>
                                   {
                                       return rt.DoorNumber;
                                   });
                            if (rt.Id != "00" )
                            {
                                SaveDoorTripAssociation(new DoorTripAssociation { DoorNumber = rt.DoorNumber, Route = rt.Route, Trip = rt.Trip });

                                if (!string.IsNullOrEmpty(rt.Id))
                                {
                                    if (AppParameters.RouteTripsList.ContainsKey(rt.Id)
                                        && AppParameters.RouteTripsList.TryGetValue(rt.Id, out currenttrip))
                                    {
                                        currenttrip.TripMin = AppParameters.Get_TripMin(rt.ScheduledDtm);
                                        currenttrip.RawData = JsonConvert.SerializeObject(rt, Formatting.None);
                                        currenttrip.DoorId = rt.DoorId;
                                        currenttrip.DoorNumber = rt.DoorNumber;
                                        currenttrip.Status = "ACTIVE";

                                        Task.Run(() => FOTFManager.Instance.UpdateDoorZone(currenttrip));

                                    }
                                    else if (!AppParameters.RouteTripsList.ContainsKey(rt.Id))
                                    {
                                        rt.TripMin = AppParameters.Get_TripMin(rt.ScheduledDtm);
                                        rt.RawData = JsonConvert.SerializeObject(rt, Formatting.None);
                                        rt.Status = "ACTIVE";

                                        AddTriptoList(rt.Id, rt);
                                        Task.Run(() => FOTFManager.Instance.UpdateDoorZone(rt));
                                    }
                                }
                                else
                                {
                                    Task.Run(() => FOTFManager.Instance.UpdateDoorZone(rt));
                                }
                            }
                            else
                            {
                                Task.Run(() => FOTFManager.Instance.UpdateDoorZone(rt));
                            }

                        }
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
                Dispose(false);
            }
        }
        internal void SaveDoorTripAssociation(DoorTripAssociation data)
        {
            try
            {

                AppParameters.DoorTripAssociation.AddOrUpdate(string.Concat(data.Route, data.Trip), data, (key, oldValue) =>
                {
                    return data;
                });

                new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "DoorTripAssociation.json", JsonConvert.SerializeObject(AppParameters.DoorTripAssociation.Select(x => x.Value).ToList(), Formatting.Indented));
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
        private void AddTriptoList(string routetripid, RouteTrips newRTData)
        {
            try
            {
                if (getDefaultDockDoor(string.Concat(newRTData.Route, newRTData.Trip), out string RouteTritDefaultDoor))
                {
                    newRTData.DoorNumber = RouteTritDefaultDoor;
                    newRTData.DoorId = !string.IsNullOrEmpty(RouteTritDefaultDoor) ? string.Concat("99D", RouteTritDefaultDoor.PadLeft(4, '-')) : "";


                    if (AppParameters.RouteTripsList.ContainsKey(routetripid) && AppParameters.RouteTripsList.TryGetValue(routetripid, out RouteTrips existingVal))
                    {
                        if (AppParameters.RouteTripsList.TryUpdate(routetripid, newRTData, existingVal))
                        {
                            //update 

                        }

                    }
                    else
                    {

                        if (AppParameters.RouteTripsList.TryAdd(routetripid, newRTData))
                        {
                            //add
                        }
                    }

                    if (newRTData.OperDate != null)
                    {
                        Task.Run(() => new ItineraryTrip_Update(GetItinerary(newRTData.Route, newRTData.Trip, AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), AppParameters.GetSvDate(newRTData.OperDate)), routetripid));
                    }
                }
                else
                {
                    if (AppParameters.RouteTripsList.TryAdd(routetripid, newRTData))
                    {
                        if (newRTData.OperDate != null)
                        {
                            Task.Run(() => new ItineraryTrip_Update(GetItinerary(newRTData.Route, newRTData.Trip, AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), AppParameters.GetSvDate(newRTData.OperDate)), routetripid));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private string GetItinerary(string route, string trip, string nasscode, DateTime start_time)
        {
            string temp = "";
            try
            {
                //string start_time = string.Concat(DateTime.Now.ToString("yyyy-MM-dd'T'"), "00:00:00");

                Uri parURL = new Uri(string.Format((string)AppParameters.AppSettings["SV_ITINERARY"], route, trip, string.Concat(start_time.ToString("yyyy-MM-dd'T'"), "00:00:00")));
                string SV_Response = new SendMessage().Get(parURL);
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
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
                dockdoor_id = string.Empty;
                doorInfo = string.Empty;
                doortempData = null;
                tempData = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProjectData()
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