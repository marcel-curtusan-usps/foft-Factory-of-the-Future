using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class TagData : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        private bool saveToFile;
        public QuuppaTag tagData = null;
        public bool update;
        public bool remove;
        public bool Coordinatesupdate;
        public GeoMarker marker = null;
        public bool posiblenewtag;
        private readonly object updateTaglock = new object();
        private volatile bool _updateTag = false;
        internal Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            try
            {
                lock (updateTaglock)
                {
                    if (!_updateTag)
                    {
                        var watch = new System.Diagnostics.Stopwatch();
                        watch.Start();
                        _ = new ErrorLogger().CustomLog(string.Concat("Start Execution for all tags in call ", "StartTime: ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), " ms"),
                        string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "TagProcesslogs"));
                        _updateTag = true;
                        saveToFile = true;
                        _data = data;
                        _Message_type = message_type;
                        _connID = connID;
                        if (_data != null)
                        {
                            if (AppParameters.AppSettings.LOG_API_DATA)
                            {
                                new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.LogFloder), string.Concat(message_type, DateTime.Now.ToString("yyyyMMdd"), ".txt"), JsonConvert.SerializeObject(_data, Formatting.Indented));
                            }
                            JToken tempData = JToken.Parse(_data);
                            if (tempData != null && tempData.HasValues)
                            {
                                tagData = tempData.ToObject<QuuppaTag>();

                               // DateTime dt = AppParameters.UnixTimeStampToDateTime(tagData.ResponseTS);
                                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                                {
                                    foreach (Tags qtitem in tagData.Tags)
                                    {
                                        qtitem.ServerTS = tagData.ResponseTS;
                                        update = false;
                                        remove = true;
                                        if (cs.Locators.ContainsKey(qtitem.TagId) && cs.Locators.TryGetValue(qtitem.TagId, out GeoMarker currentMarker))
                                        {

                                            currentMarker.Properties.PositionTS_txt = AppParameters.UnixTimeStampToDateTime(qtitem.LocationTS);
                                            currentMarker.Properties.PositionTS = qtitem.LocationTS;
                                            currentMarker.Properties.LastSeenTS_txt = AppParameters.UnixTimeStampToDateTime(qtitem.LastSeenTS);
                                            currentMarker.Properties.LastSeenTS = qtitem.LastSeenTS;
                                            currentMarker.Properties.ServerTS_txt = AppParameters.UnixTimeStampToDateTime(qtitem.ServerTS);
                                            currentMarker.Properties.ServerTS = qtitem.ServerTS;
                                            currentMarker.Properties.FloorId = qtitem.LocationCoordSysId;
                                            currentMarker.Properties.posAge = qtitem.ServerTS - qtitem.LocationTS;
                                            /*currentMarker.Properties.TagVisibleMils = (int)Math.Ceiling(currentMarker.Properties.ServerTS.Subtract(currentMarker.Properties.PositionTS).TotalMilliseconds);*/
                                            string newLocation = string.Join(",", qtitem.Location);
                                            string oldLocation = string.Join(",", currentMarker.Geometry.Coordinates);
                                            if (newLocation != oldLocation)
                                            {
                                                if (new Regex("^(noData|hidden)$", RegexOptions.IgnoreCase).IsMatch(qtitem.LocationMovementStatus))
                                                {
                                                    currentMarker.Properties.LocationMovementStatus = "noData";
                                                    currentMarker.Properties.isPosition = false;
                                                    currentMarker.Properties.Visible = false;
                                                    update = false;
                                                    remove = true;
                                                }
                                                else if (qtitem.LocationType == "presence")
                                                {
                                                    //MJG: According to the quuppa source file TagDrawable.js:
                                                    //"cannot render tags with 'presense' at all as the location coordinates are null."

                                                    currentMarker.Properties.LocationMovementStatus = "presence";
                                                    currentMarker.Properties.isPosition = false;
                                                    currentMarker.Properties.Visible = false;
                                                    update = false;
                                                    remove = true;
                                                }
                                                else if (qtitem.LocationType == "proximity")
                                                {
                                                    //MJG: According to the quuppa source file TagDrawable.js, tags with location type 'proximity'
                                                    //are not rendered by default. This can be enabled in quuppa settings but currently is not.

                                                    currentMarker.Properties.LocationMovementStatus = "proximity";
                                                    currentMarker.Properties.isPosition = false;
                                                    currentMarker.Properties.Visible = false;
                                                    update = false;
                                                    remove = true;
                                                }

                                                else if (currentMarker.Properties.posAge > AppParameters.AppSettings.TAG_TIMEOUTMILS)
                                                {
                                                    currentMarker.Properties.LocationMovementStatus = "noData";
                                                    currentMarker.Properties.isPosition = false;
                                                    currentMarker.Properties.Visible = false;
                                                    currentMarker.Properties.posAge = 0;
                                                    update = false;
                                                    remove = true;
                                                }
                                                else
                                                {
                                                    currentMarker.Geometry.Coordinates = qtitem.Location;
                                                    currentMarker.Properties.LocationMovementStatus = qtitem.LocationMovementStatus;
                                                    currentMarker.Properties.isPosition = true;
                                                    currentMarker.Properties.Visible = true;
                                                    update = true;
                                                    remove = false;
                                                }
                                            }
                                            else
                                            {
                                                //if (new Regex("^(noData)$", RegexOptions.IgnoreCase).IsMatch(qtitem.LocationMovementStatus))
                                                //{
                                                //    currentMarker.Properties.LocationMovementStatus = "noData";
                                                //    currentMarker.Properties.isPosition = false;
                                                //    currentMarker.Properties.Visible = false;
                                                //    update = false;
                                                //    remove = true;
                                                //}
                                                //else
                                                
                                                if(currentMarker.Properties.posAge > AppParameters.AppSettings.TAG_TIMEOUTMILS)
                                                {
                                                    currentMarker.Properties.Visible = false;
                                                    currentMarker.Properties.isPosition = false;
                                                    currentMarker.Properties.LocationMovementStatus = "noData";
                                                    currentMarker.Properties.posAge = 0;
                                                    update = false;
                                                    remove = true;
                                                }
                                                else
                                                {
                                                    remove = false;
                                                }
                                            }
                                            currentMarker.Properties.Zones = qtitem.LocationZoneIds;
                                            currentMarker.Properties.ZonesNames = qtitem.LocationCoordSysName;
                                            currentMarker.Properties.LocationType = qtitem.LocationType;
                                            if (update)
                                            {
                                                currentMarker.Properties.TagUpdate = true;
                                                if (currentMarker.Properties.TagType == "Person")
                                                {
                                                    FOTFManager.Instance.BroadcastPersonTagStatus(OutputDataFormat(currentMarker), qtitem.LocationCoordSysId);
                                                }
                                                else if (currentMarker.Properties.TagType.EndsWith("Vehicle"))
                                                {
                                                    FOTFManager.Instance.BroadcastVehicleTagStatus(OutputDataFormat(currentMarker), qtitem.LocationCoordSysId);
                                                }
                                            }
                                            if (remove)
                                            {
                                                FOTFManager.Instance.BroadcastPersonTagRemove(currentMarker.Properties.Id, currentMarker.Properties.FloorId);
                                            }

                                        }
                                        else
                                        {
                                            AddNewMarkerData(qtitem);
                                        }

                                    }
                              
                                }
                                //FOTFManager.Instance.UpdatePersonTagStatus(new object());
                            }
                        }
                        _updateTag = false;
                        watch.Stop();
                        _ = new ErrorLogger().CustomLog(string.Concat("End Execution for all tags in call ", "EndTime: ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "\nElapsed Time:", watch.ElapsedMilliseconds, " ms"),
                            string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "TagProcesslogs"));

                    }
                }
                return Task.FromResult(saveToFile);
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return Task.FromResult(saveToFile);
            }
            finally
            {
                Dispose();
            }

        }

        private object OutputDataFormat(GeoMarker m)
        {
            try
            {
                var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();

                jsonResolver.IgnoreProperty(typeof(Marker), "rFId");
                jsonResolver.IgnoreProperty(typeof(Marker), "color");
                jsonResolver.IgnoreProperty(typeof(Marker), "isWearingTag");
               // jsonResolver.IgnoreProperty(typeof(Marker), "craftName");
                jsonResolver.IgnoreProperty(typeof(Marker), "Tag_TS");
                jsonResolver.IgnoreProperty(typeof(Marker), "Tag_Update");
                jsonResolver.IgnoreProperty(typeof(Marker), "bdate");
                jsonResolver.IgnoreProperty(typeof(Marker), "blunch");
                jsonResolver.IgnoreProperty(typeof(Marker), "edate");
                jsonResolver.IgnoreProperty(typeof(Marker), "elunch");
                jsonResolver.IgnoreProperty(typeof(Marker), "empId");
                jsonResolver.IgnoreProperty(typeof(Marker), "name");
                jsonResolver.IgnoreProperty(typeof(Marker), "reqDate");
                jsonResolver.IgnoreProperty(typeof(Marker), "tourNumber");
                jsonResolver.IgnoreProperty(typeof(Marker), "daysOff");
                jsonResolver.IgnoreProperty(typeof(Marker), "badgeId");
                jsonResolver.IgnoreProperty(typeof(Marker), "isLdcAlert");
                jsonResolver.IgnoreProperty(typeof(Marker), "currentLDCs");
                jsonResolver.IgnoreProperty(typeof(Marker), "Mission");
                jsonResolver.IgnoreProperty(typeof(Marker), "Mission");
                jsonResolver.IgnoreProperty(typeof(Marker), "source");
                jsonResolver.IgnoreProperty(typeof(Marker), "notificationId");
                jsonResolver.IgnoreProperty(typeof(Marker), "routePath");
                jsonResolver.IgnoreProperty(typeof(Marker), "locationMovementStatus");
                jsonResolver.IgnoreProperty(typeof(Marker), "isWearingTag");
                jsonResolver.IgnoreProperty(typeof(Marker), "isLdcAlert");
                jsonResolver.IgnoreProperty(typeof(Marker), "currentLDCs");
                jsonResolver.IgnoreProperty(typeof(Marker), "Mission");
                jsonResolver.IgnoreProperty(typeof(Marker), "Tag_TS");
                jsonResolver.IgnoreProperty(typeof(Marker), "Tag_Update");
                jsonResolver.IgnoreProperty(typeof(Marker), "Raw_Data");
                jsonResolver.IgnoreProperty(typeof(Marker), "Camera_Data");
                jsonResolver.IgnoreProperty(typeof(Marker), "routePath");
                jsonResolver.IgnoreProperty(typeof(Marker), "DarvisAlerts");
                jsonResolver.IgnoreProperty(typeof(Marker), "Vehicle_Status_Data");
                jsonResolver.IgnoreProperty(typeof(Marker), "movementStatus");
                jsonResolver.IgnoreProperty(typeof(Marker), "tacs");
                jsonResolver.IgnoreProperty(typeof(Marker), "sels");
                jsonResolver.IgnoreProperty(typeof(Marker), "base64Image");
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = jsonResolver;
                return JToken.Parse(JsonConvert.SerializeObject(m, Formatting.Indented, serializerSettings));
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }

        private void AddNewMarkerData(Tags qtitem)
        {
            try
            {
                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                {
                    if (cs.Id == qtitem.LocationCoordSysId)
                    {
                        marker = new GeoMarker
                        {
                            Geometry = new MarkerGeometry
                            {
                                Coordinates = qtitem.Location
                            },
                            Properties = new Marker
                            {
                                Id = qtitem.TagId,
                                Name = !string.IsNullOrEmpty(qtitem.TagName) ? qtitem.TagName : "",
                                PositionTS_txt = AppParameters.UnixTimeStampToDateTime(qtitem.LocationTS),
                                PositionTS = qtitem.LocationTS,
                                ServerTS_txt = AppParameters.UnixTimeStampToDateTime(qtitem.ServerTS),
                                ServerTS = qtitem.ServerTS,
                                LastSeenTS_txt = AppParameters.UnixTimeStampToDateTime(qtitem.LastSeenTS),
                                LastSeenTS = qtitem.LastSeenTS,
                                FloorId = qtitem.LocationCoordSysId,
                                TagType = new GetTagType().Get(qtitem.TagName),
                                CraftName = GetCraftName(qtitem.TagName),
                                BadgeId = GetBadgeId(qtitem.TagName),
                                Zones = qtitem.LocationZoneIds,
                                Color = qtitem.Color,
                                posAge = qtitem.ServerTS - qtitem.LastSeenTS,

                            }
                        };
                        if (marker.Properties.TagType == "Person" || marker.Properties.TagType == "Locator")
                        {
                            marker.Properties.CameraData = null;
                            marker.Properties.Vehicle_Status_Data = null;
                            marker.Properties.Missison = null;
                            marker.Properties.RoutePath = null;
                        }
                        if (cs.Locators.TryAdd(qtitem.TagId, marker))
                        {
                            //if (marker.Properties.TagType == "Person")
                            //{
                            //    FOTFManager.Instance.BroadcastPersonTagStatus(marker, qtitem.LocationCoordSysId);
                            //}
                            //else if (marker.Properties.TagType.EndsWith("Vehicle"))
                            //{
                            //   FOTFManager.Instance.BroadcastVehicleTagStatus(marker, qtitem.LocationCoordSysId);
                            //}
                            //update map with new marker.
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        private string GetBadgeId(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    int equalsIndex = name.IndexOf("_", 1);
                    if ((equalsIndex > -1))
                    {
                        string[] namesplit = name.Split('_');
                        if (namesplit.Length > 1)
                        {
                            return namesplit[1];
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }
        private string GetCraftName(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    int equalsIndex = name.IndexOf("_", 1);
                    if ((equalsIndex > -1))
                    {
                        string[] namesplit = name.Split('_');
                        if (namesplit.Length > 1)
                        {
                            return namesplit[0];
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
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
                _data = string.Empty;
                _Message_type = string.Empty;
                _connID = string.Empty;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TagData()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}