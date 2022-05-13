using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
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
        public IEnumerable<JToken> GetAppSettingdata()
        {
            return _managerHub.GetAppSettingdata();
        }

        public IEnumerable<JToken> EditAppSettingdata(string data)
        {
            return _managerHub.EditAppSettingdata(data);
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
        public IEnumerable<GeoMarker> GetPersonTagsList()
        {
            return _managerHub.GetPersonTagsList();
        }

        public IEnumerable<GeoMarker> GetUndetectedTagsList()
        {
            return _managerHub.GetUndetectedTagsList();
        }

        public IEnumerable<GeoMarker> GetLDCAlertTagsList()
        {
            return _managerHub.GetLDCAlertTagsList();
        }

        /// <summary>
        /// Get Trips Data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RouteTrips> GetTripsList()
        {
            return _managerHub.GetTripsList();
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
        /// Get Camera markers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GeoMarker> getCameraMarkerList()
        {
            return _managerHub.getCameraMarkerList();
        }
        /// <summary>
        /// Get Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GeoZone> GetZonesList()
        {
            return _managerHub.GetZonesList();
        }

        public IEnumerable<ZoneInfo> EditZone(string data)
        {
            return _managerHub.EditZone(data);
        }
        /// <summary>
        /// Get dock door Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GeoZone> GetDockDoorZonesList()
        {
            return _managerHub.GetDockDoorZonesList();
        }
        public IEnumerable<string> GetDockDoorList()
        {
            return _managerHub.GetDockDoorList();
        }

        /// <summary>
        /// Get Machine Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GeoZone> GetMachineZonesList()
        {
            return _managerHub.GetMachineZonesList();
        }
        public IEnumerable<string> GetMPEList()
        {
            return _managerHub.GetMPEList();
        }
        /// <summary>
        /// Get Machine Bin Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GeoZone> GetBinZonesList()
        {
            return _managerHub.GetBinZonesList();
        }
        /// <summary>
        /// Get AGV location Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GeoZone> GetAGVLocationZonesList()
        {
            return _managerHub.GetAGVLocationZonesList();
        }
        public IEnumerable<GeoMarker> GetVehicleTagsList()
        {
            return _managerHub.GetVehicleTagsList();
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
        /// Get View Ports Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GeoZone> GetViewPortsZonesList()
        {
            return _managerHub.GetViewPortsZonesList();
        }
        /// <summary>
        /// Get Locator Tags
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GeoMarker> GetLocatorsList()
        {
            return _managerHub.GetLocatorsList();
        }

        /// <summary>
        /// Get Map settings
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BackgroundImage> GetMap()
        {
            return _managerHub.GetIndoorMap();
        }

        ///// <summary>
        ///// Get user Info section
        ///// </summary>
        /////
        public ADUser GetUserProfile()
        {
            string user_id = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim();
            
            return _managerHub.GetUserProfile(user_id);
        }
        ///// <summary>
        ///// Connection handling section
        ///// </summary>
        /////
        public IEnumerable<string> GetTimeZone()
        {
          return _managerHub.GetTimeZone();
            
        }
        /// <summary>
        /// this is to handle connection after the App has started.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            Clients.Caller.userInfo(_managerHub.AddUserProfile(Context));
            Clients.Caller.floorImage(_managerHub.GetIndoorMap());
            return base.OnConnected(); 
        }

        private object GetAuthInfo()
        {
            string userId = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim();
            AppParameters.Users.TryGetValue(userId, out ADUser user);
            return JsonConvert.SerializeObject(user, Formatting.Indented) ;
        }

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
            Task.Run(() => LeaveGroup("PeopleMarkers"));
            Task.Run(() => LeaveGroup("VehiclsMarkers"));
            Task.Run(() => LeaveGroup("MachineZones"));
            Task.Run(() => LeaveGroup("Zones"));
            Task.Run(() => LeaveGroup("BinZones"));
            Task.Run(() => LeaveGroup("DockDoorZones"));
            Task.Run(() => LeaveGroup("AGVLocationZones"));
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            _managerHub.GetConnections(Context.ConnectionId);
            return base.OnReconnected();
        }
    }
}