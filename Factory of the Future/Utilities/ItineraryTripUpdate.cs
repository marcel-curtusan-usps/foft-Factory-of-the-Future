using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Factory_of_the_Future
{
    internal class ItineraryTripUpdate
    {
        private JObject item;

        public ItineraryTripUpdate(JArray jarrayitem, string tripDirectionInd)
        {
            try
            {
                if (jarrayitem.HasValues)
                {
                    this.item = (JObject)jarrayitem.First;
                    JToken legs = item.SelectToken("legs");
                    List<Leg> Legs = JsonConvert.DeserializeObject<List<Leg>>(JsonConvert.SerializeObject(legs));
                    if (Legs.Count > 0)
                    {
                        if (Global.Trips.TryGetValue(string.Concat(item["route"], item["trip"], tripDirectionInd), out Trips existingVal))
                        {
                            string destsites = ""; 
                            existingVal.Legs = Legs;
                            bool update = false;
                            foreach (Leg legitem in Legs)
                            {
                                if (legitem.LegNumber == existingVal.LegNumber)
                                {
                                    if (legitem.ActArrivalDtm.Year > 1)
                                    {
                                        existingVal.ActArrivalDtm = legitem.ActArrivalDtm;
                                        update = true;
                                    }
                                    if (legitem.ActDepartureDtm.Year > 1)
                                    {
                                        existingVal.ActualDtm = legitem.ActDepartureDtm;
                                        update = true;
                                    }
                                }

                                // get all dest do not include origin Site if site is the same
                                if (legitem.LegDestSiteID != existingVal.OriginSiteId &&  legitem.LegNumber >= existingVal.LegNumber )
                                {

                                    destsites += ("(^"+legitem.LegDestSiteID.ToString() + "$)|");
                                }
                            }
                            if (destsites != existingVal.DestSite)
                            {
                                if (!string.IsNullOrEmpty(destsites))
                                {
                                    existingVal.DestSite = destsites.Substring(0, destsites.Length - 1);
                                    update = true;
                                }
                            }

                            
                            if (update)
                            {
                                existingVal.Trip_Update = true;
                            }
                       

                            //if (legs != null)
                            //{
                            //    foreach (JObject legitem in legs.Children())
                            //    {
                            //        if (legitem.ContainsKey("legNumber") && (int)legitem["legNumber"] == (int)existingVal.LegNumber)
                            //        {
                            //            if (legitem.ContainsKey("scheduledArrDTM"))
                            //            {
                            //                existingVal.ScheduledArrDTM = Global.SVdatetimeformat((JObject)legitem["scheduledArrDTM"]);
                            //            }
                            //            if (legitem.ContainsKey("scheduledDepDTM"))
                            //            {
                            //                existingVal.ScheduledDepDTM = Global.SVdatetimeformat((JObject)legitem["scheduledDepDTM"]);
                            //            }
                            //            if (legitem.ContainsKey("actDepartureDtm"))
                            //            {
                            //                existingVal.ActDepartureDtm = Global.SVdatetimeformat((JObject)legitem["actDepartureDtm"]);
                            //            }
                            //            if (legitem.ContainsKey("actArrivalDtm"))
                            //            {
                            //                existingVal.ActArrivalDtm = Global.SVdatetimeformat((JObject)legitem["actArrivalDtm"]);
                            //            }
                            //        }

                            //    }
                            //}
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