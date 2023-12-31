﻿using Factory_of_the_Future.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{

    public class FOTFManager : IDisposable
    {
        private readonly static Lazy<FOTFManager> _instance = new Lazy<FOTFManager>(() => new FOTFManager(GlobalHost.ConnectionManager.GetHubContext<HubManager>().Clients));
        public readonly ConcurrentDictionary<string, CoordinateSystem> CoordinateSystem = new ConcurrentDictionary<string, CoordinateSystem>();
        public static FOTFManager Instance { get { return _instance.Value; } }
        private IHubConnectionContext<dynamic> Clients { get; set; }
        //public FOTFManager(IHubConnectionContext<dynamic> clients) { Clients = clients; }
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
        //private readonly Timer Zone_timer;
        //private readonly Timer QSM_timer;
        //private readonly Timer Machine_timer;
        //private readonly Timer AGVLocation_timer;
        //private readonly Timer SVTrips_timer;
        //private readonly Timer Notification_timer;
        //private readonly Timer BinZone_timer;
        // private readonly Timer Camera_timer;
        //status
        private volatile bool _updatePersonTagStatus = false;
        private volatile bool _updateZoneStatus = false;
        private volatile bool _updateTagStatus = false;
        //private volatile bool _updatingQSMStatus = false;
        //private volatile bool _updateMachineStatus = false;
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
        private FOTFManager(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
            //VehicleTag_timer = new Timer(UpdateVehicleTagStatus, null, _250updateInterval, _250updateInterval);
            PersonTag_timer = new Timer(UpdatePersonTagStatus, null, _250updateInterval, _250updateInterval);
            /////Zone status.
            //Zone_timer = new Timer(UpdateZoneStatus, null, _2000updateInterval, _2000updateInterval);
            ////DockDoor_timer = new Timer(UpdateDockDoorStatus, null, _250updateInterval, _250updateInterval);
            //Machine_timer = new Timer(UpdateMachineStatus, null, _2000updateInterval, _2000updateInterval);
            //AGVLocation_timer = new Timer(UpdateAGVLocationStatus, null, _250updateInterval, _250updateInterval);
            //BinZone_timer = new Timer(UpdateBinZoneStatus, null, _2000updateInterval, _2000updateInterval);
            ///////SV Trips Data
            //SVTrips_timer = new Timer(UpdateSVTripsStatus, null, _30000updateInterval, _30000updateInterval);
            //////   Notification data timer
            //Notification_timer = new Timer(UpdateNotificationtatus, null, _1000updateInterval, _1000updateInterval);
            ////
            //Connection status
            //QSM_timer = new Timer(UpdateQSM, null, _250updateInterval, _250updateInterval);
            //Camera update;
            //Camera_timer = new Timer(UpdateCameraImages, null, _10000updateInterval, _60000updateInterval);
        }




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

        //internal MPESDOView GetMPESDOStatus(string mpeID)
        //{
        //    try
        //    {
        //        /*Temporary while testing*/
        //        //return AppParameters.MPEPerformance.Where(x => x.Value.MpeId == mpeID).Select(y => y.Value).ToList();
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        return null;
        //    }
        //}
        #region
        //dock door 

        internal void UpdateDoorData(string doorNumber)
        {
            try
            {
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => f.Value.Properties.ZoneType.StartsWith("DockDoor")
                    && f.Value.Properties.DoorNumber == doorNumber
                    ).Select(y => y.Value).ToList().ForEach(DockDoor =>
                    {
                        DockDoor.Properties.MPEWatchData = null;
                        DockDoor.Properties.DPSData = null;
                        DockDoor.Properties.StaffingData = null;
                        DockDoor.Properties.DockDoorData = GetDigitalDockDoorList(DockDoor.Properties.DoorNumber);
                        BroadcastDockDoorUpdateData(DockDoor, cs.Id, DockDoor.Properties.Id);
                    });
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        public void BroadcastDockDoorUpdateData(GeoZone dockDoor, string floorId, string zoneId)
        {
            Clients.Group("DockDoorZones").updateDockDoorData(dockDoor, floorId, zoneId);
            if (dockDoor.Properties.DockDoorData.Any())
            {
                Clients.Group("DockDoor_" + dockDoor.Properties.DoorNumber).updateDigitalDockDoorStatus(dockDoor.Properties.DockDoorData);

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
        internal void UpdateMpeData(string mpeName)
        {
            try
            {
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => f.Value.Properties.Name == mpeName
                    ).Select(y => y.Value).ToList().ForEach(MPE =>
                    {
                        MPE.Properties.MPEWatchData = GetMPEPerfData(mpeName);
                        if (!string.IsNullOrEmpty(MPE.Properties.MPEWatchData.CurSortplan))
                        {
                            if (Regex.IsMatch(MPE.Properties.MPEWatchData.CurOperationId.ToString(), "(918|919)", RegexOptions.IgnoreCase))
                            {
                                MPE.Properties.DPSData = GetDPSData(MPE.Properties.MPEWatchData.CurSortplan);
                            }
                        }
                        if (!string.IsNullOrEmpty(MPE.Properties.MPEGroup))
                        {
                            MPE.Properties.MPEWatchData.ScheduledStaffing = GetStaffingSortplan(string.Concat(MPE.Properties.MPEWatchData.MpeType, MPE.Properties.MPEWatchData.MpeNumber, MPE.Properties.MPEWatchData.CurSortplan));
                            MPE.Properties.MPEWatchData.MPEGroup = MPE.Properties.MPEGroup;
                            BroadcastMPESDOStatus(MPE.Properties.MPEWatchData, MPE.Properties.MPEGroup);
                        }
                        BroadcastMachineStatus(MPE, cs.Id);
                        BroadcastMPEStatus(MPE.Properties.MPEWatchData, mpeName);
                    });
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal List<RunPerf> UpdateMpeSDOData(string mpeGroupName)
        {
            List<RunPerf> MPEInGroup = new List<RunPerf>();
            try
            {
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    /*Retrieve all MPE's within the same group name*/
                   cs.Zones.Where(f => f.Value.Properties.MPEGroup == mpeGroupName).Select(y => y.Value).OrderBy(s => s.Properties.Name).ToList().ForEach(mpe =>
                    {
                        MPEInGroup.Add(mpe.Properties.MPEWatchData);
                    });

                    //foreach (var MPE in mpesInSameGroup)
                    //{
                    //    MPE.Properties.MPEWatchData = GetMPEPerfData(MPE.Properties.Name);
                    //    /*When launching sometimes the MPEWatch data is null, so we ensure to process only when data available*/
                    //    if (MPE.Properties.MPEWatchData != null)
                    //    {
                    //        if (!string.IsNullOrEmpty(MPE.Properties.MPEWatchData.CurSortplan))
                    //        {
                    //            MPE.Properties.DPSData = GetDPSData(MPE.Properties.MPEWatchData.CurSortplan);
                    //            MPE.Properties.StaffingData = GetStaffingSortplan(string.Concat(MPE.Properties.MPEWatchData.MpeType, MPE.Properties.MPEWatchData.MpeNumber, MPE.Properties.MPEWatchData.CurSortplan));
                    //        }

                    //        if (MPEGroups.ContainsKey(MPE.Properties.Name) == false)
                    //        {
                    //            var mpeSDOObject = new MPESDOView(MPE.Properties.MPEType ?? "No MPE Type",
                    //            MPE.Properties.Name ?? "No MPE Name",
                    //            MPE.Properties.MPEWatchData.CurOperationId,
                    //            MPE.Properties.StaffingData != null ? (int)MPE.Properties.StaffingData.Clerk : 0,
                    //            MPE.Properties.CurrentStaff,
                    //            MPE.Properties.MPEWatchData.TotSortplanVol,
                    //            MPE.Properties.MPEWatchData.RpgEstVol,
                    //            MPE.Properties.MPEWatchData.CurThruputOphr,
                    //            MPE.Properties.MPEWatchData.ExpectedThroughput,
                    //            MPE.Properties.MPEWatchData.CurrentRunStart,
                    //            MPE.Properties.MPEWatchData.RPGEndDtm.ToString(),
                    //            "   ");

                    //            if (MPEGroups.TryAdd(MPE.Properties.Name, mpeSDOObject) == false)
                    //            {
                    //                var error = new ErrorLogger();
                    //                error.CustomLog("MPESDO", "There was an error inserting GroupName: " + MPE.Properties.MPEGroup);
                    //            }
                    //        }
                    //    }
                    //}
                }
                return MPEInGroup;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return MPEInGroup;
            }
        }

        private RunPerf GetMPEPerfData(string mpeId)
        {
            RunPerf PerfData = new RunPerf();
            try
            {

                PerfData = AppParameters.MPEPerformance.Where(x => x.Key == mpeId).Select(y => y.Value).FirstOrDefault();
                if (PerfData == null)
                {
                    PerfData = new RunPerf();
                }
                return PerfData;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return PerfData;
            }
        }
        internal void BroadcastMachineStatus(GeoZone mPE, string id)
        {
            Clients.Group("MPEZones").updateMachineStatus(mPE, id);

        }

        internal void BroadcastMPESDOStatus(RunPerf MPE, string mpeGroupName)
        {
            Clients.Group("MPE_" + mpeGroupName).updateMPESDOStatus(MPE);
        }

        internal void BroadcastMPESDORemoval(RunPerf MPE, string mpeGroupName)
        {
            Clients.Group("MPE_" + mpeGroupName).removeMPEFromGroup(MPE);
        }

        internal void BroadcastMPESDOAddition(RunPerf MPE, string mpeGroupName)
        {
            Clients.Group("MPE_" + mpeGroupName).addMPEToGroup(MPE);
        }
       
        //    RemoveMPEFromGroup: (mpeToRemove) => { Promise.all([removeMPEFromGroup(mpetoRemove)])
        //},
        //AddMPEToGroup: (mpeToAdd) => { Promise.all([addMPEToGroup(mpetoAdd)])


        internal void BroadcastMachineAlertStatus(int status, string floorId, string zoneId)
        {
            Clients.Group("MPEZones").updateMPEAlertStatus(status, floorId, zoneId);

        }
        private void BroadcastMPEStatus(RunPerf mPEWatchData, string mpeId)
        {
            Clients.Group("MPE_" + mpeId).updateMPEStatus(mPEWatchData);
        }
        #endregion
        public List<RouteTrips> GetDigitalDockDoorList(string id)
        {
            try
            {
                return AppParameters.RouteTripsList.Where(x => !Regex.IsMatch(x.Value.State, AppParameters.AppSettings.SV_TRIP_STATUS, RegexOptions.IgnoreCase)
                && x.Value.DoorNumber == id).OrderBy(d => d.Value.ScheduledDtmfmt).OrderByDescending(d => d.Value.AtDoor).Select(y => y.Value).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
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

                if (Regex.IsMatch(newtempgZone.Properties.ZoneType, "^(MPE|Machine|Bin)", RegexOptions.IgnoreCase))
                {
                    //get the MPE Number
                    if (int.TryParse(string.Join(string.Empty, Regex.Matches(newtempgZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                    {
                        newtempgZone.Properties.MPENumber = n;
                    }
                    //get the MPE Name
                    newtempgZone.Properties.MPEType = string.Join(string.Empty, Regex.Matches(newtempgZone.Properties.Name, @"\p{L}+").OfType<Match>().Select(m => m.Value));
                    newtempgZone.Properties.MPEWatchData = GetMPEPerfData(newtempgZone.Properties.Name);
                    if (!string.IsNullOrEmpty(newtempgZone.Properties.MPEWatchData.CurSortplan))
                    {
                        newtempgZone.Properties.DPSData = GetDPSData(newtempgZone.Properties.MPEWatchData.CurSortplan);
                        newtempgZone.Properties.StaffingData = GetStaffingSortplan(string.Concat(newtempgZone.Properties.MPEWatchData.MpeType, newtempgZone.Properties.MPEWatchData.MpeNumber, newtempgZone.Properties.MPEWatchData.CurSortplan));
                    }
                }
                if (Regex.IsMatch(newtempgZone.Properties.ZoneType, "^(DockDoor)", RegexOptions.IgnoreCase))
                {
                    //get the DockDoor Number
                    if (int.TryParse(string.Join(string.Empty, Regex.Matches(newtempgZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                    {
                        newtempgZone.Properties.DoorNumber = n.ToString();
                        newtempgZone.Properties.Name = string.Concat("DockDoor", n.ToString().PadLeft(3, '0'));
                    }
                    if (!AppParameters.DockdoorList.ContainsKey(newtempgZone.Properties.DoorNumber) && AppParameters.DockdoorList.TryAdd(newtempgZone.Properties.DoorNumber, newtempgZone.Properties.DoorNumber))
                    {
                        //
                    }
                    newtempgZone.Properties.DockDoorData = GetDigitalDockDoorList(newtempgZone.Properties.DoorNumber);

                }
                if (CoordinateSystem.ContainsKey(newtempgZone.Properties.FloorId))
                {
                    if (CoordinateSystem.TryGetValue(newtempgZone.Properties.FloorId, out CoordinateSystem cs))
                    {
                        if (cs.Zones.TryAdd(newtempgZone.Properties.Id, newtempgZone))
                        {
                            _ = Task.Run(() => BroadcastAddZone(newtempgZone, newtempgZone.Properties.FloorId, newtempgZone.Properties.ZoneType)).ConfigureAwait(false);
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
            return AppParameters.AppSettings.FACILITY_TIMEZONE;
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
                        trip.Containers = (List<Container>)GetTripContainer(trip.DestSites, trip.TrailerBarcode, out int NotloadedContainers, out int loaded);
                        trip.NotloadedContainers = NotloadedContainers;
                    }
                    trip.State = state;

                }
                else
                {
                    //to remove data from list after CANCELED|DEPARTED|OMITTED|COMPLETE
                    if (AppParameters.RouteTripsList.TryRemove(routetripid, out RouteTrips r))
                    {
                        _ = Task.Run(() => CheckNotification(trip.State, state, "routetrip", trip, trip.NotificationId));
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
                   && !r.Value.hasLoadScans
                   && !r.Value.containerTerminate
                   && !r.Value.containerAtDest
                   && r.Value.hasCloseScans).Select(y => y.Value).ToList();
                    NotloadedContainers = TripContainer.Count();
                    AllContainer = TripContainer;
                    if (!string.IsNullOrEmpty(trailerBarcode))
                    {
                        IEnumerable<Container> LoadedContainer = AppParameters.Containers.Where(r => !string.IsNullOrEmpty(r.Value.Trailer)
                       && Regex.IsMatch(r.Value.Trailer, trailerBarcode, RegexOptions.IgnoreCase)
                       && r.Value.hasLoadScans).Select(y => y.Value).ToList();
                       AllContainer = TripContainer.Concat(LoadedContainer);
                        loadedContainers = LoadedContainer.Count();
                    }
                
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
                //    UpdateDoorZone(trip.DoorNumber);
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
                    _ = new ErrorLogger().CustomLog("Unable to add CoordinateSystem " + id, string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "_Applogs")).ConfigureAwait(false);
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
                       _ = AppParameters.NotificationList.TryAdd(noteification_id, newNotifi);

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
                    _ = Task.Run(() => saveDoorTripAssociation(trip.DoorNumber, trip.Route, trip.Trip)).ConfigureAwait(false);

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
            string RouteTrip = string.Concat(route, trip);
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
                    if (!string.IsNullOrEmpty(RouteTrip) && AppParameters.DoorTripAssociation.TryAdd(RouteTrip, new DoorTrip { DoorNumber = doorNumber, Route = route, Trip = trip }))
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
        public void BroadcastAddConnection(Connection Connectionitem)
        {
            Clients.Group("QSM").AddConnection(Connectionitem);
        }
        public void BroadcastRemoveConnection(Connection Connectionitem)
        {
            Clients.Group("QSM").RemoveConnection(Connectionitem);
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
                _ = AppParameters.StaffingSortplansList.TryGetValue(id, out StaffingSortplanData);
                return StaffingSortplanData;
            }
            catch (Exception ex)
            {
                new ErrorLogger().ExceptionLog(ex);
                return StaffingSortplanData;
            }
        }

        private DeliveryPointSequence GetDPSData(string curSortplan)
        {
            DeliveryPointSequence DPSData = new DeliveryPointSequence();
            try
            {
                string tempsortplan = curSortplan.Length >= 7 ? curSortplan.Substring(0, 7) : curSortplan;
                _ = AppParameters.DPSList.TryGetValue(tempsortplan, out DPSData);

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
        internal object GetIndoorMap()
        {
            try
            {
                //if (CoordinateSystem.Keys.Count == 0)
                //{
                //    ConcurrentDictionary<string, CoordinateSystem> CoordinateSystem = new ConcurrentDictionary<string, CoordinateSystem>();

                //    CoordinateSystem temp = new CoordinateSystem
                //    {
                //        Id = "temp",
                //        BackgroundImage = new BackgroundImage
                //        {
                //            Id = "temp",
                //            //FacilityName = !string.IsNullOrEmpty(AppParameters.AppSettings["FACILITY_NAME"].ToString()) ? AppParameters.AppSettings["FACILITY_NAME"].ToString() : "Site Not Configured",
                //            //ApplicationFullName = AppParameters.AppSettings["APPLICATION_FULLNAME"].ToString(),
                //            //ApplicationAbbr = AppParameters.AppSettings["APPLICATION_NAME"].ToString(),

                //        }
                //    };
                //    CoordinateSystem.TryAdd(temp.Id, temp);
                //    return CoordinateSystem.OrderByDescending(or => or.Value.Zones).Select(y => y.Value.backgroundImages).ToList();
                //}
                //else
                //{
                    return CoordinateSystem.Select(y => y.Value.BackgroundImage).ToList();
               // }

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal object GetIndoorMapZones()
        {
            try
            {
                return CoordinateSystem.OrderByDescending(or => or.Value.Zones.Any()).Select(y => y.Value.Zones).ToList();
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        internal object GetIndoorMapLocatortag()
        {
            List<GeoMarker> temp = new List<GeoMarker>();
            try
            {
                //return CoordinateSystem.Select(y => y.Value.Locators).ToList().ForEach();
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Locators.Where(f => f.Value.Properties.TagType.EndsWith("Locator")
                        ).Select(y => y.Value).ToList().ForEach(marker =>
                        {
                            temp.Add(marker);
                        });
                }
                return temp;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);

                temp = null;
                return temp;
            }
            finally
            {
                temp = null;
            }
        }
        internal object GetIndoorMapCameratag()
        {
            List<GeoMarker> temp = new List<GeoMarker>();
            try
            {
                //return CoordinateSystem.Select(y => y.Value.Locators).ToList().ForEach();
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Locators.Where(f => f.Value.Properties.TagType.EndsWith("CameraMarker")
                        ).Select(y => y.Value).ToList().ForEach(marker =>
                        {
                            temp.Add(marker);
                        });
                }
                return temp;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);

                temp = null;
                return temp;
            }
            finally
            {
                temp = null;
            }
        }
        internal object GetIndoorMapPersontag()
        {
            List<GeoMarker> temp = new List<GeoMarker>();
            try
            {
                //return CoordinateSystem.Select(y => y.Value.Locators).ToList().ForEach();
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Locators.Where(f => f.Value.Properties.TagType.EndsWith("Person")
                        && f.Value.Properties.Visible
                        ).Select(y => y.Value).ToList().ForEach(marker =>
                        {
                            temp.Add(marker);
                        });
                }
                return temp;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
              
                temp = null;
                return temp;
            }
            finally 
            {
                temp = null;
            }
        }
        internal object GetIndoorMapVehicletag()
        {
            List<GeoMarker> temp = new List<GeoMarker>();
            try
            {
                //return CoordinateSystem.Select(y => y.Value.Locators).ToList().ForEach();
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Locators.Where(f => f.Value.Properties.TagType.EndsWith("Vehicle")
                        ).Select(y => y.Value).ToList().ForEach(marker =>
                        {
                            temp.Add(marker);
                        });
                }
                return temp;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);

                temp = null;
                return temp;
            }
            finally
            {
                temp = null;
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
                        BroadcastDockDoorStatus(DockDoor, cs.Id, DockDoor.Properties.Id);
                    });
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        public void BroadcastDockDoorStatus(GeoZone dockDoor, string floorId, string zoneId)
        {
            Clients.Group("DockDoorZones").updateDockDoorData(dockDoor, floorId, zoneId);
            if (dockDoor.Properties.DockDoorData.Any())
            {
                Clients.Group("DockDoor_" + dockDoor.Properties.DoorNumber).updateDigitalDockDoorStatus(dockDoor.Properties.DockDoorData);

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
                   
                    foreach (CoordinateSystem cs in CoordinateSystem.Values)
                    {
                        cs.Locators.Where(f => f.Value.Properties.TagType.EndsWith("Vehicle")
                        && f.Value.Properties.TagUpdate
                        ).Select(y => y.Value).ToList().ForEach(marker =>
                        {
                            marker.Properties.TagUpdate = false;
                            BroadcastVehicleTagStatus(marker, cs.Id);

                        });
                    }

                    _updateTagStatus = false;
                }
            }
        }
        public void BroadcastVehicleTagStatus(object marker, string id)
        {
            Clients.Group("VehiclsMarkers").updateVehicleTagStatus(marker, id);
        }
        public void UpdatePersonTagStatus(object state)
        {
            lock (updatePersonTagStatuslock)
            {
                if (!_updatePersonTagStatus)
                {
                    _updatePersonTagStatus = true;
                    long dt = new DateTimeOffset(DateTime.Now.ToUniversalTime()).ToUnixTimeMilliseconds();
                    //  var watch = new System.Diagnostics.Stopwatch();
                    // watch.Start();
                    foreach (CoordinateSystem cs in CoordinateSystem.Values)
                    {
                        cs.Locators.Where(f => f.Value.Properties.TagType.EndsWith("Person")
                             && (dt - f.Value.Properties.PositionTS) > AppParameters.AppSettings.TAG_TIMEOUTMILS
                             && f.Value.Properties.Visible
                             ).Select(y => y.Value).ToList().ForEach((m) =>
                             {
                                 m.Properties.isPosition = false;
                                 m.Properties.posAge = 0;
                                 m.Properties.Visible = false;
                                 m.Properties.LocationMovementStatus = "noData";
                                 FOTFManager.Instance.BroadcastPersonTagRemove(m.Properties.Id, m.Properties.FloorId);

                             });


                    }
                    // watch.Stop();
                    // new ErrorLogger().CustomLog(string.Concat("Total Execution for all tags ", "Time: ", watch.ElapsedMilliseconds, " ms"), string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "TagProcesslogs"));
                    _updatePersonTagStatus = false;
                }
            }
        }
     

        public void BroadcastPersonTagStatus(object Marker, string id)
        {
            Clients.Group("PeopleMarkers").updatePersonTagStatus(Marker, id);
        }
        public void BroadcastPersonTagRemove(object Marker, string id)
        {
            Clients.Group("PeopleMarkers").updatePersonTagRemove(Marker, id);
        }
        public void BroadcastMarkerCoordinatesUpdate(MarkerGeometry Coordinates, string floorid, string markerid)
        {
            Clients.Group("PeopleMarkers").updateMarkerCoordinates(Coordinates, floorid, markerid);
        }
        #region
        // Api Section 
        internal IEnumerable<Connection> GetAPIList(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return AppParameters.RunningConnection.DataConnection.Select(y => y.ConnectionInfo).ToList();
                }
                else
                {
                    return AppParameters.RunningConnection.DataConnection.Where(w => w.ConnectionInfo.Id == id).Select(y => y.ConnectionInfo).ToList();
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
                if (!string.IsNullOrEmpty(data))
                {
                    _ = Task.Run(() => AppParameters.RunningConnection.Add(JsonConvert.DeserializeObject<Connection>(data), true)).ConfigureAwait(false);
                }
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
                if (!string.IsNullOrEmpty(data))
                {
                    _ = Task.Run(() => AppParameters.RunningConnection.EditAsync(JsonConvert.DeserializeObject<Connection>(data))).ConfigureAwait(false);
                }
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
                if (!string.IsNullOrEmpty(data))
                {
                    _ = Task.Run(() => AppParameters.RunningConnection.Remove(JsonConvert.DeserializeObject<Connection>(data))).ConfigureAwait(true);
                }

                return null;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        #endregion
        internal async Task<IEnumerable<GeoZone>> EditZoneAsync(string data)
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
                            if (CoordinateSystem.ContainsKey(floorID) && CoordinateSystem.TryGetValue(floorID, out CoordinateSystem cs))
                            {
                                if (cs.Zones.ContainsKey(id) && cs.Zones.TryGetValue(id, out GeoZone gz))
                                {
                                    string tempMPEGroup = gz.Properties.MPEGroup;//Store the previous mpegroup so we can add it to the MPEWatch object
                                    gz.Properties.MPENumber = (int)objectdata["MPE_Number"];
                                    gz.Properties.MPEType = objectdata["MPE_Type"].ToString();
                                    gz.Properties.Name = string.Concat(gz.Properties.MPEType, "-", gz.Properties.MPENumber.ToString().PadLeft(3, '0'));
                                    gz.Properties.MPEGroup = objectdata["MPE_Group"].ToString();
                                    gz.Properties.QuuppaOverride = true;
                                    gz.Properties.MPEWatchData = GetMPEPerfData(gz.Properties.Name);
                                    await Task.Run(() => BroadcastMachineStatus(gz, floorID)).ConfigureAwait(false);
                                    
                                    if (string.IsNullOrEmpty(tempMPEGroup)) //Adding a new MPE
                                    {
                                        await Task.Run(() => BroadcastMPESDOAddition(gz.Properties.MPEWatchData, gz.Properties.MPEGroup)).ConfigureAwait(false);
                                    }
                                    else  //Deleting existing MPE
                                    {
                                        await Task.Run(() => BroadcastMPESDORemoval(gz.Properties.MPEWatchData, tempMPEGroup)).ConfigureAwait(false);
                                    }
                                   
                                    fileUpdate = true;
                                }

                            }
                        }

                    }
                }

                if (fileUpdate)
                {
                    new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList()));
                }

                return null; // CoordinateSystem[floorID].Zones.Where(w => w.Key == id).Select(s => s.Value).ToList();
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
        internal IEnumerable<BackgroundImage> RemoveFloorPlanData(dynamic id)
        {
            try
            {
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    if (cs.Id == id && CoordinateSystem.TryRemove(cs.Id, out CoordinateSystem remove))
                    {
                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(CoordinateSystem.Select(x => x.Value).ToList()));
                    }
                }
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
                _ = new ErrorLogger().CustomLog(data, string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "_Applogs")).ConfigureAwait(false);

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

        internal IEnumerable<string> GetMPEGroupList()
        {
            try
            {
                /*Retrieve all group names available*/
                var groupNames = new List<string>();
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => (f.Value.Properties.ZoneType == "Machine" || f.Value.Properties.ZoneType == "MPEZone"))
                            .Select(y => y.Value)
                            .ToList().ForEach(zone =>
                            {
                                if (!string.IsNullOrEmpty(zone.Properties.MPEGroup))
                                {
                                    groupNames.Add(zone.Properties.MPEGroup);
                                }
                            });
                }
                /*Eliminate duplicates*/
                if (groupNames.Any() == true)
                {
                    var groupNamesNoDup = groupNames.Distinct();
                    return groupNamesNoDup;
                }

                return groupNames;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        internal JObject DeviceScan(JObject request_data, string v)
        {
            bool locationfound = false;
            string BarcodeType = "";
            //string BodyType = "";
            try
            {
                foreach (CoordinateSystem cs in CoordinateSystem.Values)
                {
                    cs.Zones.Where(f => (f.Value.Properties.ZoneType == "AGVLocationZone" || f.Value.Properties.ZoneType == "BullpenZone"))
                            .Select(y => y.Value)
                            .ToList().ForEach(zone =>
                            {
                                if (Regex.IsMatch(zone.Properties.Name, "(|919)", RegexOptions.IgnoreCase))
                                 //   if (zone.Properties.Name == request_data["BARCODE"].ToString())
                                {
                                    locationfound = true;
                                }
                            });
                }
                if (locationfound)
                {
                    return new JObject
                    {
                        ["RESPONSE_CODE"] = "0",
                        ["BARCODE_TYPE"] = BarcodeType == "PIC" ? "Pickup" : "Drop-Off",
                        ["Body_Type"] = BarcodeType == "AGP" ? "PALLET" : "TUGGER",
                        ["Barcode"] = request_data["BARCODE"].ToString()
                    };
                }
                else
                {
                    return new JObject
                    {
                        ["RESPONSE_CODE"] = "-1",
                        ["RESPONSE_MSG"] = ""
                    };
                }

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject
                {
                    ["RESPONSE_CODE"] = "-1",
                    ["RESPONSE_MSG"] = ""
                };
            }
        }

        internal IEnumerable<JObject> GetStaffSchedule()
        {
            IEnumerable<JObject> scheduleEmployee = new List<JObject>();
            try
            {
            
                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                {
                    var query = cs.Locators.Where(sl => sl.Value.Properties.TagType == "Person").Select(r => r.Value.Properties).ToList();
                    scheduleEmployee = from se in query
                                       select new JObject
                                       {
                                           ["tagId"] = se.Id,
                                           ["empId"] = se.EmpId,
                                           ["tagType"] = se.TagType,
                                           ["bdate"] = se.Bdate,
                                           ["blunch"] = se.Blunch,
                                           ["elunch"] = se.Elunch,
                                           ["edate"] = se.Edate,
                                           ["reqDate"] = se.ReqDate,
                                           ["emptype"] = se.Emptype,
                                           ["badgeId"] = se.BadgeId,
                                           ["empName"] = se.EmpName,
                                           ["craftName"] = se.CraftName,
                                           ["isSch"] = se.isSch,
                                           ["isTacs"] = se.isTacs,
                                           ["isePacs"] = se.isePacs,
                                           ["isPosition"] = se.isPosition
                                       };
                  
                }
                return scheduleEmployee;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return scheduleEmployee;
            }
        }

      
    }
}