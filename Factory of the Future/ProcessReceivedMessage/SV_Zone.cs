using Factory_of_the_Future.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class SV_Zone : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public JToken tempData = null;
        public List<SVBullpen> SV_Bullpen;
        private bool saveToFile;

        internal async Task<bool> LoadAsync(string data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {

                if (!string.IsNullOrEmpty(_data))
                {
                    tempData = JToken.Parse(_data);
                    SV_Bullpen = tempData.ToObject<List<SVBullpen>>();
                    if (SV_Bullpen.Any())
                    {
                        foreach (SVBullpen Bullpen_item in SV_Bullpen)
                        {
                            _ = AppParameters.SVZoneNameList.AddOrUpdate(Bullpen_item.LocationId, Bullpen_item,
                               (key, oldValue) =>
                               {
                                   return Bullpen_item;
                               });
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
                tempData = null;
                SV_Bullpen = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SV_Zone()
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