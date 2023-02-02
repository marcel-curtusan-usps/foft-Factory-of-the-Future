using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Factory_of_the_Future
{
    internal class ItineraryTrip_Update
    {
        private string _Itineraryitem;
        private string _routetripid;

        public ItineraryTrip_Update(string Itineraryitem, string routetripid)
        {
            _Itineraryitem = Itineraryitem;
            _routetripid = routetripid;
            try
            {
                string destsites = "";
                bool update = false;
                if (!string.IsNullOrEmpty(_Itineraryitem))
                {
                    JToken Itinerary = JToken.Parse(_Itineraryitem);
                    if (Itinerary.HasValues)
                    {
                        JToken legs = Itinerary[0].SelectToken("legs");
                        if (legs.Count() > 0)
                        {
                            if (AppParameters.RouteTripsList.TryGetValue(_routetripid, out RouteTrips existingVal))
                            {

                                existingVal.Legs = legs.ToObject<List<Leg>>();

                                foreach (JObject legitem in legs.Children())
                                {
                                    // get all dest do not include origin Site if site is the same
                                    if (legitem["legDestSiteID"].ToString() != existingVal.OriginSiteId && (int)legitem["legNumber"] >= existingVal.LegNumber)
                                    {
                                        destsites += ("(^" + (string)legitem["legDestSiteID"] + "$)|");
                                    }
                                }
                                if (destsites != existingVal.DestSites && !string.IsNullOrEmpty(destsites))
                                {
                                    existingVal.DestSites = destsites.Substring(0, destsites.Length - 1);
                                    update = true;
                                }
                                if (update)
                                {
                                    existingVal.TripUpdate = true;
                                }
                            }
                        }
                    }
                    else
                    {

                        if (AppParameters.RouteTripsList.TryGetValue(_routetripid, out RouteTrips existingVal))
                        {

                            if (!(destsites == existingVal.DestSites || string.IsNullOrEmpty(destsites)))
                            {
                                existingVal.DestSites = ("(^" + existingVal.DestSiteId + "$)|");
                                existingVal.TripUpdate = true;
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