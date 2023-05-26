using System;
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
                            //data Retention
                            Task.Run(() => DataRetentionProcess.Start()).ConfigureAwait(false);
                        }
                        else
                        {
                            Task.Run(() => AppParameters.ResetParameters());
                            Task.Run(() => new ErrorLogger().CustomLog("Rest Application", string.Concat(AppParameters.AppSettings.APPLICATION_NAME, "_Applogs"))).ConfigureAwait(false);
                            //AppParameters.Users = new ConcurrentDictionary<string, ADUser>();

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