using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Drawing;

namespace Factory_of_the_Future
{

    public class FOTFManager : IDisposable
    {
        private readonly static Lazy<FOTFManager> _instance = new Lazy<FOTFManager>(() => new FOTFManager(GlobalHost.ConnectionManager.GetHubContext<HubManager>().Clients));


        //blocks
        private readonly object updateZoneStatuslock = new object();
        private readonly object updateTagStatuslock = new object();
        private readonly object updatePersonTagStatuslock = new object();
        private readonly object updateDockDoorStatuslock = new object();
        private readonly object updateQSMlock = new object();
        private readonly object updateMachineStatuslock = new object();
        private readonly object updateAGVLocationStatuslock = new object();
        private readonly object updateSVTripsStatuslock = new object();
        private readonly object updateNotificationStatuslock = new object();
        private readonly object updateBinZoneStatuslock = new object();
        private readonly object updateCameralock = new object();
        
        //timers
        private readonly Timer VehicleTag_timer;
        private readonly Timer PersonTag_timer;
        private readonly Timer Zone_timer;
        private readonly Timer DockDoor_timer;
        private readonly Timer QSM_timer;
        private readonly Timer Machine_timer;
        private readonly Timer AGVLocation_timer;
        private readonly Timer SVTrips_timer;
        private readonly Timer Notification_timer;
        private readonly Timer BinZone_timer;
        private readonly Timer Camera_timer;
        private readonly Timer SVZone_timer;
        //status
        private volatile bool _updatePersonTagStatus = false;
        private volatile bool _updateZoneStatus = false;
        private volatile bool _updateTagStatus = false;
        private volatile bool _updateDockDoorStatus = false;
        private volatile bool _updatingQSMStatus = false;
        private volatile bool _updateMachineStatus = false;
        private volatile bool _updateAGVLocationStatus = false;
        private volatile bool _updateSVTripsStatus = false;
        private volatile bool _updateNotificationstatus = false;
        private volatile bool _updateBinZoneStatus = false;
        private volatile bool _updateCameraStatus = false;
        private bool disposedValue;
        private HttpClient httpClient = new HttpClient();
        //250 Milliseconds
        private readonly TimeSpan _250updateInterval = TimeSpan.FromMilliseconds(250);
        //1 seconds
        private readonly TimeSpan _1000updateInterval = TimeSpan.FromMilliseconds(1000);
        //2 seconds
        private readonly TimeSpan _2000updateInterval = TimeSpan.FromMilliseconds(2000);
        //30 seconds
        private readonly TimeSpan _30000updateInterval = TimeSpan.FromMilliseconds(30000);
        //10 seconds
        private readonly TimeSpan _10000updateInterval = TimeSpan.FromMilliseconds(10000);
        //60 seconds
        private readonly TimeSpan _60000updateInterval = TimeSpan.FromMilliseconds(60000);


