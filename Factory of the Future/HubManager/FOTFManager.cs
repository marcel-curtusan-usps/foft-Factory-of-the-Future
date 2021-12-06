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
using System.Web;

namespace Factory_of_the_Future
{
    public class FOTFManager : IDisposable
    {
        private readonly static Lazy<FOTFManager> _instance = new Lazy<FOTFManager>(() => new FOTFManager(GlobalHost.ConnectionManager.GetHubContext<HubManager>().Clients));
        private static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        //blocks
        private readonly object updateClocklock = new object();

        private readonly object updateZoneStatuslock = new object();
        private readonly object updateTagStatuslock = new object();
        private readonly object updatePersonTagStatuslock = new object();
        private readonly object updateDockDoorStatuslock = new object();
        private readonly object updateQSMlock = new object();
        private readonly object updateMachineStatuslock = new object();
        private readonly object updateAGVLocationStatuslock = new object();
        private readonly object updateCTSDepartedStatuslock = new object();
        private readonly object updateCTSLocalDepartedStatuslock = new object();
        private readonly object updateCTSInboundStatuslock = new object();
        private readonly object updateCTSOutboundStatuslock = new object();
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
        private readonly Timer CTSDeparted_timer;
        private readonly Timer CTSLocalDeparted_timer;
        private readonly Timer CTSInbound_timer;
        private readonly Timer CTSOutbound_timer;
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
        private volatile bool _updateCTSDepartedStatus = false;
        private readonly bool _updateCTSLocalDepartedStatus = false;
        private volatile bool _updateCTSInboundStatus = false;
        private volatile bool _updateCTSOutboundStatus = false;
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
            VehicleTag_timer = new Timer(UpdateVehicleTagStatus, null, _250milupdateInterval, _250milupdateInterval);
            PersonTag_timer = new Timer(UpdatePersonTagStatus, null, _250milupdateInterval, _250milupdateInterval);
            //////Zone status.
            Zone_timer = new Timer(UpdateZoneStatus, null, _updateInterval, _updateInterval);
            DockDoor_timer = new Timer(UpdateDockDoorStatus, null, _250milupdateInterval, _250milupdateInterval);
            Machine_timer = new Timer(UpdateMachineStatus, null, _updateInterval, _updateInterval);
            AGVLocation_timer = new Timer(UpdateAGVLocationStatus, null, _250milupdateInterval, _250milupdateInterval);
            /////CTS data timer
            CTSDeparted_timer = new Timer(UpdateCTSDepartedStatus, null, _updateInterval, _updateInterval);
            CTSLocalDeparted_timer = new Timer(UpdateCTSLocalDepartedStatus, null, _updateInterval, _updateInterval);
            CTSInbound_timer = new Timer(UpdateCTSInboundStatus, null, _updateInterval, _updateInterval);
            CTSOutbound_timer = new Timer(UpdateCTSOutboundStatus, null, _updateInterval, _updateInterval);
            ////   Notification data timer
            Notification_timer = new Timer(UpdateNotificationtatus, null, _updateInterval, _updateInterval);
            ////Connection status
            QSM_timer = new Timer(UpdateQSM, null, _updateInterval, _updateInterval);
        }

     

