using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class SVtripLoadContainer : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public string siteId { get; protected set; } = string.Empty;
        public SVtripLoadContainers Containers { get; protected set; }
        public JToken tempData = null;
        internal async Task LoadAsync(dynamic data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            siteId = (string)AppParameters.AppSettings["FACILITY_NASS_CODE"];

            try
            {
                if (_data != null)
                {
                    tempData = JToken.Parse(_data);
                    if (tempData.HasValues)
                    {
                        Containers = JsonConvert.DeserializeObject<SVtripLoadContainers>(_data);
                        if (Containers != null && Containers.LoadedCtrHuDetails.Any())
                        {
                         await Task.Run(() => { new LoadContainers().LoadAsync(Containers, siteId); }).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
            finally { 
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
                Containers = null;
                tempData = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SVtripLoadContainer()
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