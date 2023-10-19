using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class RFIdData : IDisposable
    {
        //private bool disposedValue;
        public string _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public JToken tempData = null;
        private bool saveToFile;
        private bool disposedValue1;

        internal async Task<bool> LoadAsync(string data, string message_type, string connID)
        {
            saveToFile = false;
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (_data != null)
                {
                    //if (_data.GetType() == typeof(System.String))
                    //{
                    //    tempData = JToken.Parse(_data);
                    //}
                    //else
                    //{
                    //    tempData = _data;
                    //}

                    if (tempData != null && tempData.HasValues)
                    {
                        //AppParameters.RFiD.TryAdd(Guid.NewGuid().ToString(), JsonConvert.SerializeObject(tempData, Formatting.Indented));
                        _ = AppParameters.RFiD.TryAdd(Guid.NewGuid().ToString(), _data);
                        new FileIO().Write(string.Concat(AppParameters.Logdirpath, AppParameters.ConfigurationFloder), "RFiD_Data_" + DateTime.Now.ToString("yyyyMMdd") + ".json", JsonConvert.SerializeObject(AppParameters.NotificationConditionsList.Select(x => x.Value).ToList(), Formatting.Indented));
                    }
                    if (AppParameters.RFiD.Count() > 2000)
                    {
                        foreach (string item in AppParameters.RFiD.Keys)
                        {
                            if (AppParameters.RFiD.TryRemove(item, out string removedItem))
                            {
                                //
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
            if (!disposedValue1)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue1 = true;
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
                tempData = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~RFIdData()
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