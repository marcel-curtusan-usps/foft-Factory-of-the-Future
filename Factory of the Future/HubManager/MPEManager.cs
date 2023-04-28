using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Factory_of_the_Future.Models;
using Factory_of_the_Future.MPEKanban;

namespace Factory_of_the_Future
{

    public class MPEManager : IDisposable
    {
        private readonly static Lazy<MPEManager> _instance = new Lazy<MPEManager>(() => new MPEManager(GlobalHost.ConnectionManager.GetHubContext<HubManager>().Clients));
        public readonly ConcurrentDictionary<string, CoordinateSystem> CoordinateSystem = new ConcurrentDictionary<string, CoordinateSystem>();
        public static MPEManager Instance { get { return _instance.Value; } }
        private IHubConnectionContext<dynamic> Clients { get; set; }
        public MPEManager(IHubConnectionContext<dynamic> clients) { Clients = clients; }
        //blocks
        private readonly object updateZoneStatuslock = new object();
        //timers
        private readonly Timer VehicleTag_timer;
        // private readonly Timer Camera_timer;
        //status
        private volatile bool _updatePersonTagStatus = false;
        //private volatile bool _updateCameraStatus = false;
        private bool disposedValue;
        private readonly HttpClient httpClient = new HttpClient();
        //250 Milliseconds
        private readonly TimeSpan _250updateInterval = TimeSpan.FromMilliseconds(250);

        internal IEnumerable<JToken> GetMPETestData(bool isDispatch)
        {
            try
            {
                string jsonFileName = isDispatch ? "KanbanDispatch.json" : "Kanban.json";
                var currentdir = System.AppDomain.CurrentDomain.BaseDirectory;
                string kanbanlist_string = new FileIO().Read(string.Format("{0}/MPEKanban/Contents/", currentdir), jsonFileName);
                JObject returnVal = new JObject();

                if (!string.IsNullOrEmpty(kanbanlist_string))
                {

                    JArray temp = JArray.Parse(kanbanlist_string);
                    if (temp.HasValues)
                    {
                        JProperty addthis = new JProperty("0", temp);
                        returnVal.Add(addthis);
                    }
                    else
                    {
                        returnVal = new JObject();
                    }

                }

                return returnVal;
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
                return new JObject();
            }
        }

        private readonly string[] _propertiesToLoad = new string[]
            {
                                    ADProperties.ContainerName,
                                    ADProperties.LoginName,
                                    ADProperties.MemberOf,
                                    ADProperties.FirstName,
                                    ADProperties.MiddleName,
                                    ADProperties.SurName,
                                    ADProperties.PostalCode
            };

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
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~FOTFManager()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}