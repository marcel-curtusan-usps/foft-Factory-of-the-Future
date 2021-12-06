using Newtonsoft.Json.Linq;
using System;

namespace Factory_of_the_Future
{
    internal class DoorTripsUpdate
    {
        private JObject item;

        public DoorTripsUpdate(JObject item)
        {
            try
            {
                this.item = item;
                if (Global.Trips.TryGetValue(string.Concat(item["route"], item["trip"]), out Trips existingVal))
                {
                    existingVal.DoorNumber = item.ContainsKey("doorNumber") ? item["doorNumber"].ToString() : "";
                    existingVal.DoorId = item.ContainsKey("doorId") ? item["doorId"].ToString() : "";
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
           
        }
       
    }
}