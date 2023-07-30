using Factory_of_the_Future.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Factory_of_the_Future
{
    internal class Door_Associated_Trips : IDisposable
    {
        private bool disposedValue;
        public dynamic _data { get; protected set; }
        public string _Message_type { get; protected set; }
        public string _connID { get; protected set; }
        public bool update { get; protected set; }
        public JToken tempData = null;
        public List<Door_Association_Trips> door_association_temp_Data;
        public Door_Associated_Trips()
        {
        }

        internal async Task LoadAsync(dynamic data, string message_type, string connID)
        {
            _data = data;
            _Message_type = message_type;
            _connID = connID;
            update = false;
            try
            {
                if (_data != null)
                {

                    tempData = JToken.Parse(_data);
                    if (tempData != null && tempData.HasValues)
                    {
                        char[] firstdelimiterChars = {','};
                        char[] seconddelimiterChars = { '-' };
                        door_association_temp_Data = tempData.ToObject<List<Door_Association_Trips>>().OrderBy(o => o.DoorBarcode).ToList();
                        foreach (Door_Association_Trips door in door_association_temp_Data)
                        {
                            if (!string.IsNullOrEmpty(door.AssociatedTrips))
                            {
                                string[] AssociatedTrips = door.AssociatedTrips.Split(firstdelimiterChars);
                                for (int i = 0; i < AssociatedTrips.Length; i++)
                                {
                                    string[] routeTrip = AssociatedTrips[i].Split(seconddelimiterChars);
                                    door.Route = routeTrip[0];
                                    door.Trip = routeTrip[1];

                                    await Task.Run(() => FOTFManager.Instance.saveDoorTripAssociation(door.DoorNum, door.Route, door.Trip)).ConfigureAwait(false);
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
                _data = null;
                _Message_type = string.Empty;
                _connID = string.Empty;
                tempData = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Door_Associated_Trips()
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