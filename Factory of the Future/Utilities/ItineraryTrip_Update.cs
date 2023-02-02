using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Factory_of_the_Future
{
    //internal class ItineraryTrip_Update
    //{
    //    public ItineraryTrip_Update(string Itineraryitem, string routtripid)
    //    {
    //        try
    //        {
    //            string destsites = "";
    //            bool update = false;
    //            if (!string.IsNullOrEmpty(Itineraryitem))
    //            {
    //                JToken Itinerary = JToken.Parse(Itineraryitem);
    //                if (Itinerary.HasValues)
    //                {
    //                    JToken legs = Itinerary[0].SelectToken("legs");
    //                    if (legs.Count() > 0)
    //                    {
    //                        if (AppParameters.RouteTripsList.TryGetValue(routtripid, out RouteTrips existingVal))
    //                        {
                            
    //                            existingVal.Legs = legs.ToObject<List<Leg>>();
                               
    //                            foreach (JObject legitem in legs.Children())
    //                            {
    //                                // get all dest do not include origin Site if site is the same
    //                                if (legitem["legDestSiteID"].ToString() != existingVal.OriginSiteId && (int)legitem["legNumber"] >= existingVal.LegNumber)
    //                                {
    //                                    destsites += ("(^" + (string)legitem["legDestSiteID"] + "$)|");
    //                                }
    //                            }
    //                            if (destsites != existingVal.DestSites && !string.IsNullOrEmpty(destsites))
    //                            {
    //                                existingVal.DestSites = destsites.Substring(0, destsites.Length - 1);
    //                                update = true;
    //                            }
    //                            if (update)
    //                            {
    //                                existingVal.TripUpdate = true;
    //                            }
    //                        }
    //                    }
    //                }
    //                else
    //                {

    //                    if (AppParameters.RouteTripsList.TryGetValue(routtripid, out RouteTrips existingVal))
    //                    {
                         
    //                        if (!(destsites == existingVal.DestSites || string.IsNullOrEmpty(destsites)))
    //                        {
    //                            existingVal.DestSites = ("(^" + existingVal.DestSiteId + "$)|");
    //                            existingVal.TripUpdate = true;
    //                        }
    //                    }
    //                }
    //                //List<Leg> Legs = JsonConvert.DeserializeObject<List<Leg>>(JsonConvert.SerializeObject(legs));
    //                //if (Legs.Count > 0)
    //                //{
    //                //    if (AppParameters.Trips.TryGetValue(string.Concat(item["route"], item["trip"], tripDirectionInd), out Trips existingVal))
    //                //    {
    //                //        string destsites = "";
    //                //        existingVal.Legs = Legs;
    //                //        bool update = false;
    //                //        foreach (Leg legitem in Legs)
    //                //        {
    //                //            if (legitem.LegNumber == existingVal.LegNumber)
    //                //            {
    //                //                if (legitem.ActArrivalDtm.Year > 1)
    //                //                {
    //                //                    existingVal.ActArrivalDtm = legitem.ActArrivalDtm;
    //                //                    update = true;
    //                //                }
    //                //                if (legitem.ActDepartureDtm.Year > 1)
    //                //                {
    //                //                    existingVal.ActualDtm = legitem.ActDepartureDtm;
    //                //                    update = true;
    //                //                }
    //                //            }

    //                //            // get all dest do not include origin Site if site is the same
    //                //            if (legitem.LegDestSiteID != existingVal.OriginSiteId && legitem.LegNumber >= existingVal.LegNumber)
    //                //            {

    //                //                destsites += ("(^" + legitem.LegDestSiteID.ToString() + "$)|");
    //                //            }
    //                //        }
    //                //        if (destsites != existingVal.DestSite)
    //                //        {
    //                //            if (!string.IsNullOrEmpty(destsites))
    //                //            {
    //                //                existingVal.DestSite = destsites.Substring(0, destsites.Length - 1);
    //                //                update = true;
    //                //            }
    //                //        }


    //                //        if (update)
    //                //        {
    //                //            existingVal.Trip_Update = true;
    //                //        }


    //                //        //if (legs != null)
    //                //        //{
    //                //        //    foreach (JObject legitem in legs.Children())
    //                //        //    {
    //                //        //        if (legitem.ContainsKey("legNumber") && (int)legitem["legNumber"] == (int)existingVal.LegNumber)
    //                //        //        {
    //                //        //            if (legitem.ContainsKey("scheduledArrDTM"))
    //                //        //            {
    //                //        //                existingVal.ScheduledArrDTM = AppParameters.SVdatetimeformat((JObject)legitem["scheduledArrDTM"]);
    //                //        //            }
    //                //        //            if (legitem.ContainsKey("scheduledDepDTM"))
    //                //        //            {
    //                //        //                existingVal.ScheduledDepDTM = AppParameters.SVdatetimeformat((JObject)legitem["scheduledDepDTM"]);
    //                //        //            }
    //                //        //            if (legitem.ContainsKey("actDepartureDtm"))
    //                //        //            {
    //                //        //                existingVal.ActDepartureDtm = AppParameters.SVdatetimeformat((JObject)legitem["actDepartureDtm"]);
    //                //        //            }
    //                //        //            if (legitem.ContainsKey("actArrivalDtm"))
    //                //        //            {
    //                //        //                existingVal.ActArrivalDtm = AppParameters.SVdatetimeformat((JObject)legitem["actArrivalDtm"]);
    //                //        //            }
    //                //        //        }

    //                //        //    }
    //                //        //}
    //                //    }
    //                //}
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            new ErrorLogger().ExceptionLog(e);
    //        }

    //    }
    //}
}