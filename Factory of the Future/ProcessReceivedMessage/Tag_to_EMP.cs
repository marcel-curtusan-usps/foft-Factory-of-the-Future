using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Factory_of_the_Future
{
    internal class Tag_to_EMP : IDisposable
    {
        public Tag_to_EMP()
        {
        }
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        private bool saveToFile;
        private bool disposedValue;
        public JToken tempData = null;
        public JToken tagEmpInfo = null;

        internal async Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                tempData = JToken.Parse(_data);
                tagEmpInfo = tempData[0]["data"];
                if (tagEmpInfo != null && tagEmpInfo.HasValues)
                {
                    foreach (var cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        foreach (JObject item in tagEmpInfo)
                        {
                            if (cs.Locators.ContainsKey(item["tag"].ToString()) && cs.Locators.TryGetValue(item["tag"].ToString(), out GeoMarker currentMarker))
                            {
                                currentMarker.Properties.EmpId = item["emp"].ToString();
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
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
                tempData = null;
                tagEmpInfo = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Tag_to_EMP()
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