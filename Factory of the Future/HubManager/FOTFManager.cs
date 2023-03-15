using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Factory_of_the_Future.Models;

namespace Factory_of_the_Future
{

    public class FOTFManager : IDisposable
    {
        private readonly static Lazy<FOTFManager> _instance = new Lazy<FOTFManager>(() => new FOTFManager(GlobalHost.ConnectionManager.GetHubContext<HubManager>().Clients));
        public readonly ConcurrentDictionary<string, CoordinateSystem> CoordinateSystem = new ConcurrentDictionary<string, CoordinateSystem>();
        public static FOTFManager Instance { get { return _instance.Value; } }
        private IHubConnectionContext<dynamic> Clients { get; set; }
        public FOTFManager(IHubConnectionContext<dynamic> clients) { Clients = clients; }
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
        private readonly Timer QSM_timer;
        private readonly Timer Machine_timer;
        private readonly Timer AGVLocation_timer;
        private readonly Timer SVTrips_timer;
        private readonly Timer Notification_timer;
        private readonly Timer BinZone_timer;
        // private readonly Timer Camera_timer;
        //status
        private volatile bool _updatePersonTagStatus = false;
        private volatile bool _updateZoneStatus = false;
        private volatile bool _updateTagStatus = false;
        private volatile bool _updatingQSMStatus = false;
        private volatile bool _updateMachineStatus = false;
        private volatile bool _updateAGVLocationStatus = false;
        private volatile bool _updateSVTripsStatus = false;
        private volatile bool _updateNotificationstatus = false;
        private volatile bool _updateBinZoneStatus = false;
        //private volatile bool _updateCameraStatus = false;
        private bool disposedValue;
        private readonly HttpClient httpClient = new HttpClient();
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
        //private FOTFManager(IHubConnectionContext<dynamic> clients)
        //{
        //    Clients = clients;
        //    VehicleTag_timer = new Timer(UpdateVehicleTagStatus, null, _250updateInterval, _250updateInterval);
        //    PersonTag_timer = new Timer(UpdatePersonTagStatus, null, _250updateInterval, _250updateInterval);
        //    /////Zone status.
        //    Zone_timer = new Timer(UpdateZoneStatus, null, _2000updateInterval, _2000updateInterval);
        //    //DockDoor_timer = new Timer(UpdateDockDoorStatus, null, _250updateInterval, _250updateInterval);
        //    Machine_timer = new Timer(UpdateMachineStatus, null, _2000updateInterval, _2000updateInterval);
        //    AGVLocation_timer = new Timer(UpdateAGVLocationStatus, null, _250updateInterval, _250updateInterval);
        //    BinZone_timer = new Timer(UpdateBinZoneStatus, null, _2000updateInterval, _2000updateInterval);
        //    /////SV Trips Data
        //    SVTrips_timer = new Timer(UpdateSVTripsStatus, null, _30000updateInterval, _30000updateInterval);
        //    ////   Notification data timer
        //    Notification_timer = new Timer(UpdateNotificationtatus, null, _1000updateInterval, _1000updateInterval);
        //    ////
        //    //Connection status
        //    //QSM_timer = new Timer(UpdateQSM, null, _250updateInterval, _250updateInterval);
        //    //Camera update;
        //    //Camera_timer = new Timer(UpdateCameraImages, null, _10000updateInterval, _60000updateInterval);
        //}




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

