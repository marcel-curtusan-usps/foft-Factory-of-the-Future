using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Factory_of_the_Future
{
    internal class ItineraryTrip_Update
    {
        private JObject item;
        public ItineraryTrip_Update(JArray jarrayitem, string tripDirectionInd, string routtripid)
        {
            try
            {
                if (jarrayitem.HasValues)
                {
                    this.item = (JObject)jarrayitem.First;
                    JToken legs = item.SelectToken("legs");
                    if (legs.Count() > 0)
                    {
                        //if (AppParameters.RouteTripsList.TryGetValue(routtripid, out JObject existingVal))
                        //{
                        //    string destsites = "";
                        //    existingVal["Legs"] = legs;
                        //    bool update = false;
                        //    foreach (JObject legitem in legs.Children())
                        //    {
                        //        // get all dest do not include origin Site if site is the same
                        //        if (legitem["legDestSiteID"].ToString() != existingVal["originSiteId"].ToString() && (int)legitem["legNumber"] >= (int)existingVal["legNumber"])
                        //        {
                        //            destsites += ("(^" + (string)legitem["legDestSiteID"] + "$)|");
                        //        }
                        //    }
                        //    if (destsites != existingVal["destSite"].ToString())
                        //    {
                        //        if (!string.IsNullOrEmpty(destsites))
                        //        {
                        //            existingVal["destSite"] = destsites.Substring(0, destsites.Length - 1);
                        //            update = true;
                        //        }
                        //    }


                        //    if (update)
                        //    {
                        //        existingVal["Trip_Update"] = true;
                        //    }
                        //}
                    }

                    //List<Leg> Legs = JsonConvert.DeserializeObject<List<Leg>>(JsonConvert.SerializeObject(legs));
                    //if (Legs.Count > 0)
                    //{
                    //    if (AppParameters.Trips.TryGetValue(string.Concat(item["route"], item["trip"], tripDirectionInd), out Trips existingVal))
                    //    {
                    //        string destsites = "";
                    //        existingVal.Legs = Legs;
                    //        bool update = false;
                    //        foreach (Leg legitem in Legs)
                    //        {
                    //            if (legitem.LegNumber == existingVal.LegNumber)
                    //            {
                    //                if (legitem.ActArrivalDtm.Year > 1)
                    //                {
                    //                    existingVal.ActArrivalDtm = legitem.ActArrivalDtm;
                    //                    update = true;
                    //                }
                    //                if (legitem.ActDepartureDtm.Year > 1)
                    //                {
                    //                    existingVal.ActualDtm = legitem.ActDepartureDtm;
                    //                    update = true;
                    //                }
                    //            }

                    //            // get all dest do not include origin Site if site is the same
                    //            if (legitem.LegDestSiteID != existingVal.OriginSiteId && legitem.LegNumber >= existingVal.LegNumber)
                    //            {

                    //                destsites += ("(^" + legitem.LegDestSiteID.ToString() + "$)|");
                    //            }
                    //        }
                    //        if (destsites != existingVal.DestSite)
                    //        {
                    //            if (!string.IsNullOrEmpty(destsites))
                    //            {
                    //                existingVal.DestSite = destsites.Substring(0, destsites.Length - 1);
                    //                update = true;
                    //            }
                    //        }


                    //        if (update)
                    //        {
                    //            existingVal.Trip_Update = true;
                    //        }


                    //        //if (legs != null)
                    //        //{
                    //        //    foreach (JObject legitem in legs.Children())
                    //        //    {
                    //        //        if (legitem.ContainsKey("legNumber") && (int)legitem["legNumber"] == (int)existingVal.LegNumber)
                    //        //        {
                    //        //            if (legitem.ContainsKey("scheduledArrDTM"))
                    //        //            {
                    //        //                existingVal.ScheduledArrDTM = AppParameters.SVdatetimeformat((JObject)legitem["scheduledArrDTM"]);
                    //        //            }
                    //        //            if (legitem.ContainsKey("scheduledDepDTM"))
                    //        //            {
                    //        //                existingVal.ScheduledDepDTM = AppParameters.SVdatetimeformat((JObject)legitem["scheduledDepDTM"]);
                    //        //            }
                    //        //            if (legitem.ContainsKey("actDepartureDtm"))
                    //        //            {
                    //        //                existingVal.ActDepartureDtm = AppParameters.SVdatetimeformat((JObject)legitem["actDepartureDtm"]);
                    //        //            }
                    //        //            if (legitem.ContainsKey("actArrivalDtm"))
                    //        //            {
                    //        //                existingVal.ActArrivalDtm = AppParameters.SVdatetimeformat((JObject)legitem["actArrivalDtm"]);
                    //        //            }
                    //        //        }

                    //        //    }
                    //        //}
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }

        }
    }
}