        //init timers
        private FOTFManager(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
            VehicleTag_timer = new Timer(UpdateVehicleTagStatus, null, _250updateInterval, _250updateInterval);
            PersonTag_timer = new Timer(UpdatePersonTagStatus, null, _250updateInterval, _250updateInterval);
            /////Zone status.
            Zone_timer = new Timer(UpdateZoneStatus, null, _2000updateInterval, _2000updateInterval);
            DockDoor_timer = new Timer(UpdateDockDoorStatus, null, _250updateInterval, _250updateInterval);
            Machine_timer = new Timer(UpdateMachineStatus, null, _2000updateInterval, _2000updateInterval);
            AGVLocation_timer = new Timer(UpdateAGVLocationStatus, null, _250updateInterval, _250updateInterval);
            BinZone_timer = new Timer(UpdateBinZoneStatus, null, _2000updateInterval, _2000updateInterval);
            /////SV Trips Data
            SVTrips_timer = new Timer(UpdateSVTripsStatus, null, _30000updateInterval, _30000updateInterval);
            ////   Notification data timer
            Notification_timer = new Timer(UpdateNotificationtatus, null, _1000updateInterval, _1000updateInterval);
            ////
            //Connection status
            QSM_timer = new Timer(UpdateQSM, null, _250updateInterval, _250updateInterval);
            //Camera update;
            Camera_timer = new Timer(UpdateCameraImages, null, _10000updateInterval, _60000updateInterval);
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
        internal GeoZone AddZone(string data)
        {
            try
            {

                GeoZone newtempgZone = JsonConvert.DeserializeObject<GeoZone>(data);
                newtempgZone.Properties.Id = Guid.NewGuid().ToString();
                newtempgZone.Properties.RawData = "";
                newtempgZone.Properties.Source = "user";
                if (newtempgZone.Properties.ZoneType == "Bin")
                {
                    newtempgZone.Properties.MPEBins = new List<string>();
                }
                else
                {
                    newtempgZone.Properties.MPEBins = null;
                }
                if (Regex.IsMatch(newtempgZone.Properties.ZoneType, "(Machine|Bin)", RegexOptions.IgnoreCase))
                {
                    //get the MPE Number
                    if (int.TryParse(string.Join(string.Empty, Regex.Matches(newtempgZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                    {
                        newtempgZone.Properties.MPENumber = n;
                    }
                    //get the MPE Name
                    newtempgZone.Properties.MPEType = string.Join(string.Empty, Regex.Matches(newtempgZone.Properties.Name, @"\p{L}+").OfType<Match>().Select(m => m.Value));
                }
                if (Regex.IsMatch(newtempgZone.Properties.ZoneType, "(DockDoor)", RegexOptions.IgnoreCase))
                {
                    //get the DockDoor Number
                    if (int.TryParse(string.Join(string.Empty, Regex.Matches(newtempgZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                    {
                        newtempgZone.Properties.DoorNumber = n.ToString();
                    }

                }
                newtempgZone.Properties.MPEWatchData = null;
                newtempgZone.Properties.MissionList = null;
                newtempgZone.Properties.DockDoorData = null;
                newtempgZone.Properties.StaffingData = null;
                newtempgZone.Properties.DPSData = null;
                newtempgZone.Properties.ZoneUpdate = true;

                if (AppParameters.CoordinateSystem.ContainsKey(newtempgZone.Properties.FloorId))
                {
                    if (AppParameters.CoordinateSystem.TryGetValue(newtempgZone.Properties.FloorId, out CoordinateSystem cs))
                    {
                        if (cs.Zones.TryAdd(newtempgZone.Properties.Id, newtempgZone))
                        {
                            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(AppParameters.CoordinateSystem.Select(x => x.Value).ToList()));
                            return newtempgZone;
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

        internal IEnumerable<SV_Bullpen> GetSVZoneNameList()
        {
            try
            {
                return AppParameters.SVZoneNameList.Select(y =>  y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<string> GetDockDoorList()
        {
            try
            {
                return AppParameters.DockdoorList.Select(y => y.Key).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<string> GetMPEList()
        {
            try
            {
                return AppParameters.MPEPerformanceList.Select(y => y.Key).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        public void UpdateCameraImages(object state)
        {
            try
            {
                lock (updateCameralock)
                {
                    if (!_updateCameraStatus)
                    {
                        foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                        {
                            cs.Locators.Where(f => f.Value.Properties.TagType != null &&
                            f.Value.Properties.TagType == "Camera").Select(y => y.Value).ToList().ForEach(Camera =>
                            {
                                if (Camera.Properties.CameraData == null)
                                {
                                    Cameras cam = new Cameras();
                                    if (AppParameters.CameraInfoList.TryGetValue(Camera.Properties.Name, out Cameras existingValue))
                                    {
                                    
                                        cam.FacilitySubtypeDesc = existingValue.FacilitySubtypeDesc;
                                        cam.AuthKey = existingValue.AuthKey;
                                        cam.Description = existingValue.Description;
                                        cam.FacilitiyLatitudeNum = existingValue.FacilitiyLatitudeNum;
                                        cam.FacilitiyLongitudeNum = existingValue.FacilitiyLongitudeNum;
                                        cam.FacilityDisplayName = existingValue.FacilityDisplayName;
                                        cam.FacilityPhysAddrTxt = existingValue.FacilityPhysAddrTxt;
                                        cam.GeoProcDivisionNm = existingValue.GeoProcDivisionNm;
                                        cam.GeoProcRegionNm = existingValue.GeoProcRegionNm;
                                        cam.LocaleKey = existingValue.LocaleKey;
                                        cam.ModelNum = existingValue.ModelNum;
                                        cam.Reachable = existingValue.Reachable;
                                        cam.CameraName = existingValue.CameraName;
                                        cam.Base64Image = AppParameters.NoImage;
                                        cam.Alerts = null;
                                    }
                                    else
                                    {
                                        cam.Base64Image = AppParameters.NoImage;
                                        cam.CameraName = Camera.Properties.Name;
                                    }
                                    Camera.Properties.CameraData = cam;
                                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(AppParameters.CoordinateSystem.Select(x => x.Value).ToList()));

                                }
                            if (Camera.Properties.CameraData.Base64Image != TryUpdateCameraStatus(Camera.Properties.CameraData.CameraName, out string Base64Img))
                            {
                                Camera.Properties.CameraData.Base64Image = Base64Img;
                                BroadcastCameraStatus(Camera, cs.Id);
                            }
                            });
                        }
                        _updateCameraStatus = false;
                    }
                }
                // UpdateCameraImages();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);

                _updateCameraStatus = false;
            }

        }

        public void BroadcastCameraStatus(GeoMarker Cameras, string id)
        {
            Clients.Group("CameraMarkers").updateCameraStatus(Cameras,id);
        }
        //public void UpdateCameraImages(object state)
        //{
        //    try
        //    {
        //        lock (updateCameralock)
        //        {

        //            if (!_updateCameraStatus)
        //            {
        //                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
        //                {
        //                    cs.Locators.Where(f => f.Value.Properties.TagType != null &&
        //                    f.Value.Properties.TagType == "Camera").Select(y => y.Value).ToList().ForEach(Camera =>
        //                    {
        //                        if (TryUpdateCameraStatus(Camera))
        //                        {
        //                            if (AppParameters.CameraInfoList[Camera.Properties.Name].Alerts == null)
        //                            {
        //                                Camera.Properties.DarvisAlerts = new List<DarvisCameraAlert>();
        //                            }
        //                            else
        //                            {
        //                                Camera.Properties.DarvisAlerts = AppParameters.CameraInfoList[Camera.Properties.Name].Alerts.ToList();
        //                            }
        //                            BroadcastCameraStatus(Camera, cs.Id);

        //                        }

        //                    });
        //                }
        //                _updateCameraStatus = false;
        //            }
        //        }
        //        // UpdateCameraImages();
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);

        //        _updateCameraStatus = false;
        //    }

        //}

        //public void BroadcastCameraStatus(GeoMarker CameraZone, string id)
        //{
        //    Clients.Group("CameraMarkers").updateCameraStatus(CameraZone, id);
        //}

        private static string TryUpdateCameraStatus(string camera, out string imageBase64)
        {
            imageBase64 = AppParameters.NoImage;
            try
            {

                string url = @"http://" + camera + @"/axis-cgi/jpg/image.cgi?resolution=320x240";
                Uri thisUri = new Uri(url);

                using (WebClient client = new WebClient())
                {
                    try
                    {
                     
                        client.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");

                        //add header
                        byte[] result = client.DownloadData(url);
                        imageBase64 = "data:image/jpeg;base64," + Convert.ToBase64String(result);
                        //if (camera.Properties.Base64Image != imageBase64)
                        //{
                        return imageBase64;
                        //}
                      

                    }
                    catch (ArgumentException ae) {
                        new ErrorLogger().ExceptionLog(ae);
                        return AppParameters.NoImage;
                    }
                    catch (WebException we) {
                        new ErrorLogger().ExceptionLog(we);
                        return AppParameters.NoImage;
                    }
                   
                }
            }
    
            catch (WebException we) {
                new ErrorLogger().ExceptionLog(we);
                return AppParameters.NoImage;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return AppParameters.NoImage;
            }
        }

        //public void UpdateCameraImages()
        //{
        //    lock (updateCameralock)
        //    {
        //        if (!_updateCameraStatus)
        //        {
        //            try
        //            {
        //                List<Task> TaskList = new List<Task>();
        //                _updateCameraStatus = true;
        //                foreach (KeyValuePair<string, GeoMarker> geoMarkerEntry in AppParameters.TagsList)
        //                {
        //                    if (!String.IsNullOrEmpty(geoMarkerEntry.Value.Properties.CameraData))
        //                    {
        //                        Func<object, Task<bool>> action = (object cameraData) =>
        //                        {
        //                            return UpdateCameraImage((GeoMarker) cameraData);
        //                        };
        //                        TaskList.Add(action(geoMarkerEntry.Value));
        //                    }
        //                }
        //                // run the tasks "simultaneously" but wait for all to complete
        //                Task.WaitAll(TaskList.ToArray());
        //                var markers = AppParameters.TagsList
        //                    .Where(u => !String.IsNullOrEmpty(u.Value.Properties.CameraData));

        //                List<GeoMarker> cameraList = new List<GeoMarker>();
        //                foreach (KeyValuePair<string, GeoMarker> tag in AppParameters.TagsList)
        //                {
        //                    if (! String.IsNullOrEmpty(tag.Value.Properties.CameraData) )
        //                    {
        //                        cameraList.Add(tag.Value);
        //                    }
        //                }
        //                BroadcastCameraStatus(cameraList);
        //            }
        //            catch (Exception e)
        //            {
        //                new ErrorLogger().ExceptionLog(e);
        //            }
        //            _updateCameraStatus = false;
        //        }
        //    }
        //}
        //internal async Task<bool> UpdateCameraImage(GeoMarker cameraData)
        //{
        //    try
        //    {
        //        string url = @"http://" + cameraData.Properties.Name + 
        //            @"/axis-cgi/jpg/image.cgi?resolution=320x240";

        //        using (var client = new HttpClient())
        //        {
        //            Uri thisUri = new Uri(url);
        //            cameraData.Properties.Base64Image =
        //                AsyncAPICall.GetImageData(thisUri);

        //            Console.WriteLine("image gotten: " + cameraData.Properties.Base64Image.Substring(0, 100));
        //            return true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Exception getting image: " + e.Message);
        //        new ErrorLogger().ExceptionLog(e);
        //        return false;
        //    }
        //}

        internal GeoMarker AddMarker(string data)
        {

            try
            {
                GeoMarker newtempgMarker = JsonConvert.DeserializeObject<GeoMarker>(data);
                newtempgMarker.Properties.Id = Guid.NewGuid().ToString();
                newtempgMarker.Properties.RawData = "";
                newtempgMarker.Properties.Source = "user";
                newtempgMarker.Properties.TagUpdate = true;

                if (AppParameters.CameraInfoList.TryGetValue(newtempgMarker.Properties.Name, out Cameras Camera))
                {
                    newtempgMarker.Properties.EmpName = Camera.Description;
                    newtempgMarker.Properties.Emptype = Camera.ModelNum;
                    newtempgMarker.Properties.CameraData = Camera;

                }

                if (AppParameters.CoordinateSystem.ContainsKey(newtempgMarker.Properties.FloorId))
                {
                    if (AppParameters.CoordinateSystem.TryGetValue(newtempgMarker.Properties.FloorId, out CoordinateSystem cs))
                    {
                        if (cs.Locators.TryAdd(newtempgMarker.Properties.Id, newtempgMarker))
                        {
                            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(AppParameters.CoordinateSystem.Select(x => x.Value).ToList()));
                            return newtempgMarker;
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

        internal IEnumerable<string> GetTimeZone()
        {
            try
            {
                return AppParameters.TimeZoneConvert.Keys.ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }

        }

        internal string GetFacilityTimeZone()
        {
            return AppParameters.AppSettings["FACILITY_TIMEZONE"].ToString();
        }

        internal GeoMarker RemoveMarker(string data)
        {
            try
            {
                bool removeMarker = false;
                GeoMarker markerinfo = null;
                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                {
                    if (cs.Locators.ContainsKey(data))
                    {
                        if (cs.Locators.TryGetValue(data, out markerinfo))
                        {
                            if (markerinfo.Properties.Source == "other")
                            {
                                markerinfo.Properties.TagVisible = false;
                                markerinfo.Properties.TagUpdate = true;
                            }
                            else
                            {
                                removeMarker = true;
                            }
                        }
                        if (removeMarker)
                        {
                            if (cs.Locators.TryRemove(data, out markerinfo))
                            {
                                new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(AppParameters.CoordinateSystem.Select(x => x.Value).ToList()));
                            }
                        }
                    }
                }
                return markerinfo;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal GeoZone RemoveZone(string data)
        {
            try
            {
                bool removeZone = false;
                GeoZone ZoneInfo = null;
                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                {
                    if (cs.Zones.ContainsKey(data))
                    {
                        if (cs.Zones.TryGetValue(data, out ZoneInfo))
                        {
                            if (ZoneInfo.Properties.Source == "other")
                            {
                                ZoneInfo.Properties.Visible = false;
                                ZoneInfo.Properties.ZoneUpdate = true;
                            }
                            else
                            {
                                removeZone = true;
                            }
                        }
                        if (removeZone)
                        {
                            if (cs.Zones.TryRemove(data, out ZoneInfo))
                            {
                                new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(AppParameters.CoordinateSystem.Select(x => x.Value).ToList()));
                            }
                        }
                    }
                }

                return ZoneInfo;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }


        internal IEnumerable<Notification> GetNotification(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    return AppParameters.NotificationList.Where(c => c.Value.Type.ToLower() == data.ToLower()
                    && c.Value.ActiveCondition).Select(x => x.Value).ToList();
                }
                else
                {
                    if (AppParameters.NotificationList.Count() > 0)
                    {
                        return AppParameters.NotificationList.Select(x => x.Value).ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        //internal ADUser GetUserProfile(string conID)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(conID))
        //        {
        //            if (AppParameters.Users.ContainsKey(conID))
        //            {
        //                if (AppParameters.Users.TryGetValue(conID, out ADUser user))
        //                {
        //                    return user;
        //                }
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

        private void UpdateNotificationtatus(object state)
        {
            lock (updateNotificationStatuslock)
            {
                if (!_updateNotificationstatus)
                {
                    _updateNotificationstatus = true;
                    foreach (var notification in from notification in AppParameters.NotificationList.Select(y => y.Value)
                                                 where TryUpdateNotificationStatus(notification)
                                                 select notification)
                    {
                        BroadcastNotificationStatus(notification);
                        notification.Notification_Update = false;
                    }

                    _updateNotificationstatus = false;
                }
            }
        }

        private bool TryUpdateNotificationStatus(Notification notification)
        {
            try
            {
                //**MPE Notifications duration calculated by MPEWatch.
                if (notification.Type == "mpe" && !notification.Delete)
                {
                    return notification.Notification_Update;
                }
                if(notification.Type == "dockdoor" && !notification.Delete && notification.Type_Duration == 0)
                {
                    return notification.Notification_Update;
                }
                notification.Notification_Update = false;

                if (notification.Delete)
                {
                    if (AppParameters.NotificationList.TryRemove(notification.Notification_ID, out Notification notifi))
                    {
                        return true;
                    }
                }
                int duration = AppParameters.Get_NotificationTTL(notification.Type_Time, DateTime.Now);

                if (duration > notification.Type_Duration)
                {
                    notification.Type_Duration = duration;
                    return true;
                }
                return false;
                //if ((bool)notification.Property("ACTIVE_CONDITION").Value)
                //{
                //    if (notification.ContainsKey("DELETE"))
                //    {
                //        if (AppParameters.NotificationList.TryRemove((string)notification.Property("notificationId").Value, out JObject outnotification))
                //        {
                //            return true;
                //        }
                //        else
                //        {
                //            return false;
                //        }
                //    }
                //    else
                //    {
                //        return true;
                //    }
                //}
                //else
                //{
                //    if (AppParameters.NotificationList.TryRemove((string)notification.Property("notificationId").Value, out JObject outnotification))
                //    {
                //        if (!notification.ContainsKey("DELETE"))
                //        {
                //            notification.Add(new JProperty("DELETE", true));
                //        }

                //        return true;
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void BroadcastNotificationStatus(Notification notification)
        {
            Clients.All.updateNotification(notification);
        }

        internal IEnumerable<NotificationConditions> GetNotification_ConditionsList()
        {
            try
            {
                return AppParameters.NotificationConditionsList.Select(x => x.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<NotificationConditions> GetNotification_Conditions(string id)
        {
            try
            {
                return AppParameters.NotificationConditionsList.Where(c => c.Value.Id == id).Select(x => x.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<NotificationConditions> AddNotification_Conditions(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    JObject Notifation = JObject.Parse(data);
                    if (Notifation.HasValues)
                    {
                        NotificationConditions newNotification = Notifation.ToObject<NotificationConditions>();
                        newNotification.Id = Guid.NewGuid().ToString();
                        newNotification.CreatedDate = DateTime.Now;
                        if (AppParameters.NotificationConditionsList.TryAdd(newNotification.Id, newNotification))
                        {
                            //write to file the new connection
                            new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Notification.json", JsonConvert.SerializeObject(AppParameters.NotificationConditionsList.Select(x => x.Value).ToList(), Formatting.Indented));
                        }
                    }
                }
                return AppParameters.NotificationConditionsList.Select(e => e.Value).ToList();
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return null;
            }
        }

        internal IEnumerable<NotificationConditions> EditNotification_Conditions(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    JObject updatenotification = JObject.Parse(data);
                    if (updatenotification.HasValues)
                    {
                        if (updatenotification.ContainsKey("Id"))
                        {
                            if (AppParameters.NotificationConditionsList.TryGetValue(updatenotification["Id"].ToString(), out NotificationConditions notification))
                            {

                                JObject tempnotification = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(notification, Formatting.Indented));
                                tempnotification["LASTUP_DATE"] = DateTime.Now;
                                tempnotification.Merge(updatenotification, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

                                NotificationConditions newTempNotification = tempnotification.ToObject<NotificationConditions>();
                                if (AppParameters.NotificationConditionsList.TryUpdate(newTempNotification.Id, newTempNotification, notification))
                                {

                                    //foreach (JObject item in AppParameters.NotificationList.Where(r => (string)r.Value["conditionId"] == (string)updatenotification["id"]).Select(y => y.Value))
                                    //{
                                    //    if (!(bool)updatenotification["ACTIVE_CONDITION"])
                                    //    {
                                    //        item["DELETE"] = true;
                                    //        item["UPDATE"] = true;
                                    //    }
                                    //    item.Merge(updatenotification, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                                    //}
                                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Notification.json", JsonConvert.SerializeObject(AppParameters.NotificationConditionsList.Select(x => x.Value).ToList(), Formatting.Indented));
                                }
                            }
                        }

                    }
                }
                return AppParameters.NotificationConditionsList.Select(e => e.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal IEnumerable<NotificationConditions> DeleteNotification_Conditions(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    JObject Notification = JObject.Parse(data);
                    if (Notification.HasValues)
                    {
                        if (Notification.ContainsKey("Id"))
                        {
                            if (AppParameters.NotificationConditionsList.TryRemove(Notification["Id"].ToString(), out NotificationConditions outtemp))
                            {
                                new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Notification.json", JsonConvert.SerializeObject(AppParameters.NotificationConditionsList.Select(x => x.Value).ToList(), Formatting.Indented));
                            }

                        }

                    }

                }
                return AppParameters.NotificationConditionsList.Select(e => e.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

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

                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                    {
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "Area"
                        && f.Value.Properties.ZoneUpdate
                        && f.Value.Properties.Visible
                        ).Select(y => y.Value).ToList().ForEach(zoneitem =>
                        {
                            if (TryUpdateZoneStatus(zoneitem))
                            {
                                BroadcastZoneStatus(zoneitem, cs.Id);
                            }
                        });
                    }

                    _updateZoneStatus = false;
                }
            }
        }

        private void BroadcastZoneStatus(GeoZone zoneitem, string id)
        {
            Clients.Group("Zones").updateZoneStatus(zoneitem, id);
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


        private void UpdateBinZoneStatus(object state)
        {
            lock (updateBinZoneStatuslock)
            {
                if (!_updateBinZoneStatus)
                {
                    _updateBinZoneStatus = true;
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                    {
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "Bin" && f.Value.Properties.ZoneUpdate && f.Value.Properties.Visible).Select(y => y.Value).ToList().ForEach(BIN =>
                        {
                            if (TryUpdateBinZoneStatus(BIN))
                            {
                                BroadcastBinZoneStatus(BIN, cs.Id);
                            }

                        });
                    }

                    _updateBinZoneStatus = false;
                }
            }
        }


        private bool TryUpdateSVZoneStatus(GeoZone svZone)
        {
            try
            {

                svZone.Properties.ZoneUpdate = false;
                return true;

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private bool TryUpdateBinZoneStatus(GeoZone binZone)
        {
            try
            {

                binZone.Properties.ZoneUpdate = false;
                return true;

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void BroadcastBinZoneStatus(GeoZone binZone, string id)
        {
            Clients.Group("BinZones").updateBinZoneStatus(binZone, id);
        }

        private void BroadcastSVZoneStatus(GeoZone svZone, string id)
        {
            Clients.Group("SVZones").updateSVZoneStatus(svZone, id);
        }
        private void UpdateSVTripsStatus(object state)
        {
            lock (updateSVTripsStatuslock)
            {
                if (!_updateSVTripsStatus)
                {
                    _updateSVTripsStatus = true;
                    foreach (var trip in from RouteTrips trip in AppParameters.RouteTripsList.Select(x => x.Value)
                                         where TrySVTripStatus(trip)
                                         select trip)
                    {
                        BroadcastSVTripsStatus(trip);
                    }

                    _updateSVTripsStatus = false;
                }
            }
        }

        private bool TrySVTripStatus(RouteTrips trip)
        {
            bool update = false;
            string state = "ACTIVE";
            string routetripid = string.Concat(trip.RouteTripId, trip.RouteTripLegId, trip.TripDirectionInd);
            try
            {
                trip.TripUpdate = false;
                if (Regex.IsMatch(trip.LegStatus, "(CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE)", RegexOptions.IgnoreCase)
                    || Regex.IsMatch(trip.Status, "(CANCELED|DEPARTED|OMITTED|COMPLETE|REMOVE)", RegexOptions.IgnoreCase))
                {
                    trip.Containers = null;
                    trip.State = "REMOVE";
                    update = true;
                }
                //validate trip
                if (trip.State != "REMOVE")
                {
                    //trip minutes 
                    int tripInMin = AppParameters.Get_TripMin(trip.ScheduledDtm);
                    if (tripInMin != trip.TripMin)
                    {
                        trip.TripMin = tripInMin;
                        update = true;
                    }
                    //check the trip state is late     
                    if (tripInMin <= -1)
                    {
                        state = "LATE";
                        if (trip.State != "LATE")
                        {
                            update = true;
                        }
                    }
                    // remove trip if it has not been CANCELED|DEPARTED|OMITTED|COMPLETE after 24 hours
                    if (tripInMin <= -1440)
                    {
                        state = "REMOVE";
                        update = true;
                    }
                    // Notification check 

                    trip.NotificationId = CheckNotification(trip.State, state, "routetrip", trip, trip.NotificationId);

                    if (!string.IsNullOrEmpty(trip.DestSites))
                    {
                        trip.Containers = GetTripContainer(trip.DestSites, trip.TrailerBarcode, out int NotloadedContainers, out int loaded);
                        trip.NotloadedContainers = NotloadedContainers;
                    }
                    trip.State = state;
                }
                else
                {
                    //to remove data from list after CANCELED|DEPARTED|OMITTED|COMPLETE
                    if (AppParameters.RouteTripsList.TryRemove(routetripid, out RouteTrips r))
                    {
                        Task.Run(() => CheckNotification(trip.State, state, "routetrip", trip, trip.NotificationId));
                    }
                }
                return update;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return update;
            }
        }
        private IEnumerable<Container> GetTripContainer(string destSites, string trailerBarcode, out int NotloadedContainers, out int loadedContainers)
        {
            NotloadedContainers = 0;
            loadedContainers = 0;
            IEnumerable<Container> AllContainer = null;
            try
            {


                IEnumerable<Container> TripContainer = AppParameters.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Dest)
               && Regex.IsMatch(r.Value.Dest, destSites, RegexOptions.IgnoreCase)
               && r.Value.Origin != r.Value.Dest
               && r.Value.hasLoadScans == false
               && r.Value.containerTerminate == false
               && r.Value.containerAtDest == false
               && r.Value.hasCloseScans == true).Select(y => y.Value).ToList();
                NotloadedContainers = TripContainer.Count();
                AllContainer = TripContainer;
                if (!string.IsNullOrEmpty(trailerBarcode))
                {
                    IEnumerable<Container> LoadedContainer = AppParameters.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Trailer)
                   && Regex.IsMatch(r.Value.Trailer, trailerBarcode, RegexOptions.IgnoreCase)
                   && r.Value.hasLoadScans == true).Select(y => y.Value).ToList();
                    AllContainer = TripContainer.Concat(LoadedContainer);
                    loadedContainers = LoadedContainer.Count();
                    LoadedContainer = null;
                }
                TripContainer = null;
                return AllContainer;

                //if ((int)trip["unloadedContainers"] != unloadedtrailerContent.Count())
                //{
                //    trip["unloadedContainers"] = unloadedtrailerContent.Count();
                //}
                //alltrailercontent = unloadedtrailerContent;
                //if (!string.IsNullOrEmpty(item["trailerBarcode"].ToString()))
                //{
                //    IEnumerable<Container> loadedtrailerContent = null;
                //    //for the loaded int the trailer
                //    loadedtrailerContent = AppParameters.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Otrailer)
                //         && Regex.IsMatch(r.Value.Otrailer, item["trailerBarcode"].ToString(), RegexOptions.IgnoreCase)
                //         && r.Value.hasLoadScans == true
                //         ).Select(y => y.Value).ToList();
                //    alltrailercontent = unloadedtrailerContent.Concat(loadedtrailerContent);
                //    loadedtrailerContent = null;
                //}
                //trip["containers"] = JArray.Parse(JsonConvert.SerializeObject(alltrailercontent, Formatting.Indented));
                //unloadedtrailerContent = null;
                //alltrailercontent = null;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
            finally
            {
                AllContainer = null;
            }
        }

        private void BroadcastSVTripsStatus(RouteTrips trip)
        {
            Clients.All.updateSVTripsStatus(trip);
        }

        private string CheckNotification(string currentState, string NewState, string type, RouteTrips trip, string noteifi_id)
        {
            string noteification_id = "";
            try
            {
                if (!string.IsNullOrEmpty(noteifi_id))
                {
                    noteification_id = noteifi_id;
                }
                if (currentState != NewState)
                {
                    if (!string.IsNullOrEmpty(noteifi_id) && AppParameters.NotificationList.ContainsKey(noteifi_id))
                    {
                        if (AppParameters.NotificationList.TryGetValue(noteification_id, out Notification notification))
                        {
                            notification.Delete = true;
                            notification.Notification_Update = true;
                            noteification_id = "";
                        }
                    }
                    //new condition
                    AppParameters.NotificationConditionsList.Where(r => Regex.IsMatch(NewState, r.Value.Conditions, RegexOptions.IgnoreCase)
                  && r.Value.Type.ToLower() == type.ToLower()
                   && r.Value.ActiveCondition).Select(x => x.Value).ToList().ForEach(conditions =>
                   {
                       noteification_id = conditions.Id + trip.RouteTripId + trip.RouteTripLegId + trip.TripDirectionInd;

                       Notification newNotifi = JsonConvert.DeserializeObject<Notification>(JsonConvert.SerializeObject(conditions, Formatting.None));
                       newNotifi.Type_ID = trip.RouteTripId.ToString() + trip.RouteTripLegId.ToString() + trip.TripDirectionInd;
                       newNotifi.Type_Name = trip.Route + "-" + trip.Trip + "|" + trip.LegSiteName + "|" + trip.DoorNumber;
                       newNotifi.Type_Duration = trip.TripMin;
                       newNotifi.Type_Status = trip.State;
                       newNotifi.Notification_ID = noteification_id;
                       newNotifi.Notification_Update = true;
                       newNotifi.Type_Time = AppParameters.GetSvDate(trip.ScheduledDtm);
                       AppParameters.NotificationList.TryAdd(noteification_id, newNotifi);

                   });

                }
                return noteification_id;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return noteification_id;
            }
        }

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

        internal IEnumerable<RouteTrips> GetTripsList()
        {
            try
            {
                return AppParameters.RouteTripsList.Where(x => !Regex.IsMatch(x.Value.State, "(CANCELED|DEPARTED|OMITTED|COMPLETE)", RegexOptions.IgnoreCase)).Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<RouteTrips> GetRouteTripsInfo(string id)
        {
            try
            {
                return AppParameters.RouteTripsList.Where(r => r.Key == id).Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<Container> GetContainerInfo(string id)
        {
            try
            {
                return AppParameters.Containers.Where(r => r.Key == id).Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
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
                    foreach (var QSMitem in from QSMitem in AppParameters.ConnectionList.Values.Where(r => r.UpdateStatus).Select(y => y)
                                            where TryUpdateQSMStaus(QSMitem)
                                            select QSMitem)
                    {
                        BroadcastQSMUpdate(QSMitem);
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
                }
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

        //internal IEnumerable<GeoZone> GetMachineZonesList()
        //{
        //    try
        //    {
        //        return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "Machine").Select(x => x.Value).ToList();

        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}

        //internal IEnumerable<GeoZone> GetAGVLocationZonesList()
        //{
        //    try
        //    {
        //        return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "AGVLocation").Select(x => x.Value).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}
        //internal IEnumerable<GeoMarker> GetVehicleTagsList()
        //{
        //    try
        //    {
        //        return AppParameters.TagsList.Where(x => x.Value.Properties.TagType.EndsWith("Vehicle")).Select(y => y.Value).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}

        //internal IEnumerable<GeoZone> GetViewPortsZonesList()
        //{
        //    try
        //    {
        //        return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "ViewPorts").Select(x => x.Value).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}
        //internal IEnumerable<GeoMarker> GetLocatorsList()
        //{
        //    try
        //    {
        //        return AppParameters.TagsList.Where(x => x.Value.Properties.TagType == "Locator").Select(y => y.Value).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}

        //internal IEnumerable<GeoZone> GetDockDoorZonesList()
        //{
        //    try
        //    {
        //        return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType == "DockDoor").Select(x => x.Value).OrderBy(o => o.Properties.DoorNumber).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}

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

        private void UpdateAGVLocationStatus(object state)
        {
            lock (updateAGVLocationStatuslock)
            {
                if (!_updateAGVLocationStatus)
                {
                    _updateAGVLocationStatus = true;

                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                    {
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "AGVLocation" && f.Value.Properties.ZoneUpdate && f.Value.Properties.Visible).Select(y => y.Value).ToList().ForEach(AGV_Location =>
                        {
                            if (TryUpdateAGVLocationStatus(AGV_Location))
                            {
                                BroadcastAGVLocationStatus(AGV_Location, cs.Id);
                            }

                        });
                    }
                    //foreach (var AGV_Location in AppParameters.ZoneList.Where(u => u.Value.Properties.ZoneUpdate
                    //&& u.Value.Properties.Visible
                    //&& u.Value.Properties.ZoneType == "AGVLocation").Select(x => x.Value).Where(AGV_Location => TryUpdateAGVLocationStatus(AGV_Location)))
                    //{
                    //    BroadcastAGVLocationStatus(AGV_Location);
                    //}

                    _updateAGVLocationStatus = false;
                }
            }
        }

        private bool TryUpdateAGVLocationStatus(GeoZone AGV_Location)
        {
            try
            {
                AGV_Location.Properties.ZoneUpdate = false;
                return true;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void BroadcastAGVLocationStatus(GeoZone AGV_Location, string id)
        {
            Clients.Group("AGVLocationZones").updateAGVLocationStatus(AGV_Location);
        }

        private void UpdateMachineStatus(object state)
        {
            lock (updateMachineStatuslock)
            {
                List<Tuple<GeoZone, string>> machineStatuses = new
                    List<Tuple<GeoZone, string>>();
                if (!_updateMachineStatus)
                {
                    _updateMachineStatus = true;
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                    {
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "Machine" && f.Value.Properties.ZoneUpdate).Select(y => y.Value).ToList().ForEach(Machine =>
                        {
                            if (TryUpdateMachineStatus(Machine))
                            {

                                machineStatuses.Add(new Tuple<GeoZone, string>(Machine, cs.Id));
                            }

                        });
                    }
                    if (machineStatuses.Count > 0)
                    {
                        BroadcastMachineStatus(machineStatuses);
                    }

                    _updateMachineStatus = false;
                }
            }
        }

        private bool TryUpdateMachineStatus(GeoZone machine)
        {
            try
            {

                machine.Properties.ZoneUpdate = false;
                //MPE Performance
                machine.Properties.MPEWatchData = GetMPEPerfData(machine.Properties.Name);

                //MPE P2P
                if (!string.IsNullOrEmpty(machine.Properties.MPEWatchData.CurSortplan))
                {
                    machine.Properties.StaffingData = GetStaffingSortplan(machine.Properties.MPEWatchData.MpeType, machine.Properties.MPEWatchData.MpeNumber, machine.Properties.MPEWatchData.CurSortplan);
                }
                //MPE DPS
                if (!string.IsNullOrEmpty(machine.Properties.MPEWatchData.CurSortplan))
                {
                    machine.Properties.DPSData = GetDPSData(machine.Properties.MPEWatchData.CurSortplan);
                }
                return true;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }
        private static StaffingSortplan GetStaffingSortplan(string machine_type, string machine_number, string sortplan)
        {
            StaffingSortplan StaffingSortplanData = new StaffingSortplan();
            try
            {

                if (!string.IsNullOrEmpty(machine_type))
                {
                    sortplan = AppParameters.SortPlan_Name_Trimer(sortplan);
                    int.TryParse(machine_number, out int number);
                    string id = machine_type + "-" + number + "-" + sortplan;

                    if (AppParameters.StaffingSortplansList.TryGetValue(id, out string sp))
                    {
                        StaffingSortplanData = JsonConvert.DeserializeObject<StaffingSortplan>(sp);
                    }
                }
                return StaffingSortplanData;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return StaffingSortplanData;
            }
        }
        private static RunPerf GetMPEPerfData(string MPEName)
        {
            RunPerf PerfData = new RunPerf();
            try
            {

                if (AppParameters.MPEPerformanceList.TryGetValue(MPEName, out string Perf))
                {
                    PerfData = JsonConvert.DeserializeObject<RunPerf>(Perf);
                    return PerfData;
                }
                return PerfData;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return PerfData;
            }
        }
        private static DPS GetDPSData(string curSortplan)
        {
            DPS DPSData = new DPS();
            try
            {
                string tempsortplan = curSortplan.Length >= 7 ? curSortplan.Substring(0, 7) : curSortplan;
                if (AppParameters.DPSList.TryGetValue(tempsortplan, out string sortplan))
                {
                    DPSData = JsonConvert.DeserializeObject<DPS>(sortplan);
                }
                return DPSData;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return DPSData;
            }
        }
        private void BroadcastMachineStatus(List<Tuple<GeoZone, string>> machineStatuses)
        {
            Clients.Group("MachineZones").updateMachineStatus(machineStatuses);
            
        }
        /* updates tag name after end user clicks on a vehicle, selects edit, and enters
           a new tag name and submits
        */
        private string GetQuuppaTagChangeFilename(string tagId, DateTime thisTime)
        {
            return thisTime.Year + "-" + thisTime.Month.ToString("00") + "-" +
                thisTime.Day.ToString("00") + "_" + tagId;
        }

        
        internal async Task<bool> UpdateTagName(string tagId, string tagName)
        {
            try
            {
                DateTime thisTime = DateTime.Now;
                // update the tag name using quuppa api

                // [IP:port]/qpe/modifyTag?&tag=<tag_id>&name=<NEW name>
                 
                HttpResponseMessage response = await
                    httpClient.GetAsync(AppParameters.QuuppaBaseUrl +
                    @"modifyTag?tag=" + tagId + "&name=" + tagName).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string fileName = GetQuuppaTagChangeFilename(tagId, thisTime);
                // export the tag name change to a file

                // [IP:port]/qpe/exportTags?tag=<tag_id>&filename=<YYYY-MM-DD_TAGID>
                HttpResponseMessage response2 =
                   await httpClient.GetAsync(AppParameters.QuuppaBaseUrl +
                   @"exportTags?tag=" + tagId + "&name=" + tagName +
                   "&filename=" + fileName).ConfigureAwait(false);
                content = await response2.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response2.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                // import the tag name change for a permanent update

                // [IP:port]/qpe/importTags?&filename=<YYYY-MM-DD_TAGID>
                HttpResponseMessage response3 =
                   await httpClient.GetAsync(AppParameters.QuuppaBaseUrl +
                   @"importTags?filename=" + fileName).ConfigureAwait(false);

                content = await response3.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response3.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                

                // update the project data for the stored json file, which includes the new update
                bool result = await UpdateProjectData().ConfigureAwait(false);

                // now update so the changes appear on the map
                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                {
                    List<GeoMarker> vehicles = cs.Locators.Where
                        (f => f.Value.Properties.TagType.EndsWith("Vehicle") &&
                    f.Key == tagId
                    ).Select(y => y.Value).ToList<GeoMarker>();
                   
                    BroadcastVehiclesUpdate(vehicles, cs.Id);
                }
                return result;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }



        /*
         *  code added, but not a feature in the app yet, no GUI for this yet
         */
                internal async Task<bool> UpdateTagGroupName(string tagId, string tagGroupName)
        {
            try
            {

                DateTime thisTime = DateTime.Now;
                // update the tag group name using api call to quuppa

                // [IP:port]/qpe/setTagGroup?tag=<tag_id>&targetGroup=<TargetTagGroup>&createNew=true
                HttpResponseMessage response = await
                    httpClient.GetAsync(AppParameters.QuuppaBaseUrl +
                 @"setTagGroup?tag=" + tagId +
                 @"&targetGroup=" + tagGroupName +
                 @"&createNew=true").ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string fileName = GetQuuppaTagChangeFilename(tagId, thisTime);
                // export the tag group name change to a file

                // [IP:port]/qpe/exportTags?tag=<tag_id>&filename=<YYYY-MM-DD_TAGID>
                HttpResponseMessage response2 =
                   await httpClient.GetAsync(AppParameters.QuuppaBaseUrl +
                   @"exportTags?tag=" + tagId + 
                   "&filename=" + fileName).ConfigureAwait(false);
                content = await response2.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response2.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                // import the tag group name change for a permanent update

                // [IP:port]/qpe/importTags?&filename=<YYYY-MM-DD_TAGID>
                HttpResponseMessage response3 =
                   await httpClient.GetAsync(AppParameters.QuuppaBaseUrl +
                   @"importTags?filename=" + fileName).ConfigureAwait(false);

                content = await response3.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response3.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                bool result = await UpdateProjectData().ConfigureAwait(false);

                // now update so the changes appear on the map
                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                {
                    List<GeoMarker> vehicles = new List<GeoMarker>();
                    GeoMarker vehicle = cs.Locators.Where(f => f.Value.Properties.TagType.EndsWith("Vehicle")
                    && f.Key == tagId).Select(y => y.Value).FirstOrDefault<GeoMarker>();

                    vehicles.Add(vehicle);

                    BroadcastVehiclesUpdate(vehicles, cs.Id);
                }
                return result;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }
        internal string GetProjectInfoConnection()
        {
            Connection conn = AppParameters.ConnectionList.Where(x => x.Value.MessageType
            == "getProjectInfo").Select(y => y.Value).FirstOrDefault<Connection>();
            return string.Format(conn.Url, "getProjectInfo");
        }
        internal async Task<bool> UpdateProjectData()
        {

            try
            {

            
                string projectInfoConnectionString = GetProjectInfoConnection();
                HttpResponseMessage response = await
                    httpClient.GetAsync(projectInfoConnectionString).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }

                string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject jo = JObject.Parse(content);
                JArray coordinateSystems = (JArray)jo["coordinateSystems"];
                string projectDataString = coordinateSystems.ToString();

           
                new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder),
                   "Project_Data.json", projectDataString);

                ProcessRecvdMsg.ProjectData(projectDataString, "");
                AppParameters.ProjectData = projectDataString;
                return true;

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }
        internal IEnumerable<CoordinateSystem> GetIndoorMapFloor(string id)
            
        {
            try
            {
                return AppParameters.CoordinateSystem.Where(r => r.Key == id).Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<CoordinateSystem> GetIndoorMap()
        {
            try
            {
                if (AppParameters.CoordinateSystem.Keys.Count == 0)
                {
                    ConcurrentDictionary<string, CoordinateSystem> CoordinateSystem = new ConcurrentDictionary<string, CoordinateSystem>();

                    CoordinateSystem temp = new CoordinateSystem
                    {
                        Id = "temp",
                        BackgroundImage = new BackgroundImage
                        {
                            Id = "temp",
                            //FacilityName = !string.IsNullOrEmpty(AppParameters.AppSettings["FACILITY_NAME"].ToString()) ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "Site Not Configured",
                            //ApplicationFullName = AppParameters.AppSettings["APPLICATION_FULLNAME"].ToString(),
                            //ApplicationAbbr = AppParameters.AppSettings["APPLICATION_NAME"].ToString(),

                        }
                    };
                    CoordinateSystem.TryAdd(temp.Id, temp);
                    return CoordinateSystem.Select(y => y.Value).ToList();
                }
                return AppParameters.CoordinateSystem.Select(y => y.Value).ToList();
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
        //internal IEnumerable<GeoZone> GetZonesList()
        //{
        //    try
        //    {

        //        return AppParameters.ZoneList.Where(r => r.Value.Properties.ZoneType.ToString() == "Area").Select(x => x.Value).ToList();

        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}

        //internal IEnumerable<GeoMarker> GetPersonTagsList()
        //{
        //    try
        //    {
        //        return AppParameters.TagsList.Where(x => x.Value.Properties.TagType.EndsWith("Person")).Select(y => y.Value).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}

        //internal IEnumerable<GeoMarker> GetUndetectedTagsList()
        //{
        //    try
        //    {
        //        return AppParameters.TagsList.Where(x => x.Value.Properties.TagType.EndsWith("Person")
        //        && x.Value.Properties.IsWearingTag == false && !x.Value.Properties.IsLdcAlert
        //        && !string.IsNullOrEmpty(x.Value.Properties.Tacs)).Select(y => y.Value).ToList();

        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}
        //internal IEnumerable<GeoMarker> GetLDCAlertTagsList()
        //{
        //    try
        //    {
        //        return AppParameters.TagsList.Where(x => x.Value.Properties.TagType.EndsWith("Person")
        //            && x.Value.Properties.IsLdcAlert
        //            && !string.IsNullOrEmpty(x.Value.Properties.Tacs)).Select(y => y.Value).ToList();
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
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
        private bool TryUpdateVehicleTagStatus(GeoMarker marker, double tagVisibleRange)
        {
            try
            {
                marker.Properties.TagUpdate = false;
                return true;
                //bool returnresult = false;
                //System.TimeSpan tagdiffResult = ((DateTime)((JObject)tag["properties"]).Property("Tag_TS").Value).ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("positionTS").Value).ToUniversalTime());
                //tag["properties"]["tagVisibleMils"] = tagdiffResult.TotalMilliseconds;
                //if ((bool)tag["properties"]["Tag_Update"] == true)
                //{
                //    ////vehicle notification
                //    //if (((JObject)tag["properties"]).Property("Tag_Type").Value.ToString().ToLower() == "Autonomous Vehicle".ToLower())
                //    //{
                //    //  if (((JObject)tag["properties"]).ContainsKey("vehicleTime"))
                //    //    {
                //    //        if (!string.IsNullOrEmpty((string)((JObject)tag["properties"]).Property("vehicleTime").Value))
                //    //        {
                //    //            //time cal lostcomm
                //    //            System.TimeSpan diffResult = DateTime.Now.ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime());

                //    //            if (AppParameters.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                //    //           && r.Value.Property("TYPE").Value.ToString().ToUpper() == "vehicle".ToUpper()
                //    //            && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList().Count() > 0)
                //    //            {
                //    //                foreach (var conditionitem in AppParameters.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                //    //                    && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                //    //                     && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList())
                //    //                {
                //    //                    double warningmil = TimeSpan.FromMinutes((int)conditionitem.Property("WARNING").Value).TotalMilliseconds;
                //    //                    if (diffResult.TotalMilliseconds > warningmil)
                //    //                    {
                //    //                        if (!AppParameters.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
                //    //                        {
                //    //                            JObject ojbMerge = (JObject)conditionitem.DeepClone();
                //    //                            ojbMerge.Add(new JProperty("VEHICLETIME", ((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime()));
                //    //                            ojbMerge.Add(new JProperty("VEHICLENAME", (string)((JObject)tag["properties"]).Property("name").Value));
                //    //                            ojbMerge.Add(new JProperty("TAGID", (string)((JObject)tag["properties"]).Property("id").Value));
                //    //                            ojbMerge.Add(new JProperty("notificationId", (string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value));
                //    //                            ojbMerge.Add(new JProperty("UPDATE", true));
                //    //                            if (AppParameters.Notification.TryAdd((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, ojbMerge))
                //    //                            {
                //    //                            }
                //    //                        }
                //    //                    }
                //    //                    else
                //    //                    {
                //    //                        if (AppParameters.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
                //    //                        {
                //    //                            if (AppParameters.Notification.TryGetValue((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, out JObject ojbMerge))
                //    //                            {
                //    //                                ojbMerge.Add(new JProperty("DELETE", true));
                //    //                            }

                //    //                        }
                //    //                    }
                //    //                }
                //    //            }

                //    //        }
                //    //    }
                //    //}
                //    if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
                //    {
                //        if ((bool)tag["properties"]["tagVisible"] != false)
                //        {
                //            tag["properties"]["tagVisible"] = false;
                //        }
                //    }
                //    else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
                //    {
                //        if ((bool)tag["properties"]["tagVisible"] != true)
                //        {
                //            tag["properties"]["tagVisible"] = true;
                //        }
                //    }
                //    tag["properties"]["Tag_Update"] = false;
                //    return true;
                //}
                //else
                //{
                //    ////vehicle notification
                //    //if (((JObject)tag["properties"]).Property("Tag_Type").Value.ToString().ToLower() == "Autonomous Vehicle".ToLower())
                //    //{
                //    //  if (((JObject)tag["properties"]).ContainsKey("vehicleTime"))
                //    //    {
                //    //        if (!string.IsNullOrEmpty((string)((JObject)tag["properties"]).Property("vehicleTime").Value))
                //    //        {
                //    //            //time cal lostcomm
                //    //            System.TimeSpan diffResult = DateTime.Now.ToUniversalTime().Subtract(((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime());

                //    //            if (AppParameters.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                //    //           && r.Value.Property("TYPE").Value.ToString().ToUpper() == "vehicle".ToUpper()
                //    //            && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList().Count() > 0)
                //    //            {
                //    //                foreach (var conditionitem in AppParameters.Notification_Conditions.Where(r => Regex.IsMatch(((JObject)tag["properties"]).Property("state").Value.ToString().ToLower(), r.Value.Property("CONDITIONS").Value.ToString(), RegexOptions.IgnoreCase)
                //    //                    && r.Value.Property("TYPE").Value.ToString().ToLower().EndsWith("vehicle".ToLower())
                //    //                     && (bool)r.Value.Property("ACTIVE_CONDITION").Value == true).Select(x => x.Value).ToList())
                //    //                {
                //    //                    double warningmil = TimeSpan.FromMinutes((int)conditionitem.Property("WARNING").Value).TotalMilliseconds;
                //    //                    if (diffResult.TotalMilliseconds > warningmil)
                //    //                    {
                //    //                        if (!AppParameters.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
                //    //                        {
                //    //                            JObject ojbMerge = (JObject)conditionitem.DeepClone();
                //    //                            ojbMerge.Add(new JProperty("VEHICLETIME", ((DateTime)((JObject)tag["properties"]).Property("vehicleTime").Value).ToUniversalTime()));
                //    //                            ojbMerge.Add(new JProperty("VEHICLENAME", (string)((JObject)tag["properties"]).Property("name").Value));
                //    //                            ojbMerge.Add(new JProperty("TAGID", (string)((JObject)tag["properties"]).Property("id").Value));
                //    //                            ojbMerge.Add(new JProperty("notificationId", (string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value));
                //    //                            ojbMerge.Add(new JProperty("UPDATE", true));
                //    //                            if (AppParameters.Notification.TryAdd((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, ojbMerge))
                //    //                            {
                //    //                            }
                //    //                        }
                //    //                    }
                //    //                    else
                //    //                    {
                //    //                        if (AppParameters.Notification.ContainsKey((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value))
                //    //                        {
                //    //                            if (AppParameters.Notification.TryGetValue((string)conditionitem.Property("ID").Value + (string)((JObject)tag["properties"]).Property("id").Value, out JObject ojbMerge))
                //    //                            {
                //    //                                ojbMerge.Add(new JProperty("DELETE", true));
                //    //                            }

                //    //                        }
                //    //                    }
                //    //                }
                //    //            }

                //    //        }
                //    //    }
                //    //}

                //    if (tagdiffResult.TotalMilliseconds > tagVisibleRange)
                //    {
                //        if ((bool)tag["properties"]["tagVisible"] != false)
                //        {
                //            tag["properties"]["tagVisible"] = false;
                //            returnresult = true;
                //        }
                //    }
                //    else if (tagdiffResult.TotalMilliseconds <= tagVisibleRange)
                //    {
                //        if ((bool)tag["properties"]["tagVisible"] != true)
                //        {
                //            tag["properties"]["tagVisible"] = true;
                //        }
                //    }
                //    tag["properties"]["Tag_Update"] = false;
                //    return returnresult;
                //}
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
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                    {
                        cs.Zones.Where(f => f.Value.Properties.ZoneType == "DockDoor"
                        && f.Value.Properties.ZoneUpdate
                        && f.Value.Properties.Visible
                        ).Select(y => y.Value).ToList().ForEach(DockDoor =>
                        {
                            if (TryUpdateDockDoorStatus(DockDoor))
                            {
                                BroadcastDockDoorStatus(DockDoor, cs.Id);
                            }
                        });
                    }
                    _updateDockDoorStatus = false;
                }
            }
        }

        private void BroadcastVehiclesUpdate(List<GeoMarker> vehicles, string id)
        {
            Clients.Group("VehiclsMarkers").updateVehicles(vehicles, id);
        }

        private void BroadcastDockDoorStatus(GeoZone dockDoor, string id)
        {
            Clients.Group("DockDoorZones").updateDockDoorStatus(dockDoor, id);
        }

        private bool TryUpdateDockDoorStatus(GeoZone dockDoor)
        {
            try
            {
                dockDoor.Properties.ZoneUpdate = false;
                return true;

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
                    double tagVisibleRange = AppParameters.AppSettings.ContainsKey("POSITION_MAX_AGE") ? !string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value) ? (long)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value : 10000 : 10000;
                    //foreach (var Tag in from GeoMarker Tag in AppParameters.TagsList.Where(u => u.Value.Properties.TagUpdate && u.Value.Properties.TagType.EndsWith("Vehicle")).Select(x => x.Value)
                    //                    where TryUpdateVehicleTagStatus(Tag, tagVisibleRange)
                    //                    select Tag)
                    //{
                    //    BroadcastVehicleTagStatus(Tag);
                    //}

                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                    {
                        cs.Locators.Where(f => f.Value.Properties.TagType.EndsWith("Vehicle") && f.Value.Properties.TagUpdate).Select(y => y.Value).ToList().ForEach(marker =>
                        {
                            if (TryUpdateVehicleTagStatus(marker, tagVisibleRange))
                            {
                                BroadcastVehicleTagStatus(marker, cs.Id);
                            }

                        });
                    }

                    _updateTagStatus = false;
                }
            }
        }
        private void BroadcastVehicleTagStatus(GeoMarker marker, string id)
        {
            Clients.Group("VehiclsMarkers").updateVehicleTagStatus(marker, id);
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
                    double tagVisibleRange = AppParameters.AppSettings.ContainsKey("POSITION_MAX_AGE") ? !string.IsNullOrEmpty((string)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value) ? (long)AppParameters.AppSettings.Property("POSITION_MAX_AGE").Value : 10000 : 10000;
                    //foreach (var Marker in from GeoMarker Marker in AppParameters.TagsList.Where(u => u.Value.Properties.TagUpdate
                    //                          && u.Value.Properties.TagType.EndsWith("Person")).Select(x => x.Value)
                    //                       where TryUpdatePersonTagStatus(Marker, tagVisibleRange)
                    //                       select Marker)
                    //{
                    //    BroadcastPersonTagStatus(Marker);
                    //}
                    foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                    {
                        cs.Locators.Where(f => f.Value.Properties.TagType.EndsWith("Person")
                        && f.Value.Properties.TagUpdate).Select(y => y.Value).ToList().ForEach(marker =>
                        {
                            if (TryUpdatePersonTagStatus(marker, tagVisibleRange))
                            {
                                BroadcastPersonTagStatus(marker, cs.Id);
                            }

                        });
                    }
                    // watch.Stop();
                    // new ErrorLogger().CustomLog(string.Concat("Total Execution for all tags ", "Time: ", watch.ElapsedMilliseconds, " ms"), string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "TagProcesslogs"));
                    _updatePersonTagStatus = false;
                }
            }
        }
        private bool TryUpdatePersonTagStatus(GeoMarker marker, double PositionMaxAge)
        {
            try
            {
                marker.Properties.TagUpdate = false;
                bool TagVisible = marker.Properties.TagVisible;
                bool tagUpdate = false;
                int PositionCurrentAge = AppParameters.Get_TagTTL(marker.Properties.PositionTS, marker.Properties.TagTS);
                if (PositionCurrentAge >= PositionMaxAge)
                {
                    TagVisible = false;
                }
                if (PositionCurrentAge <= PositionMaxAge)
                {
                    TagVisible = true;
                    tagUpdate = true;
                }
                if (marker.Properties.TagVisible != TagVisible)
                {
                    marker.Properties.TagVisible = TagVisible;
                    tagUpdate = true;
                }
                marker.Properties.TagVisibleMils = PositionCurrentAge;
                return tagUpdate;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
        }

        private void BroadcastPersonTagStatus(GeoMarker Marker, string id)
        {
            Clients.Group("PeopleMarkers").updatePersonTagStatus(Marker, id);
        }
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

        public List<ThroughputValues> GetMachineThroughputMaximums()
        {
            return AppParameters.MachineThroughputMax;
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
                    newConndata.ApiConnected = false;
                    //tempcon[i].LasttimeApiConnected = DateTime.Now.AddMinutes(-120);
                    if (newConndata.ConnectionName.ToLower() == "MPEWatch".ToLower())
                    {
                        newConndata.IpAddress = "";
                        newConndata.Port = 0;
                        newConndata.Url = "";
                        string sitename = AppParameters.AppSettings["FACILITY_NAME"].ToString().ToLower().Replace(" ", "_").Replace("&", "").Replace("(", "").Replace(")", "");
                        AppParameters.MPEWatchData.Where(r => r.Value.SiteNameLocal.ToLower() == sitename).Select(y => y.Value).ToList().ForEach(m => {
                            newConndata.IpAddress = m.Host;
                            newConndata.Port = m.Port;
                            newConndata.Url = m.URL;
                        });
                    }
                    if (AppParameters.ConnectionList.TryAdd(newConndata.Id, newConndata))
                    {
                        AppParameters.RunningConnection.Add(newConndata);
                        fileUpdate = true;
                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
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
                                        if (updateConndata.TcpConnection)
                                        {
                                            Connection_item._StopTCPListener();
                                        }
                                        else if (updateConndata.WsConnection)
                                        {
                                            Connection_item.WSStop();
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
                                        if (updateConndata.TcpConnection)
                                        {
                                            Connection_item._StartTCPListener();
                                        }
                                        else if (updateConndata.WsConnection)
                                        {
                                            Connection_item._WSThreadListener();
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
                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
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
                                        new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
                                        return AppParameters.ConnectionList.Select(e => e.Value).ToList();
                                    }
                                    else if (Connection_item.ConnectionInfo.WsConnection)
                                    {
                                        Connection_item.WSDelete();
                                        AppParameters.RunningConnection.Connection.Remove(Connection_item);
                                        new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
                                        return AppParameters.ConnectionList.Select(e => e.Value).ToList();
                                    }
                                    else
                                    {
                                        Connection_item.Stop_Delete();
                                        AppParameters.RunningConnection.Connection.Remove(Connection_item);
                                        new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", JsonConvert.SerializeObject(AppParameters.ConnectionList.Select(x => x.Value).ToList(), Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
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
        internal IEnumerable<GeoZone> EditZone(string data)
        {
            string id = string.Empty;
            string floorID = string.Empty;
            try
            {
                bool fileUpdate = false;
                if (!string.IsNullOrEmpty(data))
                {
                    if (AppParameters.IsValidJson(data))
                    {
                        JObject objectdata = JObject.Parse(data);
                        if (objectdata.HasValues)
                        {
                            if (objectdata.ContainsKey("floorId") && objectdata.ContainsKey("id"))
                            {
                                floorID = objectdata["floorId"].ToString();
                                id = objectdata["id"].ToString();
                                if (AppParameters.CoordinateSystem.ContainsKey(floorID))
                                {
                                    ZoneInfo newzinfo = new ZoneInfo();
                                    if (AppParameters.CoordinateSystem.TryGetValue(floorID, out CoordinateSystem cs))
                                    {
                                        if (cs.Zones.ContainsKey(id))
                                        {
                                            if (cs.Zones.TryGetValue(id, out GeoZone gz))
                                            {
                                                gz.Properties.MPENumber = (int)objectdata["MPE_Number"];
                                                gz.Properties.MPEType = objectdata["MPE_Type"].ToString();
                                                gz.Properties.Name = string.Concat(gz.Properties.MPEType, "-", gz.Properties.MPENumber.ToString().PadLeft(3, '0'));
                                                gz.Properties.QuuppaOverride = true;
                                                gz.Properties.ZoneUpdate = true;
                                                fileUpdate = true;
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
                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(AppParameters.CoordinateSystem.Select(x => x.Value).ToList()));
                }

                return AppParameters.CoordinateSystem[floorID].Zones.Where(w => w.Key == id).Select(s => s.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<JToken> GetAppSettingdata()
        {
            try
            {
                JToken tempsetting = AppParameters.AppSettings.DeepClone();
                foreach (dynamic item in AppParameters.AppSettings)
                {
                    if (item.Key.ToString().StartsWith("ORACONN") && !string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        tempsetting[item.Key] = AppParameters.Decrypt(item.Value.ToString());
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
                    dynamic objdict = JToken.Parse(data);
                    foreach (dynamic item in AppParameters.AppSettings)
                    {
                        foreach (var kv in objdict)
                        {
                            if (kv.Name == item.Key)
                            {
                                if (kv.Value != item.Value)
                                {
                                    fileUpdate = true;

                                    if (kv.Name.StartsWith("ORACONN"))
                                    {
                                        AppParameters.AppSettings[item.Key] = AppParameters.Encrypt(kv.Value.ToString());
                                    }
                                    else if (kv.Name == "FACILITY_NASS_CODE")
                                    {
                                        if (GetData.Get_Site_Info((string)kv.Value, out SV_Site_Info SiteInfo))
                                        {
                                            if (SiteInfo != null)
                                            {
                                                AppParameters.AppSettings[kv.Name] = kv.Value.ToString();
                                                AppParameters.AppSettings["FACILITY_NAME"] = SiteInfo.DisplayName;// .ContainsKey("displayName") ? SiteInfo["displayName"] : "Site Not Configured";
                                                AppParameters.AppSettings["FACILITY_ID"] = SiteInfo.FdbId; //.ContainsKey("fdbId") ? SiteInfo["fdbId"] : "";
                                                AppParameters.AppSettings["FACILITY_ZIP"] = SiteInfo.ZipCode; //.ContainsKey("zipCode") ? SiteInfo["zipCode"] : "";
                                                AppParameters.AppSettings["FACILITY_LKEY"] = SiteInfo.LocaleKey;//.ContainsKey("localeKey") ? SiteInfo["localeKey"] : "";
                                                Task.Run(() => AppParameters.LoglocationSetup());
                                                Task.Run(() => AppParameters.ResetParameters());
                                            }
                                            else
                                            {
                                                AppParameters.AppSettings["FACILITY_NAME"] = "Site Not Configured";
                                                AppParameters.AppSettings["FACILITY_ID"] = "";
                                                AppParameters.AppSettings["FACILITY_ZIP"] = "";
                                                AppParameters.AppSettings["FACILITY_LKEY"] = "";
                                            }
                                        }
                                    }
                                    else if (kv.Name == "LOG_LOCATION")
                                    {
                                        if (!string.IsNullOrEmpty(kv.Value.ToString()))
                                        {
                                            AppParameters.AppSettings[item.Key] = kv.Value;
                                            Task.Run(() => AppParameters.LoglocationSetup());
                                        }
                                    }
                                    else
                                    {
                                        AppParameters.AppSettings[item.Key] = kv.Value;
                                    }
                                    //if (kv.Name == "CAMERA_THUMBNAIL_INTERVAL")
                                    //{
                                    //    SetCameraThumbnailInterval();
                                    //}
                                }
                            }
                        }
                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "AppSettings.json", JsonConvert.SerializeObject(AppParameters.AppSettings, Formatting.Indented));
                }
                return AppParameters.AppSettings;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return AppParameters.AppSettings;
            }
        }
        internal IEnumerable<BackgroundImage> GetFloorPlanData()
        {
            try
            {
                List<BackgroundImage> temp = new List<BackgroundImage>();
                foreach (CoordinateSystem cs in AppParameters.CoordinateSystem.Values)
                {
                    temp.Add(cs.BackgroundImage); 
                };
                return temp;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal void Removeuser(string connectionId)
        {
            try
            {
                AppParameters._connections.Remove(connectionId, connectionId);
                string data = string.Concat("Client closed the connection. | Connection ID: : " + connectionId);
                new ErrorLogger().CustomLog(data, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));

                //remove users 
                //foreach (string user in AppParameters.Users.Where(r => r.Value.LoginDate.Subtract(DateTime.Now).TotalDays > 2).Select(y => y.Key))
                //{
                //    if (!AppParameters.Users.TryRemove(user, out ADUser ur))
                //    {
                //        new ErrorLogger().CustomLog("Unable to remove User" + ur.UserId, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                //    }

                //}
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
                //if (Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out object Ipaddress))
                //{
                //    AppParameters._connections.Add(Context.ConnectionId, Context.ConnectionId);
                //    bool firstTimeLogin = true;
                //    ADUser newuser = new ADUser
                //    {
                //        UserId = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim(),
                //        NASSCode = AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(),
                //        FDBID = AppParameters.AppSettings["FACILITY_ID"].ToString(),
                //        FacilityName = !string.IsNullOrEmpty(AppParameters.AppSettings["FACILITY_NAME"].ToString()) ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "Site Not Configured",
                //        FacilityTimeZone = AppParameters.AppSettings["FACILITY_TIMEZONE"].ToString(),
                //        AppType = AppParameters.AppSettings["APPLICATION_NAME"].ToString(),
                //        Domain = !string.IsNullOrEmpty(Context.User.Identity.Name) ? Context.User.Identity.Name.Split('\\')[0].ToLower() : "",
                //        SessionID = Context.ConnectionId,
                //        ConnectionId = Context.ConnectionId,
                //        LoginDate = DateTime.Now,
                //        Environment = AppParameters.ApplicationEnvironment,
                //        IsAuthenticated = Context.User.Identity.IsAuthenticated,
                //        SoftwareVersion = AppParameters.VersionInfo,
                //        //BrowserType = HttpContext.Current.Request.Browser.Type,
                //        //BrowserName = HttpContext.Current.Request.Browser.Browser,
                //        //BrowserVersion = HttpContext.Current.Request.Browser.Version,
                //        Role = GetUserRole(GetGroupNames(((WindowsIdentity)Context.User.Identity).Groups)),
                //        IpAddress = Ipaddress.ToString().StartsWith("::") ? "127.0.0.1" : Ipaddress.ToString(),
                //        ServerIpAddress = AppParameters.ServerIpAddress.ToString()
                //    };
                //    new FindACEUser().User(newuser, out newuser);
                //    AppParameters.Users.AddOrUpdate(newuser.UserId, newuser,
                //        (key, old_user) =>
                //        {
                //            //log out user 
                //            if (!string.IsNullOrEmpty(old_user.ConnectionId))
                //            {
                //                Task.Run(() => new User_Log().LogoutUser(old_user));
                //            }
                //            //log of user logging in.
                //            Task.Run(() => new User_Log().LoginUser(newuser));
                //            string data = string.Concat("Client has Connected | User Name:", newuser.UserId, "(", newuser.FirstName, " ", newuser.SurName, ")", " | Connection ID: ", newuser.ConnectionId);
                //            new ErrorLogger().CustomLog(data, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                //            firstTimeLogin = false;
                //            return newuser;
                //        });
                //    if (firstTimeLogin)
                //    {
                //        string data = string.Concat("Client has Connected | User Name:", newuser.UserId, "(", newuser.FirstName, " ", newuser.SurName, ")", " | Connection ID: ", newuser.ConnectionId);
                //        new ErrorLogger().CustomLog(data, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                //        Task.Run(() => new User_Log().LoginUser(newuser));
                //    }

                //}
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal object AddUserProfile(HubCallerContext Context)
        {
            ADUser newuser = new ADUser();
            try
            {
                //if (Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out object Ipaddress))
                //{
                //    AppParameters._connections.Add(Context.ConnectionId, Context.ConnectionId);
                //   string ACEId = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim();
                //    if (AppParameters.Users.TryGetValue(ACEId, out newuser))
                //    {
                //        newuser.ConnectionId = Context.ConnectionId;
                //        newuser.SessionID = Context.ConnectionId;
                //        newuser.LoginDate = DateTime.Now;
                //        newuser.IsAuthenticated = Context.User.Identity.IsAuthenticated;
                //        newuser.IpAddress = Ipaddress.ToString().StartsWith("::") ? "127.0.0.1" : Ipaddress.ToString();
                //        newuser.ServerIpAddress = AppParameters.ServerIpAddress.ToString();
                //        newuser.NASSCode = AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString();
                //        newuser.FDBID = AppParameters.AppSettings["FACILITY_ID"].ToString();
                //        newuser.FacilityName = !string.IsNullOrEmpty(AppParameters.AppSettings["FACILITY_NAME"].ToString()) ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "Site Not Configured";
                //        newuser.FacilityTimeZone = AppParameters.AppSettings["FACILITY_TIMEZONE"].ToString();
                //        newuser.AppType = AppParameters.AppSettings["APPLICATION_NAME"].ToString();
                //        newuser.Role = GetUserRole(GetGroupNames(((WindowsIdentity)Context.User.Identity).Groups));
                //    }
                //    else
                //    {
                //        newuser = new ADUser
                //        {
                //            UserId = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim(),
                //            NASSCode = AppParameters.AppSettings["FACILITY_NASS_CODE"].ToString(),
                //            FDBID = AppParameters.AppSettings["FACILITY_ID"].ToString(),
                //            FacilityName = !string.IsNullOrEmpty(AppParameters.AppSettings["FACILITY_NAME"].ToString()) ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "Site Not Configured",
                //            FacilityTimeZone = AppParameters.AppSettings["FACILITY_TIMEZONE"].ToString(),
                //            AppType = AppParameters.AppSettings["APPLICATION_NAME"].ToString(),
                //            Domain = !string.IsNullOrEmpty(Context.User.Identity.Name) ? Context.User.Identity.Name.Split('\\')[0].ToLower() : "",
                //            SessionID = Context.ConnectionId,
                //            ConnectionId = Context.ConnectionId,
                //            LoginDate = DateTime.Now,
                //            Environment = AppParameters.ApplicationEnvironment,
                //            IsAuthenticated = Context.User.Identity.IsAuthenticated,
                //            SoftwareVersion = AppParameters.VersionInfo,
                //            //BrowserType = HttpContext.Current.Request.Browser.Type,
                //            //BrowserName = HttpContext.Current.Request.Browser.Browser,
                //            //BrowserVersion = HttpContext.Current.Request.Browser.Version,
                //            Role = GetUserRole(GetGroupNames(((WindowsIdentity)Context.User.Identity).Groups)),
                //            IpAddress = Ipaddress.ToString().StartsWith("::") ? "127.0.0.1" : Ipaddress.ToString(),
                //            ServerIpAddress = AppParameters.ServerIpAddress.ToString()
                //        };
                //    }

                //    Task.Run(() => AddUserToList(newuser));

                //}
                return newuser;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return newuser;
            }
        }

        //private void AddUserToList(ADUser newuser)
        //{
        //    try
        //    {
        //        bool firstTimeLogin = true;
        //        if (FindACEUser(newuser, out newuser))
        //        {
        //            AppParameters.Users.AddOrUpdate(newuser.UserId, newuser,
        //              (key, old_user) =>
        //              {
        //              //log out user 
        //              if (!string.IsNullOrEmpty(old_user.ConnectionId))
        //                  {
        //                      Task.Run(() => new User_Log().LogoutUser(old_user));
        //                  }
        //              //log of user logging in.
        //              Task.Run(() => new User_Log().LoginUser(newuser));
        //                  string data = string.Concat("Client has Connected | User Name:", newuser.UserId, "(", newuser.FirstName, " ", newuser.SurName, ")", " | Connection ID: ", newuser.ConnectionId);
        //                  new ErrorLogger().CustomLog(data, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
        //                  firstTimeLogin = false;
        //                  return newuser;
        //              });
        //            if (firstTimeLogin)
        //            {
        //                string data = string.Concat("Client has Connected | User Name:", newuser.UserId, "(", newuser.FirstName, " ", newuser.SurName, ")", " | Connection ID: ", newuser.ConnectionId);
        //                new ErrorLogger().CustomLog(data, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
        //                Task.Run(() => new User_Log().LoginUser(newuser));
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //    }
        //}

        private bool FindACEUser(ADUser ACEUser, out ADUser user)
        {

            user = ACEUser;
            try
            {
                using (PrincipalContext domainContext = new PrincipalContext(ContextType.Domain, AppParameters.AppSettings.Property("Domain").Value.ToString().Trim(), AppParameters.AppSettings.Property("ADUSAContainer").Value.ToString().Trim()))
                {
                    using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, (string)ACEUser.UserId))
                    {
                        if (foundUser != null)
                        {
                            DirectoryEntry directoryEntry = foundUser.GetUnderlyingObject() as DirectoryEntry;
                            directoryEntry.RefreshCache(new string[] { "tokenGroups" });

                            DirectorySearcher search = new DirectorySearcher(directoryEntry)
                            {
                                Filter = string.Format("({0}={1})", "SAMAccountName", (string)ACEUser.UserId)
                            };
                            search.PropertiesToLoad.AddRange(_propertiesToLoad);
                            SearchResult result = search.FindOne();
                            if (result == null)
                            {
                                return false;
                            }
                            string getPropertyValue(ResultPropertyValueCollection p, int i) => p.Count == 0 ? null : (string)p[i];

                            user.FirstName = getPropertyValue(result.Properties[ADProperties.FirstName], 0);
                            user.MiddleName = getPropertyValue(result.Properties[ADProperties.MiddleName], 0);
                            user.SurName = getPropertyValue(result.Properties[ADProperties.SurName], 0);
                            user.ZipCode = getPropertyValue(result.Properties[ADProperties.PostalCode], 0);
                            user.EmailAddress = foundUser.EmailAddress;
                            user.Phone = !string.IsNullOrEmpty(foundUser.VoiceTelephoneNumber) ? foundUser.VoiceTelephoneNumber : "";
                            user.EIN = !string.IsNullOrEmpty(foundUser.EmployeeId) ? foundUser.EmployeeId : "";
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return false;
            }

        }
        private readonly string[] _propertiesToLoad = new string[]
            {
                                    ADProperties.ContainerName,
                                    ADProperties.LoginName,
                                    ADProperties.MemberOf,
                                    ADProperties.FirstName,
                                    ADProperties.MiddleName,
                                    ADProperties.SurName,
                                    ADProperties.PostalCode
            };
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FOTFManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

      
    }
}