        private void UpdateClockStatus(object state)
        {
            lock (updateClocklock)
            {
                if (!_updateClockStatus)
                {
                    _updateClockStatus = true;
                    DateTime dtLastUpdate = DateTime.Now;
                    if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value))
                    {
                        if (Global.TimeZoneConvert.TryGetValue((string)Global.AppSettings.Property("FACILITY_TIMEZONE").Value, out string windowsTimeZoneId))
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

        internal IEnumerable<JToken> GetNotification(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    return Global.Notification.Where(c => c.Value.Property("TYPE").Value.ToString().ToLower() == data.ToLower() && (bool)c.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList();
                }
                else
                {
                    if (Global.Notification.Count() > 0)
                    {
                        return Global.Notification.Select(x => x.Value).ToList();
                    }
                    else
                    {
                        return new JObject();
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> GetADUserProfile(string userID)
        {
            try
            {
                if (!string.IsNullOrEmpty(userID))
                {
                    if (Global._users.ContainsKey(userID))
                    {
                        if (Global._users.TryGetValue(userID, out JObject user))
                        {
                            if (string.IsNullOrEmpty((string)user.Property("EmailAddress").Value))
                            {
                                if (new FindACEUser().User(user, out JObject ADuser))
                                {
                                    user.Property("EmailAddress").Value = ADuser.Property("EmailAddress").Value;
                                    user.Property("Phone").Value = ADuser.Property("Phone").Value;
                                    user.Property("MiddleName").Value = ADuser.Property("MiddleName").Value;
                                    user.Property("SurName").Value = ADuser.Property("SurName").Value;
                                    user.Property("FirstName").Value = ADuser.Property("FirstName").Value;
                                    user.Property("ZipCode").Value = ADuser.Property("ZipCode").Value;
                                    return user;
                                }

                                return user;
                            }
                            else
                            {
                                return user;
                            }
                        }
                        else
                        {
                            return new JObject();
                        }
                    }
                    else
                    {
                        return new JObject();
                    }
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        //internal IEnumerable<JToken> UDP_Start_Listener()
        //{
        //    if (Global.udpServer != null)
        //    {
        //    }
        //    else
        //    {
        //      new UDP_Client().startListener();
        //    }
        //}

        //internal IEnumerable<JToken> UDP_Stop_Listener()
        //{
        //    throw new NotImplementedException();
        //}

        internal IEnumerable<JToken> GetUserProfile(string conID)
        {
            try
            {
                if (!string.IsNullOrEmpty(conID))
                {
                    if (Global._users.ContainsKey(conID))
                    {
                        if (Global._users.TryGetValue(conID, out JObject user))
                        {
                            return user;
                        }
                        else
                        {
                            return new JObject();
                        }
                    }
                    else
                    {
                        if (Global._users.Where(g => (string)g.Value.Property("Session_ID").Value == conID).Select(y => y.Value).ToList().Count() > 0)
                        {
                            return Global._users.Where(g => (string)g.Value.Property("Session_ID").Value == conID).Select(y => y.Value);
                        }
                        else
                        {
                            return new JObject();
                        }
                    }
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        private void UpdateNotificationtatus(object state)
        {
            lock (updateNotificationStatuslock)
            {
                if (!_updateNotificationstatus)
                {
                    _updateNotificationstatus = true;
                    foreach (JObject notification in Global.Notification.Select(x => x.Value))
                    {
                        if (TryUpdateNotificationStatus(notification, out JObject outnotification))
                        {
                            BroadcastNotificationStatus(outnotification);
                        }
                    }
                    _updateNotificationstatus = false;
                }
            }
        }

        private bool TryUpdateNotificationStatus(JObject notification, out JObject outnotification)
        {
            try
            {
                outnotification = notification;
                if (Global.Tag.ContainsKey((string)notification.Property("TAGID").Value))
                {
                    if (Global.Tag.TryGetValue((string)notification.Property("TAGID").Value, out JObject tag))
                    {
                        if ((bool)notification.Property("ACTIVE_CONDITION").Value)
                        {
                            if (outnotification.ContainsKey("DELETE"))
                            {
                                if (Global.Notification.TryRemove((string)outnotification.Property("NOTIFICATIONGID").Value, out outnotification))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (Global.Notification.TryRemove((string)outnotification.Property("NOTIFICATIONGID").Value, out outnotification))
                            {
                                if (!outnotification.ContainsKey("DELETE"))
                                {
                                    outnotification.Add(new JProperty("DELETE", true));
                                }

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (!outnotification.ContainsKey("DELETE"))
                        {
                            outnotification.Add(new JProperty("DELETE", true));
                        }
                        return true;
                    }
                }
                else
                {
                    if (Global.Notification.TryRemove((string)notification.Property("NOTIFICATIONGID").Value, out outnotification))
                    {
                        if (!outnotification.ContainsKey("DELETE"))
                        {
                            outnotification.Add(new JProperty("DELETE", true));
                        }
                        return true;
                    }
                    else
                    {
                        if (!outnotification.ContainsKey("DELETE"))
                        {
                            outnotification.Add(new JProperty("DELETE", true));
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                outnotification = notification;
                return true;
            }
        }

        private void BroadcastNotificationStatus(JObject vehiclenotification)
        {
            Clients.All.updateNotification(vehiclenotification);
        }

        internal IEnumerable<JToken> GetNotification_ConditionsList(int data)
        {
            try
            {
                if (data > 0)
                {
                    return Global.Notification_Conditions.Where(c => (int)c.Value.Property("ID").Value == data).Select(x => x.Value).ToList();
                }
                else
                {
                    if (Global.Notification_Conditions.Count() > 0)
                    {
                        return Global.Notification_Conditions.Select(x => x.Value).ToList();
                    }
                    else
                    {
                        return new JObject();
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> AddNotification_Conditions(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    JObject Notification = JObject.Parse(data);
                    if (Notification.HasValues)
                    {
                        JObject new_notification = new JObject_List().Notification_Conditions;
                        new_notification.Property("ID").Value = Global.Notification_Conditions.Count + 1;
                        new_notification.Property("CREATED_DATE").Value = DateTime.Now;
                        new_notification.Property("LASTUPDATE_DATE").Value = "";
                        bool updateFile = false;
                        foreach (dynamic kv in Notification.Children())
                        {
                            if (new_notification.ContainsKey(kv.Name))
                            {
                                if (kv.Value != new_notification.Property(kv.Name).Value)
                                {
                                    new_notification.Property(kv.Name).Value = kv.Value;
                                    updateFile = true;
                                }
                            }
                        }

                        if (!Global.Notification_Conditions.ContainsKey((int)new_notification.Property("ID").Value))
                        {
                            Global.Notification_Conditions.TryAdd((int)new_notification.Property("ID").Value, new_notification);
                        }
                        else
                        {
                            return new JObject(new JProperty("ERROR_MESSAGE", "ID already exist"));
                        }

                        if (updateFile)
                        {
                            //write to file the new connection
                            new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "Notification.json", JsonConvert.SerializeObject(Global.Notification_Conditions.Select(x => x.Value), Formatting.Indented));
                            return Global.Notification_Conditions.Select(e => e.Value).ToList();
                        }
                        else
                        {
                            return new JObject(new JProperty("ERROR_MESSAGE", "No update made"));
                        }
                    }
                    else
                    {
                        return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                    }
                }
                else
                {
                    return new JObject(new JProperty("ERROR_MESSAGE", "No data provvided"));
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return new JObject(new JProperty("ERROR_MESSAGE", "Data Not loaded"));
            }
        }

        internal IEnumerable<JToken> EditNotification_Conditions(string data)
        {
            try
            {
                JObject updatenotification = JObject.Parse(data);
                if (updatenotification.HasValues)
                {
                    if (updatenotification.ContainsKey("ID"))
                    {
                        if (Global.Notification_Conditions.ContainsKey((int)updatenotification.Property("ID").Value))
                        {
                            if (Global.Notification_Conditions.TryGetValue((int)updatenotification.Property("ID").Value, out JObject notification))
                            {
                                bool updateFile = false;
                                notification.Property("LASTUPDATE_DATE").Value = DateTime.Now;
                                foreach (dynamic kv in updatenotification.Children())
                                {
                                    if (notification.ContainsKey(kv.Name))
                                    {
                                        if (notification.Property(kv.Name).Value != kv.Value)
                                        {
                                            notification.Property(kv.Name).Value = updatenotification.Property(kv.Name).Value;
                                            updateFile = true;
                                        }
                                    }
                                }
                                foreach (var item in Global.Notification.Where(r => (int)r.Value.Property("ID").Value == (int)updatenotification.Property("ID").Value).Select(y => y.Value).ToList())
                                {
                                    item.Property("WARNING").Value = notification.Property("WARNING").Value;
                                    item.Property("CRITICAL").Value = notification.Property("CRITICAL").Value;
                                    if (notification.Property("ACTIVE_CONDITION").Value != item.Property("ACTIVE_CONDITION").Value)
                                    {
                                        item.Property("ACTIVE_CONDITION").Value = notification.Property("ACTIVE_CONDITION").Value;

                                        if (!item.ContainsKey("DELETE"))
                                        {
                                            item.Add(new JProperty("DELETE", true));
                                        }
                                        else
                                        {
                                            item.Property("DELETE").Value = true;
                                        }
                                    }
                                }
                                //write to file for backup
                                if (updateFile)
                                {
                                    new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "Notification.json", JsonConvert.SerializeObject(Global.Notification_Conditions.Select(x => x.Value), Formatting.Indented));
                                    return Global.Notification_Conditions.Select(e => e.Value).ToList();
                                }
                                else
                                {
                                    return new JObject(new JProperty("ERROR_MESSAGE", "No update made"));
                                }
                            }
                            else
                            {
                                return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                            }
                        }
                        else
                        {
                            return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                        }
                    }
                    else
                    {
                        return new JObject(new JProperty("ERROR_MESSAGE", "ID not provided"));
                    }
                }
                else
                {
                    return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject(new JProperty("ERROR_MESSAGE", "Data Not loaded"));
            }
        }

        internal IEnumerable<JToken> DeleteNotification_Conditions(string data)
        {
            try
            {
                JObject Notification = JObject.Parse(data);
                if (Notification.HasValues)
                {
                    if (Notification.ContainsKey("ID"))
                    {
                        if (Global.Notification_Conditions.TryRemove((int)Notification.Property("ID").Value, out JObject outtemp))
                        {
                            new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "Notification.json", JsonConvert.SerializeObject(Global.Notification_Conditions.Select(x => x.Value), Formatting.Indented));
                            return Global.Notification_Conditions.Select(e => e.Value).ToList();
                        }
                        else
                        {
                            return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                        }
                    }
                    else
                    {
                        return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                    }
                }
                else
                {
                    return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject(new JProperty("ERROR_MESSAGE", "Error Deleting data"));
            }
        }

        internal IEnumerable<JToken> EditTagInfo(string data)
        {
            JObject item = new JObject();
            DateTime in_time = DateTime.Now;
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    item = JObject.Parse(data);
                    if (item.HasValues)
                    {
                        if (item.ContainsKey("id"))
                        {
                            string tag_id = (string)item.Property("id").Value;
                            if (Global.Tag.ContainsKey(tag_id))
                            {
                                if (Global.Tag.TryGetValue(tag_id, out JObject tag_item))
                                {
                                    //tagInfo["properties"]["Tag_Update"] = false;
                                    bool update = false;
                                    foreach (dynamic kv in item.Children())
                                    {
                                        if (kv.Name != "id")
                                        {
                                            if (((JObject)tag_item["properties"]).ContainsKey(kv.Name))
                                            {
                                                if (((JObject)tag_item["properties"]).Property(kv.Name).Value != kv.Value)
                                                {
                                                    ((JObject)tag_item["properties"]).Property(kv.Name).Value = item.Property(kv.Name).Value;
                                                    update = true;
                                                }
                                            }
                                        }
                                    }
                                    if (update)
                                    {
                                        ((JObject)tag_item["properties"]).Property("Tag_Update").Value = true;
                                        new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "Tag_Info.json", JsonConvert.SerializeObject(Global.Tag.Select(x => x.Value).ToList(), Formatting.Indented));
                                    }
                                    return new JObject(new JProperty("MESSAGE_TYPE", "Tag has been update"));
                                }
                                else
                                {
                                    return new JObject(new JProperty("ERROR_MESSAGE", "Tag ID not Found in List"));
                                }
                            }
                            else
                            {
                                return new JObject(new JProperty("ERROR_MESSAGE", "Tag ID not Found in List"));
                            }
                        }
                        else
                        {
                            return new JObject(new JProperty("ERROR_MESSAGE", "Missing Tag ID"));
                        }
                    }
                    else
                    {
                        return new JObject(new JProperty("ERROR_MESSAGE", "Data Value is Empty"));
                    }
                }
                else
                {
                    return new JObject(new JProperty("ERROR_MESSAGE", "Data Object was Empty"));
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject(new JProperty("ERROR_MESSAGE", "Data Not loaded"));
            }
        }

        private void UpdateZoneStatus(object state)
        {
            lock (updateZoneStatuslock)
            {
                if (!_updateZoneStatus)
                {
                    _updateZoneStatus = true;
                    Global.Zones.Where(u => (bool)u.Value["properties"]["Zone_Update"] == true && u.Value["properties"]["Zone_Type"].ToString() == "Area").Select(x => x.Value).ToList().ForEach(zoneitem =>
                    {

                        if (TryUpdateZoneStatus(zoneitem))
                        {
                            BroadcastZoneStatus(zoneitem);
                        }
                    });

                    _updateZoneStatus = false;
                }
            }
        }

        private void BroadcastZoneStatus(JObject zoneitem)
        {
            Clients.All.updateZoneStatus(zoneitem);
        }

        private bool TryUpdateZoneStatus(JObject zoneitem)
        {
            try
            {
                zoneitem["properties"]["Zone_Update"] = false;
                return true;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void UpdateCTSOutboundStatus(object state)
        {
            lock (updateCTSOutboundStatuslock)
            {
                if (!_updateCTSOutboundStatus)
                {
                    _updateCTSOutboundStatus = true;
                    foreach (JObject Outbounditem in Global.CTS_Outbound_Schedualed.Where(u => (bool)u.Value.Property("CTS_Update").Value == true).Select(x => x.Value))
                    {
                        if (TryUpdateCTSOutboundStatus(Outbounditem))
                        {
                            BroadcastCTSOutboundStatus(Outbounditem);
                        }
                    }
                    foreach (JObject removeOutbounditem in Global.CTS_Outbound_Schedualed.Where(u => (bool)u.Value.Property("CTS_Remove").Value == true).Select(x => x.Value))
                    {
                        string trip = removeOutbounditem.ContainsKey("TripID") ? (string)removeOutbounditem.Property("TripID").Value : "";
                        string route = removeOutbounditem.ContainsKey("RouteID") ? (string)removeOutbounditem.Property("RouteID").Value : "";
                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
                        {
                            string outboundID = route + trip;
                            if (Global.CTS_Outbound_Schedualed.TryRemove(outboundID, out JObject exsitoutbound))
                            {
                            }
                        }
                    }
                    _updateCTSOutboundStatus = false;
                }
            }
        }

        internal IEnumerable<JToken> GetCTSDetailsList(string route, string trip)
        {
            try
            {
                if (Global.AppSettings.ContainsKey("CTS_DETAILS_URL") && Global.AppSettings.ContainsKey("CTS_API_KEY"))
                {
                    if (!string.IsNullOrEmpty((string)Global.AppSettings.Property("CTS_DETAILS_URL").Value) && !string.IsNullOrEmpty((string)Global.AppSettings.Property("CTS_API_KEY").Value))
                    {
                        Uri parURL = new Uri((string)Global.AppSettings.Property("CTS_DETAILS_URL").Value + (string)Global.AppSettings.Property("CTS_API_KEY").Value + "&nass=" + (string)Global.AppSettings.Property("FACILITY_NASS_CODE").Value + "&route=" + route + "&trip=" + trip);
                        string CTS_Response = SendMessage.SV_Get(parURL.AbsoluteUri);
                        if (!string.IsNullOrEmpty(CTS_Response))
                        {
                            if (Global.IsValidJson(CTS_Response))
                            {
                                JObject data = JObject.Parse(CTS_Response);
                                if (data.HasValues)
                                {
                                    if (data.ContainsKey("Data"))
                                    {
                                        JToken cts_data = data.SelectToken("Data");
                                        JObject cts_site = (JObject)data.SelectToken("Site");
                                        if (cts_data.Children().Count() > 0)
                                        {
                                            return cts_data.Children();
                                        }
                                        else
                                        {
                                            return new JObject();
                                        }
                                    }
                                    else
                                    {
                                        return new JObject();
                                    }
                                }
                                else
                                {
                                    return new JObject();
                                }
                            }
                            else
                            {
                                return new JObject();
                            }
                        }
                        else
                        {
                            return new JObject();
                        }
                    }
                    else
                    {
                        return new JObject();
                    }
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        private void BroadcastCTSOutboundStatus(JObject outbounditem)
        {
            Clients.All.updateCTSOutboundStatus(outbounditem);
        }

        private bool TryUpdateCTSOutboundStatus(JObject outbounditem)
        {
            try
            {
                string trip = outbounditem.ContainsKey("TripID") ? (string)outbounditem.Property("TripID").Value : "";
                string route = outbounditem.ContainsKey("RouteID") ? (string)outbounditem.Property("RouteID").Value : "";
                string outboundID = route + trip;
                if (Global.CTS_Outbound_Schedualed.ContainsKey(outboundID))
                {
                    if (Global.CTS_Outbound_Schedualed.TryGetValue(outboundID, out JObject exsitoutbound))
                    {
                        exsitoutbound.Property("CTS_Update").Value = false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void UpdateCTSInboundStatus(object state)
        {
            lock (updateCTSInboundStatuslock)
            {
                if (!_updateCTSInboundStatus)
                {
                    _updateCTSInboundStatus = true;
                    foreach (JObject Inbounditem in Global.CTS_Inbound_Schedualed.Where(u => (bool)u.Value.Property("CTS_Update").Value == true).Select(x => x.Value))
                    {
                        if (TryUpdateCTSInboundStatus(Inbounditem))
                        {
                            BroadcastCTSInboundStatus(Inbounditem);
                        }
                    }
                    foreach (JObject removeInbounditem in Global.CTS_Inbound_Schedualed.Where(u => (bool)u.Value.Property("CTS_Remove").Value == true).Select(x => x.Value))
                    {
                        string trip = removeInbounditem.ContainsKey("TripID") ? (string)removeInbounditem.Property("TripID").Value : "";
                        string route = removeInbounditem.ContainsKey("RouteID") ? (string)removeInbounditem.Property("RouteID").Value : "";
                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
                        {
                            string outboundID = route + trip;
                            if (Global.CTS_Inbound_Schedualed.TryRemove(outboundID, out JObject exsitoutbound))
                            {
                            }
                        }
                    }
                    _updateCTSInboundStatus = false;
                }
            }
        }

        private void BroadcastCTSInboundStatus(JObject inbounditem)
        {
            Clients.All.updateCTSInboundStatus(inbounditem);
        }

        private bool TryUpdateCTSInboundStatus(JObject inbounditem)
        {
            try
            {
                string trip = inbounditem.ContainsKey("TripID") ? (string)inbounditem.Property("TripID").Value : "";
                string route = inbounditem.ContainsKey("RouteID") ? (string)inbounditem.Property("RouteID").Value : "";
                string inboundID = route + trip;
                if (Global.CTS_Inbound_Schedualed.ContainsKey(inboundID))
                {
                    if (Global.CTS_Inbound_Schedualed.TryGetValue(inboundID, out JObject exsitinbound))
                    {
                        exsitinbound.Property("CTS_Update").Value = false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void UpdateCTSDepartedStatus(object state)
        {
            lock (updateCTSDepartedStatuslock)
            {
                if (!_updateCTSDepartedStatus)
                {
                    _updateCTSDepartedStatus = true;
                    foreach (JObject DockDeparted in Global.CTS_DockDeparted.Where(u => (bool)u.Value.Property("CTS_Update").Value == true).Select(x => x.Value))
                    {
                        if (TryUpdateCTSDepartedStatus(DockDeparted))
                        {
                            BroadcastCTSDepartedStatus(DockDeparted);
                        }
                    }
                    foreach (JObject removeDockDeparted in Global.CTS_DockDeparted.Where(u => (bool)u.Value.Property("CTS_Remove").Value == true).Select(x => x.Value))
                    {
                        string trip = removeDockDeparted.ContainsKey("Trip") ? (string)removeDockDeparted.Property("Trip").Value : "";
                        string route = removeDockDeparted.ContainsKey("Route") ? (string)removeDockDeparted.Property("Route").Value : "";
                        if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
                        {
                            string outboundID = route + trip;
                            if (Global.CTS_DockDeparted.TryRemove(outboundID, out JObject exsitoutbound))
                            {
                            }
                        }
                    }
                    _updateCTSDepartedStatus = false;
                }
            }
        }

        private void BroadcastCTSDepartedStatus(JObject aGV_Location)
        {
            Clients.All.updateCTSDepartedStatus(aGV_Location);
        }

        private bool TryUpdateCTSDepartedStatus(JObject DockDeparted)
        {
            try
            {
                string trip = DockDeparted.ContainsKey("Trip") ? (string)DockDeparted.Property("Trip").Value : "";
                string route = DockDeparted.ContainsKey("Route") ? (string)DockDeparted.Property("Route").Value : "";
                string DockDepartedID = route + trip;
                if (Global.CTS_DockDeparted.ContainsKey(DockDepartedID))
                {
                    if (Global.CTS_DockDeparted.TryGetValue(DockDepartedID, out JObject exsitDockDeparted))
                    {
                        exsitDockDeparted.Property("CTS_Update").Value = false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void UpdateCTSLocalDepartedStatus(object state)
        {
            lock (updateCTSLocalDepartedStatuslock)
            {
                if (!_updateCTSLocalDepartedStatus)
                {
                    _updateCTSDepartedStatus = true;
                    foreach (JObject DockDeparted in Global.CTS_LocalDockDeparted.Where(u => (bool)u.Value.Property("CTS_Update").Value == true).Select(x => x.Value))
                    {
                        if (TryUpdateCTSLocalDepartedStatus(DockDeparted))
                        {
                            if ((bool)DockDeparted.Property("CTS_Remove").Value == true)
                            {
                                string trip = DockDeparted.ContainsKey("Trip") ? (string)DockDeparted.Property("Trip").Value : "";
                                string route = DockDeparted.ContainsKey("Route") ? (string)DockDeparted.Property("Route").Value : "";
                                if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
                                {
                                    string outboundID = route + trip;
                                    if (Global.CTS_LocalDockDeparted.TryRemove(outboundID, out JObject exsitoutbound))
                                    {
                                        BroadcastCTSLocalDepartedStatus(DockDeparted);
                                    }
                                }
                            }
                            else
                            {
                                BroadcastCTSLocalDepartedStatus(DockDeparted);
                            }
                        }
                    }
                    //foreach (JObject removeDockDeparted in Global.CTS_LocalDockDeparted.Where(u => (bool)u.Value.Property("CTS_Remove").Value == true).Select(x => x.Value))
                    //{
                    //    string trip = removeDockDeparted.ContainsKey("Trip") ? (string)removeDockDeparted.Property("Trip").Value : "";
                    //    string route = removeDockDeparted.ContainsKey("Route") ? (string)removeDockDeparted.Property("Route").Value : "";
                    //    if (!string.IsNullOrEmpty(route) || !string.IsNullOrEmpty(trip))
                    //    {
                    //        string outboundID = route + trip;
                    //        if (Global.CTS_LocalDockDeparted.TryRemove(outboundID, out JObject exsitoutbound))
                    //        {
                    //        }
                    //    }
                    //}
                    _updateCTSDepartedStatus = false;
                }
            }
        }

        private void BroadcastCTSLocalDepartedStatus(JObject DockDeparted)
        {
            Clients.All.updateCTSLocalDepartedStatus(DockDeparted);
        }

        private bool TryUpdateCTSLocalDepartedStatus(JObject DockDeparted)
        {
            try
            {
                string trip = DockDeparted.ContainsKey("Trip") ? (string)DockDeparted.Property("Trip").Value : "";
                string route = DockDeparted.ContainsKey("Route") ? (string)DockDeparted.Property("Route").Value : "";
                string DockDepartedID = route + trip;
                if (Global.CTS_LocalDockDeparted.ContainsKey(DockDepartedID))
                {
                    if (Global.CTS_LocalDockDeparted.TryGetValue(DockDepartedID, out JObject exsitDockDeparted))
                    {
                        exsitDockDeparted.Property("CTS_Update").Value = false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }
        internal IEnumerable<Trips> GetTripsList()
        {
            try
            {
                return Global.Trips.Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<JToken> GetCTSList(string type)
        {
            try
            {
                if (type.StartsWith("in"))
                {
                    return Global.CTS_Inbound_Schedualed.Select(x => x.Value).ToList();
                }
                else if (type.StartsWith("dockdeparted"))
                {
                    return Global.CTS_DockDeparted.Select(x => x.Value).ToList();
                }
                else if (type.StartsWith("local"))
                {
                    return Global.CTS_LocalDockDeparted.Select(x => x.Value).ToList();
                }
                else if (type.StartsWith("out"))
                {
                    return Global.CTS_Outbound_Schedualed.Select(x => x.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        private void UpdateQSM(object state)
        {
            lock (updateQSMlock)
            {
                if (!_updatingQSMStatus)
                {
                    _updatingQSMStatus = true;
                    if (Global.API_List.Count > 0)
                    {
                        foreach (JObject QSMitem in Global.API_List.Where(r => (bool)r.Value["UPDATE_STATUS"] == true).Select(x => x.Value).ToList())
                        {
                            if (TryUpdateQSMStaus(QSMitem))
                            {
                                BroadcastQSMUpdate(QSMitem);
                            }
                        }
                    }
                    _updatingQSMStatus = false;
                }
            }
        }

        internal IEnumerable<JToken> GetMachineZonesList()
        {
            try
            {
                if (Global.Zones.Count() > 0)
                {
                    return Global.Zones.Where(r => r.Value["properties"]["Zone_Type"].ToString() == "Machine").Select(x => x.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> GetAGVLocationZonesList()
        {
            try
            {
                if (Global.Zones.Count() > 0)
                {
                    return Global.Zones.Where(r => r.Value["properties"]["Zone_Type"].ToString() == "AGVLocation").Select(x => x.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> GetViewPortsZonesList()
        {
            try
            {
                if (Global.Zones.Count() > 0)
                {
                    return Global.Zones.Where(r => r.Value["properties"]["Zone_Type"].ToString() == "ViewPorts").Select(x => x.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> GetDockDoorZonesList()
        {
            try
            {
                if (Global.Zones.Count() > 0)
                {
                    return Global.Zones.Where(r => r.Value["properties"]["Zone_Type"].ToString() == "DockDoor").Select(x => x.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        private string GetDoorAlertState(JObject DockDoor)
        {
            // Set the status for the dock doors, which is represented visually with colors.
            // Business rules need to be added, so for now just gets set to OKAY.
            string status;
            status = "OKAY";
            //status = "WARNING";
            //status = "CRITICAL";

            // For testing:
            //if (dockDoor["properties"]["svDoorData"]["doorNumber"].ToString() == "22")
            //{
            //    status = "CRITICAL";
            //}

            return status;
        }

        internal IEnumerable<JToken> GetViewConfigList()
        {
            try
            {
                return new JObject();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        private void UpdateAGVLocationStatus(object state)
        {
            lock (updateAGVLocationStatuslock)
            {
                if (!_updateAGVLocationStatus)
                {
                    _updateAGVLocationStatus = true;
                    foreach (JObject AGV_Location in Global.Zones.Where(u => u.Value["properties"]["Zone_Type"].ToString() == "AGVLocation" && (bool)u.Value["properties"]["Zone_Update"] == true).Select(x => x.Value))
                    {
                        if (TryUpdateAGVLocationStatus(AGV_Location))
                        {
                            BroadcastAGVLocationStatus(AGV_Location);
                        }
                    }
                    _updateAGVLocationStatus = false;
                }
            }
        }

        private bool TryUpdateAGVLocationStatus(JObject AGV_Location)
        {
            try
            {
                AGV_Location["properties"]["Zone_Update"] = false;
                return true;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void BroadcastAGVLocationStatus(JObject machine)
        {
            Clients.All.updateAGVLocationStatus(machine);
        }

        private void UpdateMachineStatus(object state)
        {
            lock (updateMachineStatuslock)
            {
                if (!_updateMachineStatus)
                {
                    _updateMachineStatus = true;
                    Global.Zones.Where(u => (bool)u.Value["properties"]["Zone_Update"] == true && u.Value["properties"]["Zone_Type"].ToString() == "Machine").Select(x => x.Value).ToList().ForEach(Machine =>
                    {
                        if (TryUpdateMachineStatus(Machine))
                        {
                            BroadcastMachineStatus(Machine);
                        }
                    });
                        
                    _updateMachineStatus = false;
                }
            }
        }

        private bool TryUpdateMachineStatus(JObject machine)
        {
            try
            {

                machine["properties"]["Zone_Update"] = false;
                return true;

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void BroadcastMachineStatus(JObject machine)
        {
            Clients.All.updateMachineStatus(machine);
        }

        internal IEnumerable<JToken> GetMap()
        {
            try
            {
                if (Global.Map.Count() > 0)
                {
                    return Global.Map.Select(x => x.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> GetZonesList()
        {
            try
            {
                if (Global.Zones.Count() > 0)
                {
                    return Global.Zones.Where(r => r.Value["properties"]["Zone_Type"].ToString() == "Area").Select(x => x.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> GetPersonTagsList()
        {
            try
            {
                if (Global.Tag.Count() > 0)
                {
                    return Global.Tag.Where(x => x.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")).Select(y => y.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> GetUndetectedTagsList()
        {
            try
            {
                if (Global.Tag.Count() > 0)
                {
                    return Global.Tag.Where(x => x.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")
                    && (bool)x.Value["properties"]["isWearingTag"] == false
                    && ((JObject)x.Value["properties"]["Tacs"]).HasValues).Select(y => y.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<Container> GetContainer(string data, string direction, string route, string trip)
        {
            try
            {
                if (Global.Containers.Count() > 0)
                {
                    if (direction == "I")
                    {
                        return Global.Containers.Where(r =>
                         r.Value.Iroute == route &&
                        r.Value.Itrip == trip).Select(y => y.Value).ToList();

                        //return Global.Containers.Where(r => r.Value.ContainsKey("Iroute") &&
                        //r.Value.Property("dest").Value.ToString().StartsWith(data)  &&
                        //(string)r.Value.Property("Iroute").Value == route &&
                        //(string)r.Value.Property("Itrip").Value == trip).Select(y => y.Value).ToList();
                    }
                    else
                    {
                        return Global.Containers.Where(r => r.Value.Dest == data).Select(y => y.Value).ToList();
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

        internal IEnumerable<JToken> GetMarkerList()
        {
            try
            {
                if (Global.Tag.Count() > 0)
                {
                    return Global.Tag.Where(x => x.Value["properties"]["Tag_Type"].ToString().EndsWith("Vehicle")).Select(y => y.Value).ToList();
                }
                else
                {
                    return new JObject();
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        private void BroadcastVehicleTagStatus(JObject tag)
        {
            Clients.All.updateVehicleTagStatus(tag);
        }

        private bool TryUpdateVehicleTagStatus(JObject tag, double tagVisibleRange)
        {
            try
            {
                bool returnresult = false;
                System.TimeSpan tagdiffResult = ((DateTime)((JObject)tag["properties"]).Property("Tag_TS").Value).ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("positionTS").Value).ToUniversalTime());
                tag["properties"]["tagVisibleMils"] = tagdiffResult.TotalMilliseconds;
                if ((bool)tag["properties"]["Tag_Update"] == true)
                {
                    ////vehicle notification
                    //if (((JObject)tag["properties"]).Property("Tag_Type").Value.ToString().ToLower() == "Autonomous Vehicle".ToLower())
                    //{
                    //  if (((JObject)tag["properties"]).ContainsKey("vehicleTime"))
                    //    {
                    //        if (!string.IsNullOrEmpty((string)((JObject)tag["properties"]).Property("vehicleTime").Value))
                    //        {
                    //            //time cal lostcomm
                    //            System.TimeSpan diffResult = DateTime.Now.ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime());

                    //            if (Global.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                    //           && r.Value.Property("TYPE").Value.ToString().ToUpper() == "vehicle".ToUpper()
                    //            && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList().Count() > 0)
                    //            {
                    //                foreach (var conditionitem in Global.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                    //                    && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                    //                     && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList())
                    //                {
                    //                    double warningmil = TimeSpan.FromMinutes((int)conditionitem.Property("WARNING").Value).TotalMilliseconds;
                    //                    if (diffResult.TotalMilliseconds > warningmil)
                    //                    {
                    //                        if (!Global.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
                    //                        {
                    //                            JObject ojbMerge = (JObject)conditionitem.DeepClone();
                    //                            ojbMerge.Add(new JProperty("VEHICLETIME", ((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime()));
                    //                            ojbMerge.Add(new JProperty("VEHICLENAME", (string)((JObject)tag["properties"]).Property("name").Value));
                    //                            ojbMerge.Add(new JProperty("TAGID", (string)((JObject)tag["properties"]).Property("id").Value));
                    //                            ojbMerge.Add(new JProperty("NOTIFICATIONGID", (string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value));
                    //                            ojbMerge.Add(new JProperty("UPDATE", true));
                    //                            if (Global.Notification.TryAdd((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, ojbMerge))
                    //                            {
                    //                            }
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        if (Global.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
                    //                        {
                    //                            if (Global.Notification.TryGetValue((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, out JObject ojbMerge))
                    //                            {
                    //                                ojbMerge.Add(new JProperty("DELETE", true));
                    //                            }

                    //                        }
                    //                    }
                    //                }
                    //            }

                    //        }
                    //    }
                    //}
                    if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
                    {
                        if ((bool)tag["properties"]["tagVisible"] != false)
                        {
                            tag["properties"]["tagVisible"] = false;
                        }
                    }
                    else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
                    {
                        if ((bool)tag["properties"]["tagVisible"] != true)
                        {
                            tag["properties"]["tagVisible"] = true;
                        }
                    }
                    tag["properties"]["Tag_Update"] = false;
                    return true;
                }
                else
                {
                    ////vehicle notification
                    //if (((JObject)tag["properties"]).Property("Tag_Type").Value.ToString().ToLower() == "Autonomous Vehicle".ToLower())
                    //{
                    //  if (((JObject)tag["properties"]).ContainsKey("vehicleTime"))
                    //    {
                    //        if (!string.IsNullOrEmpty((string)((JObject)tag["properties"]).Property("vehicleTime").Value))
                    //        {
                    //            //time cal lostcomm
                    //            System.TimeSpan diffResult = DateTime.Now.ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime());

                    //            if (Global.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                    //           && r.Value.Property("TYPE").Value.ToString().ToUpper() == "vehicle".ToUpper()
                    //            && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList().Count() > 0)
                    //            {
                    //                foreach (var conditionitem in Global.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                    //                    && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                    //                     && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList())
                    //                {
                    //                    double warningmil = TimeSpan.FromMinutes((int)conditionitem.Property("WARNING").Value).TotalMilliseconds;
                    //                    if (diffResult.TotalMilliseconds > warningmil)
                    //                    {
                    //                        if (!Global.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
                    //                        {
                    //                            JObject ojbMerge = (JObject)conditionitem.DeepClone();
                    //                            ojbMerge.Add(new JProperty("VEHICLETIME", ((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime()));
                    //                            ojbMerge.Add(new JProperty("VEHICLENAME", (string)((JObject)tag["properties"]).Property("name").Value));
                    //                            ojbMerge.Add(new JProperty("TAGID", (string)((JObject)tag["properties"]).Property("id").Value));
                    //                            ojbMerge.Add(new JProperty("NOTIFICATIONGID", (string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value));
                    //                            ojbMerge.Add(new JProperty("UPDATE", true));
                    //                            if (Global.Notification.TryAdd((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, ojbMerge))
                    //                            {
                    //                            }
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        if (Global.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
                    //                        {
                    //                            if (Global.Notification.TryGetValue((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, out JObject ojbMerge))
                    //                            {
                    //                                ojbMerge.Add(new JProperty("DELETE", true));
                    //                            }

                    //                        }
                    //                    }
                    //                }
                    //            }

                    //        }
                    //    }
                    //}

                    if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
                    {
                        if ((bool)tag["properties"]["tagVisible"] != false)
                        {
                            tag["properties"]["tagVisible"] = false;
                            returnresult = true;
                        }
                    }
                    else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
                    {
                        if ((bool)tag["properties"]["tagVisible"] != true)
                        {
                            tag["properties"]["tagVisible"] = true;
                        }
                    }
                    tag["properties"]["Tag_Update"] = false;
                    return returnresult;
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void BroadcastQSMUpdate(JObject qSMitem)
        {
            Clients.All.UpdateQSMStatus(qSMitem);
        }

        private bool TryUpdateQSMStaus(JObject qSMitem)
        {
            try
            {
                qSMitem.Property("UPDATE_STATUS").Value = false;
                return true;

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void UpdateDockDoorStatus(object state)
        {
            lock (updateDockDoorStatuslock)
            {
                if (!_updateDockDoorStatus)
                {
                    _updateDockDoorStatus = true;

                    Global.Zones.Where(u => (bool)u.Value["properties"]["Zone_Update"] == true && u.Value["properties"]["Zone_Type"].ToString() == "DockDoor").Select(x => x.Value).ToList().ForEach(DockDoor =>
                    {
                        if (TryUpdateDockDoorStatus(DockDoor))
                        {
                            BroadcastDockDoorStatus(DockDoor);
                        }
                    });

                    _updateDockDoorStatus = false;
                }
            }
        }

        private void BroadcastDockDoorStatus(JObject dockDoor)
        {
            Clients.All.updateDockDoorStatus(dockDoor);
        }

        private bool TryUpdateDockDoorStatus(JObject dockDoor)
        {
            try
            {
                // Make call to update svDoorData.alertState
                //dockDoor["properties"]["alertState"] = GetDoorAlertState(dockDoor);
                //if (dockDoor["properties"]["alertState"].ToString().ToUpper() != "OKAY")
                //{
                //    dockDoor["properties"]["alertMessage"] = dockDoor["properties"]["alertState"].ToString().ToUpper();
                //}
                dockDoor["properties"]["Zone_Update"] = false;
                return true;
                //string DockDoor_id = (string)dockDoor["properties"]["id"];

                //if (Global.DockDoorZones.ContainsKey(DockDoor_id))
                //{
                //    if (Global.DockDoorZones.TryGetValue(DockDoor_id, out JObject dockDoorInfo))
                //    {
                //        dockDoorInfo["properties"]["Zone_Update"] = false;
                //        return true;
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}
                //else
                //{
                //    return false;
                //}
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void UpdateVehicleTagStatus(object state)
        {
            lock (updateTagStatuslock)
            {
                if (!_updateTagStatus)
                {
                    _updateTagStatus = true;
                    double tagVisibleRange = Global.AppSettings.ContainsKey("POSITION_MAX_AGE") ? !string.IsNullOrEmpty((string)Global.AppSettings.Property("POSITION_MAX_AGE").Value) ? (long)Global.AppSettings.Property("POSITION_MAX_AGE").Value : 10000 : 10000;

                    foreach (JObject Tag in Global.Tag.Where(u => (bool)u.Value["properties"]["Tag_Update"] == true && u.Value["properties"]["Tag_Type"].ToString().EndsWith("Vehicle")).Select(x => x.Value))
                    {
                        if (TryUpdateVehicleTagStatus(Tag, tagVisibleRange))
                        {
                            BroadcastVehicleTagStatus(Tag);
                        }
                    }

                    _updateTagStatus = false;
                }
            }
        }

        private void UpdatePersonTagStatus(object state)
        {
            lock (updatePersonTagStatuslock)
            {
                if (!_updatePersonTagStatus)
                {
                    _updatePersonTagStatus = true;
                    //  var watch = new System.Diagnostics.Stopwatch();
                    // watch.Start();
                    double tagVisibleRange = Global.AppSettings.ContainsKey("POSITION_MAX_AGE") ? !string.IsNullOrEmpty((string)Global.AppSettings.Property("POSITION_MAX_AGE").Value) ? (long)Global.AppSettings.Property("POSITION_MAX_AGE").Value : 10000 : 10000;

                    Global.Tag.Where(u => (bool)u.Value["properties"]["Tag_Update"] == true && u.Value["properties"]["Tag_Type"].ToString().EndsWith("Person")).Select(x => x.Value).ToList().ForEach(Tag =>
                    {
                        if (TryUpdatePersonTagStatus(Tag, tagVisibleRange))
                        {
                            BroadcastPersonTagStatus(Tag);
                        }
                    });
                    // watch.Stop();
                    // new ErrorLogger().CustomLog(string.Concat("Total Execution for all tags ", "Time: ", watch.ElapsedMilliseconds, " ms"), string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "TagProcesslogs"));
                    _updatePersonTagStatus = false;
                }
            }
        }

        private void BroadcastPersonTagStatus(JObject tag)
        {
            Clients.All.updatePersonTagStatus(tag);
        }

        private bool TryUpdatePersonTagStatus(JObject tag, double tagVisibleRange)
        {
            try
            {
                bool returnresult = false;
                System.TimeSpan tagdiffResult = ((DateTime)((JObject)tag["properties"]).Property("Tag_TS").Value).ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("positionTS").Value).ToUniversalTime());
                tag["properties"]["tagVisibleMils"] = tagdiffResult.TotalMilliseconds;
                if ((bool)tag["properties"]["Tag_Update"] == true)
                {
                    if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
                    {
                        if ((bool)tag["properties"]["tagVisible"] != false)
                        {
                            tag["properties"]["tagVisible"] = false;
                        }
                    }
                    else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
                    {
                        if ((bool)tag["properties"]["tagVisible"] != true)
                        {
                            tag["properties"]["tagVisible"] = true;
                        }
                    }
                    if (string.IsNullOrEmpty((string)tag["properties"]["craftName"]))
                    {
                        if (!string.IsNullOrEmpty(tag["properties"]["name"].ToString()))
                        {
                            int equalsIndex = tag["properties"]["name"].ToString().IndexOf("_", 1);
                            if ((equalsIndex > -1))
                            {
                                string[] namesplit = tag["properties"]["name"].ToString().Split('_');
                                if (namesplit.Length > 1)
                                {
                                    tag["properties"]["craftName"] = namesplit[0];
                                    tag["properties"]["badgeId"] = namesplit[1];
                                }
                            }
                        }
                    }
                    tag["properties"]["Tag_Update"] = false;
                    return true;
                }
                else
                {
                    if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
                    {
                        if ((bool)tag["properties"]["tagVisible"] != false)
                        {
                            tag["properties"]["tagVisible"] = false;
                            returnresult = true;
                        }
                    }
                    else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
                    {
                        if ((bool)tag["properties"]["tagVisible"] != true)
                        {
                            tag["properties"]["tagVisible"] = true;
                        }
                    }
                    return returnresult;
                }

                //string tag_id = (string)tag["properties"]["id"];

                //if (Global.Tag.ContainsKey(tag_id))
                //{
                //    if (Global.Tag.TryGetValue(tag_id, out JObject tagInfo))
                //    {
                //        tagInfo["properties"]["Tag_Update"] = false;
                //        return true;
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}
                //else
                //{
                //    return false;
                //}
                //string tag_id = (string)tag["properties"]["id"];
                //if (Global.Tag.ContainsKey(tag_id))
                //{
                //    if (Global.Tag.TryGetValue(tag_id, out JObject tagInfo))
                //    {
                //        double tagVisibleRange = Global.AppSettings.ContainsKey("POSITION_MAX_AGE") ? !string.IsNullOrEmpty((string)Global.AppSettings.Property("POSITION_MAX_AGE").Value) ? (long)Global.AppSettings.Property("POSITION_MAX_AGE").Value : 10000 : 10000;

                //        System.TimeSpan tagdiffResult = DateTime.Now.ToUniversalTime().Subtract(((DateTime)((JObject)tagInfo["properties"]).Property("positionTS").Value).ToUniversalTime());
                //        if ((bool)((JObject)tagInfo["properties"]).Property("Tag_Update").Value)
                //        {
                //            if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
                //            {
                //                if ((bool)((JObject)tagInfo["properties"]).Property("tagVisible").Value)
                //                {
                //                    ((JObject)tagInfo["properties"]).Property("Tag_Update").Value = false;
                //                    ((JObject)tagInfo["properties"]).Property("tagVisible").Value = false;
                //                    return true;
                //                }
                //            }
                //                ((JObject)tagInfo["properties"]).Property("Tag_Update").Value = false;
                //            return true;
                //        }
                //        else
                //        {
                //            if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
                //            {
                //                if ((bool)((JObject)tagInfo["properties"]).Property("tagVisible").Value)
                //                {
                //                    ((JObject)tagInfo["properties"]).Property("Tag_Update").Value = false;
                //                    ((JObject)tagInfo["properties"]).Property("tagVisible").Value = false;
                //                    return true;
                //                }
                //            }
                //            return false;
                //        }

                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}
                //else
                //{
                //    return false;
                //}
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
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
                if (!_connections.GetConnections(connectionId).Contains(connectionId))
                {
                    _connections.Add(connectionId, connectionId);
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal IEnumerable<JToken> GetAPIList(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Global.API_List.Select(x => x.Value);
                }
                else
                {
                    return Global.API_List.Where(r => (int)r.Value.Property("ID").Value == id).Select(x => x.Value);
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> AddAPI(string data)
        {
            int id = 0;
            try
            {
                bool fileUpdate = false;
                if (!string.IsNullOrEmpty(data))
                {
                    if (Global.IsValidJson(data))
                    {
                        JObject objectdata = JObject.Parse(data);
                        if (objectdata.HasValues)
                        {
                            if (!string.IsNullOrEmpty(Global.AppSettings["FACILITY_NASS_CODE"].ToString()))
                            {
                                objectdata["NASS_CODE"] = Global.AppSettings["FACILITY_NASS_CODE"].ToString();
                            }
                            
                            objectdata["ID"] = Global.API_List.Count > 0 ? (Global.API_List.Count() + 1) : 1;
                            id = (int)objectdata["ID"];
                            JObject api = new JObject_List().API;
                            api.Merge(objectdata, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                            if (Global.API_List.TryAdd((int)api["ID"], api))
                            {
                                fileUpdate = true;
                            }

                        }
                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "API_Connection.json", JsonConvert.SerializeObject(Global.API_List.Select(x => x.Value), Formatting.Indented));
                }
                return Global.API_List.Where(w => w.Key == id).Select(s => s.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return Global.API_List.Select(s => s.Value).ToList();
            }
        }

        internal IEnumerable<JToken> EditAPI(string data)
        {
            int id = 0;
            try
            {
                bool fileUpdate = false;
                if (!string.IsNullOrEmpty(data))
                {
                    if (Global.IsValidJson(data))
                    {
                        JObject objectdata = JObject.Parse(data);
                        if (objectdata.HasValues)
                        {
                            objectdata["LASTUP_DATE"] = DateTime.Now;
                            objectdata["UPDATE_STATUS"] = true;
                            if (objectdata.ContainsKey("ID"))
                            {
                                id = (int)objectdata["ID"];
                                if (Global.API_List.ContainsKey((int)objectdata.Property("ID").Value))
                                {
                                    if (Global.API_List.TryGetValue((int)objectdata.Property("ID").Value, out JObject oldobjectdata))
                                    {
                                        oldobjectdata.Merge(objectdata, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                        fileUpdate = true;
                                        if (!(bool)oldobjectdata["ACTIVE_CONNECTION"])
                                        {
                                            oldobjectdata["API_CONNECTED"] = false;
                                            if ((bool)oldobjectdata["UDP_CONNECTION"])
                                            {
                                                Global.StopUdpClient();
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "API_Connection.json", JsonConvert.SerializeObject(Global.API_List.Select(x => x.Value), Formatting.Indented));
                }
                return Global.API_List.Where(w => w.Key == id).Select(s => s.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return Global.API_List.Select(s => s.Value).ToList();
            }
        }

        internal IEnumerable<JToken> RemoveAPI(string data)
        {
            try
            {
                JObject api = JObject.Parse(data);
                if (api.HasValues)
                {
                    if (api.ContainsKey("ID"))
                    {
                        if (Global.API_List.TryRemove((int)api["ID"], out JObject outtemp))
                        {
                            new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "API_Connection.json", JsonConvert.SerializeObject(Global.API_List.Select(x => x.Value), Formatting.Indented));
                            return Global.API_List.Select(e => e.Value).ToList();
                        }
                        else
                        {
                            return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                        }
                    }
                    else
                    {
                        return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                    }
                }
                else
                {
                    return new JObject(new JProperty("ERROR_MESSAGE", "ID not found in list"));
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject(new JProperty("ERROR_MESSAGE", "Error Deleting data"));
            }
        }
        internal IEnumerable<JToken> EditZone(string data)
        {
            string id = "0";
            try
            {
                bool fileUpdate = false;
                if (!string.IsNullOrEmpty(data))
                {
                    if (Global.IsValidJson(data))
                    {
                        JObject objectdata = JObject.Parse(data);
                        if (objectdata.HasValues)
                        {
                            if (objectdata.ContainsKey("id"))
                            {
                                id = objectdata["id"].ToString();
                                if (Global.Zones.ContainsKey(objectdata["id"].ToString()))
                                {
                                    if (Global.Zones.TryGetValue(objectdata["id"].ToString(), out JObject oldobjectdata))
                                    {
                                        ((JObject)oldobjectdata["properties"]).Merge(objectdata, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                       
                                        if ((bool)oldobjectdata["properties"]["nameOverride"] != true)
                                        {
                                            oldobjectdata["properties"]["nameOverride"] = true;
                                        }
                                        oldobjectdata["properties"]["name"] = oldobjectdata["properties"]["MPE_Type"] + "-"
                                            + oldobjectdata["properties"]["MPE_Number"].ToString().PadLeft(3, '0');

                                        fileUpdate = true;
                                        if (fileUpdate)
                                        {
                                            oldobjectdata["properties"]["Zone_Update"] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "Zones.json", JsonConvert.SerializeObject(Global.Zones.Select(x => x.Value).ToList(), Formatting.Indented));
                }
                return Global.Zones.Where(w => w.Key == id).Select(s => s.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return Global.Zones.Select(s => s.Value).ToList();
            }
        }
        internal IEnumerable<JToken> GetAppSettingdata()
        {
            try
            {
                JObject tempsetting = (JObject)Global.AppSettings.DeepClone();
                foreach (var item in Global.AppSettings)
                {
                    if (item.Key.StartsWith("ORACONN"))
                    {
                        tempsetting.Property(item.Key).Value = Global.Decrypt(item.Value.ToString());
                    }
                }
                return tempsetting;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        internal IEnumerable<JToken> EditAppSettingdata(string data)
        {
            try
            {
                bool fileUpdate = false;
                if (!string.IsNullOrEmpty(data))
                {
                    if (Global.IsValidJson(data))
                    {
                        JObject objdict = JObject.Parse(data);
                        foreach (dynamic item in Global.AppSettings)
                        {
                            foreach (var kv in objdict)
                            {
                                if (kv.Key == item.Key)
                                {
                                    if (kv.Value != item.Value)
                                    {
                                        fileUpdate = true;

                                        if (kv.Key.StartsWith("ORACONN"))
                                        {
                                            Global.AppSettings.Property(item.Key).Value = Global.Encrypt(kv.Value.ToString());
                                        }
                                        else if (kv.Key == "FACILITY_NASS_CODE")
                                        {
                                            if (GetData.Get_Site_Info((string)kv.Value, out JObject SiteInfo))
                                            {
                                                if (SiteInfo.HasValues)
                                                {
                                                    Global.AppSettings.Property(item.Key).Value = kv.Value.ToString();
                                                    Global.AppSettings.Property("FACILITY_NAME").Value = SiteInfo.ContainsKey("displayName") ? SiteInfo.Property("displayName").Value : "";
                                                    Global.AppSettings.Property("FACILITY_ID").Value = SiteInfo.ContainsKey("fdbId") ? SiteInfo.Property("fdbId").Value : "";
                                                    Global.AppSettings.Property("FACILITY_ZIP").Value = SiteInfo.ContainsKey("zipCode") ? SiteInfo.Property("zipCode").Value : "";
                                                    Global.AppSettings.Property("FACILITY_LKEY").Value = SiteInfo.ContainsKey("localeKey") ? SiteInfo.Property("localeKey").Value : "";
                                                }
                                            }
                                        }
                                        else if (kv.Key == "FACILITY_NASS_CODE")
                                        {
                                            if (GetData.Get_Site_Info((string)kv.Value, out JObject SiteInfo))
                                            {
                                                if (SiteInfo.HasValues)
                                                {
                                                    Global.AppSettings.Property(item.Key).Value = kv.Value.ToString();
                                                    Global.AppSettings.Property("FACILITY_NAME").Value = SiteInfo.ContainsKey("displayName") ? SiteInfo.Property("displayName").Value : "";
                                                    Global.AppSettings.Property("FACILITY_ID").Value = SiteInfo.ContainsKey("fdbId") ? SiteInfo.Property("fdbId").Value : "";
                                                    Global.AppSettings.Property("FACILITY_ZIP").Value = SiteInfo.ContainsKey("zipCode") ? SiteInfo.Property("zipCode").Value : "";
                                                    Global.AppSettings.Property("FACILITY_LKEY").Value = SiteInfo.ContainsKey("localeKey") ? SiteInfo.Property("localeKey").Value : "";
                                                }
                                            }
                                        }
                                        else if (kv.Key == "LOG_LOCATION")
                                        {
                                            if (!string.IsNullOrEmpty(kv.Value.ToString()))
                                            {
                                                Global.AppSettings.Property(item.Key).Value = kv.Value.ToString();
                                                Global.Logdirpath = new DirectoryInfo(kv.Value.ToString());
                                                new Directory_Check().DirPath(Global.Logdirpath);
                                               
                                            }
                                        }
                                        else
                                        {
                                            Global.AppSettings.Property(item.Key).Value = kv.Value.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(Global.CodeBase.Parent.FullName.ToString(), Global.Appsetting), "AppSettings.json", JsonConvert.SerializeObject(Global.AppSettings, Formatting.Indented));
                    new FileIO().Write(string.Concat(Global.Logdirpath, Global.ConfigurationFloder), "AppSettings.json", JsonConvert.SerializeObject(Global.AppSettings, Formatting.Indented));
                }
                return Global.AppSettings;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return Global.AppSettings;
            }
        }

        internal void Removeuser(string connectionId)
        {
            try
            {
                _connections.Remove(connectionId, connectionId);

                Global._users.Where(r => r.Value["ConnectionId"].ToString() == connectionId).Select(z => z.Value).ToList().ForEach(user =>
                {
                    string data = string.Concat("Client closed the connection. User ID:" + user["UserId"].ToString()," | Connection ID: : " + connectionId);
                    new ErrorLogger().CustomLog(data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                    new User_Log().LogoutUser(Global.CodeBase.Parent.FullName.ToString(), user);
                   
                });

                //remove users 
                Global._users.Where(r => ((DateTime)r.Value["Login_Date"]).Subtract(DateTime.Now).TotalDays > 2).Select(z => z.Value).ToList().ForEach(user =>
                {
                        user.Remove();
                });

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
                    Ipaddress = Ipaddress.ToString().StartsWith("::") ? "127.0.0.1" : Ipaddress;
                    string user_id = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim();
                  
                    string data = string.Concat("Client has connected. User ID:" + user_id," | Connection ID: : " + Context.ConnectionId);
                    new ErrorLogger().CustomLog(data, string.Concat((string)Global.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));

                    if (Global._users.ContainsKey(user_id))
                    {
                        if (Global._users.TryGetValue(user_id, out JObject user))
                        {
                            user.Property("ConnectionId").Value = Context.ConnectionId;
                            user.Property("NASS_Code").Value = ((string)user.Property("NASS_Code").Value != Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString()) ? Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString() : Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString();
                            user.Property("FDB_ID").Value = ((string)user.Property("FDB_ID").Value != Global.AppSettings.Property("FACILITY_ID").Value.ToString()) ? Global.AppSettings.Property("FACILITY_ID").Value.ToString() : Global.AppSettings.Property("FACILITY_ID").Value.ToString();
                            user.Property("Facility_Name").Value = ((string)user.Property("Facility_Name").Value != Global.AppSettings.Property("FACILITY_NAME").Value.ToString()) ? Global.AppSettings.Property("FACILITY_NAME").Value.ToString() : Global.AppSettings.Property("FACILITY_NAME").Value.ToString();
                            user.Property("Facility_TimeZone").Value = ((string)user.Property("Facility_TimeZone").Value != Global.AppSettings.Property("FACILITY_TIMEZONE").Value.ToString()) ? Global.AppSettings.Property("FACILITY_TIMEZONE").Value.ToString() : Global.AppSettings.Property("FACILITY_TIMEZONE").Value.ToString();
                            user.Property("App_Type").Value = ((string)user.Property("App_Type").Value != Global.AppSettings.Property("APPLICATION_NAME").Value.ToString()) ? Global.AppSettings.Property("APPLICATION_NAME").Value.ToString() : Global.AppSettings.Property("APPLICATION_NAME").Value.ToString();
                            user.Property("Login_Date").Value = DateTime.Now;
                           //log of user logging in.
                           new User_Log().LoginUser(Global.CodeBase.Parent.FullName.ToString(), user);
                        }
                    }
                    else
                    {
                        JObject new_user = new JObject_List().ADuser;
                        new_user.Property("GroupNames").Value = GetGroupNames(((WindowsIdentity)Context.User.Identity).Groups);
                        new_user.Property("Session_ID").Value = Context.ConnectionId;
                        new_user.Property("UserId").Value = user_id;
                        new_user.Property("Domain").Value = !string.IsNullOrEmpty(Context.User.Identity.Name) ? Context.User.Identity.Name.Split('\\')[0].ToLower() : "";
                        new_user.Property("ConnectionId").Value = Context.ConnectionId;
                        new_user.Property("IpAddress").Value = Ipaddress.ToString();
                        new_user.Property("Server_IpAddress").Value = Global.ipaddress.ToString();
                        new_user.Property("NASS_Code").Value = Global.AppSettings.Property("FACILITY_NASS_CODE").Value.ToString();
                        new_user.Property("FDB_ID").Value = Global.AppSettings.Property("FACILITY_ID").Value.ToString();
                        new_user.Property("Facility_Name").Value = Global.AppSettings.Property("FACILITY_NAME").Value.ToString();
                        new_user.Property("Facility_TimeZone").Value = Global.AppSettings.Property("FACILITY_TIMEZONE").Value.ToString();
                        new_user.Property("Environment").Value = Global.Application_Environment;
                        new_user.Property("IsAuthenticated").Value = Context.User.Identity.IsAuthenticated;
                        new_user.Property("Software_Version").Value = Global.VersionInfo;
                        new_user.Property("App_Type").Value = Global.AppSettings.Property("APPLICATION_NAME").Value.ToString();
                        new_user.Property("Browser_Type").Value = HttpContext.Current.Request.Browser.Type;
                        new_user.Property("Browser_Name").Value = HttpContext.Current.Request.Browser.Browser;
                        new_user.Property("Browser_Version").Value = HttpContext.Current.Request.Browser.Version;
                        new_user.Property("Role").Value = "Operator";
                        new_user.Property("Role").Value = GetUserRole((string)new_user.Property("GroupNames").Value);
                        if (new FindACEUser().User(new_user, out JObject ADuser))
                        {
                            new_user.Property("EmailAddress").Value = ADuser.Property("EmailAddress").Value;
                            new_user.Property("Phone").Value = ADuser.Property("Phone").Value;
                            new_user.Property("MiddleName").Value = ADuser.Property("MiddleName").Value;
                            new_user.Property("SurName").Value = ADuser.Property("SurName").Value;
                            new_user.Property("FirstName").Value = ADuser.Property("FirstName").Value;
                            new_user.Property("ZipCode").Value = ADuser.Property("ZipCode").Value;
                        }
                        //log of user logging in.
                        new User_Log().LoginUser(Global.CodeBase.Parent.FullName.ToString(), new_user);

                        Global._users.TryAdd(user_id, new_user);
                    }
                    _connections.Add(Context.ConnectionId, Context.ConnectionId);
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        private string GetGroupNames(IdentityReferenceCollection groups)
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

                    string temp_list = Global.AppSettings.ContainsKey("ROLES_ADMIN") ? Global.AppSettings.Property("ROLES_ADMIN").Value.ToString().Trim() : "";
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
                    temp_list = Global.AppSettings.ContainsKey("ROLES_OIE") ? Global.AppSettings.Property("ROLES_OIE").Value.ToString().Trim() : "";
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
                    temp_list = Global.AppSettings.ContainsKey("ROLES_MAINTENANCE") ? Global.AppSettings.Property("ROLES_MAINTENANCE").Value.ToString().Trim() : "";
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
    }
}