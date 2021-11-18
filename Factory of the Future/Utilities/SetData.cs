using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Factory_of_the_Future
{
    public class SetData
    {


        //internal void Call_VehicleStateHistory(JObject vehicle)
        //{
        //    try
        //    {
        //        DateTime F_key = Global.Vehicle_State_History.Where(f => (string)f.Value.Property("MAC_ADDRESS").Value == (string)vehicle.Property("VEHICLE_MAC_ADDRESS").Value)
        //              .OrderByDescending(d => ((DateTime)d.Value.Property("STATUS_CHANGE_START_DATE_TIME")).Date)
        //              .ThenByDescending(d => ((DateTime)d.Value.Property("STATUS_CHANGE_START_DATE_TIME")).Month)
        //              .ThenByDescending(d => ((DateTime)d.Value.Property("STATUS_CHANGE_START_DATE_TIME")).Day)
        //              .ThenByDescending(d => ((DateTime)d.Value.Property("STATUS_CHANGE_START_DATE_TIME")).Hour)
        //              .ThenByDescending(d => ((DateTime)d.Value.Property("STATUS_CHANGE_START_DATE_TIME")).Minute)
        //              .ThenByDescending(d => ((DateTime)d.Value.Property("STATUS_CHANGE_START_DATE_TIME")).Second)
        //              .Select(r => r.Key).FirstOrDefault();

        //        if (Global.Vehicle_State_History.ContainsKey(F_key))
        //        {
        //            if (Global.Vehicle_State_History.TryGetValue(F_key, out JObject current_vehicle_history))
        //            {
        //                //if fleet status not eq then update and create new recored
        //                if (current_vehicle_history.Property("STATE").Value.ToString().Trim() != vehicle.Property("STATE").Value.ToString().Trim())
        //                {
        //                    if (current_vehicle_history.ContainsKey("STATUS_CHANGE_END_DATE_TIME"))
        //                    { current_vehicle_history.Property("STATUS_CHANGE_END_DATE_TIME").Value = DateTime.Now; }
        //                    else
        //                    {
        //                        current_vehicle_history.Add(new JProperty("STATUS_CHANGE_END_DATE_TIME", DateTime.Now));
        //                    }
        //                    if ((string)current_vehicle_history.Property("VEHICLE").Value != (string)vehicle.Property("VEHICLE").Value)
        //                    {
        //                        current_vehicle_history.Property("VEHICLE").Value = (string)vehicle.Property("VEHICLE").Value;
        //                    }
        //                    if ((string)current_vehicle_history.Property("BATTERYLEVEL").Value != (string)vehicle.Property("BATTERYPERCENT").Value)
        //                    {
        //                        current_vehicle_history.Property("BATTERYLEVEL").Value = (string)vehicle.Property("BATTERYPERCENT").Value;
        //                    }
        //                    if ((string)current_vehicle_history.Property("CATEGORY_ID").Value != (string)vehicle.Property("CATEGORY").Value)
        //                    {
        //                        current_vehicle_history.Property("CATEGORY_ID").Value = (string)vehicle.Property("CATEGORY").Value;
        //                    }
        //                    if (vehicle.ContainsKey("Request_Id"))
        //                    {
        //                        if (current_vehicle_history.ContainsKey("REQUEST_ID"))
        //                        {
        //                            if ((string)current_vehicle_history.Property("REQUEST_ID").Value != (string)vehicle.Property("Request_Id").Value)
        //                            {
        //                                current_vehicle_history.Property("REQUEST_ID").Value = (string)vehicle.Property("Request_Id").Value;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            current_vehicle_history.Add(new JProperty("REQUEST_ID", (string)vehicle.Property("Request_Id").Value));
        //                        }
        //                    }
        //                    if (vehicle.ContainsKey("BODYTYPE"))
        //                    {
        //                        if (current_vehicle_history.ContainsKey("BODYTYPE"))
        //                        {
        //                            if ((string)current_vehicle_history.Property("BODYTYPE").Value != (string)vehicle.Property("BODYTYPE").Value)
        //                            {
        //                                current_vehicle_history.Property("BODYTYPE").Value = (string)vehicle.Property("BODYTYPE").Value;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            current_vehicle_history.Add(new JProperty("BODYTYPE", (string)vehicle.Property("BODYTYPE").Value));
        //                        }
        //                    }

        //                    //update previous vehicle recored
        //                    new Oracle_DB_Calls().Vehicle_History_Update(current_vehicle_history);

        //                    // add new recored
        //                    JObject new_vehicle_history = new JObject
        //                    {
        //                        new JProperty("BODYTYPE", vehicle.ContainsKey("BODYTYPE") ? (string)vehicle.Property("BODYTYPE").Value : ""),
        //                        new JProperty("BATTERYLEVEL", vehicle.ContainsKey("BATTERYPERCENT") ? (string)vehicle.Property("BATTERYPERCENT").Value : ""),
        //                        new JProperty("STATE", vehicle.ContainsKey("STATE") ? (string)vehicle.Property("STATE").Value : ""),
        //                        new JProperty("CATEGORY_ID", vehicle.ContainsKey("CATEGORY") ? (string)vehicle.Property("CATEGORY").Value : ""),
        //                        new JProperty("MAC_ADDRESS", vehicle.ContainsKey("VEHICLE_MAC_ADDRESS") ? (string)vehicle.Property("VEHICLE_MAC_ADDRESS").Value : ""),
        //                        new JProperty("STATUS_CHANGE_START_DATE_TIME", vehicle.ContainsKey("TIME") ? vehicle.Property("TIME").Value : ""),
        //                        new JProperty("VEHICLE", vehicle.ContainsKey("VEHICLE") ? (string)vehicle.Property("VEHICLE").Value : ""),
        //                        new JProperty("VEHICLE_NUMBER", vehicle.ContainsKey("VEHICLE_NUMBER") ? (string)vehicle.Property("VEHICLE_NUMBER").Value : "")
        //                    };
        //                    if (!Global.Vehicle_State_History.ContainsKey((DateTime)vehicle.Property("TIME").Value))
        //                    {
        //                        if (Global.Vehicle_State_History.TryAdd((DateTime)vehicle.Property("TIME").Value, new_vehicle_history))
        //                        {
        //                            new Oracle_DB_Calls().Vehicle_History_Insert(new_vehicle_history);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //this if vehicle doesn't exist add new record.
        //            // add new recored
        //            JObject new_vehicle_history = new JObject
        //            {
        //                new JProperty("BODYTYPE", vehicle.ContainsKey("BODYTYPE") ? (string)vehicle.Property("BODYTYPE").Value : ""),
        //                new JProperty("BATTERYLEVEL", vehicle.ContainsKey("BATTERYPERCENT") ? (string)vehicle.Property("BATTERYPERCENT").Value : ""),
        //                new JProperty("STATE", vehicle.ContainsKey("STATE") ? (string)vehicle.Property("STATE").Value : ""),
        //                new JProperty("CATEGORY_ID", vehicle.ContainsKey("CATEGORY") ? (string)vehicle.Property("CATEGORY").Value : ""),
        //                new JProperty("MAC_ADDRESS", vehicle.ContainsKey("VEHICLE_MAC_ADDRESS") ? (string)vehicle.Property("VEHICLE_MAC_ADDRESS").Value : ""),
        //                new JProperty("STATUS_CHANGE_START_DATE_TIME", vehicle.ContainsKey("TIME") ? vehicle.Property("TIME").Value : ""),
        //                new JProperty("STATUS_CHANGE_END_DATE_TIME", ""),
        //                new JProperty("VEHICLE", vehicle.ContainsKey("VEHICLE") ? (string)vehicle.Property("VEHICLE").Value : ""),
        //                new JProperty("VEHICLE_NUMBER", vehicle.ContainsKey("VEHICLE_NUMBER") ? (string)vehicle.Property("VEHICLE_NUMBER").Value : "")
        //            };
        //            if (!Global.Vehicle_State_History.ContainsKey((DateTime)vehicle.Property("TIME").Value))
        //            {
        //                if (Global.Vehicle_State_History.TryAdd((DateTime)vehicle.Property("TIME").Value, new_vehicle_history))
        //                {
        //                    new Oracle_DB_Calls().Vehicle_History_Insert(new_vehicle_history);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}
    }
}