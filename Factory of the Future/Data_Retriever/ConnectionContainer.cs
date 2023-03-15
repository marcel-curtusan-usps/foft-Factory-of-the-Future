﻿using Factory_of_the_Future.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Factory_of_the_Future
{
    public class ConnectionContainer : IDisposable
    {
        public List<Api_Connection> Connection = new List<Api_Connection>();
        private bool disposedValue;
        public bool updateFile;
        public Connection _conn { get; protected set; }
        public Api_Connection NewConnection { get; protected set; }
        public void Add(Connection con, bool saveToFile)
        {
            try
            {
                NewConnection = new Api_Connection();
                con.Status = "Idle/Active";
                if (con.ConnectionName.ToLower() == "MPEWatch".ToLower())
                {
                    con.IpAddress = "";
                    con.Port = 0;
                    con.Url = "";
                    string sitename = AppParameters.AppSettings["FACILITY_NAME"].ToString().ToLower().Replace(" ", "_").Replace("&", "").Replace("(", "").Replace(")", "");
                    AppParameters.MPEWatchData.Where(r => r.Value.SiteNameLocal.ToLower() == sitename).Select(y => y.Value).ToList().ForEach(m =>
                    {
                        con.IpAddress = m.Host;
                        con.Port = m.Port;
                        con.Url = m.URL;
                    });
                }
                NewConnection.ID = con.Id;
                NewConnection.ConnectionInfo = con;
                Connection.Add(NewConnection);
                if (con.ActiveConnection)
                {
                    if (con.UdpConnection)
                    {
                        NewConnection._UDPThreadListener();
                    }
                    else if (con.TcpIpConnection)
                    {
                        NewConnection._TCPThreadListener();
                    }
                    else if (con.WsConnection)
                    {
                        NewConnection._WSThreadListener();
                    }
                    else if (con.ApiConnection)
                    {
                        if (con.DataRetrieve != 0)
                        {
                            NewConnection.ConstantRefresh = true;
                            NewConnection._ThreadDownload();
                            NewConnection._ThreadRefresh();
                        }
                    }
                }
                else
                {
                    NewConnection.Status = 2;
                    NewConnection.ConnectionInfo.Status = "Deactived";
                }
                if (saveToFile)
                {
                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", AppParameters.ConnectionOutPutdata(Connection.Select(y => y.ConnectionInfo).ToList()));
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
        public async Task EditAsync(Connection updateConndata)
        {
            try
            {
                foreach (Api_Connection Connection_item in Connection)
                {
                    if (Connection_item.ConnectionInfo.Id == updateConndata.Id)
                    {
                        updateConndata.LastupDate = DateTime.Now;
                        foreach (PropertyInfo prop in Connection_item.ConnectionInfo.GetType().GetProperties())
                        {
                            if (!new Regex("^(UdpConnection|TcpIpConnection|WsConnection|ApiConnection|ConstantRefresh|Status)$", RegexOptions.IgnoreCase).IsMatch(prop.Name))
                            {
                                if (prop.GetValue(Connection_item.ConnectionInfo, null).ToString() != prop.GetValue(updateConndata, null).ToString())
                                {
                                    updateFile = true;
                                    prop.SetValue(Connection_item.ConnectionInfo, prop.GetValue(updateConndata, null));

                                }
                            }
                        }
                        if (!updateConndata.ActiveConnection)
                        {
                            Connection_item.ConnectionInfo.DeactivatedDate = DateTime.Now;
                            Connection_item.ConnectionInfo.DeactivatedByUsername = updateConndata.LastupdateByUsername;
                            Connection_item.ConnectionInfo.ActiveConnection = updateConndata.ActiveConnection;
                            if (updateConndata.UdpConnection)
                            {
                                Connection_item._StopUDPListener();
                            }
                            else if (updateConndata.TcpIpConnection)
                            {
                                Connection_item._StopTCPListener();
                            }
                            else if (updateConndata.WsConnection)
                            {
                                Connection_item.WSStop();
                            }
                            else if (updateConndata.ApiConnection)
                            {
                                Connection_item.ConstantRefresh = false;
                                Connection_item.Stop();
                            }

                        }
                        else if (updateConndata.ActiveConnection)
                        {

                            if (updateConndata.UdpConnection)
                            {
                                Connection_item.ConnectionInfo.UdpConnection = updateConndata.UdpConnection;
                                if (Connection_item.ConnectionInfo.Port != updateConndata.Port)
                                {
                                    Connection_item._StartUDPListener();
                                }
                                Connection_item._StartUDPListener();
                            }
                            else if (updateConndata.TcpIpConnection)
                            {
                                Connection_item.ConnectionInfo.TcpIpConnection = updateConndata.TcpIpConnection;
                                if (Connection_item.ConnectionInfo.Port != updateConndata.Port)
                                {
                                    Connection_item._StopTCPListener();
                                }
                                Connection_item._StartTCPListener();
                            }
                            else if (updateConndata.WsConnection)
                            {
                                Connection_item.ConnectionInfo.WsConnection = updateConndata.WsConnection;
                                Connection_item._WSThreadListener();
                            }
                            else if (updateConndata.ApiConnection)
                            {
                                if (updateConndata.DataRetrieve != 0)
                                {
                                    Connection_item.ConstantRefresh = true;
                                    Connection_item.ConnectionInfo.Url = updateConndata.Url;
                                    Connection_item.ConnectionInfo.DataRetrieve = updateConndata.DataRetrieve;
                                    Connection_item.ConnectionInfo.ApiConnection = updateConndata.ApiConnection;
                                    if (Connection_item.Status == 2)
                                    {
                                        Connection_item.ConstantRefresh = true;
                                        Connection_item._ThreadDownload();
                                        Connection_item._ThreadRefresh();
                                    }
                                }

                            }
                        }
                        if (updateFile)
                        {
                         await Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(Connection_item.ConnectionInfo)).ConfigureAwait(false);
                        }
                    }
                }
                if (updateFile)
                {
                    new FileIO().Write(string.Concat(AppParameters.CodeBase.Parent.FullName.ToString(), AppParameters.Appsetting), "Connection.json", AppParameters.ConnectionOutPutdata(Connection.Select(y => y.ConnectionInfo).ToList()));
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

        internal async Task ConnectionUpdate(string connID, int status)
        {
            try
            {
                foreach (Api_Connection Connection_item in Connection)
                {
                    if (Connection_item.ConnectionInfo.Id == connID)
                    {
                        Connection_item.Status = status;
                        switch (status)
                        {
                            case 0:
                                Connection_item.ConnectionInfo.Status = "Active";
                                break;
                            case 1:
                                Connection_item.ConnectionInfo.Status = "Running";
                                break;
                            case 2:
                                Connection_item.ConnectionInfo.Status = "Deactived";
                                break;
                            case 3:
                                Connection_item.ConnectionInfo.Status = "Invaild URL";
                                break;
                            case 4:
                                Connection_item.ConnectionInfo.Status = "No data";
                                break;
                            default:
                                break;
                        }
                        
                        await Task.Run(() => FOTFManager.Instance.BroadcastQSMUpdate(Connection_item.ConnectionInfo)).ConfigureAwait(false);
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

        internal void Remove(Connection connection)
        {
            throw new NotImplementedException();
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
                _conn = null;
                NewConnection = null;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DataConnectionContainer()
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