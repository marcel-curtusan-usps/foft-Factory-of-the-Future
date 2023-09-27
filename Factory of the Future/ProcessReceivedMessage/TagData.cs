using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger;
using System;
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
                        _ = new ErrorLogger().CustomLog(string.Concat("Start Execution for all tags in call ", "StartTime: ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") ," ms"),
                        string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "TagProcesslogs"));
                        _updateTag = true;
                        saveToFile = true;
                        _data = data;
                        _Message_type = message_type;
                        _connID = connID;
                        if (_data != null)
                        {
                            JToken tempData = JToken.Parse(_data);
                            if (tempData != null && tempData.HasValues)
                            {
                                tagData = tempData.ToObject<QuuppaTag>();

                                DateTime dt = AppParameters.UnixTimeStampToDateTime(tagData.ResponseTS);
                                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                                {
                                    foreach (Tags qtitem in tagData.Tags)
                                    {
                                        if (cs.Id == qtitem.LocationCoordSysId)
                                        {
                                            if (cs.Locators.ContainsKey(qtitem.TagId) && cs.Locators.TryGetValue(qtitem.TagId, out GeoMarker currentMarker))
                                            {
                                                currentMarker.Properties.PositionTS = AppParameters.UnixTimeStampToDateTime(qtitem.LocationTS);
                                                currentMarker.Properties.LastSeenTS = AppParameters.UnixTimeStampToDateTime(qtitem.LastSeenTS);
                                                currentMarker.Properties.TagTS = AppParameters.UnixTimeStampToDateTime(qtitem.LastSeenTS);
                                                currentMarker.Properties.ServerTS = AppParameters.UnixTimeStampToDateTime(tagData.ResponseTS);
                                                currentMarker.Properties.FloorId = qtitem.LocationCoordSysId;
                                                currentMarker.Properties.TagVisibleMils = (int)Math.Ceiling(currentMarker.Properties.ServerTS.Subtract(currentMarker.Properties.PositionTS).TotalMilliseconds);
                                                string newLocation = string.Join(",", qtitem.Location);
                                                string oldLocation = string.Join(",", currentMarker.Geometry.Coordinates);
                                                if (newLocation != oldLocation)
                                                {
                                                    currentMarker.Geometry.Coordinates = qtitem.Location;
                                                    currentMarker.Properties.LocationMovementStatus = "moving";
                                                    update = true;
                                                    //if (new Regex("^(stationary|moving)$", RegexOptions.IgnoreCase).IsMatch(qtitem.LocationMovementStatus))
                                                    //{
                                                    //    currentMarker.Properties.LocationMovementStatus = "moving";
                                                    //    currentMarker.Properties.isPosition = true;
                                                    //    update = true;
                                                    //}
                                                    //else
                                                    //{
                                                    //    currentMarker.Properties.LocationMovementStatus = qtitem.LocationMovementStatus;
                                                    //    currentMarker.Properties.isPosition = false;
                                                    //    update = true;
                                                    //}
                                                }
                                                else
                                                {
                                                    currentMarker.Properties.LocationMovementStatus = "stationary";
                                                    update = true;
                                                }
                                                currentMarker.Properties.Zones = qtitem.LocationZoneIds;
                                                currentMarker.Properties.ZonesNames = qtitem.LocationCoordSysName;
                                                currentMarker.Properties.LocationType = qtitem.LocationType;
                                                if (update)
                                                {
                                                    currentMarker.Properties.TagUpdate = true;
                                                    //if (currentMarker.Properties.TagType == "Person")
                                                    //{
                                                    //    await Task.Run(() => FOTFManager.Instance.BroadcastPersonTagStatus(currentMarker, qtitem.LocationCoordSysId)).ConfigureAwait(false);
                                                    //}
                                                    //else if (currentMarker.Properties.TagType.EndsWith("Vehicle"))
                                                    //{
                                                    //    await Task.Run(() => FOTFManager.Instance.BroadcastVehicleTagStatus(currentMarker, qtitem.LocationCoordSysId)).ConfigureAwait(false);
                                                    //}
                                                }

                                            }
                                            else
                                            {
                                                AddNewMarkerData(qtitem);

                                            }
                                        }
                                    }
                                }
                                FOTFManager.Instance.UpdatePersonTagStatus(new object());
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
                                Id = !string.IsNullOrEmpty(qtitem.TagId) ? qtitem.TagId : "",
                                Name = !string.IsNullOrEmpty(qtitem.TagName) ? qtitem.TagName : "",
                                PositionTS = AppParameters.UnixTimeStampToDateTime(qtitem.LocationTS),
                                TagTS = AppParameters.UnixTimeStampToDateTime(qtitem.LocationTS),
                                FloorId = qtitem.LocationCoordSysId,
                                TagType = new GetTagType().Get(qtitem.TagName),
                                CraftName = GetCraftName(qtitem.TagName),
                                BadgeId = GetBadgeId(qtitem.TagName),
                                Zones = qtitem.LocationZoneIds,
                                Color = qtitem.Color
                            }
                        };
                        if (marker.Properties.TagType == "Person" || marker.Properties.TagType == "Locator")
                        {
                            marker.Properties.CameraData = null;
                            marker.Properties.Vehicle_Status_Data = null;
                            marker.Properties.Missison = null;
                            marker.Properties.RoutePath = null;
                        }
                            marker.Properties.TagVisibleMils = (int)Math.Ceiling(marker.Properties.TagTS.Subtract(marker.Properties.PositionTS).TotalMilliseconds);
                        if (cs.Locators.TryAdd(qtitem.TagId, marker))
                        {
                            if (marker.Properties.TagType == "Person")
                            {
                                FOTFManager.Instance.BroadcastPersonTagStatus(marker, qtitem.LocationCoordSysId);
                            }
                            else if (marker.Properties.TagType.EndsWith("Vehicle"))
                            {
                               FOTFManager.Instance.BroadcastVehicleTagStatus(marker, qtitem.LocationCoordSysId);
                            }
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