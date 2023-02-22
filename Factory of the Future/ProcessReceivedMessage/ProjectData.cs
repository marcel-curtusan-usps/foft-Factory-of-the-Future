using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        internal async Task<bool> LoadAsync(string data, string message_type, string connID)
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
                    //if (tempData.HasValues && tempData.Type != JTokenType.Array)
                    //{
                    //    //if (((JObject)tempData).ContainsKey("coordinateSystems"))
                    //    //{
                    //    //    //if (FOTFManager.Instance.FirstOrDefault().Key == "temp")
                    //    //    //{
                    //    //    //    AppParameters.CoordinateSystem.TryRemove("temp", out tempCoordinateSystem);
                    //    //    //}
                    //    //    // loop though the Coordinate system
                    //    //    CoordinateSystem = tempData.SelectToken("coordinateSystems");
                    //    //    for (int i = 0; i < CoordinateSystem.Count(); i++)
                    //    //    {
                    //    //        if (AppParameters.CoordinateSystem.ContainsKey(CoordinateSystem[i]["id"].ToString()))
                    //    //        {
                    //    //            if (AppParameters.CoordinateSystem.TryGetValue(CoordinateSystem[i]["id"].ToString(), out CoordinateSystem updateCS))
                    //    //            {
                    //    //                //the background image
                    //    //                LoadBackgroundImage(CoordinateSystem[i].SelectToken("backgroundImages"), updateCS.Id, CoordinateSystem[i]["name"].ToString(), out saveToFile);
                    //    //                //this is for Zones
                    //    //                LoadZones(CoordinateSystem[i].SelectToken("zones"), updateCS.Id, out saveToFile);
                    //    //                //this is for Locators
                    //    //                LoadLocators(CoordinateSystem[i].SelectToken("locators"), updateCS.Id, out saveToFile);

                    //    //            }
                    //    //        }
                    //    //        else
                    //    //        {
                    //    //            CoordinateSystem CSystem = new CoordinateSystem
                    //    //            {
                    //    //                Name = CoordinateSystem[i]["name"].ToString(),
                    //    //                Id = CoordinateSystem[i]["id"].ToString()
                    //    //            };
                    //    //            ///this is used to add new Coordinate System images
                    //    //            if (AppParameters.CoordinateSystem.TryAdd(CSystem.Id, CSystem))
                    //    //            {
                    //    //                //the background image
                    //    //                LoadBackgroundImage(CoordinateSystem[i].SelectToken("backgroundImages"), CSystem.Id, CSystem.Name, out saveToFile);
                    //    //                //this is for Zones
                    //    //                LoadZones(CoordinateSystem[i].SelectToken("zones"), CSystem.Id, out saveToFile);
                    //    //                //this is for Locators
                    //    //                LoadLocators(CoordinateSystem[i].SelectToken("locators"), CSystem.Id, out saveToFile);
                    //    //            }
                    //    //            else
                    //    //            {
                    //    //                new ErrorLogger().CustomLog("Unable to add CoordinateSystem " + CSystem.Id, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
                    //    //            }
                    //    //        }
                    //    //    }
                    //    //}
                    //}
                    // else
                    //{
                    for (int i = 0; i < tempData.Count(); i++)
                    {
                        tempCoordinateSystem = tempData[i].ToObject<CoordinateSystem>();
                        FOTFManager.Instance.AddMap(tempCoordinateSystem.Id, tempCoordinateSystem);
                    }
                    //}

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


        private string GetZoneType(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (Regex.IsMatch(name, (string)AppParameters.AppSettings["AGV_ZONE"], RegexOptions.IgnoreCase))
                    {
                        return "AGVLocation";
                    }
                    else if (Regex.IsMatch(name, (string)AppParameters.AppSettings["DOCKDOOR_ZONE"], RegexOptions.IgnoreCase))
                    {
                        return "DockDoor";
                    }
                    else if (Regex.IsMatch(name, (string)AppParameters.AppSettings["MANUAL_ZONE"], RegexOptions.IgnoreCase))
                    {
                        return "Area";
                    }
                    else if (Regex.IsMatch(name, (string)AppParameters.AppSettings["VIEWPORTS"], RegexOptions.IgnoreCase))
                    {
                        return "ViewPorts";
                    }
                    else
                    {
                        return "Machine";
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

        //private void LoadLocators(JToken locatorlist, string csid, out bool saveToFile)
        //{
        //    saveToFile = false;
        //    try
        //    {
        //        if (locatorlist != null && locatorlist.Count() > 0)
        //        {
        //            foreach (JObject locatorsitem in locatorlist.Children())
        //            {
        //                bool locatorupdate = false;
        //                if (AppParameters.CoordinateSystem[csid].Locators.TryGetValue(locatorsitem["id"].ToString(), out GeoMarker geoLmarker))
        //                {
        //                    // check if position changed

        //                    MarkerGeometry tempGeometry = GetQuuppaTagGeometry(locatorsitem["location"]);
        //                    if (JsonConvert.SerializeObject(geoLmarker.Geometry.Coordinates, Formatting.None) != JsonConvert.SerializeObject(tempGeometry.Coordinates, Formatting.None))
        //                    {
        //                        geoLmarker.Geometry.Coordinates = tempGeometry.Coordinates;
        //                        locatorupdate = true;
        //                    }

        //                    if (geoLmarker.Properties.Name != locatorsitem["name"].ToString())
        //                    {
        //                        geoLmarker.Properties.Name = "Locator";
        //                        string tempTagype = GetTagType(geoLmarker.Properties.Name);
        //                        if (geoLmarker.Properties.TagType != tempTagype)
        //                        {
        //                            geoLmarker.Properties.TagType = tempTagype;
        //                            locatorupdate = true;
        //                            saveToFile = true;
        //                        }
        //                        if (geoLmarker.Properties.TagType == "Person" ||
        //                            geoLmarker.Properties.TagType == "Vehicle")
        //                        {
        //                            geoLmarker.Properties.CraftName = GetCraftName(geoLmarker.Properties.Name);
        //                            geoLmarker.Properties.BadgeId = GetBadgeId(geoLmarker.Properties.Name);
        //                            locatorupdate = true;
        //                            saveToFile = true;
        //                        }

        //                        locatorupdate = true;
        //                    }
        //                    if (locatorupdate)
        //                    {
        //                        geoLmarker.Properties.TagUpdate = true;
        //                        saveToFile = true;
        //                    }

        //                }
        //                else
        //                {
        //                    GeoMarker Lmarker = new GeoMarker
        //                    {
        //                        Type = "Feature"
        //                    };
        //                    Lmarker.Properties.Id = locatorsitem["id"].ToString();
        //                    Lmarker.Properties.Name = locatorsitem.ContainsKey("name") ? locatorsitem["name"].ToString() : "Locator";
        //                    Lmarker.Properties.Color = locatorsitem.ContainsKey("color") ? locatorsitem["color"].ToString() : "";
        //                    Lmarker.Properties.TagType = GetTagType(Lmarker.Properties.Name);
        //                    Lmarker.Geometry = GetQuuppaTagGeometry(locatorsitem["location"]);
        //                    Lmarker.Properties.TagVisible = (bool)locatorsitem["visible"];
        //                    Lmarker.Properties.Source = "other";
        //                    if (AppParameters.CoordinateSystem[csid].Locators.TryAdd(Lmarker.Properties.Id, Lmarker))
        //                    {
        //                        Lmarker.Properties.TagUpdate = true;
        //                        saveToFile = true;
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        new ErrorLogger().ExceptionLog(e);
        //        saveToFile = false;
        //    }
        //}

        private string GetTagType(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (Regex.IsMatch(name, (string)AppParameters.AppSettings["TAG_AGV"], RegexOptions.IgnoreCase))
                    {
                        return "Autonomous Vehicle";
                    }
                    else if (Regex.IsMatch(name, (string)AppParameters.AppSettings["TAG_PIV"], RegexOptions.IgnoreCase))
                    {
                        return "Vehicle";
                    }
                    else if (Regex.IsMatch(name, (string)AppParameters.AppSettings["TAG_PERSON"], RegexOptions.IgnoreCase))
                    {
                        return "Person";
                    }
                    else if (Regex.IsMatch(name, (string)AppParameters.AppSettings["TAG_LOCATOR"], RegexOptions.IgnoreCase))
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