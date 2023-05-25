using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
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
        internal async Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (_data != null)
                {
                    JToken tempData = JToken.Parse(_data);
                    if (tempData != null && tempData.HasValues)
                    {
                        tagData = tempData.ToObject<QuuppaTag>();

                        foreach (Tag qtitem in tagData.Tags)
                        {
                            if (!string.IsNullOrEmpty(qtitem.LocationCoordSysId) && qtitem.Location != null)
                            {
                                update = false;
                                Coordinatesupdate = false;
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
                                        TagTS = AppParameters.UnixTimeStampToDateTime(tagData.ResponseTS),
                                        FloorId = qtitem.LocationCoordSysId,
                                        TagType = new GetTagType().Get(qtitem.TagName),
                                        CraftName = GetCraftName(qtitem.TagName),
                                        BadgeId = GetBadgeId(qtitem.TagName),
                                        Color = !string.IsNullOrEmpty(qtitem.Color) ? qtitem.Color : "",
                                        Zones = qtitem.LocationZoneIds,
                                        TagVisible = !(qtitem.LocationMovementStatus == "noData")
                                    }
                                };
                                marker.Properties.TagVisibleMils = (int)Math.Ceiling(marker.Properties.TagTS.Subtract(marker.Properties.PositionTS).TotalMilliseconds);
                                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                                {
                                    if (cs.Id == qtitem.LocationCoordSysId && cs.Locators.ContainsKey(qtitem.TagId) && cs.Locators.TryGetValue(qtitem.TagId, out GeoMarker currentMarker))
                                    {
                                        currentMarker.Properties.Zones = marker.Properties.Zones;
                                        posiblenewtag = false;
                                        //geomatry update
                                        if (JsonConvert.SerializeObject(currentMarker.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(marker.Geometry.Coordinates, Formatting.None))
                                        {
                                            currentMarker.Geometry.Coordinates = marker.Geometry.Coordinates;
                                            update = true;
                                        }

                                        //properties update
                                        foreach (PropertyInfo prop in currentMarker.Properties.GetType().GetProperties())
                                        {
                                            if (!new Regex("^(Id|RFid|IsWearingTag|Zones|CraftName|TagUpdate|EmpId|Emptype|EmpName|IsLdcAlert|CurrentLDCs|Tacs|Sels|RawData|CameraData|Camera_Data|Vehicle_Status_Data|Missison|Source|NotificationId|RoutePath)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                            {
                                                if (prop.GetValue(marker.Properties, null).ToString() != prop.GetValue(currentMarker.Properties, null).ToString())
                                                {
                                                    prop.SetValue(currentMarker.Properties, prop.GetValue(marker.Properties, null));
                                                    update = true;
                                                }
                                            }
                                        }
                                        if (update)
                                        {
                                            if (currentMarker.Properties.TagType == "Person")
                                            {
                                                await Task.Run(() => FOTFManager.Instance.BroadcastPersonTagStatus(currentMarker, qtitem.LocationCoordSysId)).ConfigureAwait(false);
                                            }
                                            else if (currentMarker.Properties.TagType.EndsWith("Vehicle"))
                                            {
                                                await Task.Run(() => FOTFManager.Instance.BroadcastVehicleTagStatus(currentMarker, qtitem.LocationCoordSysId)).ConfigureAwait(false);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        posiblenewtag = true;

                                    }
                                }
                                if (posiblenewtag)
                                {
                                    await Task.Run(action: () => AddNewMarkerData(qtitem)).ConfigureAwait(false);
                                }
                            }
                        }
                    }
                }
                return saveToFile;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return saveToFile;
            }
            finally
            {
                Dispose();
            }
        }

        private async Task<bool> AddNewMarkerData(Tag qtitem)
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
                        if (cs.Locators.TryAdd(qtitem.TagId, marker))
                        {
                            if (marker.Properties.TagType == "Person")
                            {
                                await Task.Run(() => FOTFManager.Instance.BroadcastPersonTagStatus(marker, qtitem.LocationCoordSysId)).ConfigureAwait(false);
                            }
                            else if (marker.Properties.TagType.EndsWith("Vehicle"))
                            {
                                await Task.Run(() => FOTFManager.Instance.BroadcastVehicleTagStatus(marker, qtitem.LocationCoordSysId)).ConfigureAwait(false);
                            }
                            //update map with new marker.
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return false;
            }
            finally
            {
                Dispose();
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