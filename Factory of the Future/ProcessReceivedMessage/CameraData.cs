using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class CameraData : IDisposable
    {
        private bool disposedValue;

        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        private bool saveToFile;
        private List<Cameras> newCameras = new List<Cameras>();
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
                    newCameras = JsonConvert.DeserializeObject<List<Cameras>>(_data);
                    if (newCameras.Any())
                    {
                        foreach (Cameras camera_item in newCameras)
                        {
                            if (AppParameters.CameraInfoList.ContainsKey(camera_item.CameraName) && AppParameters.CameraInfoList.TryGetValue(camera_item.CameraName, out Cameras currentcamera))
                            {
                                foreach (PropertyInfo prop in currentcamera.GetType().GetProperties())
                                {
                                    if (prop.GetValue(camera_item, null).ToString() != prop.GetValue(currentcamera, null).ToString())
                                    {
                                        prop.SetValue(currentcamera, prop.GetValue(camera_item, null));
                                    }
                                }
                            }
                            else
                            {
                                if (!AppParameters.CameraInfoList.TryAdd(camera_item.CameraName, camera_item))
                                {
                                    new ErrorLogger().CustomLog("Unable to Able to add Camera" + camera_item.CameraName, string.Concat((string)AppParameters.AppSettings.Property("APPLICATION_NAME").Value, "_Applogs"));
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
        // ~CameraData()
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