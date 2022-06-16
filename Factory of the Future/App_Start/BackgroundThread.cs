using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

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

                            //if (AppParameters.ZoneInfo.Keys.Count == 0)
                            //{
                            //   AppParameters.LoadData("Zones.json");
                            //}
                            //if (AppParameters.ZoneList.Count == 0)
                            //{
                            //    AppParameters.LoadData("CustomZones.json");
                            //}
                            //if (AppParameters.CoordinateSystem.Count == 0)
                            //{
                            //    AppParameters.LoadIndoorapData("Project_Data.json");
                            //}
                            //if (AppParameters.TagsList.Count == 0)
                            //{
                            //    AppParameters.LoadData("Markers.json");
                            //}

                            //data Retention
                            Task.Run(() => DataRetentionProcess.Start());

                        }
                        else
                        {
                            Task.Run(() => AppParameters.ResetParameters());
                            AppParameters.Users = new ConcurrentDictionary<string, ADUser>();

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