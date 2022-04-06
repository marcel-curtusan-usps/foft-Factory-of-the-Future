using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Factory_of_the_Future
{
    public class FOTFManager : IDisposable
    {
        private readonly static Lazy<FOTFManager> _instance = new Lazy<FOTFManager>(() => new FOTFManager(GlobalHost.ConnectionManager.GetHubContext<HubManager>().Clients));
       

        //blocks
        private readonly object updateClocklock = new object();

        private readonly object updateZoneStatuslock = new object();
        private readonly object updateTagStatuslock = new object();
        private readonly object updatePersonTagStatuslock = new object();
        private readonly object updateDockDoorStatuslock = new object();
        private readonly object updateQSMlock = new object();
        private readonly object updateMachineStatuslock = new object();
        private readonly object updateAGVLocationStatuslock = new object();
        private readonly object updateSVTripsStatuslock = new object();
        //private readonly object updateCTSDepartedStatuslock = new object();
        //private readonly object updateCTSLocalDepartedStatuslock = new object();
        //private readonly object updateCTSInboundStatuslock = new object();
        //private readonly object updateCTSOutboundStatuslock = new object();
        private readonly object updateNotificationStatuslock = new object();

        //timers
        private readonly Timer Clock_timer;

        private readonly Timer VehicleTag_timer;
        private readonly Timer PersonTag_timer;
        private readonly Timer Zone_timer;
        private readonly Timer DockDoor_timer;
        private readonly Timer QSM_timer;
        private readonly Timer Machine_timer;
        private readonly Timer AGVLocation_timer;
        private readonly Timer SVTrips_timer;
        //private readonly Timer CTSDeparted_timer;
        //private readonly Timer CTSLocalDeparted_timer;
        //private readonly Timer CTSInbound_timer;
        //private readonly Timer CTSOutbound_timer;
        private readonly Timer Notification_timer;
        //status
        private volatile bool _updateClockStatus = false;

        private volatile bool _updatePersonTagStatus = false;
        private volatile bool _updateZoneStatus = false;
        private volatile bool _updateTagStatus = false;
        private volatile bool _updateDockDoorStatus = false;
        private volatile bool _updatingQSMStatus = false;
        private volatile bool _updateMachineStatus = false;
        private volatile bool _updateAGVLocationStatus = false;
        private volatile bool _updateSVTripsStatus = false;
        //private volatile bool _updateCTSDepartedStatus = false;
        //private readonly bool _updateCTSLocalDepartedStatus = false;
        //private volatile bool _updateCTSInboundStatus = false;
        //private volatile bool _updateCTSOutboundStatus = false;
        private volatile bool _updateNotificationstatus = false;

        //private readonly Random _updateOrNotRandom = new Random();
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(2000);

        private readonly TimeSpan _250milupdateInterval = TimeSpan.FromMilliseconds(250);
        private readonly TimeSpan _1000milupdateInterval = TimeSpan.FromMilliseconds(1000);

    

        //init timers
        private FOTFManager(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
            Clock_timer = new Timer(UpdateClockStatus, null, _1000milupdateInterval, _1000milupdateInterval);
            //VehicleTag_timer = new Timer(UpdateVehicleTagStatus, null, _250milupdateInterval, _250milupdateInterval);
            //PersonTag_timer = new Timer(UpdatePersonTagStatus, null, _250milupdateInterval, _250milupdateInterval);
            ////////Zone status.
            Zone_timer = new Timer(UpdateZoneStatus, null, _updateInterval, _updateInterval);
            //DockDoor_timer = new Timer(UpdateDockDoorStatus, null, _250milupdateInterval, _250milupdateInterval);
            //Machine_timer = new Timer(UpdateMachineStatus, null, _updateInterval, _updateInterval);
            //AGVLocation_timer = new Timer(UpdateAGVLocationStatus, null, _250milupdateInterval, _250milupdateInterval);
            /////SV Trips Data
            //SVTrips_timer = new Timer(UpdateSVTripsStatus, null, _updateInterval, _updateInterval);
            /////CTS data timer
            //CTSDeparted_timer = new Timer(UpdateCTSDepartedStatus, null, _updateInterval, _updateInterval);
            //CTSLocalDeparted_timer = new Timer(UpdateCTSLocalDepartedStatus, null, _updateInterval, _updateInterval);
            //CTSInbound_timer = new Timer(UpdateCTSInboundStatus, null, _updateInterval, _updateInterval);
            //CTSOutbound_timer = new Timer(UpdateCTSOutboundStatus, null, _updateInterval, _updateInterval);
            // SV Trip Data

            ////   Notification data timer
            //Notification_timer = new Timer(UpdateNotificationtatus, null, _updateInterval, _updateInterval);
            //////Connection status
            QSM_timer = new Timer(UpdateQSM, null, _updateInterval, _updateInterval);
        }
        public static FOTFManager Instance
        {
            get { return _instance.Value; }
        }

        private IHubConnectionContext<dynamic> Clients { get; set; }

        internal void GetConnections(string connectionId)
        {
            try
            {
                if (!AppParameters._connections.GetConnections(connectionId).Contains(connectionId))
                {
                    AppParameters._connections.Add(connectionId, connectionId);
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private void UpdateClockStatus(object state)
        {
            lock (updateClocklock)
            {
                if (!_updateClockStatus)
                {
                    _updateClockStatus = true;
                    DateTime dtLastUpdate = DateTime.Now;
                    if (!string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("FACILITY_TIMEZONE").Value))
                    {
                        if (AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
                        {
                            dtLastUpdate = TimeZoneInfo.ConvertTime(dtLastUpdate, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
                           
                        }
                    }
                    BroadcastClockStatus(dtLastUpdate);
                    _updateClockStatus = false;
                }
            }
        }


        private void BroadcastClockStatus(DateTime dtLastUpdate)
        {
            Clients.All.updateClock(dtLastUpdate);
        }

        ////internal IEnumerable<JToken> GetNotification(string data)
        ////{
        ////    try
        ////    {
        ////        if (!string.IsNullOrEmpty(data))
        ////        {
        ////            return AppParameters.NotificationList.Where(c => c.Value.Property("TYPE").Value.ToString().ToLower() == data.ToLower()
        ////            && (bool)c.Value.Property("ACTIVE_CONDITION").Value).Select(x => x.Value).ToList();
        ////        }
        ////        else
        ////        {
        ////            if (AppParameters.NotificationList.Count() > 0)
        ////            {
        ////                return AppParameters.NotificationList.Select(x => x.Value).ToList();
        ////            }
        ////            else
        ////            {
        ////                return new JObject();
        ////            }
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return new JObject();
        ////    }
        ////}

        //internal IEnumerable<JToken> GetADUserProfile(string userID)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(userID))
        //        {
        //            if (AppParameters.Users.ContainsKey(userID))
        //            {
        //                if (AppParameters.Users.TryGetValue(userID, out JObject user))
        //                {
        //                    if (string.IsNullOrEmpty((string)user.Property("EmailAddress").Value))
        //                    {
        //                        if (new FindACEUser().User(user, out JObject ADuser))
        //                        {
        //                            user.Property("EmailAddress").Value = ADuser.Property("EmailAddress").Value;
        //                            user.Property("Phone").Value = ADuser.Property("Phone").Value;
        //                            user.Property("MiddleName").Value = ADuser.Property("MiddleName").Value;
        //                            user.Property("SurName").Value = ADuser.Property("SurName").Value;
        //                            user.Property("FirstName").Value = ADuser.Property("FirstName").Value;
        //                            user.Property("ZipCode").Value = ADuser.Property("ZipCode").Value;
        //                            return user;
        //                        }

        //                        return user;
        //                    }
        //                    else
        //                    {
        //                        return user;
        //                    }
        //                }
        //                else
        //                {
        //                    return new JObject();
        //                }
        //            }
        //            else
        //            {
        //                return new JObject();
        //            }
        //        }
        //        else
        //        {
        //            return new JObject();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return new JObject();
        //    }
        //}

        ////internal IEnumerable<JToken> UDP_Start_Listener()
        ////{
        ////    if (AppParameters.udpServer != null)
        ////    {
        ////    }
        ////    else
        ////    {
        ////      new UDP_Client().startListener();
        ////    }
        ////}

        ////internal IEnumerable<JToken> UDP_Stop_Listener()
        ////{
        ////    throw new NotImplementedException();
        ////}

        internal ADUser GetUserProfile(string conID)
        {
            try
            {
                if (!string.IsNullOrEmpty(conID))
                {
                    if (AppParameters.Users.ContainsKey(conID))
                    {
                        if (AppParameters.Users.TryGetValue(conID, out ADUser user))
                        {
                            return user;
                        }
                    }
                    
                }
                return null;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        ////private void UpdateNotificationtatus(object state)
        ////{
        ////    lock (updateNotificationStatuslock)
        ////    {
        ////        if (!_updateNotificationstatus)
        ////        {
        ////            _updateNotificationstatus = true;
        ////            foreach (var notification in AppParameters.NotificationList.Values)
        ////            {
        ////                if (TryUpdateNotificationStatus(notification))
        ////                {
        ////                    BroadcastNotificationStatus(notification);
        ////                }
        ////            }
        ////            _updateNotificationstatus = false;
        ////        }
        ////    }
        ////}

        ////private bool TryUpdateNotificationStatus(JObject notification)
        ////{
        ////    try
        ////    {
        ////        notification["UPDATE"] = false;
        ////        if ((bool)notification.Property("ACTIVE_CONDITION").Value)
        ////        {
        ////            if (notification.ContainsKey("DELETE"))
        ////            {
        ////                if (AppParameters.NotificationList.TryRemove((string)notification.Property("notificationId").Value, out JObject outnotification))
        ////                {
        ////                    return true;
        ////                }
        ////                else
        ////                {
        ////                    return false;
        ////                }
        ////            }
        ////            else
        ////            {
        ////                return true;
        ////            }
        ////        }
        ////        else
        ////        {
        ////            if (AppParameters.NotificationList.TryRemove((string)notification.Property("notificationId").Value, out JObject outnotification))
        ////            {
        ////                if (!notification.ContainsKey("DELETE"))
        ////                {
        ////                    notification.Add(new JProperty("DELETE", true));
        ////                }

        ////                return true;
        ////            }
        ////            else
        ////            {
        ////                return false;
        ////            }
        ////        }

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return true;
        ////    }
        ////}

        ////private void BroadcastNotificationStatus(JObject vehiclenotification)
        ////{
        ////    Clients.All.updateNotification(vehiclenotification);
        ////}

        ////internal IEnumerable<JToken> GetNotification_ConditionsList(string data)
        ////{
        ////    try
        ////    {
        ////        if (!string.IsNullOrEmpty(data))
        ////        {
        ////            return AppParameters.NotificationConditionsList.Where(c => (string)c.Value.Property("id").Value == data).Select(x => x.Value).ToList();
        ////        }
        ////        else
        ////        {
        ////            return AppParameters.NotificationConditionsList.Select(x => x.Value).ToList();
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return new JObject();
        ////    }
        ////}

        ////internal IEnumerable<JToken> AddNotification_Conditions(string data)
        ////{
        ////    try
        ////    {
        ////        if (!string.IsNullOrEmpty(data))
        ////        {
        ////            JObject Notification = JObject.Parse(data);
        ////            if (Notification.HasValues)
        ////            {
        ////                JObject new_notification = new JObject_List().Notification_Conditions;
        ////                new_notification.Property("LASTUPDATE_DATE").Value = "";
        ////                bool updateFile = false;
        ////                foreach (var kv in from dynamic kv in Notification.Children()
        ////                                   where new_notification.ContainsKey(kv.Name)
        ////                                   where kv.Value != new_notification.Property(kv.Name).Value
        ////                                   select kv)
        ////                {
        ////                    new_notification.Property(kv.Name).Value = kv.Value;
        ////                    updateFile = true;
        ////                }

        ////                if (!AppParameters.NotificationConditionsList.ContainsKey((string)new_notification.Property("id").Value))
        ////                {
        ////                    AppParameters.NotificationConditionsList.TryAdd((string)new_notification.Property("id").Value, new_notification);
        ////                }
        ////                else
        ////                {
        ////                    return new JObject(new JProperty("ERROR_MESSAGE", "ID already exist"));
        ////                }

        ////                if (updateFile)
        ////                {
        ////                    //write to file the new connection
        ////                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Notification.json", JsonConvert.SerializeObject(AppParameters.NotificationConditionsList.Select(x => x.Value), Formatting.Indented));
        ////                    return AppParameters.NotificationConditionsList.Select(e => e.Value).ToList();
        ////                }
        ////                else
        ////                {
        ////                    return new JObject(new JProperty("ERROR_MESSAGE", "No update made"));
        ////                }
        ////            }
        ////            else
        ////            {
        ////                return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
        ////            }
        ////        }
        ////        else
        ////        {
        ////            return new JObject(new JProperty("ERROR_MESSAGE", "No data provvided"));
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        new ErrorLogger().ExceptionLog(ex);
        ////        return new JObject(new JProperty("ERROR_MESSAGE", "Data Not loaded"));
        ////    }
        ////}

        ////internal IEnumerable<JToken> EditNotification_Conditions(string data)
        ////{
        ////    try
        ////    {
        ////        JObject updatenotification = JObject.Parse(data);
        ////        if (updatenotification.HasValues)
        ////        {
        ////            if (updatenotification.ContainsKey("id"))
        ////            {
        ////                if (AppParameters.NotificationConditionsList.ContainsKey((string)updatenotification.Property("id").Value))
        ////                {
        ////                    if (AppParameters.NotificationConditionsList.TryGetValue((string)updatenotification.Property("id").Value, out JObject notification))
        ////                    {
        ////                        bool updateFile = false;
        ////                        updatenotification["LASTUPDATE_DATE"] = DateTime.Now;
        ////                        notification.Merge(updatenotification, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        ////                        updateFile = true;
        ////                        //update notification
        ////                        foreach (JObject item in AppParameters.NotificationList.Where(r => (string)r.Value["conditionId"] == (string)updatenotification["id"]).Select(y => y.Value))
        ////                        {
        ////                            if (!(bool)updatenotification["ACTIVE_CONDITION"] )
        ////                            {
        ////                                item["DELETE"] = true;
        ////                                item["UPDATE"] = true;
        ////                            }
        ////                            item.Merge(updatenotification, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });


        ////                        }
        ////                        //write to file for backup
        ////                        if (updateFile)
        ////                        {
        ////                            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Notification.json", JsonConvert.SerializeObject(AppParameters.NotificationConditionsList.Select(x => x.Value), Formatting.Indented));
        ////                            return AppParameters.NotificationConditionsList.Select(e => e.Value).ToList();
        ////                        }
        ////                        else
        ////                        {
        ////                            return new JObject(new JProperty("ERROR_MESSAGE", "No update made"));
        ////                        }
        ////                    }
        ////                    else
        ////                    {
        ////                        return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
        ////                }
        ////            }
        ////            else
        ////            {
        ////                return new JObject(new JProperty("ERROR_MESSAGE", "ID not provided"));
        ////            }
        ////        }
        ////        else
        ////        {
        ////            return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return new JObject(new JProperty("ERROR_MESSAGE", "Data Not loaded"));
        ////    }
        ////}

        ////internal IEnumerable<JToken> DeleteNotification_Conditions(string data)
        ////{
        ////    try
        ////    {
        ////        JObject Notification = JObject.Parse(data);
        ////        if (Notification.HasValues)
        ////        {
        ////            if (Notification.ContainsKey("id"))
        ////            {
        ////                if (AppParameters.NotificationConditionsList.TryRemove((string)Notification.Property("id").Value, out JObject outtemp))
        ////                {
        ////                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Notification.json", JsonConvert.SerializeObject(AppParameters.NotificationConditionsList.Select(x => x.Value), Formatting.Indented));

        ////                    foreach(JObject item in AppParameters.NotificationList.Where(r => (int)r.Value["id"] == (int)Notification["id"])
        ////                          .Select(y => y.Value))
        ////                          {
        ////                              item["DELETE"] = true;
        ////                              item["UPDATE"] = true;

        ////                          };
        ////                    return AppParameters.NotificationConditionsList.Select(e => e.Value).ToList();
        ////                }
        ////                else
        ////                {
        ////                    return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
        ////                }
        ////            }
        ////            else
        ////            {
        ////                return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
        ////            }
        ////        }
        ////        else
        ////        {
        ////            return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return new JObject(new JProperty("ERROR_MESSAGE", "Error Deleting data"));
        ////    }
        ////}

        //internal IEnumerable<JToken> EditTagInfo(string data)
        //{
        //    JObject item = new JObject();
        //    DateTime in_time = DateTime.Now;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(data))
        //        {
        //            item = JObject.Parse(data);
        //            if (item.HasValues)
        //            {
        //                if (item.ContainsKey("id"))
        //                {
        //                    string tag_id = (string)item.Property("id").Value;
        //                    if (AppParameters.Tag.ContainsKey(tag_id))
        //                    {
        //                        if (AppParameters.Tag.TryGetValue(tag_id, out JObject tag_item))
        //                        {
        //                            //tagInfo["properties"]["Tag_Update"] = false;
        //                            bool update = false;
        //                            foreach (dynamic kv in item.Children())
        //                            {
        //                                if (kv.Name != "id")
        //                                {
        //                                    if (((JObject)tag_item["properties"]).ContainsKey(kv.Name))
        //                                    {
        //                                        if (((JObject)tag_item["properties"]).Property(kv.Name).Value != kv.Value)
        //                                        {
        //                                            ((JObject)tag_item["properties"]).Property(kv.Name).Value = item.Property(kv.Name).Value;
        //                                            update = true;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            if (update)
        //                            {
        //                                ((JObject)tag_item["properties"]).Property("Tag_Update").Value = true;
        //                                new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Tag_Info.json", JsonConvert.SerializeObject(AppParameters.Tag.Select(x => x.Value).ToList(), Formatting.Indented));
        //                            }
        //                            return new JObject(new JProperty("MESSAGE_TYPE", "Tag has been update"));
        //                        }
        //                        else
        //                        {
        //                            return new JObject(new JProperty("ERROR_MESSAGE", "Tag ID not Found in List"));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return new JObject(new JProperty("ERROR_MESSAGE", "Tag ID not Found in List"));
        //                    }
        //                }
        //                else
        //                {
        //                    return new JObject(new JProperty("ERROR_MESSAGE", "Missing Tag ID"));
        //                }
        //            }
        //            else
        //            {
        //                return new JObject(new JProperty("ERROR_MESSAGE", "Data Value is Empty"));
        //            }
        //        }
        //        else
        //        {
        //            return new JObject(new JProperty("ERROR_MESSAGE", "Data Object was Empty"));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return new JObject(new JProperty("ERROR_MESSAGE", "Data Not loaded"));
        //    }
        //}

        private void UpdateZoneStatus(object state)
        {
            lock (updateZoneStatuslock)
            {
                if (!_updateZoneStatus)
                {
                    _updateZoneStatus = true;
                    foreach (var zoneitem in from GeoZone zoneitem in AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneUpdate && r.Value.Properties.ZoneType == "Area").Select(y => y.Value)
                                             where TryUpdateZoneStatus(zoneitem)
                                             select zoneitem)
                    {
                        BroadcastZoneStatus(zoneitem);
                    }

                    _updateZoneStatus = false;
                }
            }
        }

        private void BroadcastZoneStatus(GeoZone zoneitem)
        {
            Clients.All.updateZoneStatus(zoneitem);
        }

        private bool TryUpdateZoneStatus(GeoZone zoneitem)
        {
            try
            {
                zoneitem.Properties.ZoneUpdate = false;
                return true;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }
        ////private void UpdateSVTripsStatus(object state)
        ////{
        ////    lock (updateSVTripsStatuslock)
        ////    {
        ////        if (!_updateSVTripsStatus)
        ////        {
        ////            _updateSVTripsStatus = true;
        ////            foreach (var trip in from JObject trip in AppParameters.RouteTripsList.Where(r => (bool)r.Value["Trip_Update"]).Select(x => x.Value)
        ////                                 where TrySVTripStatus(trip)
        ////                                 select trip)
        ////            {
        ////                BroadcastSVTripsStatus(trip);
        ////            };

        ////            _updateSVTripsStatus = false;
        ////        }
        ////    }
        ////}

        ////private bool TrySVTripStatus(JObject trip)
        ////{
        ////    bool update = false;
        ////    string state = "ACTIVE";
        ////    try
        ////    {
        ////        trip["Trip_Update"] = false;
        ////        if (!trip.ContainsKey("unloadedContainers"))
        ////        {
        ////            trip["unloadedContainers"] = 0;
        ////        }
        ////        if (!trip.ContainsKey("containers"))
        ////        {
        ////            trip["containers"] = "";
        ////        }
        ////        //destSite
        ////        if (!trip.ContainsKey("destSite"))
        ////        {
        ////            trip["destSite"] = "";
        ////        }
        ////        if (!trip.ContainsKey("tripMin"))
        ////        {
        ////            trip["tripMin"] = 0;
        ////        }
        ////        if (!trip.ContainsKey("state"))
        ////        {
        ////            trip["state"] = state;
        ////        }
        ////        if (trip.ContainsKey("status"))
        ////        {
        ////            switch (trip["status"].ToString())
        ////            {
        ////                case "CANCELED":
        ////                    state = "CANCELED";
        ////                    break;
        ////                case "DEPARTED":
        ////                    state = "DEPARTED";
        ////                    break;
        ////                case "OMITTED":
        ////                    state = "OMITTED";
        ////                    break;
        ////                case "COMPLETE":
        ////                    state = "COMPLETE";
        ////                    break;
        ////                default:
        ////                    break;
        ////            }

        ////        }
        ////        if (trip.ContainsKey("legStatus"))
        ////        {
        ////            switch (trip["legStatus"].ToString())
        ////            {
        ////                case "CANCELED":
        ////                    state = "CANCELED";
        ////                    break;
        ////                case "DEPARTED":
        ////                    state = "DEPARTED";
        ////                    break;
        ////                case "OMITTED":
        ////                    state = "OMITTED";
        ////                    break;
        ////                case "COMPLETE":
        ////                    state = "COMPLETE";
        ////                    break;
        ////                default:
        ////                    break;
        ////            }
        ////        }

        ////        int tripInMin = AppParameters.Get_TripMin((JObject)trip["scheduledDtm"]);
        ////        if (tripInMin != (int)trip["tripMin"])
        ////        {
        ////            trip["tripMin"] = tripInMin;
        ////            update = true;
        ////        }
        ////        //check the trip state     
        ////        if (tripInMin <= -1 && state == "ACTIVE")
        ////        {
        ////            state = "LATE";
        ////            update = true;
        ////        }
        ////        if (tripInMin <= -30 && Regex.IsMatch(state, "(CANCELED|DEPARTED|OMITTED|COMPLETE)", RegexOptions.IgnoreCase) )
        ////        {
        ////            state = "REMOVE";
        ////            update = true;
        ////        }
        ////        if (tripInMin <= -1440 )
        ////        {
        ////            state = "REMOVE";
        ////            update = true;
        ////        }
        ////        if (update)
        ////        {
        ////            trip["notificationId"] = CheckNotification(trip["state"].ToString(), state, "routetrip", trip, trip["notificationId"].ToString());
        ////        }
        ////        if (string.IsNullOrEmpty(trip["state"].ToString()))
        ////        {
        ////            trip["state"] = trip.ContainsKey("status") ? trip["status"].ToString() : trip.ContainsKey("legStatus") ? trip["legStatus"].ToString() : state;
        ////        }
        ////        else
        ////        {
        ////            trip["state"] = state;
        ////        }
        ////        if (state == "REMOVE")
        ////        {
        ////            string routetripid = string.Concat(trip["routeTripId"].ToString() + trip["routeTripLegId"].ToString());
        ////            if (AppParameters.RouteTripsList.TryRemove(routetripid, out JObject r))
        ////            {
        ////                trip["state"] = state;
        ////                trip["notificationId"] = CheckNotification(trip["state"].ToString(), state, "routetrip", trip, trip["notificationId"].ToString());
        ////                update = true;
        ////            }

        ////        }
        ////        if (trip["state"].ToString() == state)
        ////        {
        ////            update = false;
        ////        }
        ////        return update;
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return update;
        ////    }
        ////}

        ////private string CheckNotification(string currentState, string NewState, string type, JObject trip, string noteifi_id)
        ////{
        ////    string noteification_id = noteifi_id;
        ////    try
        ////    {
        ////        if (currentState != NewState)
        ////        {
        ////            if (!string.IsNullOrEmpty(noteification_id) && AppParameters.NotificationList.ContainsKey(noteification_id))
        ////            {
        ////                if (AppParameters.NotificationList.TryGetValue(noteification_id, out JObject ojbMerge))
        ////                {
        ////                    if (!ojbMerge.ContainsKey("DELETE"))
        ////                    {
        ////                        ojbMerge["DELETE"] = true;
        ////                        ojbMerge["UPDATE"] = true;
        ////                        noteification_id = "";
        ////                    }
        ////                }
        ////            }
        ////            //new condition
        ////            foreach (JObject newCondition in AppParameters.NotificationConditionsList.Where(r => Regex.IsMatch(NewState, r.Value["CONDITIONS"].ToString(), RegexOptions.IgnoreCase)
        ////         && r.Value["TYPE"].ToString().ToLower() == type.ToLower()
        ////          && (bool)r.Value["ACTIVE_CONDITION"]).Select(x => x.Value))
        ////            {
        ////                noteification_id = (string)newCondition["id"] + (string)trip["id"];
        ////                if (!AppParameters.NotificationList.ContainsKey(noteification_id))
        ////                {
        ////                    JObject ojbMerge = (JObject)newCondition.DeepClone();
        ////                    ojbMerge.Merge(trip, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        ////                    ojbMerge["SHOWTOAST"] = true;
        ////                    ojbMerge["TAGID"] = (string)trip["id"];
        ////                    ojbMerge["conditionId"] = (string)newCondition["id"] ;
        ////                    ojbMerge["notificationId"] = (string)newCondition["id"] + (string)trip["id"];
        ////                    ojbMerge["UPDATE"] = true;
        ////                    AppParameters.NotificationList.TryAdd((string)newCondition["id"] + (string)trip["id"], ojbMerge);
        ////                }
        ////            }

        ////        }
        ////        return noteification_id;
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return noteification_id;
        ////    }
        ////}

        //private double GetTripMin(DateTime scheduledDtm)
        //{
        //    try
        //    {
        //        DateTime dtNow = DateTime.Now;
        //        if (AppParameters.TimeZoneConvert.TryGetValue((string)AppParameters.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
        //        {
        //            dtNow = TimeZoneInfo.ConvertTime(dtNow, TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId));
        //        }
        //        return Math.Round(scheduledDtm.Subtract(dtNow).TotalMinutes);
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return 0;
        //    }
        //}

        ////private void BroadcastSVTripsStatus(JObject trip)
        ////{
        ////    Clients.All.updateSVTripsStatus(trip);
        ////}

        //internal IEnumerable<JToken> GetCTSDetailsList(string route, string trip)
        //{
        //    try
        //    {
        //        if (AppParameters.AppSettings.ContainsKey("CTS_DETAILS_URL") && AppParameters.AppSettings.ContainsKey("CTS_API_KEY"))
        //        {
        //            if (!string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("CTS_DETAILS_URL").Value) && !string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("CTS_API_KEY").Value))
        //            {
        //                Uri parURL = new Uri((string)AppParameters.AppSettings.Property("CTS_DETAILS_URL").Value + (string)AppParameters.AppSettings.Property("CTS_API_KEY").Value + "&nass=" + (string)AppParameters.AppSettings.Property("FACILITY_NASS_CODE").Value + "&route=" + route + "&trip=" + trip);
        //                string CTS_Response = SendMessage.SV_Get(parURL.AbsoluteUri);
        //                if (!string.IsNullOrEmpty(CTS_Response))
        //                {
        //                    if (AppParameters.IsValidJson(CTS_Response))
        //                    {
        //                        JObject data = JObject.Parse(CTS_Response);
        //                        if (data.HasValues)
        //                        {
        //                            if (data.ContainsKey("Data"))
        //                            {
        //                                JToken cts_data = data.SelectToken("Data");
        //                                JObject cts_site = (JObject)data.SelectToken("Site");
        //                                if (cts_data.Children().Count() > 0)
        //                                {
        //                                    return cts_data.Children();
        //                                }
        //                                else
        //                                {
        //                                    return new JObject();
        //                                }
        //                            }
        //                            else
        //                            {
        //                                return new JObject();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            return new JObject();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return new JObject();
        //                    }
        //                }
        //                else
        //                {
        //                    return new JObject();
        //                }
        //            }
        //            else
        //            {
        //                return new JObject();
        //            }
        //        }
        //        else
        //        {
        //            return new JObject();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return new JObject();
        //    }
        //}

        ////internal IEnumerable<JObject> GetTripsList()
        ////{
        ////    try
        ////    {
        ////        return AppParameters.RouteTripsList.Where(x => !Regex.IsMatch(x.Value["state"].ToString(), "(CANCELED|DEPARTED|OMITTED|COMPLETE)", RegexOptions.IgnoreCase)).Select(y => y.Value).ToList();
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return null;
        ////    }
        ////}
        ////internal IEnumerable<JObject> GetRouteTripsInfo(string id)
        ////{
        ////    try
        ////    {
        ////        return AppParameters.RouteTripsList.Where(r => r.Key == id).Select(y => y.Value).ToList();
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return null;
        ////    }
        ////}
        ////internal IEnumerable<Container> GetContainerInfo(string id)
        ////{
        ////    try
        ////    {
        ////        return AppParameters.Containers.Where(r => r.Key == id).Select(y => y.Value).ToList();
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return null;
        ////    }
        ////}
        ////internal IEnumerable<JToken> GetCTSList(string type)
        ////{
        ////    try
        ////    {
        ////        if (type.StartsWith("in"))
        ////        {
        ////            return AppParameters.CTS_Inbound_Schedualed.Select(x => x.Value).ToList();
        ////        }
        ////        else if (type.StartsWith("dockdeparted"))
        ////        {
        ////            return AppParameters.CTS_DockDeparted.Select(x => x.Value).ToList();
        ////        }
        ////        else if (type.StartsWith("local"))
        ////        {
        ////            return AppParameters.CTS_LocalDockDeparted.Select(x => x.Value).ToList();
        ////        }
        ////        else if (type.StartsWith("out"))
        ////        {
        ////            return AppParameters.CTS_Outbound_Schedualed.Select(x => x.Value).ToList();
        ////        }
        ////        else
        ////        {
        ////            return new JObject();
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return new JObject();
        ////    }
        ////}

        private void UpdateQSM(object state)
        {
            lock (updateQSMlock)
            {
                if (!_updatingQSMStatus)
                {
                    _updatingQSMStatus = true;
                    foreach (var QSMitem in AppParameters.ConnectionList.Values)
                    {
                        if (TryUpdateQSMStaus(QSMitem))
                        {
                            BroadcastQSMUpdate(QSMitem);
                        }
                    };
                    _updatingQSMStatus = false;
                }
            }
        }
       

        private bool TryUpdateQSMStaus(Connection con)
        {
            try
            {
                if (con.UpdateStatus)
                {
                    con.UpdateStatus = false;
                    return true;
                };
                return false;

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }
        private void BroadcastQSMUpdate(Connection qSMitem)
        {
            Clients.All.UpdateQSMStatus(qSMitem);
        }

        internal IEnumerable<GeoZone> GetMachineZonesList()
        {
            try
            {
                return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Machine").Select(x => x.Value).ToList();

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<GeoZone> GetAGVLocationZonesList()
        {
            try
            {
                return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "AGVLocation").Select(x => x.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<GeoZone> GetViewPortsZonesList()
        {
            try
            {
                return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "ViewPorts").Select(x => x.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<GeoMarker> GetLocatorsList()
        {
            try
            {
                return AppParameters.TagsList.Where(x => x.Value.Properties.TagType == "Locator").Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<GeoZone> GetDockDoorZonesList()
        {
            try
            {
                return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "DockDoor").Select(x => x.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        ////internal IEnumerable<JToken> GetViewConfigList()
        ////{
        ////    try
        ////    {
        ////        return new JObject();
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        new ErrorLogger().ExceptionLog(e);
        ////        return new JObject();
        ////    }
        ////}

        //private void UpdateAGVLocationStatus(object state)
        //{
        //    lock (updateAGVLocationStatuslock)
        //    {
        //        if (!_updateAGVLocationStatus)
        //        {
        //            _updateAGVLocationStatus = true;
        //            foreach (JObject AGV_Location in AppParameters.ZonesList.Where(u => u.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" && (bool)u.Value["properties"]["Zone_Update"] == true).Select(x => x.Value))
        //            {
        //                if (TryUpdateAGVLocationStatus(AGV_Location))
        //                {
        //                    BroadcastAGVLocationStatus(AGV_Location);
        //                }
        //            }
        //            _updateAGVLocationStatus = false;
        //        }
        //    }
        //}

        //private bool TryUpdateAGVLocationStatus(JObject AGV_Location)
        //{
        //    try
        //    {
        //        AGV_Location["properties"]["Zone_Update"] = false;
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return false;
        //    }
        //}

        //private void BroadcastAGVLocationStatus(JObject machine)
        //{
        //    Clients.All.updateAGVLocationStatus(machine);
        //}

        //private void UpdateMachineStatus(object state)
        //{
        //    lock (updateMachineStatuslock)
        //    {
        //        if (!_updateMachineStatus)
        //        {
        //            _updateMachineStatus = true;
        //            foreach (JObject Machine in AppParameters.ZonesList.Where(u => (bool)u.Value["properties"]["Zone_Update"]
        //            && u.Value["properties"]["Zone_Type"].ToString() == "Machine").Select(x => x.Value))
        //            {
        //                if (TryUpdateMachineStatus(Machine))
        //                {
        //                    BroadcastMachineStatus(Machine);
        //                }
        //            };

        //            _updateMachineStatus = false;
        //        }
        //    }
        //}

        //private bool TryUpdateMachineStatus(JObject machine)
        //{
        //    try
        //    {

        //        machine["properties"]["Zone_Update"] = false;
        //        return true;

        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return false;
        //    }
        //}

        //private void BroadcastMachineStatus(JObject machine)
        //{
        //    Clients.All.updateMachineStatus(machine);
        //}

        internal IEnumerable<BackgroundImage> GetIndoorMap()
        {
            try
            {
                return AppParameters.IndoorMap.Values;   
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<Cameras> GetCameraList()
        {
            try
            {
                return AppParameters.CameraInfoList.Select(x => x.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<GeoZone> GetZonesList()
        {
            try
            {

                return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType.ToString() == "Area").Select(x => x.Value).ToList();

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        //internal IEnumerable<JToken> GetPersonTagsList()
        //{
        //    try
        //    {
        //        if (AppParameters.Tag.Count() > 0)
        //        {
        //            return AppParameters.Tag.Where(x => x.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")).Select(y => y.Value).ToList();
        //        }
        //        else
        //        {
        //            return new JObject();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return new JObject();
        //    }
        //}

        //internal IEnumerable<JToken> GetUndetectedTagsList()
        //{
        //    try
        //    {
        //        if (AppParameters.Tag.Count() > 0)
        //        {
        //            return AppParameters.Tag.Where(x => x.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")
        //            && (bool)x.Value["properties"]["isWearingTag"] == false && !(bool)x.Value["properties"]["isLdcAlert"] == true
        //            && ((JObject)x.Value["properties"]["tacs"]).HasValues).Select(y => y.Value).ToList();
        //        }
        //        else
        //        {
        //            return new JObject();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return new JObject();
        //    }
        //}
        //internal IEnumerable<JToken> GetLDCAlertTagsList()
        //{
        //    try
        //    {
        //        if (AppParameters.Tag.Count() > 0)
        //        {
        //            return AppParameters.Tag.Where(x => x.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")
        //            && (bool)x.Value["properties"]["isLdcAlert"] == true
        //            && ((JObject)x.Value["properties"]["tacs"]).HasValues).Select(y => y.Value).ToList();
        //        }
        //        else
        //        {
        //            return new JObject();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return new JObject();
        //    }
        //}

        //internal IEnumerable<Container> GetContainer(string data, string direction, string route, string trip)
        //{
        //    try
        //    {
        //        if (AppParameters.Containers.Count() > 0)
        //        {
        //            if (direction == "I")
        //            {
        //                return AppParameters.Containers.Where(r =>
        //                 r.Value.Iroute == route &&
        //                r.Value.Itrip == trip).Select(y => y.Value).ToList();

        //                //return AppParameters.Containers.Where(r => r.Value.ContainsKey("Iroute") &&
        //                //r.Value.Property("dest").Value.ToString().StartsWith(data)  &&
        //                //(string)r.Value.Property("Iroute").Value == route &&
        //                //(string)r.Value.Property("Itrip").Value == trip).Select(y => y.Value).ToList();
        //            }
        //            else
        //            {
        //                return AppParameters.Containers.Where(r => r.Value.Dest == data).Select(y => y.Value).ToList();
        //            }
        //        }
        //        return null; 
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}

        //internal IEnumerable<JToken> GetMarkerList()
        //{
        //    try
        //    {
        //        if (AppParameters.Tag.Count() > 0)
        //        {
        //            return AppParameters.Tag.Where(x => x.Value["properties"]["Tag_Type"].ToString().EndsWith("Vehicle")).Select(y => y.Value).ToList();
        //        }
        //        else
        //        {
        //            return new JObject();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return new JObject();
        //    }
        //}

        //private void BroadcastVehicleTagStatus(JObject tag)
        //{
        //    Clients.All.updateVehicleTagStatus(tag);
        //}

        //private bool TryUpdateVehicleTagStatus(JObject tag, double tagVisibleRange)
        //{
        //    try
        //    {
        //        bool returnresult = false;
        //        System.TimeSpan tagdiffResult = ((DateTime)((JObject)tag["properties"]).Property("Tag_TS").Value).ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("positionTS").Value).ToUniversalTime());
        //        tag["properties"]["tagVisibleMils"] = tagdiffResult.TotalMilliseconds;
        //        if ((bool)tag["properties"]["Tag_Update"] == true)
        //        {
        //            ////vehicle notification
        //            //if (((JObject)tag["properties"]).Property("Tag_Type").Value.ToString().ToLower() == "Autonomous Vehicle".ToLower())
        //            //{
        //            //  if (((JObject)tag["properties"]).ContainsKey("vehicleTime"))
        //            //    {
        //            //        if (!string.IsNullOrEmpty((string)((JObject)tag["properties"]).Property("vehicleTime").Value))
        //            //        {
        //            //            //time cal lostcomm
        //            //            System.TimeSpan diffResult = DateTime.Now.ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime());

        //            //            if (AppParameters.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
        //            //           && r.Value.Property("TYPE").Value.ToString().ToUpper() == "vehicle".ToUpper()
        //            //            && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList().Count() > 0)
        //            //            {
        //            //                foreach (var conditionitem in AppParameters.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
        //            //                    && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
        //            //                     && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList())
        //            //                {
        //            //                    double warningmil = TimeSpan.FromMinutes((int)conditionitem.Property("WARNING").Value).TotalMilliseconds;
        //            //                    if (diffResult.TotalMilliseconds > warningmil)
        //            //                    {
        //            //                        if (!AppParameters.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
        //            //                        {
        //            //                            JObject ojbMerge = (JObject)conditionitem.DeepClone();
        //            //                            ojbMerge.Add(new JProperty("VEHICLETIME", ((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime()));
        //            //                            ojbMerge.Add(new JProperty("VEHICLENAME", (string)((JObject)tag["properties"]).Property("name").Value));
        //            //                            ojbMerge.Add(new JProperty("TAGID", (string)((JObject)tag["properties"]).Property("id").Value));
        //            //                            ojbMerge.Add(new JProperty("notificationId", (string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value));
        //            //                            ojbMerge.Add(new JProperty("UPDATE", true));
        //            //                            if (AppParameters.Notification.TryAdd((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, ojbMerge))
        //            //                            {
        //            //                            }
        //            //                        }
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        if (AppParameters.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
        //            //                        {
        //            //                            if (AppParameters.Notification.TryGetValue((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, out JObject ojbMerge))
        //            //                            {
        //            //                                ojbMerge.Add(new JProperty("DELETE", true));
        //            //                            }

        //            //                        }
        //            //                    }
        //            //                }
        //            //            }

        //            //        }
        //            //    }
        //            //}
        //            if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
        //            {
        //                if ((bool)tag["properties"]["tagVisible"] != false)
        //                {
        //                    tag["properties"]["tagVisible"] = false;
        //                }
        //            }
        //            else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
        //            {
        //                if ((bool)tag["properties"]["tagVisible"] != true)
        //                {
        //                    tag["properties"]["tagVisible"] = true;
        //                }
        //            }
        //            tag["properties"]["Tag_Update"] = false;
        //            return true;
        //        }
        //        else
        //        {
        //            ////vehicle notification
        //            //if (((JObject)tag["properties"]).Property("Tag_Type").Value.ToString().ToLower() == "Autonomous Vehicle".ToLower())
        //            //{
        //            //  if (((JObject)tag["properties"]).ContainsKey("vehicleTime"))
        //            //    {
        //            //        if (!string.IsNullOrEmpty((string)((JObject)tag["properties"]).Property("vehicleTime").Value))
        //            //        {
        //            //            //time cal lostcomm
        //            //            System.TimeSpan diffResult = DateTime.Now.ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime());

        //            //            if (AppParameters.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
        //            //           && r.Value.Property("TYPE").Value.ToString().ToUpper() == "vehicle".ToUpper()
        //            //            && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList().Count() > 0)
        //            //            {
        //            //                foreach (var conditionitem in AppParameters.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
        //            //                    && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
        //            //                     && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList())
        //            //                {
        //            //                    double warningmil = TimeSpan.FromMinutes((int)conditionitem.Property("WARNING").Value).TotalMilliseconds;
        //            //                    if (diffResult.TotalMilliseconds > warningmil)
        //            //                    {
        //            //                        if (!AppParameters.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
        //            //                        {
        //            //                            JObject ojbMerge = (JObject)conditionitem.DeepClone();
        //            //                            ojbMerge.Add(new JProperty("VEHICLETIME", ((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime()));
        //            //                            ojbMerge.Add(new JProperty("VEHICLENAME", (string)((JObject)tag["properties"]).Property("name").Value));
        //            //                            ojbMerge.Add(new JProperty("TAGID", (string)((JObject)tag["properties"]).Property("id").Value));
        //            //                            ojbMerge.Add(new JProperty("notificationId", (string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value));
        //            //                            ojbMerge.Add(new JProperty("UPDATE", true));
        //            //                            if (AppParameters.Notification.TryAdd((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, ojbMerge))
        //            //                            {
        //            //                            }
        //            //                        }
        //            //                    }
        //            //                    else
        //            //                    {
        //            //                        if (AppParameters.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
        //            //                        {
        //            //                            if (AppParameters.Notification.TryGetValue((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, out JObject ojbMerge))
        //            //                            {
        //            //                                ojbMerge.Add(new JProperty("DELETE", true));
        //            //                            }

        //            //                        }
        //            //                    }
        //            //                }
        //            //            }

        //            //        }
        //            //    }
        //            //}

        //            if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
        //            {
        //                if ((bool)tag["properties"]["tagVisible"] != false)
        //                {
        //                    tag["properties"]["tagVisible"] = false;
        //                    returnresult = true;
        //                }
        //            }
        //            else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
        //            {
        //                if ((bool)tag["properties"]["tagVisible"] != true)
        //                {
        //                    tag["properties"]["tagVisible"] = true;
        //                }
        //            }
        //            tag["properties"]["Tag_Update"] = false;
        //            return returnresult;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return false;
        //    }
        //}



        //private void UpdateDockDoorStatus(object state)
        //{
        //    lock (updateDockDoorStatuslock)
        //    {
        //        if (!_updateDockDoorStatus)
        //        {
        //            _updateDockDoorStatus = true;

        //            foreach (JObject DockDoor in AppParameters.ZonesList.Where(u => (bool)u.Value["properties"]["Zone_Update"] 
        //            && u.Value["properties"]["Zone_Type"].ToString() == "DockDoor").Select(x => x.Value))
        //            {
        //                if (TryUpdateDockDoorStatus(DockDoor))
        //                {
        //                    BroadcastDockDoorStatus(DockDoor);
        //                }
        //            };

        //            _updateDockDoorStatus = false;
        //        }
        //    }
        //}

        //private void BroadcastDockDoorStatus(JObject dockDoor)
        //{
        //    Clients.All.updateDockDoorStatus(dockDoor);
        //}

        //private bool TryUpdateDockDoorStatus(JObject dockDoor)
        //{
        //    try
        //    {

        //            // Make call to update routetripData.alertState
        //            //dockDoor["properties"]["alertState"] = GetDoorAlertState(dockDoor);
        //            //if (dockDoor["properties"]["alertState"].ToString().ToUpper() != "OKAY")
        //            //{
        //            //    dockDoor["properties"]["alertMessage"] = dockDoor["properties"]["alertState"].ToString().ToUpper();
        //            //}
        //         dockDoor["properties"]["Zone_Update"] = false;
        //        return true;
        //        //string DockDoor_id = (string)dockDoor["properties"]["id"];

        //        //if (AppParameters.DockDoorZones.ContainsKey(DockDoor_id))
        //        //{
        //        //    if (AppParameters.DockDoorZones.TryGetValue(DockDoor_id, out JObject dockDoorInfo))
        //        //    {
        //        //        dockDoorInfo["properties"]["Zone_Update"] = false;
        //        //        return true;
        //        //    }
        //        //    else
        //        //    {
        //        //        return false;
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    return false;
        //        //}
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return false;
        //    }
        //}

        //private void UpdateVehicleTagStatus(object state)
        //{
        //    lock (updateTagStatuslock)
        //    {
        //        if (!_updateTagStatus)
        //        {
        //            _updateTagStatus = true;
        //            double tagVisibleRange = AppParameters.AppSettings.ContainsKey("POSITION_MAX_AGE") ? !string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value) ? (long)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value : 10000 : 10000;

        //            foreach (JObject Tag in AppParameters.Tag.Where(u => (bool)u.Value["properties"]["Tag_Update"] == true && u.Value["properties"]["Tag_Type"].ToString().EndsWith("Vehicle")).Select(x => x.Value))
        //            {
        //                if (TryUpdateVehicleTagStatus(Tag, tagVisibleRange))
        //                {
        //                    BroadcastVehicleTagStatus(Tag);
        //                }
        //            }

        //            _updateTagStatus = false;
        //        }
        //    }
        //}

        //private void UpdatePersonTagStatus(object state)
        //{
        //    lock (updatePersonTagStatuslock)
        //    {
        //        if (!_updatePersonTagStatus)
        //        {
        //            _updatePersonTagStatus = true;
        //            //  var watch = new System.Diagnostics.Stopwatch();
        //            // watch.Start();
        //            double tagVisibleRange = AppParameters.AppSettings.ContainsKey("POSITION_MAX_AGE") ? !string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value) ? (long)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value : 10000 : 10000;

        //            foreach (JObject Tag in AppParameters.Tag.Where(u => (bool)u.Value["properties"]["Tag_Update"]
        //             && u.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")).Select(x => x.Value))
        //            {
        //                if (TryUpdatePersonTagStatus(Tag, tagVisibleRange))
        //                {
        //                    BroadcastPersonTagStatus(Tag);
        //                }
        //            }
        //            // watch.Stop();
        //            // new ErrorLogger().CustomLog(string.Concat("Total Execution for all tags ", "Time: ", watch.ElapsedMilliseconds, " ms"), string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "TagProcesslogs"));
        //            _updatePersonTagStatus = false;
        //        }
        //    }
        //}

        //private void BroadcastPersonTagStatus(JObject tag)
        //{
        //    Clients.All.updatePersonTagStatus(tag);
        //}

        //private bool TryUpdatePersonTagStatus(JObject tag, double tagVisibleRange)
        //{
        //    try
        //    {
        //        bool returnresult = false;
        //        System.TimeSpan tagdiffResult = ((DateTime)((JObject)tag["properties"]).Property("Tag_TS").Value).ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("positionTS").Value).ToUniversalTime());
        //        tag["properties"]["tagVisibleMils"] = tagdiffResult.TotalMilliseconds;
        //        if ((bool)tag["properties"]["Tag_Update"] == true)
        //        {
        //            if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
        //            {
        //                if ((bool)tag["properties"]["tagVisible"] != false)
        //                {
        //                    tag["properties"]["tagVisible"] = false;
        //                }
        //            }
        //            else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
        //            {
        //                if ((bool)tag["properties"]["tagVisible"] != true)
        //                {
        //                    tag["properties"]["tagVisible"] = true;
        //                }
        //            }
        //            if (string.IsNullOrEmpty((string)tag["properties"]["craftName"]))
        //            {
        //                if (!string.IsNullOrEmpty(tag["properties"]["name"].ToString()))
        //                {
        //                    int equalsIndex = tag["properties"]["name"].ToString().IndexOf("_", 1);
        //                    if ((equalsIndex > -1))
        //                    {
        //                        string[] namesplit = tag["properties"]["name"].ToString().Split('_');
        //                        if (namesplit.Length > 1)
        //                        {
        //                            tag["properties"]["craftName"] = namesplit[0];
        //                            tag["properties"]["badgeId"] = namesplit[1];
        //                        }
        //                    }
        //                }
        //            }
        //            tag["properties"]["Tag_Update"] = false;
        //            return true;
        //        }
        //        else
        //        {
        //            if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
        //            {
        //                if ((bool)tag["properties"]["tagVisible"] != false)
        //                {
        //                    tag["properties"]["tagVisible"] = false;
        //                    returnresult = true;
        //                }
        //            }
        //            else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
        //            {
        //                if ((bool)tag["properties"]["tagVisible"] != true)
        //                {
        //                    tag["properties"]["tagVisible"] = true;
        //                }
        //            }
        //            return returnresult;
        //        }

        //        //string tag_id = (string)tag["properties"]["id"];

        //        //if (AppParameters.Tag.ContainsKey(tag_id))
        //        //{
        //        //    if (AppParameters.Tag.TryGetValue(tag_id, out JObject tagInfo))
        //        //    {
        //        //        tagInfo["properties"]["Tag_Update"] = false;
        //        //        return true;
        //        //    }
        //        //    else
        //        //    {
        //        //        return false;
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    return false;
        //        //}
        //        //string tag_id = (string)tag["properties"]["id"];
        //        //if (AppParameters.Tag.ContainsKey(tag_id))
        //        //{
        //        //    if (AppParameters.Tag.TryGetValue(tag_id, out JObject tagInfo))
        //        //    {
        //        //        double tagVisibleRange = AppParameters.AppSettings.ContainsKey("POSITION_MAX_AGE") ? !string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value) ? (long)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value : 10000 : 10000;

        //        //        System.TimeSpan tagdiffResult = DateTime.Now.ToUniversalTime().Subtract(((DateTime)((JObject)tagInfo["properties"]).Property("positionTS").Value).ToUniversalTime());
        //        //        if ((bool)((JObject)tagInfo["properties"]).Property("Tag_Update").Value)
        //        //        {
        //        //            if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
        //        //            {
        //        //                if ((bool)((JObject)tagInfo["properties"]).Property("tagVisible").Value)
        //        //                {
        //        //                    ((JObject)tagInfo["properties"]).Property("Tag_Update").Value = false;
        //        //                    ((JObject)tagInfo["properties"]).Property("tagVisible").Value = false;
        //        //                    return true;
        //        //                }
        //        //            }
        //        //                ((JObject)tagInfo["properties"]).Property("Tag_Update").Value = false;
        //        //            return true;
        //        //        }
        //        //        else
        //        //        {
        //        //            if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
        //        //            {
        //        //                if ((bool)((JObject)tagInfo["properties"]).Property("tagVisible").Value)
        //        //                {
        //        //                    ((JObject)tagInfo["properties"]).Property("Tag_Update").Value = false;
        //        //                    ((JObject)tagInfo["properties"]).Property("tagVisible").Value = false;
        //        //                    return true;
        //        //                }
        //        //            }
        //        //            return false;
        //        //        }

        //        //    }
        //        //    else
        //        //    {
        //        //        return false;
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    return false;
        //        //}
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return false;
        //    }
        //}





        internal IEnumerable<Connection> GetAPIList(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return AppParameters.ConnectionList.Select(x => x.Value);
                }
                else
                {
                    return AppParameters.ConnectionList.Where(r => r.Key == id).Select(x => x.Value);
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<Connection> AddAPI(string data)
        {
            string id = string.Empty;
            try
            {
                bool fileUpdate = false;
                if (!string.IsNullOrEmpty(data))
                {
                    Connection newConndata = JsonConvert.DeserializeObject<Connection>(data);
                    newConndata.CreatedDate = DateTime.Now;
                    id = newConndata.Id;
                    if (AppParameters.ConnectionList.TryAdd(newConndata.Id, newConndata))
                    {
                        AppParameters.RunningConnection.Add(newConndata);
                        fileUpdate = true;
                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Connection.json", JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
                }
                return AppParameters.ConnectionList.Where(w => w.Key == id).Select(s => s.Value);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<Connection> EditAPI(string data)
        {
            string id = string.Empty;
            try
            {
                bool fileUpdate = false;
                if (!string.IsNullOrEmpty(data))
                {
                    Connection updateConndata = JsonConvert.DeserializeObject<Connection>(data);
                    id = updateConndata.Id;

                    if (AppParameters.ConnectionList.ContainsKey(updateConndata.Id))
                    {
                        AppParameters.ConnectionList.AddOrUpdate(updateConndata.Id, updateConndata, (key, oldConndata) =>
                         {
                             fileUpdate = true;
                             updateConndata.CreatedByUsername = oldConndata.CreatedByUsername;
                             updateConndata.CreatedDate = oldConndata.CreatedDate;
                             updateConndata.ApiConnected = oldConndata.ApiConnected;
                            
                             updateConndata.UpdateStatus = true;
                             foreach (Api_Connection Connection_item in AppParameters.RunningConnection.Connection)
                             {
                                 if (Connection_item.ID == updateConndata.Id)
                                 {
                                     if (!updateConndata.ActiveConnection)
                                     {
                                         if (updateConndata.UdpConnection)
                                         {
                                             Connection_item._StopUDPListener();
                                         }
                                         else
                                         {
                                             Connection_item.ConstantRefresh = false;
                                             Connection_item.Stop();
                                         }

                                     }
                                     else if (updateConndata.ActiveConnection)
                                     {
                                         if (updateConndata.UdpConnection)
                                         {
                                             Connection_item._StartUDPListener();
                                         }
                                         else
                                         {
                                             Connection_item.ConstantRefresh = true;
                                             Connection_item._ThreadDownload();
                                             Connection_item._ThreadRefresh();
                                         }
                                     }

                                     Connection_item.ConnectionInfo = updateConndata;

                                 }
                             }
                             return updateConndata;
                         });
                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Connection.json", JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
                }
                return AppParameters.ConnectionList.Where(w => w.Key == id).Select(s => s.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<Connection> RemoveAPI(string data)
        {
            string id = string.Empty;
            try
            {
                JObject conn = JObject.Parse(data);
                if (conn.HasValues)
                {
                    if (conn.ContainsKey("Id"))
                    {
                        id = (string)conn["Id"];
                        foreach (Api_Connection Connection_item in AppParameters.RunningConnection.Connection)
                        {
                            if (Connection_item.ID == id)
                            {
                                if (AppParameters.ConnectionList.TryRemove(id, out Connection outtemp))
                                {
                                    if (Connection_item.ConnectionInfo.UdpConnection)
                                    {
                                        Connection_item.UDPDelete();
                                        AppParameters.RunningConnection.Connection.Remove(Connection_item);
                                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Connection.json", JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented));
                                        return AppParameters.ConnectionList.Select(e => e.Value).ToList();
                                    }
                                    else
                                    {
                                        Connection_item.Stop_Delete();
                                        AppParameters.RunningConnection.Connection.Remove(Connection_item);
                                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Connection.json", JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented));
                                        return AppParameters.ConnectionList.Select(e => e.Value).ToList();
                                    }
                                }

                            }
                        }
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        //internal IEnumerable<JToken> EditZone(string data)
        //{
        //    string id = string.Empty;

        //    try
        //    {
        //        bool fileUpdate = false;
        //        bool updateZone = false;
        //        if (!string.IsNullOrEmpty(data))
        //        {
        //            if (AppParameters.IsValidJson(data))
        //            {
        //                JObject objectdata = JObject.Parse(data);
        //                if (objectdata.HasValues)
        //                {
        //                    if (objectdata.ContainsKey("id"))
        //                    {
        //                        id = objectdata["id"].ToString();
        //                        objectdata["Zone_Update"] = true;
        //                        if (AppParameters.ZoneInfo.ContainsKey(id))
        //                        {
        //                            if (AppParameters.ZoneInfo.TryGetValue(id, out JObject zoneinfodata))
        //                            {
        //                                (zoneinfodata).Merge(objectdata, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        //                                zoneinfodata["name"] = zoneinfodata["MPE_Type"] + "-" + zoneinfodata["MPE_Number"].ToString().PadLeft(3, '0');
        //                                updateZone = true;
        //                                fileUpdate = true;
        //                            } 
        //                        }
        //                        else
        //                        {
        //                            objectdata["name"] = objectdata["MPE_Type"] + "-" + objectdata["MPE_Number"].ToString().PadLeft(3, '0');
        //                            if (AppParameters.ZoneInfo.TryAdd(id, objectdata))
        //                            {
        //                                updateZone = true;
        //                                fileUpdate = true;
        //                            } 
        //                        }
        //                        if (updateZone)
        //                        {
        //                            if (AppParameters.ZoneInfo.TryGetValue(id, out JObject zoneinfodata))
        //                            {
        //                                if (AppParameters.ZonesList.ContainsKey(id))
        //                                {
        //                                    if (AppParameters.ZonesList.TryGetValue(id, out JObject oldobjectdata))
        //                                    {
        //                                        ((JObject)oldobjectdata["properties"]).Merge(zoneinfodata, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        if (fileUpdate)
        //        {
        //            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Zones.json", JsonConvert.SerializeObject(AppParameters.ZoneInfo.Select(x => x.Value).ToList(), Formatting.Indented));
        //        }
        //        return AppParameters.ZonesList.Where(w => w.Key == id).Select(s => s.Value).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return AppParameters.ZonesList.Select(s => s.Value).ToList();
        //    }
        //}
        //internal IEnumerable<JToken> GetAppSettingdata()
        //{
        //    try
        //    {
        //        JToken tempsetting = AppParameters.AppSettings.DeepClone();
        //        foreach (dynamic item in AppParameters.AppSettings)
        //        {
        //            if (item.Key.ToString().StartsWith("ORACONN") && !string.IsNullOrEmpty(item.Value.ToString()))
        //            {
        //                tempsetting[item.Key] = AppParameters.Decrypt(item.Value.ToString());
        //            }
        //        }
        //        return tempsetting;
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return new JObject();
        //    }
        //}

        //internal IEnumerable<JToken> EditAppSettingdata(string data)
        //{
        //    try
        //    {
        //        bool fileUpdate = false;
        //        if (!string.IsNullOrEmpty(data))
        //        {
        //            dynamic objdict = JToken.Parse(data);
        //            foreach (dynamic item in AppParameters.AppSettings)
        //            {
        //                foreach (var kv in objdict)
        //                {
        //                    if (kv.Name == item.Key)
        //                    {
        //                        if (kv.Value != item.Value)
        //                        {
        //                            fileUpdate = true;

        //                            if (kv.Name.StartsWith("ORACONN"))
        //                            {
        //                                AppParameters.AppSettings[item.Key] = AppParameters.Encrypt(kv.Value.ToString());
        //                            }
        //                            if (kv.Name == "FACILITY_NASS_CODE")
        //                            {
        //                                if (GetData.Get_Site_Info((string)kv.Value, out JObject SiteInfo))
        //                                {
        //                                    if (SiteInfo.HasValues)
        //                                    {
        //                                        AppParameters.AppSettings[kv.Name] = kv.Value.ToString();
        //                                        AppParameters.AppSettings["FACILITY_NAME"] = SiteInfo.ContainsKey("displayName") ? SiteInfo["displayName"] : "";
        //                                        AppParameters.AppSettings["FACILITY_ID"] = SiteInfo.ContainsKey("fdbId") ? SiteInfo["fdbId"] : "";
        //                                        AppParameters.AppSettings["FACILITY_ZIP"] = SiteInfo.ContainsKey("zipCode") ? SiteInfo["zipCode"] : "";
        //                                        AppParameters.AppSettings["FACILITY_LKEY"] = SiteInfo.ContainsKey("localeKey") ? SiteInfo["localeKey"] : "";
        //                                        Task.Run(() => AppParameters.LoglocationSetup());
        //                                    }
        //                                }
        //                            }
        //                            else if (kv.Name == "LOG_LOCATION")
        //                            {
        //                                if (!string.IsNullOrEmpty(kv.Value.ToString()))
        //                                {
        //                                    AppParameters.AppSettings[item.Key] = kv.Value.ToString();
        //                                    Task.Run(() => AppParameters.LoglocationSetup());
        //                                    //AppParameters.Logdirpath = new DirectoryInfo(kv.Value.ToString());
        //                                    //new Directory_Check().DirPath(AppParameters.Logdirpath);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                AppParameters.AppSettings[item.Key] = kv.Value.ToString();
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        if (fileUpdate)
        //        {
        //            new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "AppSettings.json", JsonConvert.SerializeObject(AppParameters.AppSettings, Formatting.Indented));
        //            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "AppSettings.json", JsonConvert.SerializeObject(AppParameters.AppSettings, Formatting.Indented));
        //        }
        //        return AppParameters.AppSettings;
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return AppParameters.AppSettings;
        //    }
        //}

        internal void Removeuser(string connectionId)
        {
            try
            {
                AppParameters._connections.Remove(connectionId, connectionId);
                string data = string.Concat("Client closed the connection. | Connection ID: : " + connectionId);
                new ErrorLogger().CustomLog(data, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));

                //remove users 
                foreach (string user in AppParameters.Users.Where(r => r.Value.LoginDate.Subtract(DateTime.Now).TotalDays > 2).Select(y => y.Key))
                {
                    if (!AppParameters.Users.TryRemove(user, out ADUser ur))
                    {
                        new ErrorLogger().CustomLog("Unable to remove User" + ur.UserId, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                    }
                    
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal void Adduser(HubCallerContext Context)
        {
            try
            {
                if (Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out object Ipaddress))
                {
                    bool firstTimeLogin = true;
                    ADUser newuser = new ADUser
                    {
                        UserId = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim(),
                        NASSCode = AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(),
                        FDBID = AppParameters.AppSettings["FACILITY_ID"].ToString(),
                        FacilityName = AppParameters.AppSettings["FACILITY_NAME"].ToString(),
                        FacilityTimeZone = AppParameters.AppSettings["FACILITY_TIMEZONE"].ToString(),
                        AppType = AppParameters.AppSettings["APPLICATION_NAME"].ToString(),
                        Domain = !string.IsNullOrEmpty(Context.User.Identity.Name) ? Context.User.Identity.Name.Split('\\')[0].ToLower() : "",
                        SessionID = Context.ConnectionId,
                        ConnectionId = Context.ConnectionId,
                        LoginDate = DateTime.Now,
                        Environment = AppParameters.ApplicationEnvironment,
                        IsAuthenticated = Context.User.Identity.IsAuthenticated,
                        SoftwareVersion = AppParameters.VersionInfo,
                        BrowserType = HttpContext.Current.Request.Browser.Type,
                        BrowserName = HttpContext.Current.Request.Browser.Browser,
                        BrowserVersion = HttpContext.Current.Request.Browser.Version,
                        Role = GetUserRole(GetGroupNames(((WindowsIdentity)Context.User.Identity).Groups)),
                        IpAddress = Ipaddress.ToString().StartsWith("::") ? "127.0.0.1" : Ipaddress.ToString(),
                        ServerIpAddress = AppParameters.ServerIpAddress.ToString()
                    };
                    new FindACEUser().User(newuser, out newuser);
                    AppParameters.Users.AddOrUpdate(newuser.UserId, newuser,
                        (key, old_user) =>
                        {
                            //log out user 
                            if (!string.IsNullOrEmpty(old_user.ConnectionId))
                            {
                                Task.Run(() => new User_Log().LogoutUser(old_user));
                            }
                            //log of user logging in.
                            Task.Run(() => new User_Log().LoginUser(newuser));
                            string data = string.Concat("Client has Connected | User Name:", newuser.UserId, "(", newuser.FirstName, " ", newuser.SurName, ")", " | Connection ID: ", newuser.ConnectionId);
                            new ErrorLogger().CustomLog(data, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                            firstTimeLogin = false;
                            return newuser;
                        });
                    if (firstTimeLogin)
                    {
                        string data = string.Concat("Client has Connected | User Name:", newuser.UserId ,"(", newuser.FirstName ," ",newuser.SurName,")" ," | Connection ID: ", newuser.ConnectionId);
                        new ErrorLogger().CustomLog(data, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                        Task.Run(() => new User_Log().LoginUser(newuser));
                    }
                    AppParameters._connections.Add(Context.ConnectionId, Context.ConnectionId);
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        public static string GetGroupNames(IdentityReferenceCollection groups)
        {
            try
            {
                string item = string.Empty;
                if (groups != null)
                {
                    int propertyCount = groups.Count;
                    int propertyCounter;
                    for (propertyCounter = 0; propertyCounter <= propertyCount - 1; propertyCounter++)
                    {
                        string dn = groups[propertyCounter].Translate(typeof(System.Security.Principal.NTAccount)).ToString();

                        int equalsIndex = dn.IndexOf("\\", 1);
                        if ((equalsIndex == -1))
                        {
                            continue;
                        }

                        var groupName = dn.Substring((equalsIndex + 1), (dn.Length - equalsIndex) - 1);
                        if ((propertyCount - 1) == propertyCounter)
                        {
                            item += groupName;
                        }
                        else
                        {
                            item = item + groupName + "|";
                        }
                    }
                }
                return item;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }

        private string GetUserRole(string groups)
        {
            try
            {
                if (!string.IsNullOrEmpty(groups))
                {
                    List<string> user_groups = groups.Split('|').ToList();

                    string temp_list = AppParameters.AppSettings.ContainsKey("ROLES_ADMIN") ? AppParameters.AppSettings.Property("ROLES_ADMIN").Value.ToString().Trim() : "";
                    if (!string.IsNullOrEmpty(temp_list))
                    {
                        List<string> Webconfig_roles = temp_list.Split(',').ToList();
                        List<string> commonrolse = Webconfig_roles.Intersect(user_groups, StringComparer.OrdinalIgnoreCase).ToList();
                        if ((commonrolse != null) && commonrolse.Count > 0)
                        {
                            return "Admin".ToUpper();
                        }
                    }
                    //Check for OIE Access
                    temp_list = AppParameters.AppSettings.ContainsKey("ROLES_OIE") ? AppParameters.AppSettings.Property("ROLES_OIE").Value.ToString().Trim() : "";
                    if (!string.IsNullOrEmpty(temp_list))
                    {
                        List<string> Webconfig_roles = temp_list.Split(',').ToList();
                        List<string> commonrolse = Webconfig_roles.Intersect(user_groups, StringComparer.OrdinalIgnoreCase).ToList();
                        if ((commonrolse != null) && commonrolse.Count > 0)
                        {
                            return "OIE".ToUpper();
                        }
                    }
                    //Check for Maintenance Access
                    temp_list = AppParameters.AppSettings.ContainsKey("ROLES_MAINTENANCE") ? AppParameters.AppSettings.Property("ROLES_MAINTENANCE").Value.ToString().Trim() : "";
                    if (!string.IsNullOrEmpty(temp_list))
                    {
                        List<string> Webconfig_roles = temp_list.Split(',').ToList();
                        List<string> commonrolse = Webconfig_roles.Intersect(user_groups, StringComparer.OrdinalIgnoreCase).ToList();
                        if ((commonrolse != null) && commonrolse.Count > 0)
                        {
                            return "MAINTENANCE".ToUpper();
                        }
                    }
                    return "Operator".ToUpper();
                }
                else
                {
                    return "Operator".ToUpper();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "Operator".ToUpper();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<JToken> AddCustomZone(string data)
        {
            if(!string.IsNullOrEmpty(data))
            {
                JObject item = new JObject();
                item = JObject.Parse(data);
                if(item != null)
                {
                    if(item.ContainsKey("properties"))
                    {
                        JObject itemProperties = item["properties"] as JObject;
                        string zoneid = itemProperties.ContainsKey("id") ? itemProperties["id"].ToString() : "";
                        string zoneType = itemProperties.ContainsKey("zone_type") ? itemProperties["zone_type"].ToString() : "";
                        string zoneName = itemProperties.ContainsKey("name") ? itemProperties["name"].ToString() : "";
                        string mpeName = itemProperties.ContainsKey("machine_name") ? itemProperties["machine_name"].ToString() : "";
                        string mpeNumber = itemProperties.ContainsKey("machine_number") ? itemProperties["machine_number"].ToString() : "";
                        string tempbins = itemProperties.ContainsKey("bins") ? itemProperties["bins"].ToString() : "";
                        List<string> bins = tempbins.Split(',').Select(p => p.Trim()).ToList();
                        string zonelocUpdate = itemProperties.ContainsKey("location_update") ? itemProperties["location_Update"].ToString() : "";
                        string zoneVisible = itemProperties.ContainsKey("visible") ? itemProperties["visible"].ToString() : "";

                        var geoType = item["geometry"]["type"].ToString();

                        JObject zoneGeo = new JObject();
                        JArray tempCords = item["geometry"]["coordinates"] as JArray;
                        zoneGeo["Coordinates"] = tempCords;
                        zoneGeo["type"] = geoType;
                        Geometry newgeo = zoneGeo.ToObject<Geometry>();

                        Properties newProperties = new Properties
                        {
                            Id = zoneid,
                            Visible = true,
                            Color = "#ff006699",
                            Name = zoneName,
                            ZoneUpdate = true,
                            ZoneType = zoneType,
                            MPEType = mpeName,
                            MPENumber = mpeNumber,
                            Bins = bins
                        };

                        GeoZone newGeoZone = new GeoZone
                        {
                            Type = "Feature",
                            Geometry = newgeo,
                            Properties = newProperties
                        };
                        if (!AppParameters.ZoneList.TryAdd(newGeoZone.Properties.Id, newGeoZone))
                        {
                            //new ErrorLogger().CustomLog("Unable to Update Zone" + newtempgZone.Properties.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                        }
                        var xx = AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Bin").Select(x => x.Value).ToList();
                    }
                }
            }

            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "CustomZones.json", JsonConvert.SerializeObject(GetAllCustomZonesList(), Formatting.Indented));
            return null;
        }

        internal IEnumerable<GeoZone> GetCustomBinZonesList()
        {
            try
            {

                return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType.ToString() == "Custom_Bin").Select(x => x.Value).ToList();

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<GeoZone> GetAllCustomZonesList()
        {
            try
            {
                var zones = AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType.ToString().StartsWith("Custom")).Select(x => x.Value).ToList();
                return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType.ToString().StartsWith("Custom")).Select(x => x.Value).ToList();

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
    }
}