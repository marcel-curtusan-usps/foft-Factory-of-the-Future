
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
        public List<RouteTrips> temp;
        public RouteTrips currentRTData = null;


        internal async Task<bool> LoadAsync(string data, string message_type, string connID)
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

                            if ((Regex.IsMatch(rt.LegStatus, "(CANCELED|DEPARTED|OMITTED)", RegexOptions.IgnoreCase)
                                || Regex.IsMatch(rt.Status, "(CANCELED|DEPARTED|OMITTED)", RegexOptions.IgnoreCase)))
                            {
                                if (AppParameters.RouteTripsList.ContainsKey(rt.Id))
                                {
                                    //remove trip
                                    FOTFManager.Instance.BroadcastTripsRemove(rt.Id);
                                }
                                continue;
                            }
                            else
                            {
                                if (rt.ActualDtm.Year == DateTime.Now.Year)
                                {
                                    //remove trip
                                    if (!AppParameters.RouteTripsList.ContainsKey(rt.Id))
                                    {
                                        if (AppParameters.RouteTripsList.TryRemove(rt.Id, out currentRTData))
                                        {
                                            FOTFManager.Instance.BroadcastTripsRemove(rt.Id);
                                        }
                                        continue;
                                    }
                                }
                                else
                                {
                                    rt.RawData = JsonConvert.SerializeObject(rt, Formatting.None);
                                    rt.DoorNumber = getDefaultDockDoor(string.Concat(rt.Route, rt.Trip));
                                    if (!string.IsNullOrEmpty(rt.DoorNumber))
                                    {
                                        rt.DoorId = !string.IsNullOrEmpty(rt.DoorNumber) ? string.Concat("99D", rt.DoorNumber.PadLeft(4, '-')) : "";
                                    }

                                    if (AppParameters.RouteTripsList.ContainsKey(rt.Id) && AppParameters.RouteTripsList.TryGetValue(rt.Id, out currentRTData))
                                    {
                                        currentRTData.Containers = FOTFManager.Instance.GetTripContainer(currentRTData.DestSites, rt.TrailerBarcode, out int NotloadedContainers, out int loaded);
                                        currentRTData.NotloadedContainers = NotloadedContainers;
                                        currentRTData.RawData = JsonConvert.SerializeObject(rt, Formatting.None);

                                        foreach (PropertyInfo prop in currentRTData.GetType().GetProperties())
                                        {

                                            if (!new Regex("^(Containers|Legs|RawData)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                            {
                                                if (prop.GetValue(rt, null).ToString() != prop.GetValue(currentRTData, null).ToString())
                                                {
                                                    update = true;
                                                    prop.SetValue(currentRTData, prop.GetValue(rt, null));

                                                }
                                            }
                                        }
                                        if (update)
                                        {
                                            await Task.Run(() => FOTFManager.Instance.BroadcastTripsUpdate(currentRTData)).ConfigureAwait(false);
                                        }

                                    }
                                    else if (AppParameters.RouteTripsList.TryAdd(rt.Id, rt))
                                    {
                                        FOTFManager.Instance.BroadcastTripsAdd(rt);

                                    }
                                    //get trip Itinerary
                                    if (!rt.Legs.Any())
                                    {
                                        await Task.Run(() => new ItineraryTrip_Update(GetItinerary(rt.Route, rt.Trip, AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(), new Utility().GetSvDate(rt.OperDate)), rt.Id)).ConfigureAwait(false);
                                    }
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
                Dispose();
            }
        }

        private string GetItinerary(string route, string trip, string nasscode, DateTime start_time)
        {
            string temp = "";
            try
            {
                //string start_time = string.Concat(DateTime.Now.ToString("yyyy-MM-dd'T'"), "00:00:00");

                Uri parURL = new Uri(string.Format((string)AppParameters.AppSettings["SV_ITINERARY"], route, trip, string.Concat(start_time.ToString("yyyy-MM-dd'T'"), "00:00:00")));
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

        internal void SaveDoorTripAssociation(string doorNumber, string route, string trip)
        {
            try
            {
                if (AppParameters.DoorTripAssociation.ContainsKey(string.Concat(route, trip))
                    && AppParameters.DoorTripAssociation.TryGetValue(string.Concat(route, trip), out DoorTrip dr))
                {
                    dr.DoorNumber = doorNumber;
                    dr.Route = route;
                    dr.Trip = trip;
                }
                else
                {
                    if (AppParameters.DoorTripAssociation.TryAdd(string.Concat(route, trip), new DoorTrip { DoorNumber = doorNumber, Route = route, Trip = trip }))
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