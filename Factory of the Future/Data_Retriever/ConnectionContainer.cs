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
       
        public void Add(Connection con) 
        {
            Api_Connection NewConnection = new Api_Connection();
            NewConnection.ID = con.Id;
            NewConnection.ConnectionInfo = con;
            this.Connection.Add(NewConnection);
            if (con.ActiveConnection)
            {
                if (con.UdpConnection)
                {
                    NewConnection._UDPThreadListener();
                }
                else
                {
                    NewConnection._ThreadDownload();

                    if (!con.UdpConnection)
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