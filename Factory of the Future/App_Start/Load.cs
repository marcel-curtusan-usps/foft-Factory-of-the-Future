using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class Load : IDisposable
    {
        private bool disposedValue;
        public string file_content { get; protected set; } = "";
        public JToken tempData = null;
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

        internal void GetConnectionDefaultAsync()
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
                        Task.Run(() => AppParameters.RunningConnection.Add(tempcon[i], false)).ConfigureAwait(true);
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
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