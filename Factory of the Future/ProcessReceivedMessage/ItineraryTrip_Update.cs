using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Factory_of_the_Future
{
    internal class ItineraryTrip_Update : IDisposable
    {
        private string _Itineraryitem { get; set; }
        private string _routetripid { get; set; }
        private StringBuilder destsites = new StringBuilder();
        private bool disposedValue;
        private JToken Itinerary { get; set; }
        private JToken legs { get; set; }

        public ItineraryTrip_Update(string Itineraryitem, string routetripid)
        {
            _Itineraryitem = Itineraryitem;
            _routetripid = routetripid;
            try
            {
                if (!string.IsNullOrEmpty(_Itineraryitem))
                {
                    Itinerary = JToken.Parse(_Itineraryitem);
                    if (Itinerary.HasValues)
                    {
                        legs = Itinerary[0].SelectToken("legs");
                        if (legs.HasValues)
                        {
                            if (AppParameters.RouteTripsList.TryGetValue(_routetripid, out RouteTrips existingVal))
                            {

                                existingVal.Legs = legs.ToObject<List<Leg>>();
                                if (existingVal.Legs.Any())
                                {
                                    foreach (Leg legitem in existingVal.Legs)
                                    {
                                        if (legitem.LegDestSiteID != existingVal.OriginSiteId && existingVal.LegNumber >= legitem.LegNumber)
                                        {
                                            destsites.Append("(^" + legitem.LegDestSiteID + "$)|");
                                        }
                                    }
                                }
                                if (destsites.Length > 0)
                                {
                                    existingVal.DestSites = destsites.ToString().Substring(0, destsites.Length - 1);
                                    existingVal.TripUpdate = true;
                                }
                                //foreach (JObject legitem in legs.Children().OfType<JObject>())
                                //{
                                //    // get all dest do not include origin Site if site is the same
                                //    if (legitem["legDestSiteID"].ToString() != existingVal.OriginSiteId && (int)legitem["legNumber"] >= existingVal.LegNumber)
                                //    {
                                //        destsites += "(^" + (string)legitem["legDestSiteID"] + "$)|";
                                //    }
                                //}
                                //if (destsites != existingVal.DestSites && !string.IsNullOrEmpty(destsites))
                                //{
                                //    existingVal.DestSites = destsites.Substring(0, destsites.Length - 1);
                                //    update = true;
                                //}
                                //if (update)
                                //{
                                //    existingVal.TripUpdate = true;
                                //}
                            }
                        }
                    }
                    else
                    {

                        if (AppParameters.RouteTripsList.TryGetValue(_routetripid, out RouteTrips existingVal))
                        {
                            existingVal.DestSites = ("(^" + existingVal.LegSiteId + "$)");
                            existingVal.TripUpdate = true;
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
                Dispose();
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
                legs = null;
                Itinerary = null;
                _routetripid = string.Empty;
                _Itineraryitem = string.Empty;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ItineraryTrip_Update()
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