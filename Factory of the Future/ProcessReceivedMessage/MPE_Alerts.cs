using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Factory_of_the_Future.ProcessReceivedMessage
{
    public class MPE_Alerts : IDisposable
    {
        private bool disposedValue;
        public string _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public JToken tempData = null;
        public List<GPIOStatus> newGPIOStatus = null;

        internal Task LoadAsync(string data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            try
            {
                if (!string.IsNullOrEmpty(_data))
                {
                    tempData = JToken.Parse(_data);
                    if (tempData != null && tempData.HasValues)
                    {
                        newGPIOStatus = tempData.ToObject<List<GPIOStatus>>();// JsonConvert.DeserializeObject<Dictionary<string, int>>(_data);
                        if (newGPIOStatus.Any())
                        {
                            foreach (GPIOStatus GPIOitem in newGPIOStatus)
                            {
                                foreach (CoordinateSystem cs in FOTFManager.Instance.CoordinateSystem.Values)
                                {
                                    cs.Zones.Where(f => (f.Value.Properties.ZoneType == "Machine" || f.Value.Properties.ZoneType == "MPEZone") && f.Value.Properties.Name == GPIOitem.MachineId)
                                            .Select(y => y.Value)
                                            .ToList().ForEach(async zone => {
                                                if (zone.Properties.GpioValue != GPIOitem.GpioStatus)
                                                {
                                                    zone.Properties.GpioValue = GPIOitem.GpioStatus;
                                                    await Task.Run(() => FOTFManager.Instance.BroadcastMachineAlertStatus(GPIOitem.GpioStatus, cs.Id, zone.Properties.Id)).ConfigureAwait(false);
                                                }  

                                            });

                                    /*Traverse the list returned from API to find the match of machines in the current map
                                     if found then update the GpioValue in the MPEList object  */
                                    //for (int i = 0; i < mpeList.Count; i++)
                                    //{
                                    //    var parsedMPEName = mpeList[i].Properties.Name;
                                    //    if (newGPIOStatus.TryGetValue(parsedMPEName, out int gpioValue))
                                    //    {
                                    //        if (mpeList[i].Properties.GpioValue != gpioValue)
                                    //        {
                                    //            mpeList[i].Properties.GpioValue = gpioValue;
                                             
                                    //        }
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                }
               // return saveToFile;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
               // return saveToFile;
            }
            finally
            {
                Dispose();
            }

            return Task.CompletedTask;
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