using Newtonsoft.Json.Linq;
using System;

namespace Factory_of_the_Future
{
    internal class ItineraryTripUpdate
    {
        private JObject item;

        public ItineraryTripUpdate(JArray jarrayitem)
        {
            try
            {
                if (jarrayitem.HasValues)
                {
                    this.item = (JObject)jarrayitem.First;
                    JToken legs = item.SelectToken("legs");
                    if (legs.HasValues)
                    {
                        if (Global.Trips.TryGetValue(string.Concat(item["route"], item["trip"]), out Trips existingVal))
                        {
                            if (legs != null)
                            {
                                foreach (JObject legitem in legs.Children())
                                {
                                    if (legitem.ContainsKey("legNumber") && (int)legitem["legNumber"] == (int)existingVal.LegNumber)
                                    {
                                        if (legitem.ContainsKey("scheduledArrDTM"))
                                        {
                                            existingVal.ScheduledArrDTM = Global.SVdatetimeformat((JObject)legitem["scheduledArrDTM"]);
                                        }
                                        if (legitem.ContainsKey("scheduledDepDTM"))
                                        {
                                            existingVal.ScheduledDepDTM= Global.SVdatetimeformat((JObject)legitem["scheduledDepDTM"]);
                                        }
                                        if (legitem.ContainsKey("actDepartureDtm"))
                                        {
                                            existingVal.ActDepartureDtm = Global.SVdatetimeformat((JObject)legitem["actDepartureDtm"]);
                                        }
                                        if (legitem.ContainsKey("actArrivalDtm"))
                                        {
                                            existingVal.ActArrivalDtm = Global.SVdatetimeformat((JObject)legitem["actArrivalDtm"]);
                                        }
                                    }
                                }
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