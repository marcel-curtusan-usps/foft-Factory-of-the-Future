using Factory_of_the_Future.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Factory_of_the_Future
{
    internal class SVcontainerTypeCode : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public JToken tempData = null;
        public List<SVCodeTypes> sVcontainerTypeCodes = new List<SVCodeTypes>();

        internal void LoadAsync(dynamic data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (_data != null)
                {
                    tempData = JToken.Parse(_data);
                    if (tempData.HasValues)
                    {
                        sVcontainerTypeCodes = tempData.ToObject<List<SVCodeTypes>>();
                        foreach (SVCodeTypes newCodeType in sVcontainerTypeCodes)
                        {
                            if (AppParameters.SVcontainerTypeCode.ContainsKey(newCodeType.Code) && AppParameters.SVcontainerTypeCode.TryGetValue(newCodeType.Code, out SVCodeTypes currentCodeType))
                            {
                                if (AppParameters.SVcontainerTypeCode.TryUpdate(newCodeType.Code, newCodeType, currentCodeType))
                                {
                                    //
                                }
                            }
                            else
                            {
                                if (AppParameters.SVcontainerTypeCode.TryAdd(newCodeType.Code, newCodeType))
                                {
                                    //
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);

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
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SVcontainerTypeCode()
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