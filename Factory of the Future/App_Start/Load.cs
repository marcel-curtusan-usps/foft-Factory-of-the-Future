using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class Load : IDisposable
    {
        private bool disposedValue;
        public string file_content { get; protected set; } = "";
        public JToken tempData = null;
        //internal async Task GetDoorTripAssociationAsync()
        internal void GetDoorTripAssociationAsync()
        {
            try
            {
                file_content = new FileIO().Read(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "DoorTripAssociation.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                    tempData = JToken.Parse(file_content);

                    if (tempData.HasValues)
                    {
                        List<DoorTrip> tempdata = tempData.ToObject<List<DoorTrip>>();
                        for (int i = 0; i < tempdata.Count; i++)
                        {
                            if (AppParameters.DoorTripAssociation.TryAdd(string.Concat(tempdata[i].Route, tempdata[i].Trip), tempdata[i]))
                            {
                                //
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal async Task GetConnectionDefaultAsync()
        {
            try
            {
                file_content = new FileIO().Read(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                    List<Connection> tempcon = JsonConvert.DeserializeObject<List<Connection>>(file_content);

                    AppParameters.AppSettings.MPE_WATCH_ID = "";

                    for (int i = 0; i < tempcon.Count; i++)
                    {
                       await Task.Run(() => AppParameters.RunningConnection.Add(tempcon[i], false)).ConfigureAwait(true);
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        //internal async Task GetNotificationDefault()
        internal void GetNotificationDefault()
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Notification.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                    List<NotificationConditions> tempnotification = JsonConvert.DeserializeObject<List<NotificationConditions>>(file_content);
                    for (int i = 0; i < tempnotification.Count; i++)
                    {
                        if (!AppParameters.NotificationConditionsList.ContainsKey(tempnotification[i].Id))
                        {
                            _ = AppParameters.NotificationConditionsList.TryAdd(tempnotification[i].Id, tempnotification[i]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        internal void GetConnectionTypes()
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "ConnectionType.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                  AppParameters.connectionTypes = JsonConvert.DeserializeObject(file_content); 
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
        //internal async Task GetRTLSSites()
        internal void GetRTLSSites()
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "RTLS_Site_List.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                    List<MachData> rtls_data = JsonConvert.DeserializeObject<List<MachData>>(file_content);
                    if (rtls_data.Any())
                    {
                        foreach (MachData item in rtls_data)
                        {
                            Uri tempUrl = new Uri(item.LocalLink);
                            item.Port = tempUrl.Port;
                            item.Host = tempUrl.Host;
                            item.URL = item.LocalLink;
                            if (!string.IsNullOrEmpty(item.Host) && !AppParameters.RTLShData.ContainsKey(item.Host))
                            {
                                _ = AppParameters.RTLShData.TryAdd(item.Host, item);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        //internal async Task GetMPEWatchSite()
        internal void GetMPEWatchSite()
        {
            try
            {
                string file_content = new FileIO().Read(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "MPEWatch_Site_List.json");

                if (!string.IsNullOrEmpty(file_content))
                {
                    List<MachData> mpewatch_data = JsonConvert.DeserializeObject<List<MachData>>(file_content);
                    if (mpewatch_data.Any())
                    {
                        foreach (MachData item in mpewatch_data)
                        {
                            Uri tempUrl = new Uri(item.LocalLink);
                            item.Port = tempUrl.Port;
                            item.Host = tempUrl.Host;
                            item.URL = string.Concat(item.LocalLink, "mpemaster.api_page.get_data_by_time?id={0}&data_source={1}&start_time={2}&end_time={3}");
                            if (!string.IsNullOrEmpty(item.Host) && !AppParameters.MPEWatchData.ContainsKey(item.Host))
                            {
                                _ = AppParameters.MPEWatchData.TryAdd(item.Host, item);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }

        internal async Task LoadTempIndoorapData(string fileName)
        {
            string data = string.Empty;
            try
            {
                if (AppParameters.Logdirpath != null)
                {
                    data = new FileIO().Read(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), fileName);

                    if (!string.IsNullOrEmpty(data))
                    {
                       await Task.Run(() => new ProcessRecvdMsg().StartProcess(data, "getProjectInfo", "")).ConfigureAwait(true);
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally
            {
                data = string.Empty;
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
                file_content = string.Empty;
                tempData = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Load()
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