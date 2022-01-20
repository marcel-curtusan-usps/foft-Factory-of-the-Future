using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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
        /// <returns></returns>
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
        public IEnumerable<JToken> GetAPIList(string data)
        {
            return _managerHub.GetAPIList(data);
        }

        public IEnumerable<JToken> AddAPI(string data)
        {
            return _managerHub.AddAPI(data);
        }

        public IEnumerable<JToken> EditAPI(string data)
        {
            return _managerHub.EditAPI(data);
        }

        public IEnumerable<JToken> RemoveAPI(string data)
        {
            return _managerHub.RemoveAPI(data);
        }

        /// <summary>
        ///  Notification Conditions section
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetNotification_ConditionsList(int data)
        {
            return _managerHub.GetNotification_ConditionsList(data);
        }

        public IEnumerable<JToken> AddNotification_Conditions(string data)
        {
            return _managerHub.AddNotification_Conditions(data);
        }

        public IEnumerable<JToken> EditNotification_Conditions(string data)
        {
            return _managerHub.EditNotification_Conditions(data);
        }

        public IEnumerable<JToken> DeleteNotification_Conditions(string data)
        {
            return _managerHub.DeleteNotification_Conditions(data);
        }

        public IEnumerable<JToken> EditTagInfo(string data)
        {
            return _managerHub.EditTagInfo(data);
        }

        public IEnumerable<JToken> GetNotification(string data)
        {
            return _managerHub.GetNotification(data);
        }

        /// <summary>
        /// Get Containers content.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Container> GetContainer(string data, string Direction, string route, string trip)
        {
            return _managerHub.GetContainer(data, Direction, route, trip);
        }

        /// <summary>
        /// Get Vehicles Markers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetMarkerList()
        {
            return _managerHub.GetMarkerList();
        }

        /// <summary>
        /// Get Person Tags
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetPersonTagsList()
        {
            return _managerHub.GetPersonTagsList();
        }

        public IEnumerable<JToken> GetUndetectedTagsList()
        {
            return _managerHub.GetUndetectedTagsList();
        }

        public IEnumerable<JToken> GetLDCAlertTagsList()
        {
            return _managerHub.GetLDCAlertTagsList();
        }

        /// <summary>
        /// Get Trips Data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JObject> GetTripsList()
        {
            return _managerHub.GetTripsList();
        }
        /// <summary>
        /// Get Specific Trips Data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Trips> GetRouteTripsInfo(string id)
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
        /// Get CTS Data
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<JToken> GetCTSList(string type)
        //{
        //    return _managerHub.GetCTSList(type);
        //}

        /// <summary>
        /// Get CTS OB Details Data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetCTSDetailsList(string route, string trip)
        {
            return _managerHub.GetCTSDetailsList(route, trip);
        }

        /// <summary>
        /// Get Camera feed
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetCameraList()
        {
            return _managerHub.GetCameraList();
        }
        /// <summary>
        /// Get Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetZonesList()
        {
            return _managerHub.GetZonesList();
        }

        public IEnumerable<JToken> EditZone(string data)
        {
            return _managerHub.EditZone(data);
        }
        /// <summary>
        /// Get dock door Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetDockDoorZonesList()
        {
            return _managerHub.GetDockDoorZonesList();
        }

        /// <summary>
        /// Get Machine Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetMachineZonesList()
        {
            return _managerHub.GetMachineZonesList();
        }

        /// <summary>
        /// Get AGV location Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetAGVLocationZonesList()
        {
            return _managerHub.GetAGVLocationZonesList();
        }

        /// <summary>
        /// Get AGV location Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetViewConfigList()
        {
            return _managerHub.GetViewConfigList();
        }

        /// <summary>
        /// Get View Ports Zones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetViewPortsZonesList()
        {
            return _managerHub.GetViewPortsZonesList();
        }
        /// <summary>
        /// Get Locator Tags
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetLocatorsList()
        {
            return _managerHub.GetLocatorsList();
        }

        /// <summary>
        /// Get Map settings
        /// </summary>
        /// <returns></returns>
        public IEnumerable<JToken> GetMap()
        {
            return _managerHub.GetMap();
        }

        /// <summary>
        /// Connection handling section
        /// </summary>
        ///
        public IEnumerable<JToken> GetUserProfile()
        {
            string user_id = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim();
            if (string.IsNullOrEmpty(user_id))
            {
                user_id = Context.ConnectionId;
            }
            return _managerHub.GetUserProfile(user_id);
        }

        public IEnumerable<JToken> GetADUserProfile()
        {
            string user_id = Regex.Replace(Context.User.Identity.Name, @"(USA\\|ENG\\)", "").Trim();
            if (string.IsNullOrEmpty(user_id))
            {
                user_id = Context.ConnectionId;
            }
            return _managerHub.GetADUserProfile(user_id);
        }
        public override Task OnConnected()
        {
            _managerHub.Adduser(Context);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            if (stopCalled)
            {
                _managerHub.Removeuser(Context.ConnectionId);
            }
            else
            {
                _managerHub.Removeuser(Context.ConnectionId);
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