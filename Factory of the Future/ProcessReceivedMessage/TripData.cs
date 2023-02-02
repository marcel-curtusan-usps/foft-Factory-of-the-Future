
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class TripData : IDisposable
    {
        private bool disposedValue;
        public string _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public JToken tempData = null;
        public List<RouteTrips> temp;
        public RouteTrips currentRTData = null;

        internal bool Load(string data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (data != null)
                {
                    tempData = JToken.Parse(_data);
                    if (tempData != null && tempData.HasValues)
                    {
                        temp = tempData.ToObject<List<RouteTrips>>();
                        foreach (RouteTrips rt in temp)
                        {
                            
                            if ((Regex.IsMatch(rt.LegStatus, "(CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE)", RegexOptions.IgnoreCase)
                                || Regex.IsMatch(rt.Status, "(CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE)", RegexOptions.IgnoreCase)))
                            {
                                if (AppParameters.RouteTripsList.ContainsKey(rt.Id) && AppParameters.RouteTripsList.TryRemove(rt.Id, out currentRTData))
                                {
                                    //remove trip
                                    FOTFManager.Instance.BroadcastSVTripsRemove(currentRTData.Id);
                                }
                            }
                            else
                            {
                                rt.RawData = JsonConvert.SerializeObject(rt, Formatting.None);
                                rt.TripMin = AppParameters.Get_TripMin(rt.ScheduledDtm);
                                rt.DoorNumber = getDefaultDockDoor(string.Concat(rt.Route, rt.Trip));
                                if (!string.IsNullOrEmpty(rt.DoorNumber))
                                {
                                    rt.DoorId = !string.IsNullOrEmpty(rt.DoorNumber) ? string.Concat("99D", rt.DoorNumber.PadLeft(4, '-')) : "";
                                }

                                if (AppParameters.RouteTripsList.ContainsKey(rt.Id) && AppParameters.RouteTripsList.TryGetValue(rt.Id, out currentRTData))
                                {
                                    AppParameters.RouteTripsList.TryUpdate(rt.Id, rt, currentRTData);
                                    FOTFManager.Instance.BroadcastSVTripsUpdate(currentRTData);
                                }
                                else if (AppParameters.RouteTripsList.TryAdd(rt.Id, rt))
                                {
                                    FOTFManager.Instance.BroadcastSVTripsAdd(rt);

                                }
                                //get trip Itinerary
                                if (true)
                                {
                                    Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt.Route, rt.Trip, AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), AppParameters.GetSvDate(rt.OperDate)), rt.Id));
                                }
                            }
                        }

                    }
                }
                return true;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return true;
            }
            finally
            {
                disposedValue = true;
                Dispose(false);

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

        internal void SaveDoorTripAssociation(string doorNumber, string route, string trip)
        {
            try
            {
                if (AppParameters.DoorTripAssociation.ContainsKey(string.Concat(route, trip))
                    && AppParameters.DoorTripAssociation.TryGetValue(string.Concat(route, trip), out DoorTripAssociation dr))
                {
                    dr.DoorNumber = doorNumber;
                    dr.Route = route;
                    dr.Trip = trip;
                }
                else
                {
                    if (AppParameters.DoorTripAssociation.TryAdd(string.Concat(route, trip), new DoorTripAssociation { DoorNumber = doorNumber, Route = route, Trip = trip }))
                    {
                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "DoorTripAssociation.json", JsonConvert.SerializeObject(AppParameters.DoorTripAssociation.Select(x => x.Value).ToList(), Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
                    }

                }


            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                Dispose(false);
            }
        }

        private string getDefaultDockDoor(string rt)
        {
            try
            {
                return AppParameters.DoorTripAssociation.Where(f => f.Key == rt).Select(y => y.Value.DoorNumber).FirstOrDefault();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
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
                temp = null;

            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TripData()
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