        internal IEnumerable<RunPerf> GetMPEStatusList(string mpeID)
        {
            try
            {
                return AppParameters.MPEPerformance.Where(x => x.Value.MpeId == mpeID).Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        #region
        //dock door 

        internal void UpdateDoorData(string doorNumber)
        {
            try
            {
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => f.Value.Properties.ZoneType == "DockDoor"
                    && f.Value.Properties.DoorNumber == doorNumber
                    ).Select(y => y.Value).ToList().ForEach(DockDoor =>
                    {
                        DockDoor.Properties.DockDoorData = GetDigitalDockDoorList(DockDoor.Properties.DoorNumber);
                        BroadcastDockDoorUpdateData(DockDoor, cs.Id);
                    });
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        public void BroadcastDockDoorUpdateData(GeoZone dockDoor, string id)
        {
            Clients.Group("DockDoorZones").updateDockDoorData(dockDoor, id, dockDoor.Properties.Id);
            if (dockDoor.Properties.DockDoorData.Any())
            {
                Clients.Group("DockDoor_" + dockDoor.Properties.DoorNumber).updateDigitalDockDoorStatus(dockDoor.Properties.DockDoorData, id);

            }
        }
        public void BroadcastDockDoorAddData(GeoZone dockDoor, string id)
        {
            Clients.Group("DockDoorZones").addDockDoorData(dockDoor, id);
            if (dockDoor.Properties.DockDoorData.Any())
            {
                Clients.Group("DockDoor_" + dockDoor.Properties.DoorNumber).updateDigitalDockDoorStatus(dockDoor.Properties.DockDoorData, id);

            }
        }
        public void BroadcastDockDoorRemoveData(GeoZone dockDoor, string id)
        {
            Clients.Group("DockDoorZones").removeDockDoorData(dockDoor, id);
            if (dockDoor.Properties.DockDoorData.Any())
            {
                Clients.Group("DockDoor_" + dockDoor.Properties.DoorNumber).updateDigitalDockDoorStatus(dockDoor.Properties.DockDoorData, id);

            }
        }
        #endregion

        #region
        //MPE data
        internal void UpdateMpeData(string mpeId)
        {
            try
            {
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => f.Value.Properties.Name == mpeId
                    ).Select(y => y.Value).ToList().ForEach(MPE =>
                    {
                        MPE.Properties.MPEWatchData = GetMPEPerfData(mpeId);
                        if (!string.IsNullOrEmpty(MPE.Properties.MPEWatchData.CurSortplan))
                        {
                            MPE.Properties.DPSData = GetDPSData(MPE.Properties.MPEWatchData.CurSortplan);
                            MPE.Properties.StaffingData = GetStaffingSortplan(string.Concat(MPE.Properties.MPEWatchData.MpeType, MPE.Properties.MPEWatchData.MpeNumber, MPE.Properties.MPEWatchData.CurSortplan));
                        }
                        
                        BroadcastMachineStatus(MPE, cs.Id);
                        BroadcastMPEStatus(MPE.Properties.MPEWatchData, mpeId);
                    });
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

      

        private RunPerf GetMPEPerfData(string mpeId)
        {
            RunPerf PerfData = new RunPerf();
            try
            {

                PerfData = AppParameters.MPEPerformance.Where(x => x.Key == mpeId).Select(y => y.Value).FirstOrDefault();
                return PerfData;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return PerfData;
            }
        }
        private void BroadcastMachineStatus(GeoZone mPE, string id)
        {
            Clients.Group("MPEZones").updateMachineStatus(mPE, id);

        }
        private void BroadcastMPEStatus(RunPerf mPEWatchData, string mpeId)
        {
            Clients.Group("MPE_"+ mpeId).updateMPEStatus(mPEWatchData);
        }
        #endregion
        public List<RouteTrips> GetDigitalDockDoorList(string id)
        {
            try
            {
                List<RouteTrips> doorTrips = new List<RouteTrips>();
                doorTrips = AppParameters.RouteTripsList.Where(x => !Regex.IsMatch(x.Value.State, "(CANCELED|DEPARTED|OMITTED)", RegexOptions.IgnoreCase)
                && x.Value.DoorNumber == id).OrderBy(d => d.Value.ScheduledDtmfmt).OrderByDescending(d => d.Value.AtDoor).Select(y => y.Value).ToList();
                return doorTrips;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
            //try
            //{
            //    List<RouteTrips> doorData = new List<RouteTrips>();
            //    foreach (CoordinateSystem cs in CoordinateSystem.Values)
            //    {
            //        cs.Zones.Where(f => f.Value.Properties.ZoneType == "DockDoor" && f.Value.Properties.DoorNumber.ToLower() == id.ToLower()
            //        ).Select(y => y.Value).ToList().ForEach(DockDoor =>
            //        {
            //            doorData.Add(DockDoor.Properties.DockDoorData);
            //        });
            //    }
            //    return doorData;
            //}
            //catch (Exception e)
            //{
            //    new ErrorLogger().ExceptionLog(e);
            //    return null;
            //}
        }

        internal GeoZone AddZone(string data)
        {
            try
            {

                GeoZone newtempgZone = JsonConvert.DeserializeObject<GeoZone>(data);
                newtempgZone.Properties.Id = Guid.NewGuid().ToString();
                newtempgZone.Properties.RawData = "";
                newtempgZone.Properties.Source = "user";
                newtempgZone.Properties.MPEWatchData = null;
                newtempgZone.Properties.MissionList = null;
                newtempgZone.Properties.DockDoorData = null;
                newtempgZone.Properties.StaffingData = null;
                newtempgZone.Properties.DPSData = null;
                newtempgZone.Properties.ZoneUpdate = true;
               
                if (Regex.IsMatch(newtempgZone.Properties.ZoneType, "^(MPE|Machine|Bin)", RegexOptions.IgnoreCase))
                {
                    //get the MPE Number
                    if (int.TryParse(string.Join(string.Empty, Regex.Matches(newtempgZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                    {
                        newtempgZone.Properties.MPENumber = n;
                    }
                    //get the MPE Name
                    newtempgZone.Properties.MPEType = string.Join(string.Empty, Regex.Matches(newtempgZone.Properties.Name, @"\p{L}+").OfType<Match>().Select(m => m.Value));
                }
                if (Regex.IsMatch(newtempgZone.Properties.ZoneType, "^(DockDoor)", RegexOptions.IgnoreCase))
                {
                    //get the DockDoor Number
                    if (int.TryParse(string.Join(string.Empty, Regex.Matches(newtempgZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                    {
                        newtempgZone.Properties.DoorNumber = n.ToString();
                    }
                    newtempgZone.Properties.DockDoorData = GetDigitalDockDoorList(newtempgZone.Properties.DoorNumber);

                  
                }

         
                if (CoordinateSystem.ContainsKey(newtempgZone.Properties.FloorId))
                {
                    if (CoordinateSystem.TryGetValue(newtempgZone.Properties.FloorId, out CoordinateSystem cs))
                    {
                        if (cs.Zones.TryAdd(newtempgZone.Properties.Id, newtempgZone))
                        {
                            BroadcastAddZone(newtempgZone, newtempgZone.Properties.FloorId, newtempgZone.Properties.ZoneType);
                            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList()));
                            return null;
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
        private void BroadcastAddZone(GeoZone Zone, string floorId, string zoneType)
        {
            Clients.All.addZone(Zone, floorId, zoneType);
        }
        private void BroadcastRemoveZone(GeoZone Zone, string floorId, string zoneType)
        {
            Clients.All.removeZone(Zone, floorId, zoneType);
        }
        private void BroadcastUpdateZone(GeoZone Zone, string floorId, string zoneType)
        {
            Clients.Group(zoneType).updateZone(Zone, floorId, zoneType);
        }

        internal IEnumerable<string> GetSVZoneNameList()
        {
            try
            {
                return AppParameters.SVZoneNameList.Select(y => y.Value.LocationName).ToList();
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
                return AppParameters.MPEPerformance.Select(y => y.Key).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        //public void UpdateCameraImages(object state)
        //{
        //    try
        //    {
        //        lock (updateCameralock)
        //        {
        //            if (!_updateCameraStatus)
        //            {
        //                foreach (CoordinateSystem cs in CoordinateSystem.Values)
        //                {
        //                    cs.Locators.Where(f => f.Value.Properties.TagType != null &&
        //                    f.Value.Properties.TagType == "Camera").Select(y => y.Value).ToList().ForEach(Camera =>
        //                    {
        //                        if (Camera.Properties.CameraData == null)
        //                        {
        //                            Cameras cam = new Cameras();
        //                            if (AppParameters.CameraInfoList.TryGetValue(Camera.Properties.Name, out Cameras existingValue))
        //                            {

        //                                cam.FacilitySubtypeDesc = existingValue.FacilitySubtypeDesc;
        //                                cam.AuthKey = existingValue.AuthKey;
        //                                cam.Description = existingValue.Description;
        //                                cam.FacilitiyLatitudeNum = existingValue.FacilitiyLatitudeNum;
        //                                cam.FacilitiyLongitudeNum = existingValue.FacilitiyLongitudeNum;
        //                                cam.FacilityDisplayName = existingValue.FacilityDisplayName;
        //                                cam.FacilityPhysAddrTxt = existingValue.FacilityPhysAddrTxt;
        //                                cam.GeoProcDivisionNm = existingValue.GeoProcDivisionNm;
        //                                cam.GeoProcRegionNm = existingValue.GeoProcRegionNm;
        //                                cam.LocaleKey = existingValue.LocaleKey;
        //                                cam.ModelNum = existingValue.ModelNum;
        //                                cam.Reachable = existingValue.Reachable;
        //                                cam.CameraName = existingValue.CameraName;
        //                                cam.Base64Image = AppParameters.NoImage;
        //                                cam.Alerts = null;
        //                            }
        //                            else
        //                            {
        //                                cam.Base64Image = AppParameters.NoImage;
        //                                cam.CameraName = Camera.Properties.Name;
        //                            }
        //                            Camera.Properties.CameraData = cam;
        //                            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList()));

        //                        }
        //                    //if (Camera.Properties.CameraData.Base64Image != TryUpdateCameraStatus(Camera.Properties.CameraData.CameraName, out string Base64Img))
        //                    //{
        //                    //    Camera.Properties.CameraData.Base64Image = Base64Img;
        //                    //    BroadcastCameraStatus(Camera, cs.Id);
        //                    //}
        //                        if(TryUpdateCameraStatus(Camera))
        //                        {
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

        public void BroadcastCameraStatus(GeoMarker marker, string floorId)
        {
            Clients.Group("CameraMarkers").updateCameraStatus(marker, floorId);
        }
        public void BroadcastAddMarker(GeoMarker marker, string floorId)
        {
            Clients.All.addMarker(marker, floorId);
        }
        public void BroadcastRemoveMarker(GeoMarker marker, string floorId)
        {
            Clients.All.removeMarker(marker, floorId);
        }
        //public void UpdateCameraImages(object state)
        //{
        //    try
        //    {
        //        lock (updateCameralock)
        //        {

        //            if (!_updateCameraStatus)
        //            {
        //                foreach (CoordinateSystem cs in CoordinateSystem.Values)
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

        //private bool TryUpdateCameraStatus(GeoMarker camera)
        //{
        //    bool updateImage = false;
        //    if(!string.IsNullOrEmpty(AppParameters.ConnectionList.Where(x => x.Value.MessageType.ToUpper() == "getCameraStills".ToUpper() 
        //    && x.Value.ActiveConnection).Select(y => y.Value.Id).FirstOrDefault()))
        //    {
        //        updateImage = camera.Properties.TagUpdate;
        //    }
        //    else if(camera.Properties.CameraData.Base64Image != AppParameters.NoImage)
        //    {
        //        camera.Properties.CameraData.Base64Image = AppParameters.NoImage;
        //        updateImage = true;
        //    }
        //    camera.Properties.TagUpdate = false;
        //    return updateImage;
        //}
        //private static string TryUpdateCameraStatus(string camera, out string imageBase64)
        //{
        //    imageBase64 = AppParameters.NoImage;
        //    try
        //    {
        //        string url = @"http://" + camera + @"/axis-cgi/jpg/image.cgi?resolution=320x240";
        //        Uri thisUri = new Uri(url);

        //        using (WebClient client = new WebClient())
        //        {
        //            try
        //            {

        //                client.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");

        //                //add header
        //                byte[] result = client.DownloadData(url);
        //                imageBase64 = "data:image/jpeg;base64," + Convert.ToBase64String(result);
        //                //if (camera.Properties.Base64Image != imageBase64)
        //                //{
        //                return imageBase64;
        //                //}


        //            }
        //            catch (ArgumentException ae) {
        //                new ErrorLogger().ExceptionLog(ae);
        //                return AppParameters.NoImage;
        //            }
        //            catch (WebException we) {
        //                new ErrorLogger().ExceptionLog(we);
        //                return AppParameters.NoImage;
        //            }

        //        }
        //    }

        //    catch (WebException we) {
        //        new ErrorLogger().ExceptionLog(we);
        //        return AppParameters.NoImage;
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return AppParameters.NoImage;
        //    }
        //}

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

                if (CoordinateSystem.ContainsKey(newtempgMarker.Properties.FloorId))
                {
                    if (CoordinateSystem.TryGetValue(newtempgMarker.Properties.FloorId, out CoordinateSystem cs))
                    {
                        if (cs.Locators.TryAdd(newtempgMarker.Properties.Id, newtempgMarker))
                        {
                            BroadcastAddMarker(newtempgMarker, newtempgMarker.Properties.FloorId);
                            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList()));
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
                GeoMarker markerinfo = null;
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    if (cs.Locators.ContainsKey(data))
                    {
                        if (cs.Locators.TryRemove(data, out markerinfo))
                        {
                            BroadcastRemoveMarker(markerinfo, markerinfo.Properties.FloorId);
                            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList()));
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
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    if (cs.Zones.ContainsKey(data))
                    {
                        if (cs.Zones.TryRemove(data, out GeoZone ZoneInfo))
                        {
                            BroadcastRemoveZone(ZoneInfo, ZoneInfo.Properties.FloorId, ZoneInfo.Properties.ZoneType);
                            new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList()));
                        }
                        //if (cs.Zones.TryGetValue(data, out ZoneInfo))
                        //{
                        //    if (ZoneInfo.Properties.Source == "other")
                        //    {
                        //        ZoneInfo.Properties.Visible = false;
                        //        ZoneInfo.Properties.ZoneUpdate = true;
                        //    }
                        //    else
                        //    {
                        //        removeZone = true;
                        //    }
                        //}
                        //if (removeZone)
                        //{
                        //    if (cs.Zones.TryRemove(data, out ZoneInfo))
                        //    {
                        //        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList()));
                        //    }
                        //}
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
                    if (AppParameters.NotificationList.Any())
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
                if (notification.Type == "dockdoor" && !notification.Delete && notification.Type_Duration == 0)
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
            Clients.Group("Notification").updateNotification(notification);
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

                    foreach (CoordinateSystem cs in CoordinateSystem.Values)
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
                    foreach (CoordinateSystem cs in CoordinateSystem.Values)
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

        internal void BroadcastBinZoneStatus(GeoZone binZone, string id)
        {
            Clients.Group("BinZones").updateBinZoneStatus(binZone, id);
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
                        BroadcastTripsUpdate(trip);

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
                    int tripInMin = new Utility().Get_TripMin(trip.ScheduledDtm);//AppParameters.Get_TripMin(trip.ScheduledDtm);
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
                        BroadcastTripsRemove(r.Id);
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
        public IEnumerable<Container> GetTripContainer(string destSites, string trailerBarcode, out int NotloadedContainers, out int loadedContainers)
        {
            NotloadedContainers = 0;
            loadedContainers = 0;
            IEnumerable<Container> AllContainer = null;
            try
            {
                if (!string.IsNullOrEmpty(destSites) && AppParameters.Containers.Any())
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
                }
                return AllContainer;

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
        public void BroadcastTripsUpdate(RouteTrips trip)
        {
            try
            {
                Clients.Group("Trips").TripsUpdate(trip);
                //if (!string.IsNullOrEmpty(trip.DoorNumber))
                //{
                //    UpdateDoorZone(trip);
                //}
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }


        }
        public void BroadcastTripsAdd(RouteTrips trip)
        {

            try
            {
                Clients.Group("Trips").TripsAdd(trip);
                //if (!string.IsNullOrEmpty(trip.DoorNumber))
                //{
                //    UpdateDoorZone(trip);
                //}
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }


        }
        public void BroadcastTripsRemove(string id)
        {
            Clients.All.TripsRemove(id);
        }
        internal void AddMap(string id, CoordinateSystem cSystem)
        {
            try
            {
                if (!CoordinateSystem.TryAdd(id, cSystem))
                {
                    new ErrorLogger().CustomLog("Unable to add CoordinateSystem " + id, string.Concat(AppParameters.AppSettings["APPLICATION_NAME"].ToString(), "_Applogs"));
                }

                _ = Task.Run(() => new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList())));

            }
            catch (Exception e)
            {

                new ErrorLogger().ExceptionLog(e);
            }
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
                       newNotifi.Type_Time = new Utility().GetSvDate(trip.ScheduledDtm);
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
        internal void UpdateRouteTripDoorAssigment(JToken data)
        {
            try
            {

                if (AppParameters.RouteTripsList.TryGetValue(data["RouteTrip"].ToString(), out RouteTrips trip))
                {
                    trip.DoorId = string.Concat("99D", data["DoorNumber"].ToString().PadLeft(4, '-'));
                    trip.DoorNumber = data["DoorNumber"].ToString();
                    Task.Run(() => saveDoorTripAssociation(trip.DoorNumber, trip.Route, trip.Trip)).ConfigureAwait(false);
                    
                    UpdateDoorZone(trip.DoorNumber);
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        public void saveDoorTripAssociation(string doorNumber, string route, string trip)
        {
            bool update = false;
            try
            {
                if (AppParameters.DoorTripAssociation.ContainsKey(string.Concat(route, trip))
                    && AppParameters.DoorTripAssociation.TryGetValue(string.Concat(route, trip), out DoorTrip dr))
                {
                    if (dr.DoorNumber != doorNumber)
                    {
                        dr.DoorNumber = doorNumber;
                        update = true;
                    }
                    if (dr.Route != route)
                    {
                        dr.Route = route;
                        update = true;
                    }
                    if (dr.Trip != trip)
                    {
                        dr.Trip = trip;
                        update = true;
                    }
                }
                else
                {
                    if (AppParameters.DoorTripAssociation.TryAdd(string.Concat(route, trip), new DoorTrip { DoorNumber = doorNumber, Route = route, Trip = trip }))
                    {
                        update = true;
                    }

                }
                if (update)
                {
                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "DoorTripAssociation.json", JsonConvert.SerializeObject(AppParameters.DoorTripAssociation.Select(x => x.Value).ToList(), Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() }));
                }

            }
            catch (Exception)
            {

                throw;
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
        public void BroadcastQSMUpdate(Connection qSMitem)
        {
            Clients.Group("QSM").UpdateQSMStatus(qSMitem);
        }
        private void UpdateAGVLocationStatus(object state)
        {
            lock (updateAGVLocationStatuslock)
            {
                if (!_updateAGVLocationStatus)
                {
                    _updateAGVLocationStatus = true;

                    foreach (CoordinateSystem cs in CoordinateSystem.Values)
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

        //private void UpdateMachineStatus(object state)
        //{
        //    lock (updateMachineStatuslock)
        //    {
        //        List<Tuple<GeoZone, string>> machineStatuses = new
        //            List<Tuple<GeoZone, string>>();
        //        if (!_updateMachineStatus)
        //        {
        //            _updateMachineStatus = true;
        //            foreach (CoordinateSystem cs in CoordinateSystem.Values)
        //            {
        //                cs.Zones.Where(f => f.Value.Properties.ZoneType == "Machine" && f.Value.Properties.ZoneUpdate).Select(y => y.Value).ToList().ForEach(Machine =>
        //                {
        //                    if (TryUpdateMachineStatus(Machine))
        //                    {

        //                        machineStatuses.Add(new Tuple<GeoZone, string>(Machine, cs.Id));
        //                    }

        //                });
        //            }
        //            if (machineStatuses.Count > 0)
        //            {
        //                BroadcastMachineStatus(machineStatuses);
        //            }

        //            _updateMachineStatus = false;
        //        }
        //    }
        //}

        //private bool TryUpdateMachineStatus(GeoZone machine)
        //{
        //    try
        //    {

        //        machine.Properties.ZoneUpdate = false;
        //        //MPE Performance
        //        machine.Properties.MPEWatchData = GetMPEPerfData(machine.Properties.Name);

        //        //MPE P2P
        //        if (!string.IsNullOrEmpty(machine.Properties.MPEWatchData.CurSortplan))
        //        {
        //            machine.Properties.StaffingData = GetStaffingSortplan(machine.Properties.MPEWatchData.MpeType, machine.Properties.MPEWatchData.MpeNumber, machine.Properties.MPEWatchData.CurSortplan);
        //        }
        //        //MPE DPS
        //        if (!string.IsNullOrEmpty(machine.Properties.MPEWatchData.CurSortplan))
        //        {
        //            machine.Properties.DPSData = GetDPSData(machine.Properties.MPEWatchData.CurSortplan);
        //        }
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return false;
        //    }
        //}
        private Staff GetStaffingSortplan(string id)
        {
            Staff StaffingSortplanData = new Staff();
            try
            {
                AppParameters.StaffingSortplansList.TryGetValue(id, out StaffingSortplanData);
                return StaffingSortplanData;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return StaffingSortplanData;
            }
        }
       
        private static DeliveryPointSequence GetDPSData(string curSortplan)
        {
            DeliveryPointSequence DPSData = new DeliveryPointSequence();
            try
            {
                string tempsortplan = curSortplan.Length >= 7 ? curSortplan.Substring(0, 7) : curSortplan;
                AppParameters.DPSList.TryGetValue(tempsortplan, out DPSData);
                
                return DPSData;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return DPSData;
            }
        }
      
        /* updates tag name after end user clicks on a vehicle, selects edit, and enters
           a new tag name and submits
        */
        private string GetQuuppaTagChangeFilename(string tagId, DateTime thisTime)
        {
            return thisTime.Year + "-" + thisTime.Month.ToString("00") + "-" +
                thisTime.Day.ToString("00") + "_" + tagId;
        }

        internal IEnumerable<CoordinateSystem> GetIndoorMapFloor(string id)

        {
            try
            {
                return CoordinateSystem.Where(r => r.Key == id).Select(y => y.Value).ToList();
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
                if (CoordinateSystem.Keys.Count == 0)
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
                else
                {
                    return CoordinateSystem.Select(y => y.Value).ToList();
                }

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

        private void BroadcastVehiclesUpdate(List<GeoMarker> vehicles, string id)
        {
            Clients.Group("VehiclsMarkers").updateVehicles(vehicles, id);
        }

        internal void UpdateDoorZone(string DoorNumber)
        {
            try
            {

                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => f.Value.Properties.ZoneType == "DockDoor"
                    && f.Value.Properties.DoorNumber == DoorNumber
                    ).Select(y => y.Value).ToList().ForEach(DockDoor =>
                    {
                        DockDoor.Properties.DockDoorData = GetDigitalDockDoorList(DockDoor.Properties.DoorNumber);
                        BroadcastDockDoorStatus(DockDoor, cs.Id);
                    });
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        public void BroadcastDockDoorStatus(GeoZone dockDoor, string id)
        {
            Clients.Group("DockDoorZones").updateDockDoorStatus(dockDoor, id);
            if (dockDoor.Properties.DockDoorData.Any())
            {
                Clients.Group("DockDoor_" + dockDoor.Properties.DoorNumber).updateDigitalDockDoorStatus(dockDoor.Properties.DockDoorData, id);

            }
        }
        //public void BroadcastDockdoorZoneStatus(RouteTrips dockdoortrips, string id)
        //{
        //    Clients.Group("DockDoor_" + dockdoortrips.DoorNumber).updateDigitalDockDoorStatus(dockdoortrips, id);
        //}


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

                    foreach (CoordinateSystem cs in CoordinateSystem.Values)
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
        public void BroadcastVehicleTagStatus(GeoMarker marker, string id)
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
                    foreach (CoordinateSystem cs in CoordinateSystem.Values)
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
        public List<ThroughputValues> GetMachineThroughputMaximums()
        {
            return AppParameters.MachineThroughputMax;
        }
        #region
        // Api Section 
        internal IEnumerable<Connection> GetAPIList(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return AppParameters.RunningConnection.Connection.Select(y => y.ConnectionInfo).ToList();
                }
                else
                {
                    return AppParameters.RunningConnection.Connection.Where(w => w.ConnectionInfo.Id == id).Select(y => y.ConnectionInfo).ToList();
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
            try
            {
                Task.Run(() => AppParameters.RunningConnection.Add(JsonConvert.DeserializeObject<Connection>(data), true)).ConfigureAwait(false);
               // AppParameters.RunningConnection.Add(JsonConvert.DeserializeObject<Connection>(data));

                return null;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<Connection> EditAPI(string data)
        {

            try
            {
              Task.Run(() => AppParameters.RunningConnection.EditAsync(JsonConvert.DeserializeObject<Connection>(data))).ConfigureAwait(false);

                return null;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal IEnumerable<Connection> RemoveAPI(string data)
        {
            try
            {
                AppParameters.RunningConnection.Remove(JsonConvert.DeserializeObject<Connection>(data));
                return null;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        #endregion
        internal IEnumerable<GeoZone> EditZone(string data)
        {
            string id = string.Empty;
            string floorID = string.Empty;
            JObject objectdata = null;
            try
            {
                bool fileUpdate = false;
                if (!string.IsNullOrEmpty(data))
                {
                    objectdata = JObject.Parse(data);
                    if (objectdata.HasValues)
                    {
                        if (objectdata.ContainsKey("floorId") && objectdata.ContainsKey("id"))
                        {
                            floorID = objectdata["floorId"].ToString();
                            id = objectdata["id"].ToString();
                            if (CoordinateSystem.ContainsKey(floorID))
                            {
                                ZoneInfo newzinfo = new ZoneInfo();
                                if (CoordinateSystem.TryGetValue(floorID, out CoordinateSystem cs))
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

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList()));
                }

                return CoordinateSystem[floorID].Zones.Where(w => w.Key == id).Select(s => s.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
            finally
            {
                data = null;
                objectdata = null;
                id = string.Empty;
                floorID = string.Empty;


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
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
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
        internal IEnumerable<BackgroundImage> RemoveFloorPlanData(dynamic data)
        {
            try
            {
                List<BackgroundImage> temp = new List<BackgroundImage>();
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    if (cs.BackgroundImage.Id == data["id"])
                    {
                        //remove image
                    }
                };
                return GetFloorPlanData();
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