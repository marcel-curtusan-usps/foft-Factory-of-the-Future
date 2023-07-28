using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Factory_of_the_Future
{
    internal class Tag_Types :IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public JToken tempData = null;
        private bool saveToFile;
        internal async Task<bool> LoadAsync(dynamic data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                tempData = JToken.Parse(_data);
                if (tempData != null && tempData.HasValues)
                {
                    foreach (var cs in FOTFManager.Instance.CoordinateSystem.Values)
                    {
                        foreach (JToken item in tempData)
                        {
                            string key = ((JProperty)item).Name;
                            if (!new Regex("^(ts|issues)$", RegexOptions.IgnoreCase).IsMatch(key))
                            {
                                string value = ((JProperty)item).Value.ToString();
                                if (cs.Locators.ContainsKey(key) && cs.Locators.TryGetValue(key, out GeoMarker currentMarker))
                                {
                                    currentMarker.Properties.CraftName = value;
                                    currentMarker.Properties.Emptype = value;
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
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
                tempData = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Tag_Types()
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