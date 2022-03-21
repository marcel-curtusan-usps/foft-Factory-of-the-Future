﻿using Newtonsoft.Json;
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
                            if (AppParameters.ConnectionList.Keys.Count == 0)
                            {
                                AppParameters.ConnectionList = AppParameters.LoadData("Connection.json");
                            }
                            if (AppParameters.ZoneInfo.Keys.Count == 0)
                            {
                                AppParameters.ZoneInfo = AppParameters.LoadData("Zones.json");
                            }
                            if (AppParameters.NotificationConditionsList.Keys.Count == 0)
                            {
                                AppParameters.NotificationConditionsList = AppParameters.LoadData("Notification.json");
                            }
                            if (AppParameters.IndoorMap.Keys.Count == 0)
                            {
                                AppParameters.LoadIndoorapData("ProjectData.json");
                            }
                            if (AppParameters.RunningConnection.Connection.Count == 0)
                            {
                                Thread ConectionSetupThread = new Thread(new ThreadStart(ConectionSetup));
                                ConectionSetupThread.IsBackground = true;
                                ConectionSetupThread.Start();
                            }
                        }
                        else
                        {
                            AppParameters.ConnectionList = new ConcurrentDictionary<string, JObject>();
                            AppParameters.ZoneInfo = new ConcurrentDictionary<string, JObject>();
                            AppParameters.IndoorMap = new ConcurrentDictionary<string, JObject>();
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
                foreach (JObject item in AppParameters.ConnectionList.Values)
                {
                    AppParameters.RunningConnection.Add(item);
                }
            }
            catch (Exception e)
            {
                new ErrorLogger().ExceptionLog(e);
            }
        }
    }
}