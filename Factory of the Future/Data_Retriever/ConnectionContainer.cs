﻿using Newtonsoft.Json;
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
                if (con.TcpIpConnection)
                {
                    NewConnection._TCPThreadListener();
                }
                else if (con.WsConnection)
                {
                    NewConnection._WSThreadListener();
                }
                else
                {
                    NewConnection._ThreadDownload();

                    if (!con.TcpIpConnection || !con.TcpIpConnection || !con.WsConnection)
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