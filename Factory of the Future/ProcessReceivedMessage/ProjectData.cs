using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Factory_of_the_Future
{
    internal class ProjectData : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public JToken tempData = null;
        public JToken CoordinateSystem = null;
        public CoordinateSystem tempCoordinateSystem = null;
        public QuuppaCoordinateSystem qc = null;
        public ConcurrentDictionary<string, GeoMarker> templocators = null;
        public ConcurrentDictionary<string, GeoZone> tempZones = null;
        public bool update = false;
        internal Task<bool> LoadAsync(string data, string message_type, string connID)
        {
            bool saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (!string.IsNullOrEmpty(_data))
                {
                    tempData = JToken.Parse(_data);
                    if (tempData.HasValues && tempData.Type != JTokenType.Array)
                    {
                        if (((JObject)tempData).ContainsKey("coordinateSystems"))
                        {
                            qc = tempData.ToObject<QuuppaCoordinateSystem>();
                            foreach (Quuppa_CoordinateSystem qcitem in qc.CoordinateSystems)
                            {
                                if (qcitem.BackgroundImages.Any())
                                {
                                    qcitem.BackgroundImages[0].Name = qcitem.Name;
                                    qcitem.BackgroundImages[0].CoordinateSystemId = qcitem.Id;
                                }
                              
                                if (FOTFManager.Instance.CoordinateSystem.ContainsKey(qcitem.Id))
                                {
                                    foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                                    {
                                        if (cs.Id == qcitem.Id)
                                        {
                                            //background image proccess 
                                            foreach (var qcbkgitem in qcitem.BackgroundImages)
                                            {

                                                if (cs.BackgroundImage != null && cs.BackgroundImage.Id == qcbkgitem.Id)
                                                {
                                                    update = false;

                                                    foreach (PropertyInfo prop in qcbkgitem.GetType().GetProperties())
                                                    {   
                                                        
                                                     
                                                        // Check if qcbkgitem and cs.BackgroundImage objects are null
                                                        if (qcbkgitem == null || cs.BackgroundImage == null)
                                                        {
                                                            // Handle null object error
                                                            Console.WriteLine("Error: qcbkgitem or cs.BackgroundImage is null");
                                                            continue;
                                                        }

                                                        // Get the property value from the qcbkgitem object
                                                        object qcbkgitemValue = prop.GetValue(qcbkgitem, null);

                                                        // Get the property value from the cs.BackgroundImage object
                                                        object backgroundImageValue = prop.GetValue(cs.BackgroundImage, null);

                                                        // Check if the property value types are the same
                                                        if (qcbkgitemValue == null || backgroundImageValue == null || qcbkgitemValue.GetType() != backgroundImageValue.GetType())
                                                        {
                                                            // Debug the type mismatch error
                                                            Console.WriteLine($"Error: Property type mismatch for {prop.Name} ({prop.PropertyType})");
                                                            continue;
                                                        }

                                                        // Compare the property values and update if necessary
                                                        if (!new Regex("^(coordinateSystemId)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                                        {
                                                            if (!qcbkgitemValue.Equals(backgroundImageValue))
                                                            {
                                                                prop.SetValue(cs.BackgroundImage, qcbkgitemValue);
                                                                update = true;
                                                            }
                                                        }
                                                        //if (prop.GetValue(qcbkgitem, null).ToString() != prop.GetValue(cs.BackgroundImage, null).ToString())
                                                        //{
                                                        //    prop.SetValue(cs.BackgroundImage, prop.GetValue(qcbkgitem, null));
                                                        //    update = true;
                                                        //}
                                                    }


                                                    if (update)
                                                    {
                                                        _ = Task.Run(() => new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(FOTFManager.Instance.CoordinateSystem.Select(x => x.Value).ToList()))).ConfigureAwait(false);
                                                    }
                                                }
                                            }

                                            //zones 
                                            tempZones = getZoness(qcitem.qcZones, qcitem.Id);
                                            foreach (var newzone in tempZones)
                                            {
                                                if (cs.Zones.ContainsKey(newzone.Key) && cs.Zones.TryGetValue(newzone.Key, out GeoZone currentZone))
                                                {

                                                    //geomatry update
                                                    update = false;
                                                    if (JsonConvert.SerializeObject(currentZone.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(newzone.Value.Geometry.Coordinates, Formatting.None))
                                                    {
                                                        currentZone.Geometry.Coordinates = newzone.Value.Geometry.Coordinates;
                                                        update = true;
                                                    }

                                                    //properties update
                                                    foreach (PropertyInfo prop in currentZone.Properties.GetType().GetProperties())
                                                    {
                                                        if (!new Regex("^(Zone_Update|Quuppa_Override|MPEWatchData|bins|MPEBins|DPSData|StaffingData|GpioValue|MissionList|Source|MPEGroup)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                                        {
                                                            if (prop.GetValue(newzone.Value.Properties, null).ToString() != prop.GetValue(currentZone.Properties, null).ToString())
                                                            {
                                                                prop.SetValue(currentZone.Properties, prop.GetValue(newzone.Value.Properties, null));
                                                                update = true;
                                                            }
                                                        }
                                                    }
                                                    if (update)
                                                    {
                                                        _ = Task.Run(() => new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(FOTFManager.Instance.CoordinateSystem.Select(x => x.Value).ToList())));
                                                    }

                                                }
                                                else
                                                {
                                                    if (cs.Zones.TryAdd(newzone.Key, newzone.Value))
                                                    {
                                                        //add new zone
                                                        _ = Task.Run(() => new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(FOTFManager.Instance.CoordinateSystem.Select(x => x.Value).ToList())));
                                                    }
                                                }
                                            }

                                            //locators
                                            templocators = getLocators(qcitem.Locators, qcitem.Id); getLocators(qcitem.Locators, qcitem.Id);
                                            foreach (var newlocator in templocators)
                                            {
                                                if (cs.Locators.ContainsKey(newlocator.Key) && cs.Locators.TryGetValue(newlocator.Key, out GeoMarker currentMarker))
                                                {
                                                    update = false;
                                                    //geomatry update
                                                    if (JsonConvert.SerializeObject(currentMarker.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(newlocator.Value.Geometry.Coordinates, Formatting.None))
                                                    {
                                                        currentMarker.Geometry.Coordinates = newlocator.Value.Geometry.Coordinates;
                                                        update = true;
                                                    }
                                                    //properties update
                                                    foreach (PropertyInfo prop in currentMarker.Properties.GetType().GetProperties())
                                                    {
                                                        if (!new Regex("^(Id|RFid|Zones|TagVisible|TagVisibleMils|IsWearingTag|CraftName|PositionTS|TagTS|TagUpdate|EmpId|Emptype|EmpName|IsLdcAlert|CurrentLDCs|Tacs|Sels|RawData|CameraData|Vehicle_Status_Data|Missison|Source|NotificationId|RoutePath)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                                                        {
                                                            if (prop.GetValue(newlocator.Value.Properties, null).ToString() != prop.GetValue(currentMarker.Properties, null).ToString())
                                                            {
                                                                prop.SetValue(currentMarker.Properties, prop.GetValue(newlocator.Value.Properties, null));
                                                            }
                                                        }
                                                    }
                                                    if (update)
                                                    {
                                                        _ = Task.Run(() => new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(FOTFManager.Instance.CoordinateSystem.Select(x => x.Value).ToList())));
                                                    }
                                                }
                                                else
                                                {
                                                    if (cs.Locators.TryAdd(newlocator.Key, newlocator.Value))
                                                    {
                                                        //add new zone
                                                        _ = Task.Run(() => new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(FOTFManager.Instance.CoordinateSystem.Select(x => x.Value).ToList())));
                                                    }
                                                }
                                            }


                                        }
                                    }
                                }
                                else
                                {
                                    // this will add new floors
                                    if (FOTFManager.Instance.CoordinateSystem.TryAdd(qcitem.Id, new CoordinateSystem
                                    {
                                        Name = qcitem.Name,
                                        Id = qcitem.Id,
                                        BackgroundImage = JToken.Parse(JsonConvert.SerializeObject(qcitem.BackgroundImages.FirstOrDefault(), Formatting.Indented)).ToObject<BackgroundImage>(),
                                        Locators = getLocators(qcitem.Locators, qcitem.Id),
                                        Zones = getZoness(qcitem.qcZones, qcitem.Id)
                                    }))
                                    {
                                        //update file with map data.
                                        _ = Task.Run(() => new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "Project_Data.json", AppParameters.ZoneOutPutdata(FOTFManager.Instance.CoordinateSystem.Select(x => x.Value).ToList())));

                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        for (int i = 0; i < tempData.Count(); i++)
                        {
                            tempCoordinateSystem = tempData[i].ToObject<CoordinateSystem>();
                            if (tempCoordinateSystem.BackgroundImage != null)
                            {
                                tempCoordinateSystem.BackgroundImage.CoordinateSystemId = tempCoordinateSystem.Id;
                            }
                          
                            FOTFManager.Instance.AddMap(tempCoordinateSystem.Id, tempCoordinateSystem);
                        }
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

        private ConcurrentDictionary<string, GeoMarker> getLocators(List<Locator> locators, string id)
        {
            ConcurrentDictionary<string, GeoMarker> temp = new ConcurrentDictionary<string, GeoMarker>();
            try
            {
                foreach (Locator item in locators)
                {
                    temp.TryAdd(item.Id, new GeoMarker
                    {
                        Geometry = new MarkerGeometry
                        {
                            Coordinates = item.Location
                        },
                        Properties = new Marker
                        {
                            Id = item.Id,
                            Name = item.Name,
                            FloorId = id,
                            TagType = new GetTagType().Get(item.Name),
                            CraftName = GetCraftName(item.Name),
                            BadgeId = GetBadgeId(item.Name),
                            Color = item.Color,
                            TagVisible = item.Visible
                        }
                    });
                }
                return temp;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return temp;
            }
        }

        private ConcurrentDictionary<string, GeoZone> getZoness(List<qcZone> zones, string id)
        {
            ConcurrentDictionary<string, GeoZone> temp = new ConcurrentDictionary<string, GeoZone>();
            try
            {
                foreach (qcZone item in zones)
                {
                    temp.TryAdd(item.Id, new GeoZone
                    {
                        Geometry = QuuppaZoneGeometry(item.PolygonData),
                        Properties = new Properties
                        {
                            Id = item.Id,
                            FloorId = id,
                            Name = item.Name,
                            ZoneType = GetZoneType(item.Name),
                            Color = item.Color,
                            Visible = item.Visible,
                            DoorNumber = GetNumber(item.Name).ToString(),
                            MPENumber = GetNumber(item.Name),
                            MPEType = GetMpeType(item.Name)//string.Join(string.Empty, Regex.Matches(item.Name, @"\p{L}+").OfType<Match>().Select(m => m.Value)),
                        }
                    });
                }
                return temp;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return temp;
            }
        }

        private string GetMpeType(string name)
        {
            try
            {

                return string.Join(string.Empty, Regex.Matches(name, @"\p{L}+").OfType<Match>().Select(m => m.Value));
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "";
            }
        }

        private int GetNumber(string name)
        {
            try
            {
                int.TryParse(string.Join(string.Empty, Regex.Matches(name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n);
                return n;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return 0;
            }
        }

        private string GetZoneType(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (Regex.IsMatch(name, AppParameters.AppSettings.AGV_ZONE, RegexOptions.IgnoreCase))
                    {
                        return "AGVLocationZone";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.DOCKDOOR_ZONE, RegexOptions.IgnoreCase))
                    {
                        return "DockDoorZone";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.MANUAL_ZONE, RegexOptions.IgnoreCase))
                    {
                        return "Area";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.VIEWPORTS, RegexOptions.IgnoreCase))
                    {
                        return "ViewPortsZone";
                    }
                    else
                    {
                        return "MPEZone";
                    }
                }
                else
                {
                    return "None";
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "None";
            }
        }
        private ZoneGeometry GetQuuppaZoneGeometry(JToken zoneitem)
        {
            try
            {
                JObject geometry = new JObject();
                JArray temp = new JArray();

                string[] polygonDatasplit = zoneitem.ToString().Split('|');
                if (polygonDatasplit.Length > 0)
                {
                    JArray xyar = new JArray();
                    foreach (var polygonitem in polygonDatasplit)
                    {
                        string[] polygonitemsplit = polygonitem.Split(',');
                        xyar.Add(new JArray(Convert.ToDouble(polygonitemsplit[0]), Convert.ToDouble(polygonitemsplit[1])));
                    }
                    temp.Add(xyar);
                }

                geometry["coordinates"] = temp;

                ZoneGeometry result = geometry.ToObject<ZoneGeometry>();

                return result;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        private ZoneGeometry QuuppaZoneGeometry(string zoneitem)
        {
            try
            {
                JObject geometry = new JObject();
                JArray temp = new JArray();

                string[] polygonDatasplit = zoneitem.Split('|');
                if (polygonDatasplit.Length > 0)
                {
                    JArray xyar = new JArray();
                    foreach (var polygonitem in polygonDatasplit)
                    {
                        string[] polygonitemsplit = polygonitem.Split(',');
                        xyar.Add(new JArray(Convert.ToDouble(polygonitemsplit[0]), Convert.ToDouble(polygonitemsplit[1])));
                    }
                    temp.Add(xyar);
                }

                geometry["coordinates"] = temp;

                ZoneGeometry result = geometry.ToObject<ZoneGeometry>();

                return result;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
            }
        }
        private void LoadZones(JToken zoneslist, string csid, out bool saveToFile)
        {
            saveToFile = false;
            try
            {
                if (zoneslist != null && zoneslist.Any())
                {
                    foreach (JObject zoneitem in zoneslist.Children().Cast<JObject>())
                    {
                        bool zoneUpdate = false;

                        if (FOTFManager.Instance.CoordinateSystem[csid].Zones.TryGetValue(zoneitem["id"].ToString(), out GeoZone gZone))
                        {
                            ZoneGeometry tempGeometry = GetQuuppaZoneGeometry(zoneitem["polygonData"]);
                            if (JsonConvert.SerializeObject(gZone.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(tempGeometry.Coordinates, Formatting.None))
                            {
                                gZone.Geometry.Coordinates = tempGeometry.Coordinates;
                                zoneUpdate = true;
                                saveToFile = true;
                            }
                            if (!gZone.Properties.QuuppaOverride)
                            {
                                if (gZone.Properties.Name != zoneitem["name"].ToString())
                                {
                                    gZone.Properties.Name = zoneitem["name"].ToString();
                                    zoneUpdate = true;
                                    saveToFile = true;
                                }
                            }
                            string temptype = GetZoneType(gZone.Properties.Name);
                            if (temptype != gZone.Properties.ZoneType)
                            {
                                gZone.Properties.Name = temptype;
                                zoneUpdate = true;
                                saveToFile = true;
                            }
                            if (zoneUpdate)
                            {
                                gZone.Properties.ZoneUpdate = true;
                                saveToFile = true;
                                saveToFile = true;
                            }
                        }
                        else
                        {
                            GeoZone newGZone = new GeoZone();
                            newGZone.Geometry = GetQuuppaZoneGeometry(zoneitem["polygonData"]);
                            newGZone.Properties.FloorId = csid;
                            newGZone.Properties.Id = zoneitem["id"].ToString();
                            newGZone.Properties.Name = zoneitem["name"].ToString();
                            newGZone.Properties.Color = zoneitem["color"].ToString();
                            newGZone.Properties.Visible = (bool)zoneitem["visible"];
                            newGZone.Properties.ZoneType = GetZoneType(newGZone.Properties.Name);
                            newGZone.Properties.Source = "other";

                            if (newGZone.Properties.ZoneType.StartsWith("DockDoor"))
                            {
                                //get the DockDoor Number
                                if (int.TryParse(string.Join(string.Empty, Regex.Matches(newGZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                                {
                                    newGZone.Properties.DoorNumber = n.ToString();
                                }
                            }

                            if (newGZone.Properties.ZoneType == "Machine")
                            {

                                //get the MPE Number
                                if (int.TryParse(string.Join(string.Empty, Regex.Matches(newGZone.Properties.Name, @"\d+").OfType<Match>().Select(m => m.Value)).ToString(), out int n))
                                {
                                    newGZone.Properties.MPENumber = n;
                                }
                                //get the MPE Name
                                newGZone.Properties.MPEType = string.Join(string.Empty, Regex.Matches(newGZone.Properties.Name, @"\p{L}+").OfType<Match>().Select(m => m.Value));
                            }

                            if (FOTFManager.Instance.CoordinateSystem[csid].Zones.TryAdd(newGZone.Properties.Id, newGZone))
                            {
                                newGZone.Properties.ZoneUpdate = true;
                                saveToFile = true;
                                //new ErrorLogger().CustomLog("Unable to Add Zone" + newGZone.Properties.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                            }
                        }

                    }
                }

            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                saveToFile = false;
            }
        }
        private void LoadLocators(JToken locatorlist, string csid, out bool saveToFile)
        {
            saveToFile = false;
            try
            {
                if (locatorlist != null && locatorlist.Any())
                {
                    foreach (JObject locatorsitem in locatorlist.Children().Cast<JObject>())
                    {
                        bool locatorupdate = false;
                        if (FOTFManager.Instance.CoordinateSystem[csid].Locators.TryGetValue(locatorsitem["id"].ToString(), out GeoMarker geoLmarker))
                        {
                            // check if position changed

                            MarkerGeometry tempGeometry = GetQuuppaTagGeometry(locatorsitem["location"]);
                            if (JsonConvert.SerializeObject(geoLmarker.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(tempGeometry.Coordinates, Formatting.None))
                            {
                                geoLmarker.Geometry.Coordinates = tempGeometry.Coordinates;
                                locatorupdate = true;
                            }

                            if (geoLmarker.Properties.Name != locatorsitem["name"].ToString())
                            {
                                geoLmarker.Properties.Name = "Locator";
                                string tempTagype = GetTagType(geoLmarker.Properties.Name);
                                if (geoLmarker.Properties.TagType != tempTagype)
                                {
                                    geoLmarker.Properties.TagType = tempTagype;
                                    locatorupdate = true;
                                    saveToFile = true;
                                }
                                if (geoLmarker.Properties.TagType == "Person" ||
                                    geoLmarker.Properties.TagType == "Vehicle")
                                {
                                    geoLmarker.Properties.CraftName = GetCraftName(geoLmarker.Properties.Name);
                                    geoLmarker.Properties.BadgeId = GetBadgeId(geoLmarker.Properties.Name);
                                    locatorupdate = true;
                                    saveToFile = true;
                                }

                                locatorupdate = true;
                            }
                            if (locatorupdate)
                            {
                                geoLmarker.Properties.TagUpdate = true;
                                saveToFile = true;
                            }

                        }
                        else
                        {
                            GeoMarker Lmarker = new GeoMarker
                            {
                                Type = "Feature"
                            };
                            Lmarker.Properties.Id = locatorsitem["id"].ToString();
                            Lmarker.Properties.Name = locatorsitem.ContainsKey("name") ? locatorsitem["name"].ToString() : "Locator";
                            Lmarker.Properties.Color = locatorsitem.ContainsKey("color") ? locatorsitem["color"].ToString() : "";
                            Lmarker.Properties.TagType = GetTagType(Lmarker.Properties.Name);
                            Lmarker.Geometry = GetQuuppaTagGeometry(locatorsitem["location"]);
                            Lmarker.Properties.TagVisible = (bool)locatorsitem["visible"];
                            Lmarker.Properties.Source = "other";
                            if (FOTFManager.Instance.CoordinateSystem[csid].Locators.TryAdd(Lmarker.Properties.Id, Lmarker))
                            {
                                Lmarker.Properties.TagUpdate = true;
                                saveToFile = true;
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                saveToFile = false;
            }
        }

        private string GetTagType(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_AGV, RegexOptions.IgnoreCase))
                    {
                        return "Autonomous Vehicle";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_PIV, RegexOptions.IgnoreCase))
                    {
                        return "Vehicle";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_PERSON, RegexOptions.IgnoreCase))
                    {
                        return "Person";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_HVI, RegexOptions.IgnoreCase))
                    {
                        return "HVI";
                    }
                    else if (Regex.IsMatch(name, AppParameters.AppSettings.TAG_LOCATOR, RegexOptions.IgnoreCase))
                    {
                        return "Locator";
                    }
                    else
                    {
                        return name;
                    }
                }
                else
                {
                    return "Unknown_Tag";
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return "Error_Unknown_Tag";
            }
        }

        private string GetBadgeId(string name)
        {
            try
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
                int equalsIndex = name.IndexOf("_", 1);
                if ((equalsIndex > -1))
                {
                    string[] namesplit = name.Split('_');
                    if (namesplit.Length > 1)
                    {
                        return namesplit[0];
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

        private MarkerGeometry GetQuuppaTagGeometry(JToken tagitemsplit)
        {

            try
            {
                JObject geometry = new JObject();
                JArray temp = new JArray();
                if (tagitemsplit.HasValues)
                {
                    temp = new JArray(tagitemsplit[0], tagitemsplit[1]);
                }
                else
                {
                    temp = new JArray(0.0, 0.0);
                }
                geometry["coordinates"] = temp;
                MarkerGeometry result = geometry.ToObject<MarkerGeometry>();
                return result;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return null;
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
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
                tempData = null;
                CoordinateSystem = null;
                tempCoordinateSystem = null;
                templocators = null;
                tempZones = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ProjectData()
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