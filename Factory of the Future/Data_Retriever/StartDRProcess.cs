namespace Factory_of_the_Future
{
    public class StartDRProcess
    {
        //private static int SPworkItemCount = 0;
        //private StartDRProcess()
        //    { }
        //internal static void StartProcess(object SP)
        //{
        //    //int SPworkItemNumber = SPworkItemCount;
        //    //Interlocked.Increment(ref SPworkItemCount);
        //    //SourceAPI.StartProcess _SP = (SourceAPI.StartProcess)SP;

        //    //try
        //    //    {
        //    //    if (Global.API_List.Count > 0)
        //    //        {
        //    //        JArray svr_conn_list = new JArray();

        //    //        foreach (JObject item in Global.API_List.Values)
        //    //            {
        //    //            if ((bool)item.Property("ACTIVE_CONNECTION").Value)
        //    //                {
        //    //                if (DateTime.Now.Subtract((DateTime)item.Property("LASTTIME_API_CONNECTED").Value).TotalMilliseconds >= (int)item.Property("DATA_RETRIEVE").Value)
        //    //                    {
        //    //                    svr_conn_list.Add(item);
        //    //                    }
        //    //                }
        //    //                if ((bool)item.Property("ACTIVE_CONNECTION").Value == false && (bool)item.Property("API_CONNECTED").Value == true)
        //    //                    {
        //    //                    if (Global.API_List.ContainsKey((int)item.Property("ID").Value))
        //    //                        {
        //    //                        if (Global.API_List.TryGetValue((int)item.Property("ID").Value, out JObject Svr_con))
        //    //                            {
        //    //                            Svr_con.Property("API_CONNECTED").Value = false;
        //    //                            }
        //    //                        }
        //    //                    }

        //    //            }
        //    //        if (svr_conn_list.Count > 0)
        //    //            {
        //    //            SourceData_Process SourceData_list;
        //    //            ManualResetEvent[] manualEvents;
        //    //            manualEvents = new ManualResetEvent[svr_conn_list.Count];
        //    //            int intc = -1;
        //    //            foreach (JObject item in svr_conn_list.Children())
        //    //                {
        //    //                intc++;
        //    //                manualEvents[intc] = new ManualResetEvent(false);
        //    //                SourceData_list = new SourceData_Process(item, manualEvents[intc], intc);
        //    //                ThreadPool.QueueUserWorkItem(new WaitCallback(SourceDataProcess.Start_processor), SourceData_list);
        //    //                }
        //    //            WaitHandle.WaitAll(manualEvents);
        //    //            }
        //    //        }
        //    //    }
        //    //catch (Exception ex)
        //    //    {
        //    //    new ErrorLogger().ExceptionLog(ex);
        //    //    }
        //    //finally
        //    //    {
        //    //    _SP.manualEvents.Set();
        //    //    }
        //}
    }
}