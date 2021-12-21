using Newtonsoft.Json.Linq;
using System;

namespace Factory_of_the_Future
{
    internal class DoorTripsUpdate
    {
        private JObject item;

        public DoorTripsUpdate(JObject item, string tripDirectionInd)
        {
            try
            {
                bool update = false;
                this.item = item;
                if (Global.Trips.TryGetValue(string.Concat(item["route"], item["trip"], tripDirectionInd), out Trips existingVal))
                {
                    if (existingVal.DoorNumber != item["doorNumber"].ToString())
                    {
                        existingVal.DoorNumber =  item["doorNumber"].ToString();
                        update = true;
                    }
                    if (existingVal.DoorId != item["doorId"].ToString())
                    {
                        existingVal.DoorId = item["doorId"].ToString();
                        update = true;
                    }
                    if (update)
                    {
                        existingVal.Trip_Update = true;
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