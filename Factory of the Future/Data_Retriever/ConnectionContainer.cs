using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Factory_of_the_Future
{
    public class ConnectionContainer
    {
        public List<Api_Connection> Connection = new List<Api_Connection>();
       
        public void Add(JObject con) 
        {
            Api_Connection NewConnection = new Api_Connection();
            NewConnection.ID = con["id"].ToString();
            NewConnection.API_Info.Merge(con, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            this.Connection.Add(NewConnection);
            if ((bool)con["ACTIVE_CONNECTION"])
            {
                if ((bool)con["UDP_CONNECTION"])
                {
                    NewConnection._UDPThreadListener();
                }
                else
                {
                    NewConnection._ThreadDownload();

                    if (!(bool)con["UDP_CONNECTION"])
                    {
                        NewConnection._ThreadRefresh();
                        NewConnection.ConstantRefresh = true;

                    }
                }
            }
            else
            {
                NewConnection.Stop();
            }
            
        }
    }
}