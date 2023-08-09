
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
    internal class TripData : IDisposable
    {
        private bool disposedValue;
        public string _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public bool update { get; protected set; }
        public JToken tempData = null;
        public List<RouteTrips> temp = null;
        public RouteTrips currentRTData = null;


        internal bool Load(string data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    tempData = JToken.Parse(_data);
                    if (tempData != null && tempData.HasValues)
                    {
                        temp = tempData.ToObject<List<RouteTrips>>();
                        foreach (RouteTrips rt in temp)
                        {
                            ProcessTrip(rt);

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
                Dispose();
            }
        }

        internal bool ProcessTrip(RouteTrips rt)
        {
            bool update = false;
            try
            {
                if ((Regex.IsMatch(rt.LegStatus, AppParameters.AppSettings.SV_TRIP_STATUS, RegexOptions.IgnoreCase)
                              || Regex.IsMatch(rt.Status, AppParameters.AppSettings.SV_TRIP_STATUS, RegexOptions.IgnoreCase)))
                {
                    if (AppParameters.RouteTripsList.ContainsKey(rt.Id) && AppParameters.RouteTripsList.TryRemove(rt.Id, out currentRTData))
                    {
                        //remove trip
                        FOTFManager.Instance.BroadcastTripsRemove(rt.Id);
                    }

                }
                else
                {
                    if (rt.TripDirectionInd == "O" && rt.ActualDtm.Year == DateTime.Now.Year)
                    {
                        //remove trip

                        if (!AppParameters.RouteTripsList.ContainsKey(rt.Id) && AppParameters.RouteTripsList.TryRemove(rt.Id, out currentRTData))
                        {
                            FOTFManager.Instance.BroadcastTripsRemove(rt.Id);
                        }
                    }
                    else
                    {
                        //rt.RawData = JsonConvert.SerializeObject(rt, Formatting.None);
                        if (string.IsNullOrEmpty(rt.DoorNumber))
                        {
                            rt.DoorNumber = getDefaultDockDoor(string.Concat(rt.Route, rt.Trip));
                            rt.DoorId = !string.IsNullOrEmpty(rt.DoorNumber) ? string.Concat("99D", rt.DoorNumber.PadLeft(4, '-')) : "";
                        }

                        if (AppParameters.RouteTripsList.ContainsKey(rt.Id) && AppParameters.RouteTripsList.TryGetValue(rt.Id, out currentRTData))
                        {
                            //get trip trailer content.


                            rt.Containers = FOTFManager.Instance.GetTripContainer(currentRTData.DestSites, rt.TrailerBarcode, out int NotloadedContainers, out int loaded);
                            rt.NotloadedContainers = NotloadedContainers;
                            //currentRTData.RawData = JsonConvert.SerializeObject(rt, Formatting.None);

                            //foreach (PropertyInfo prop in currentRTData.GetType().GetProperties())
                            //{

                            //    if (!new Regex("^(Containers|Legs|RawData)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                            //    {
                            //        if (prop.GetValue(currentRTData, null) != prop.GetValue(rt, null))
                            //        {
                            //            update = true;
                            //            prop.SetValue(currentRTData, prop.GetValue(rt, null));

                            //        }
                            //    }
                            //}
                            //if (update)
                            //{
                            if (AppParameters.RouteTripsList.TryUpdate(rt.Id, rt, currentRTData))
                            {
                                Task.Run(() => FOTFManager.Instance.BroadcastTripsUpdate(rt)).ConfigureAwait(false);
                                update = true;
                            }
                            //}

                        }
                        else if (AppParameters.RouteTripsList.TryAdd(rt.Id, rt))
                        {
                            FOTFManager.Instance.BroadcastTripsAdd(rt);
                            update = true;
                        }
                        //get trip Itinerary
                        if (!rt.Legs.Any())
                        {
                            Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt.Route, rt.Trip, AppParameters.AppSettings.FACILITY_NASS_CODE, new Utility().GetSvDate(rt.OperDate)), rt.Id)).ConfigureAwait(false);
                        }
                    }
                }
                return update;
            }
            catch (Exception e)
            {
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
                if (start_time.Year >= DateTime.Now.Year && !string.IsNullOrEmpty(route) && !string.IsNullOrEmpty(trip))
                {
                    Uri parURL = new Uri(string.Format(AppParameters.AppSettings.SV_ITINERARY, route, trip, string.Concat(start_time.ToString("yyyy-MM-dd'T'"), "00:00:00")));
                    string SV_Response = new SendMessage().Get(parURL, new JObject());
                    if (!string.IsNullOrEmpty(SV_Response))
                    {
                        temp = SV_Response;
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

        private string getDefaultDockDoor(string rt)
        {
            try
            {
                string door = AppParameters.DoorTripAssociation.Where(f => f.Key == rt).Select(y => y.Value.DoorNumber).FirstOrDefault();
                if (!string.IsNullOrEmpty(door))
                {
                    return door;
                }
                else
                {
                    return "";
                }
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
                update = false;

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