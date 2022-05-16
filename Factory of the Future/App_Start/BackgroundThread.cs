using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Factory_of_the_Future
{
    public static class BackgroundThread
    {
        public static void Start()
        {
            _ = ThreadPool.QueueUserWorkItem(_ =>
            {

                while (true)
                {
                    try
                    {
                        if (AppParameters.ActiveServer)
                        {
                            if (AppParameters.DefaulConnectionLoaded)
                            {
                                int tempConn = AppParameters.ConnectionList.Keys.Count;

                                AppParameters.LoadData("Connection.json");

                                if (AppParameters.ConnectionList.Keys.Count > tempConn)
                                {
                                    AppParameters.DefaulConnectionLoaded = false;
                                }
                            }
                            if (AppParameters.ZoneInfo.Keys.Count == 0)
                            {
                               AppParameters.LoadData("Zones.json");
                            }
                            if (AppParameters.IndoorMap.Count == 0)
                            {
                                AppParameters.LoadIndoorapData("ProjectData.json");
                            }
                            if (AppParameters.TagsList.Count == 0)
                            {
                                AppParameters.LoadData("Markers.json");
                            }
                            if (AppParameters.ZoneList.Count == 0)
                            {
                                AppParameters.LoadData("CustomZones.json");
                            }
                        }
                        else
                        {
                            AppParameters.ConnectionList = new ConcurrentDictionary<string, Connection>();
                            AppParameters.ZoneInfo = new ConcurrentDictionary<string, ZoneInfo>();
                            AppParameters.IndoorMap = new ConcurrentDictionary<string, BackgroundImage>();
                            AppParameters.RunningConnection = new ConnectionContainer();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.TraceError("SignalR error thrown in Streaming broadcast: {0}", ex);
                    }
                    Thread.Sleep(5000);
                }
            });
        }
        private static void ConectionSetup()
        {
            try
            {
                //foreach (JObject item in AppParameters.ConnectionList.Values)
                //{
                //    AppParameters.RunningConnection.Add(item);
                //}
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
    }
}