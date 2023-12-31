﻿
using Factory_of_the_Future.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    [HubName("FOTFManager")]
    public class HubManager : Hub
    {
        private readonly FOTFManager _managerHub;

        public HubManager() : this(FOTFManager.Instance)
        {
        }

        public HubManager(FOTFManager managerHub)
        {
            _managerHub = managerHub;
        }

        /// <summary>
        /// /Application setting section
        /// </summary>
        public AppSetting GetAppSettingdata()
        {
            return new ApplicationSetting().GetAppSetting();
        }


        public AppSetting EditAppSettingdata(string data)
        {
            return new ApplicationSetting().EditAppSetting(data);

        }
        public object GetConnectionTypes()
        {
            return new ConnectionTypes().Get();
        }


        public object EditConnectionTypes(string data)
        {
            return new ConnectionTypes().Edit(data);

        }
        public object EditSiteInfo(string data)
        {
            return new SiteInfo().Edit(data);
        }

        public IEnumerable<BackgroundImage> GetFloorPlanData()
        {
            return _managerHub.GetFloorPlanData();
        }
        public IEnumerable<BackgroundImage> RemoveFloorPlanData(dynamic Data)
        {
            return _managerHub.RemoveFloorPlanData(Data);
        }
        /// <summary>
        /// /API section
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Connection> GetAPIList(string data)
        {
            return _managerHub.GetAPIList(data);
        }

        public IEnumerable<Connection> AddAPI(string data)
        {
            return _managerHub.AddAPI(data);
        }

        public IEnumerable<Connection> EditAPI(string data)
        {
            return _managerHub.EditAPI(data);
        }

        public IEnumerable<Connection> RemoveAPI(string data)
        {
            return _managerHub.RemoveAPI(data);
        }

        /// <summary>
        ///  Notification Conditions section
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NotificationConditions> GetNotification_ConditionsList()
        {
            return _managerHub.GetNotification_ConditionsList();
        }
        public IEnumerable<NotificationConditions> GetNotification_Conditions(string data)
        {
            return _managerHub.GetNotification_Conditions(data);
        }
        public IEnumerable<NotificationConditions> AddNotification_Conditions(string data)
        {
            return _managerHub.AddNotification_Conditions(data);
        }

        public IEnumerable<NotificationConditions> EditNotification_Conditions(string data)
        {
            return _managerHub.EditNotification_Conditions(data);
        }

        public IEnumerable<NotificationConditions> DeleteNotification_Conditions(string data)
        {
            return _managerHub.DeleteNotification_Conditions(data);
        }

        public IEnumerable<Notification> GetNotification(string data)
        {
            return _managerHub.GetNotification(data);
        }

        /// <summary>
        /// Get Person Tags
        /// </summary>
        /// <returns></returns>

        //public IEnumerable<GeoMarker> GetUndetectedTagsList()
        //{
        //    return _managerHub.GetUndetectedTagsList();
        //}

        //public IEnumerable<GeoMarker> GetLDCAlertTagsList()
        //{
        //    return _managerHub.GetLDCAlertTagsList();
        //}

        /// <summary>
        /// Trips Data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RouteTrips> GetTripsList()
        {
            return _managerHub.GetTripsList();
        }
        public void UpdateRouteTripDoorAssigment(JToken data)
        {
            _managerHub.UpdateRouteTripDoorAssigment(data);
        }

        /// <summary>
        /// Get Specific Trips Data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RouteTrips> GetRouteTripsInfo(string id)
        {
            return _managerHub.GetRouteTripsInfo(id);
        }
        /// <summary>
        /// Get Specific placard data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Container> GetContainerInfo(string id)
        {
            return _managerHub.GetContainerInfo(id);
        }

        /// <summary>
        /// Get Camera feed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cameras> GetCameraList()
        {
            return _managerHub.GetCameraList();
        }

        /// <summary>
        /// Get Zones
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<GeoZone> GetZonesList()
        //{
        //    return _managerHub.GetZonesList();
        //}

        public void EditZone(string data)
        {
            _ = Task.Run(() => _managerHub.EditZoneAsync(data)).ConfigureAwait(false);
        }

        /// <summary>
        /// Get dock door Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetDockDoorList()
        {
            return _managerHub.GetDockDoorList();
        }
        public IEnumerable<RouteTrips> GetDigitalDockDoorList(string id)
        {
            return _managerHub.GetDigitalDockDoorList(id);
        }
        public IEnumerable<RunPerf> GetMPEStatusList(string id)
        {
            return _managerHub.GetMPEStatusList(id);
        }

        public List<RunPerf> GetMPESDOStatus(string mpeGroupName)
        {
            return _managerHub.UpdateMpeSDOData(mpeGroupName);
        }
        /// <summary>
        /// Get SV Zones
        /// </summary>
        /// <returns></returns>

        public IEnumerable<string> GetSVZoneNameList()
        {
            return _managerHub.GetSVZoneNameList();
        }
        /// <summary>
        /// Get Machine Zones
        /// </summary>
        /// <returns></returns>

        public IEnumerable<string> GetMPEList()
        //public MPEGroupsNames GetMPEList()
        {
            return _managerHub.GetMPEList();
        }
        public IEnumerable<string> GetMPEGroupList()
        {
            return _managerHub.GetMPEGroupList();
        }
        /// <summary>
        /// Add New Custom Zones
        /// </summary>
        /// <returns></returns>
        public GeoZone AddZone(string data)
        {
            return _managerHub.AddZone(data);
        }
        public GeoZone RemoveZone(string data)
        {
            return _managerHub.RemoveZone(data);
        }
        public GeoMarker AddMarker(string data)
        {
            return _managerHub.AddMarker(data);
        }
        public GeoMarker RemoveMarker(string data)
        {
            return _managerHub.RemoveMarker(data);
        }

        /// <summary>
        /// Get Map settings
        /// </summary>
        /// <returns></returns>
        public object GetMap()
        {
            return _managerHub.GetIndoorMap();
        }
        public IEnumerable<CoordinateSystem> GetIndoorMapFloor(string id)
        {
            return _managerHub.GetIndoorMapFloor(id);
        }


        /// <summary>
        /// Get Staffing info
        /// </summary>
        /// <returns></returns>
        /// 
        public IEnumerable<JObject> GetStaffSchedule()
        {
            return _managerHub.GetStaffSchedule();
        }
        //public async Task<string> UpdateTagName(string tagId, string tagName)
        //{
        //        bool result = await _managerHub.UpdateTagName(tagId, tagName).ConfigureAwait(false);
        //        string updatedString = result ? "updated" : "error";
        //        return @"{""status"":""" + updatedString + @"""}";
        //}

        /// <summary>
        /// Get Vehicle tags
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<GeoMarker> GetVehicleTagsList()
        //{
        //    return _managerHub.GetVehicleTagsList();
        //}
        ///// <summary>
        ///// Get user Info section
        ///// </summary>
        /////
        //public ADUser GetUserProfile()
        //{
        //    string user_id = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim();

        //    return _managerHub.GetUserProfile(user_id);
        //}
        ///// <summary>
        ///// Connection handling section
        ///// </summary>
        /////
        public IEnumerable<string> GetTimeZone()
        {
            return _managerHub.GetTimeZone();

        }

        public string GetFacilityTimeZone()
        {
            return _managerHub.GetFacilityTimeZone();
        }
        /// <summary>
        /// this is to handle connection after the App has started.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            if (Context.QueryString["page_type"] == "CF")
            {
                Clients.Caller.floorImage(_managerHub.GetIndoorMap());
                Clients.Caller.floorZones(_managerHub.GetIndoorMapZones());
                Clients.Caller.floorLocators(_managerHub.GetIndoorMapLocatortag());
                Clients.Caller.floorCameraMarkers(_managerHub.GetIndoorMapCameratag());
                Clients.Caller.floorPeopleMarkers(_managerHub.GetIndoorMapPersontag());
                Clients.Caller.floorVehiclesMarkers(_managerHub.GetIndoorMapVehicletag());
                Clients.Caller.siteInfo(AppParameters.SiteInfo);
                Clients.Caller.connectionType(new ConnectionTypes().Get());
            }
            _ = Task.Run(() => new ErrorLogger().CustomLog(string.Concat(" Client Connected. User ID:"), string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "_Applogs"))).ConfigureAwait(false);

            return base.OnConnected();
        }

        //private object GetAuthInfo()
        //{
        //    string userId = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim();
        //    AppParameters.Users.TryGetValue(userId, out ADUser user);
        //    return JsonConvert.SerializeObject(user, Formatting.Indented) ;
        //}

        public Task JoinGroup(string groupName)
        {
            return Groups.Add(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.Remove(Context.ConnectionId, groupName);
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            _managerHub.Removeuser(Context.ConnectionId);
            if (Context.QueryString["page_type"] == "CF")
            {
                _ = LeaveGroup("PeopleMarkers");
                _ = LeaveGroup("VehiclsMarkers");
                _ = LeaveGroup("CameraMarkers");
                _ = LeaveGroup("MPEZones");
                _ = LeaveGroup("Zones");
                _ = LeaveGroup("BinZones");
                _ = LeaveGroup("DockDoorZones");
                _ = LeaveGroup("AGVLocationZones");
                _ = LeaveGroup("QSM");
                _ = LeaveGroup("Trips");
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            _managerHub.GetConnections(Context.ConnectionId);
            return base.OnReconnected();
        }
    }
}